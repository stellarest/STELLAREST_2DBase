using System.Collections;
using System.Runtime.CompilerServices;
using STELLAREST_2D.Data;
using UnityEditor.Rendering.Universal;
using UnityEngine;

namespace STELLAREST_2D
{
    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // +++ Spear is a actually commander for controlling spears[LEFT, RIGHT] +++
    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    public class Spear : RepeatSkill
    {
        private enum SpearDirection { Left, Right, Max }
        #region Utilities
        private int LEFT => (int)SpearDirection.Left;
        private Spear LEFT_SPEAR => _spears[LEFT];
        private Vector3 LEFT_DEFAULT => this.Owner.transform.position + new Vector3(X_AXIS_INTERVAL_POSITION_FROM_OWNER * -1, Y_AXIS_INTERVAL_POSITION_FROM_OWNER, 0f);
        private Vector3 LEFT_IDLE_TOP => this.LEFT_DEFAULT + new Vector3(0f, IDLE_TOP_DOWN_POSITION_INTENSITY, 0f);
        private Vector3 LEFT_IDLE_BOTTOM => this.LEFT_DEFAULT + new Vector3(0f, IDLE_TOP_DOWN_POSITION_INTENSITY * -1, 0f); // Actually Default Pos (Bot To Top)

        private int RIGHT => (int)SpearDirection.Right;
        private Spear RIGHT_SPEAR => _spears[RIGHT];
        private Vector3 RIGHT_DEFAULT => this.Owner.transform.position + new Vector3(X_AXIS_INTERVAL_POSITION_FROM_OWNER, Y_AXIS_INTERVAL_POSITION_FROM_OWNER, 0);
        private Vector3 RIGHT_IDLE_TOP => this.RIGHT_DEFAULT + new Vector3(0f, IDLE_TOP_DOWN_POSITION_INTENSITY, 0f);
        private Vector3 RIGHT_IDLE_BOTTOM => this.RIGHT_DEFAULT + new Vector3(0f, IDLE_TOP_DOWN_POSITION_INTENSITY * -1, 0f);  // Actually Default Pos (Bot To Top)
        #endregion

        #region Fields && Field's Method && Temp Constant Options
        private Spear[] _spears = null;
        private int _dir = -1;
        private Vector3 _toTargetDir = Vector3.zero;


        private float _percent = 0f;
        private int _maxStabCount = 0;
        private SpriteTrail.SpriteTrail _trail = null;

        [SerializeField] private AnimationCurve _curveEaseOut = null;
        [SerializeField] private AnimationCurve _curveLinear = null;

        private enum CurveType { Idle, Stab, Rotation, Back }
        private AnimationCurve Curve(CurveType type)
        {
            switch (type)
            {
                case CurveType.Idle:
                case CurveType.Stab:
                    return _curveEaseOut;
            }

            return _curveLinear;
        }

        private GameObject _head = null;
        private CreatureController _target = null;

        private Vector3 _lerpStartPos = Vector3.zero;
        private Vector3 _lerpEndPos = Vector3.zero;
        private Vector3 _savingLerpStartPos = Vector3.zero; // FOR ULTIMATE

        private Quaternion _lerpStartRot = Quaternion.identity;
        private Quaternion _lerpEndRot = Quaternion.identity;

        private bool _idleDirFlag = false;
        private bool _lockTarget = false;

        private void ResetIdle(Spear spear)
        {
            spear._target = null;
            spear._idleDirFlag = false;
            spear._lockTarget = false;
            ResetDelta(spear);
        }

        private float _delta = 0f;
        private float _coolTimeDelta = 0f;
        private void ResetDelta(Spear spear)
        {
            if (spear._dir == (int)SpearDirection.Left)
            {
                LEFT_SPEAR._delta = 0f;
                LEFT_SPEAR._coolTimeDelta = 0f;
                LEFT_SPEAR._percent = 0f;
            }
            else
            {
                RIGHT_SPEAR._delta = 0f;
                RIGHT_SPEAR._coolTimeDelta = 0f;
                RIGHT_SPEAR._percent = 0f;
            }
        }

        private const float X_AXIS_INTERVAL_POSITION_FROM_OWNER = 4.5f;
        private const float Y_AXIS_INTERVAL_POSITION_FROM_OWNER = 0.25f;
        private const float IDLE_TOP_DOWN_POSITION_INTENSITY = 0.5f;
        private const float ROTATION_SPEED = 30f;
        private const float SEARCH_TARGET_RANGE = 6f; // sqrMag : 36f

        private const float DESIRED_TIME_IDLE_DIR_SWITCH = 2f;
        private const float DESIRED_TIME_FORCE_ROTATE_TO_TARGET_ULTIMATE = 0.075f; // TEMP
        private const float DESIRED_TIME_STAB_TO_REACH_TARGET = 0.15f;
        private const float DESIRED_TIME_RETURN_TO_OWNER = DESIRED_TIME_STAB_TO_REACH_TARGET * 2;
        private const float DESIRED_TIME_RETURN_TO_OWNER_ULTIMATE = 0.1f;
        private const float DESIRED_TIME_AFTER_STAB_WAIT = 0.5f;
        #endregion

        #region Overrides
        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            RigidBody = GetComponent<Rigidbody2D>();
            RigidBody.simulated = false;

            HitCollider = GetComponent<Collider2D>();
            HitCollider.enabled = false;

            SR = GetComponentInChildren<SpriteRenderer>();
            SR.enabled = false;
            if (data.Grade == data.MaxGrade)
            {
                _trail = GetComponentInChildren<SpriteTrail.SpriteTrail>();
                _trail.enabled = false;
            }
        }

        public override void Activate()
        {
            IsStopped = false;
            gameObject.SetActive(true);
            _spears = new Spear[(int)SpearDirection.Max];
            for (int i = 0; i < (int)SpearDirection.Max; ++i)
                CreateSpears(i);

            this.DoSkillJob();
        }

        public override void Deactivate()
        {
            IsStopped = true;
            gameObject.SetActive(false);
            if (this._spears == null)
                return;

            for (int i = 0; i < (int)SpearDirection.Max; ++i)
            {
                if (_spears[i].IsValid())
                    Managers.Resource.Destroy(_spears[i].gameObject);
            }
        }

        protected override void SetSortingOrder()
        {
            if (this.Data.Grade < this.Data.MaxGrade)
                this.SR.sortingOrder = (int)Define.SortingOrder.Skill;
            else
            {
                this.SR.sortingOrder = (int)Define.SortingOrder.Skill;
                this._trail.m_OrderInSortingLayer = (int)Define.SortingOrder.Skill;
            }
        }

        protected override void DoSkillJob()
        {
            this.StartCoroutine(CoDoSpear(LEFT_SPEAR));
            this.StartCoroutine(CoDoSpear(RIGHT_SPEAR));
        }
        #endregion

        private IEnumerator CoDoSpear(Spear spear)
        {
            if (spear._dir == LEFT)
            {
                while (true)
                {
                    yield return null;
                    yield return new WaitUntil(() => Init(LEFT_SPEAR));
                    yield return new WaitUntil(() => Idle(LEFT_SPEAR));

                    // STAB -> WAIT -> BACK
                    for (int i = 1; i <= spear._maxStabCount; ++i)
                    {
                        yield return new WaitUntil(() => Stab(LEFT_SPEAR, i));
                        yield return new WaitUntil(() => Wait(LEFT_SPEAR, i));
                        yield return new WaitUntil(() => Back(LEFT_SPEAR, i));

                        if (spear.Data.Grade != spear.Data.MaxGrade)
                            break;
                        else // ULTIMATE
                        {
                            if (i < spear._maxStabCount)
                                yield return new WaitUntil(() => Rotate(LEFT_SPEAR, DESIRED_TIME_FORCE_ROTATE_TO_TARGET_ULTIMATE, false));

                            spear.RigidBody.simulated = true;
                            spear.HitCollider.enabled = true;
                        }
                    }

                    yield return new WaitUntil(() => CanContinue(LEFT_SPEAR));
                    yield return null;
                }
            }
            else if (spear._dir == RIGHT)
            {
                while (true)
                {
                    yield return new WaitUntil(() => Init(RIGHT_SPEAR));
                    yield return new WaitUntil(() => Idle(RIGHT_SPEAR));

                    // STAB -> WAIT -> BACK
                    for (int i = 1; i <= spear._maxStabCount; ++i)
                    {
                        yield return new WaitUntil(() => Stab(RIGHT_SPEAR, i));
                        yield return new WaitUntil(() => Wait(RIGHT_SPEAR, i));
                        yield return new WaitUntil(() => Back(RIGHT_SPEAR, i));

                        if (spear.Data.Grade != spear.Data.MaxGrade)
                            break;
                        else // ULTIMATE
                        {
                            if (i < spear._maxStabCount)
                                yield return new WaitUntil(() => Rotate(RIGHT_SPEAR, DESIRED_TIME_FORCE_ROTATE_TO_TARGET_ULTIMATE, false));

                            spear.RigidBody.simulated = true;
                            spear.HitCollider.enabled = true;
                        }
                    }

                    yield return new WaitUntil(() => CanContinue(RIGHT_SPEAR));
                    yield return null;
                }
            }
        }

        private bool Init(Spear spear)
        {
            spear.RigidBody.simulated = false;
            spear.HitCollider.enabled = false;
            ResetIdle(spear);

            return true;
        }

        private bool Idle(Spear spear)
        {
            if (spear._lockTarget == false)
            {
                spear._target = Utils.GetClosestCreatureTargetFromAndRange<CreatureController>(spear._head, spear.Owner, SEARCH_TARGET_RANGE);
                if (spear._target == null)
                    IdleMovement(spear);
                else if (spear._target != null && spear._target.IsValid())
                {
                    spear._lockTarget = true;
                    ResetDelta(spear);
                }
            }
            else
            {
                // When target is dead
                if (spear._target.IsValid() == false)
                {
                    ResetIdle(spear);
                    return false;
                }
                else if (IsOverCoolDown(spear))
                {
                    ReadyToStab(spear);
                    ResetDelta(spear);
                    return true;
                }
                else
                    RotateToTarget(spear);
            }

            return false;
        }

        private void IdleMovement(Spear spear)
        {
            switch (spear._dir)
            {
                case (int)SpearDirection.Left:
                    {
                        if (spear._idleDirFlag == false)
                        {
                            if (LerpIdleAnimationLoop(spear, LEFT_IDLE_BOTTOM, LEFT_IDLE_TOP))
                                spear._idleDirFlag = (!spear._idleDirFlag);
                        }
                        else
                        {
                            if (LerpIdleAnimationLoop(spear, LEFT_IDLE_TOP, LEFT_IDLE_BOTTOM))
                                spear._idleDirFlag = (!spear._idleDirFlag);
                        }

                        float degrees = Mathf.Atan2(this.Owner.ShootDir.y, this.Owner.ShootDir.x) * Mathf.Rad2Deg;
                        spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation,
                                    Quaternion.Euler(0, 0, degrees - 90f), Time.deltaTime * ROTATION_SPEED);
                    }
                    break;

                case (int)SpearDirection.Right:
                    {
                        if (spear._idleDirFlag == false)
                        {
                            if (LerpIdleAnimationLoop(spear, RIGHT_IDLE_BOTTOM, RIGHT_IDLE_TOP))
                                spear._idleDirFlag = (!spear._idleDirFlag);
                        }
                        else
                        {
                            if (LerpIdleAnimationLoop(spear, RIGHT_IDLE_TOP, RIGHT_IDLE_BOTTOM))
                                spear._idleDirFlag = (!spear._idleDirFlag);
                        }

                        float degrees = Mathf.Atan2(this.Owner.ShootDir.y, this.Owner.ShootDir.x) * Mathf.Rad2Deg;
                        spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation,
                                    Quaternion.Euler(0, 0, degrees - 90f), Time.deltaTime * ROTATION_SPEED);
                    }
                    break;
            }
        }

        private bool LerpIdleAnimationLoop(Spear spear, Vector3 startPos, Vector3 endPos)
        {
            spear._delta += Time.deltaTime;
            spear._percent = spear._delta / DESIRED_TIME_IDLE_DIR_SWITCH;
            spear.transform.position = Vector3.Lerp(startPos, endPos, this.Curve(CurveType.Idle).Evaluate(spear._percent));
            if (spear._percent > 1f)
            {
                this.ResetDelta(spear);
                return true;
            }

            return false;
        }

        private bool IsOverCoolDown(Spear spear)
        {
            if (spear._percent > 1f)
            {
                spear._percent = 1f;
                return true;
            }
            else
            {
                spear._coolTimeDelta += Time.deltaTime;
                spear._percent = spear._coolTimeDelta / spear.Data.CoolTime;
            }

            return false;
        }

        private void ReadyToStab(Spear spear)
        {
            Vector3 startPos = spear._head.transform.position;
            Vector3 targetPos = spear._target.Center.position;
            //Vector3 toTargetDir = (targetPos - startPos).normalized;

            spear._lerpStartPos = startPos;
            spear._savingLerpStartPos = startPos;

            //spear._lerpEndPos = startPos + (toTargetDir * SEARCH_TARGET_RANGE);
            spear._lerpEndPos = startPos + (spear.transform.up * SEARCH_TARGET_RANGE);

            if (spear._trail != null)
                spear._trail.enabled = true;

            spear.RigidBody.simulated = true;
            spear.HitCollider.enabled = true;
        }

        private void RotateToTarget(Spear spear)
        {
            // +++ USE SPEAR._HEAD +++
            Vector3 toTargetDir = (spear._target.Center.position - spear._head.transform.position).normalized;
            float degrees = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
            float toTargetDist = (spear._target.Center.position - spear._head.transform.position).sqrMagnitude;
            if (toTargetDist > 1f)
                spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, Quaternion.Euler(0, 0, degrees - 90f), Time.deltaTime * ROTATION_SPEED);
        }

        private bool Stab(Spear spear, int stabCount)
        {
            spear._delta += Time.deltaTime;
            spear._percent = spear._delta / DESIRED_TIME_STAB_TO_REACH_TARGET;
            spear.transform.position = Vector3.Lerp(spear._lerpStartPos, spear._lerpEndPos, this.Curve(CurveType.Stab).Evaluate(spear._percent));

            if (spear._percent > 1f)
            {
                spear._lerpStartPos = spear.transform.position;
                //spear._lerpEndPos = this.Owner.Center.transform.position; // nono
                spear.RigidBody.simulated = false;
                spear.HitCollider.enabled = false;

                if (spear._trail != null)
                    spear._trail.enabled = false;

                ResetDelta(spear);
                return true;
            }

            return false;
        }

        private bool Wait(Spear spear, int stabCount)
        {
            if (spear.Data.Grade == spear.Data.MaxGrade)
                return true;

            spear._delta += Time.deltaTime;
            spear._percent = spear._delta / DESIRED_TIME_AFTER_STAB_WAIT;
            if (spear._percent > 1f)
            {
                ResetDelta(spear);
                return true;
            }

            return false;
        }

        private bool Back(Spear spear, int stabCount)
        {
            spear._delta += Time.deltaTime;
            if (spear.Data.Grade < spear.Data.MaxGrade)
            {
                spear._percent = spear._delta / DESIRED_TIME_RETURN_TO_OWNER;
                if (spear._dir == LEFT)
                    spear.transform.position = Vector3.Lerp(spear._lerpStartPos, LEFT_IDLE_BOTTOM, this.Curve(CurveType.Back).Evaluate(spear._percent));
                else
                    spear.transform.position = Vector3.Lerp(spear._lerpStartPos, RIGHT_IDLE_BOTTOM, this.Curve(CurveType.Back).Evaluate(spear._percent));

                Vector3 toOwnerDir = (this.Owner.Center.transform.position - spear.transform.position).normalized;
                float degrees = Mathf.Atan2(toOwnerDir.y, toOwnerDir.x) * Mathf.Rad2Deg;
                spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, Quaternion.Euler(0, 0, degrees - 90f), Time.deltaTime * ROTATION_SPEED);
                if (spear._percent > 1f)
                {
                    ResetDelta(spear);
                    return true;
                }
            }
            else // +++++ Back : ULTIMATE SPEAR +++++
            {
                if (stabCount < spear._maxStabCount)
                {
                    spear._percent = spear._delta / DESIRED_TIME_RETURN_TO_OWNER_ULTIMATE;
                    spear.transform.position = Vector3.Lerp(spear._lerpStartPos, spear._savingLerpStartPos, this.Curve(CurveType.Back).Evaluate(spear._percent));
                    if (spear._percent > 1f)
                    {
                        spear._lerpStartRot = spear.transform.rotation;
                        if (stabCount == 1)
                        {
                            Vector3 toNextDir = (Quaternion.Euler(0, 0, 30f * stabCount) * spear._head.transform.up).normalized;
                            float degrees = Mathf.Atan2(toNextDir.y, toNextDir.x) * Mathf.Rad2Deg;
                            spear._lerpEndRot = Quaternion.Euler(0, 0, degrees - 90f);
                        }
                        else if (stabCount == 2)
                        {
                            Vector3 toNextDir = (Quaternion.Euler(0, 0, 30f * -stabCount) * spear._head.transform.up).normalized;
                            float degrees = Mathf.Atan2(toNextDir.y, toNextDir.x) * Mathf.Rad2Deg;
                            spear._lerpEndRot = Quaternion.Euler(0, 0, degrees - 90f);
                        }

                        ResetDelta(spear);
                        return true;
                    }
                }
                else
                {
                    spear._percent = spear._delta / DESIRED_TIME_RETURN_TO_OWNER_ULTIMATE;
                    if (spear._dir == LEFT)
                        spear.transform.position = Vector3.Lerp(spear._lerpStartPos, LEFT_IDLE_BOTTOM, this.Curve(CurveType.Back).Evaluate(spear._percent));
                    else
                        spear.transform.position = Vector3.Lerp(spear._lerpStartPos, RIGHT_IDLE_BOTTOM, this.Curve(CurveType.Back).Evaluate(spear._percent));

                    Vector3 toOwnerDir = (this.Owner.Center.transform.position - spear.transform.position).normalized;
                    float degrees = Mathf.Atan2(toOwnerDir.y, toOwnerDir.x) * Mathf.Rad2Deg;
                    spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, Quaternion.Euler(0, 0, degrees - 90f), Time.deltaTime * ROTATION_SPEED);
                    if (spear._percent > 1f)
                    {
                        ResetDelta(spear);
                        return true;
                    }
                }
            }

            return false;
        }

        // FOR ULTIMATE
        private bool Rotate(Spear spear, float desiredTime, bool isOnLockTarget = false)
        {
            spear._delta += Time.deltaTime;
            spear._percent = spear._delta / desiredTime;
            spear.transform.rotation = Quaternion.Slerp(spear._lerpStartRot, spear._lerpEndRot, this.Curve(CurveType.Rotation).Evaluate(spear._percent));
            if (spear._percent > 1f)
            {
                // spear._lockTargetToRot = isOnLockTarget;
                // SET START POS
                spear._lerpStartPos = spear.transform.position;
                spear._lerpEndPos = spear.transform.position + (spear.transform.up * SEARCH_TARGET_RANGE);

                if (spear._trail != null)
                    spear._trail.enabled = true;

                ResetDelta(spear);
                return true;
            }

            return false;
        }

        private bool CanContinue(Spear spear)
        {
            // TODO : Check if owner is dead, in CC state, or something else
            return true;
        }

        private void CreateSpears(int dir)
        {
            GameObject go = Managers.Resource.Instantiate(key: this.Data.PrimaryLabel, parent: null, pooling: false);
            if (dir == (int)SpearDirection.Left)
            {
                go.name = $"{this.Data.Name}_Left";
                go.transform.position = LEFT_IDLE_BOTTOM;
            }
            else
            {
                go.name = $"{this.Data.Name}_Right";
                go.transform.position = RIGHT_IDLE_BOTTOM;
            }

            Spear spear = go.GetComponent<Spear>();
            if (dir == (int)SpearDirection.Left)
                spear._dir = (int)SpearDirection.Left;
            else
                spear._dir = (int)SpearDirection.Right;

            spear.Owner = this.Owner;
            spear.Data = this.Data;

            spear.RigidBody = go.GetComponent<Rigidbody2D>();
            spear.RigidBody.simulated = false;
            spear._head = go.transform.GetChild(1).gameObject;

            spear.HitCollider = go.GetComponent<Collider2D>();
            spear.HitCollider.enabled = false;

            spear.SR = go.GetComponentInChildren<SpriteRenderer>();
            spear.SR.enabled = true;
            if (this.Data.Grade == this.Data.MaxGrade)
            {
                spear._trail = go.GetComponentInChildren<SpriteTrail.SpriteTrail>();
                spear._trail.enabled = false;
                spear._maxStabCount = 3;
            }
            else
                spear._maxStabCount = 1;

            if (this.Owner?.IsPlayer() == true)
                Managers.Collision.InitCollisionLayer(spear.gameObject, Define.CollisionLayers.PlayerAttack);
            else
                Managers.Collision.InitCollisionLayer(spear.gameObject, Define.CollisionLayers.MonsterAttack);

            _spears[dir] = spear;
            _spears[dir].SetSortingOrder();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            cc.OnDamaged(attacker: this.Owner, from: this);
            // TEMP
            // cc.GetComponent<MonsterController>().Stop();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
        }
    }
}

// -------------------------------------------------------------------------------------
// -------------------------------------------------------------------------------------
// private bool IsTargetInRange(Spear spear, float range)
// {
//     if (spear._target.IsValid() == false)
//     {
//         ResetIdle(spear);
//         return false;
//     }

//     float toTargetDistance = (spear._target.Center.transform.position - spear._head.transform.position).sqrMagnitude;
//     if (toTargetDistance < range * range)
//         return true;

//     return false;
// }
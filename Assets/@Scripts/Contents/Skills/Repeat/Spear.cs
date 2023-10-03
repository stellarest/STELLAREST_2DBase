using System.Collections;
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

        #region Fields && Field's Method && Constant Options
        private Spear[] _spears = null;
        private int _dir = -1;
        private float _distanceToTarget = 0f;
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
        private bool IsTargetDead(Spear spear) => spear._target.IsValid() == false;

        private Vector3 _lockedPos = Vector3.zero;
        private Vector3 _lerpStartPos = Vector3.zero;
        private Vector3 _lerpEndPos = Vector3.zero;
        
        private Vector3 _savingLerpStartPot = Vector3.zero; // FOR ULTIMATE

        private Quaternion _lerpStartRot = Quaternion.identity;
        private Quaternion _lerpEndRot = Quaternion.identity;

        private bool _idleDirFlag = false;
        private bool _lockTarget = false;
        private bool HasLockTarget(Spear spear) => spear._lockTarget;
        private void LockTarget(Spear spear)
        {
            // SET TARGET
            spear._lockTarget = true;

            // _lockPos 잡혔을때만,,
            spear._lockedPos = spear.transform.position;
            spear._readyToBack = true;

            // SET STAB ROTS ONLY
            spear._lerpStartRot = spear.transform.rotation;

            // Target Dir을 spear._head가 아닌, spear.transform으로 하니까 잘됨
            Vector3 toTargetDir = (spear._target.Center.position - spear.transform.position);
            float degrees = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
            spear._lerpEndRot = Quaternion.Euler(0, 0, degrees - 90f);
        }

        private bool _readyToBack = false;
        private bool _lockTargetToRot = false;
        private bool IsLockTargetToRot(Spear spear) => spear._lockTargetToRot;

        private void ResetIdle(Spear spear)
        {
            spear._target = null;
            spear._idleDirFlag = false;
            spear._lockTarget = false;
            //spear._readyToBack = false; // nono
            spear._lockTargetToRot = false;
            ResetDelta(spear);
        }

        private float _delta = 0f;
        private void ResetDelta(Spear spear)
        {
            if (spear._dir == (int)SpearDirection.Left)
                LEFT_SPEAR._delta = 0f;
            else
                RIGHT_SPEAR._delta = 0f;

            spear._percent = 0f;
        }

        private const float X_AXIS_INTERVAL_POSITION_FROM_OWNER = 4.5f;
        private const float Y_AXIS_INTERVAL_POSITION_FROM_OWNER = 0.25f;
        private const float IDLE_TOP_DOWN_POSITION_INTENSITY = 0.5f;
        private const float ROTATION_SPEED = 30f;
        private const float SEARCH_TARGET_RANGE = 6f; // sqrMag : 36f

        private const float DESIRED_TIME_IDLE_DIR_SWITCH = 2f;
        private const float DESIRED_TIME_FORCE_ROTATE_TO_TARGET = 0.15f;
        private const float DESIRED_TIME_FORCE_ROTATE_TO_TARGET_ULTIMATE = DESIRED_TIME_FORCE_ROTATE_TO_TARGET * 0.5f; // TEMP
        private const float DESIRED_TIME_STAB_TO_REACH_TARGET = 0.15f;
        private const float DESIRED_TIME_RETURN_TO_OWNER = DESIRED_TIME_STAB_TO_REACH_TARGET * 2;
        private const float DESIRED_TIME_RETURN_TO_OWNER_ULTIMATE = DESIRED_TIME_RETURN_TO_OWNER * 0.5f; // TEMP
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
            gameObject.SetActive(true);
            _spears = new Spear[(int)SpearDirection.Max];
            for (int i = 0; i < (int)SpearDirection.Max; ++i)
            {
                CreateSpears(i);
                Utils.Log($"{_spears[i].gameObject.name} is created.");
            }

            this.DoSkillJob();
        }

        public override void Deactivate(bool isPoolingClear = false)
        {
            gameObject.SetActive(false);
            if (this._spears == null)
                return;

            for (int i = 0; i < (int)SpearDirection.Max; ++i)
            {
                if (_spears[i].IsValid())
                {
                    Utils.Log($"{_spears[i].gameObject.name} is destroyed.");
                    Managers.Resource.Destroy(_spears[i].gameObject);
                }
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
            RIGHT_SPEAR.gameObject.SetActive(false);

            this.StartCoroutine(CoDoSpear(LEFT_SPEAR));
            //this.StartCoroutine(CoDoSpear(RIGHT_SPEAR));
        }
        #endregion

        #region Main Logic
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
                        else
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
        }

        private bool Init(Spear spear)
        {
            ResetIdle(spear);
            spear._readyToBack = false;
            return true;
        }

        private bool Idle(Spear spear)
        {
            if (HasLockTarget(spear) == false)
            {
                spear._target = Utils.GetClosestCreatureTargetFromAndRange<CreatureController>(spear._head, spear.Owner, SEARCH_TARGET_RANGE);
                if (spear._target == null)
                    IdleMovement(spear);
                else if (spear._target.IsValid())
                {
                    LockTarget(spear);
                    ResetDelta(spear);
                }
            }
            else
            {
                if (IsTargetDead(spear))
                {
                    ResetIdle(spear);
                    return false;
                }

                // +++ FORCE ROTATE TO TARGET +++
                if (IsLockTargetToRot(spear) == false)
                {
                    // DoForceRotateToTarget(spear);
                    Rotate(spear, DESIRED_TIME_FORCE_ROTATE_TO_TARGET, true);
                }
                else if (IsLockTargetToRot(spear))
                {
                    // +++ NEXT STEP +++
                    if (IsOverCoolDown(spear))
                    {
                        SetStab(spear);
                        ResetDelta(spear);
                        return true;
                    }
                    else if (IsTargetInRange(spear, SEARCH_TARGET_RANGE))
                        RotateToTarget(spear);
                    else if (IsTargetInRange(spear, SEARCH_TARGET_RANGE) == false)
                        ResetIdle(spear);
                }
            }

            return false;
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
                if (spear.Data.Grade == spear.Data.MaxGrade)
                {
                    spear.RigidBody.simulated = false;
                    spear.HitCollider.enabled = false;
                }

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
            else // ULTIMATE
            {
                if (stabCount < spear._maxStabCount)
                {
                    spear._percent = spear._delta / DESIRED_TIME_RETURN_TO_OWNER_ULTIMATE;
                    spear.transform.position = Vector3.Lerp(spear._lerpStartPos, spear._savingLerpStartPot, this.Curve(CurveType.Back).Evaluate(spear._percent));
                    if (spear._percent > 1f)
                    {
                        spear._lerpStartRot = spear.transform.rotation;
                        spear._lerpStartPos = spear.transform.position;
                        if (stabCount == 1)
                        {
                            Vector3 toNextDir = (Quaternion.Euler(0, 0, 30f * stabCount) * spear._head.transform.up).normalized;
                            float degrees = Mathf.Atan2(toNextDir.y, toNextDir.x) * Mathf.Rad2Deg;
                            spear._lerpEndRot = Quaternion.Euler(0, 0, degrees - 90f);
                            spear._lerpEndPos = spear.transform.position + (toNextDir.normalized * spear._distanceToTarget);
                        }
                        else if (stabCount == 2)
                        {
                            Vector3 toNextDir = (Quaternion.Euler(0, 0, 30f * -stabCount) * spear._head.transform.up).normalized;
                            float degrees = Mathf.Atan2(toNextDir.y, toNextDir.x) * Mathf.Rad2Deg;
                            spear._lerpEndRot = Quaternion.Euler(0, 0, degrees - 90f);
                            spear._lerpEndPos = spear.transform.position + (toNextDir.normalized * spear._distanceToTarget);
                        }

                        if (spear._trail != null)
                            spear._trail.enabled = true;

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

        // private bool BackUltimate(Spear spear, int i)
        // {
        //     spear._delta += Time.deltaTime;
        //     float percent = spear._delta / DESIRED_TIME_RETURN_TO_OWNER_ULTIMATE;
        //     if (spear._dir == LEFT)
        //         spear.transform.position = Vector3.Lerp(spear._lerpStartPos, spear._savingLerpStartPot, this.Curve(CurveType.Back).Evaluate(percent));

        //     if (percent > 1f)
        //     {
        //         // 다 돌아가면 다시 켜야함 얼티밋은
        //         spear.RigidBody.simulated = true;
        //         spear.HitCollider.enabled = true;

        //         // SET ROT
        //         spear._lerpStartRot = spear.transform.rotation;
        //         Vector3 toNextDir = Vector3.zero;
        //         if (i == 0)
        //         {
        //             toNextDir = (Quaternion.Euler(0, 0, 30f) * spear._head.transform.up).normalized;
        //             float degrees = Mathf.Atan2(toNextDir.y, toNextDir.x) * Mathf.Rad2Deg;
        //             spear._lerpEndRot = Quaternion.Euler(0, 0, degrees - 90f);

        //             // SET POS
        //             spear._lerpStartPos = spear.transform.position;
        //             spear._lerpEndPos = spear.transform.position + (toNextDir.normalized * spear._distanceToTarget);
        //         }
        //         else if (i == 1)
        //         {
        //             toNextDir = (Quaternion.Euler(0, 0, -60f) * spear._head.transform.up).normalized;
        //             float degrees = Mathf.Atan2(toNextDir.y, toNextDir.x) * Mathf.Rad2Deg;
        //             spear._lerpEndRot = Quaternion.Euler(0, 0, degrees - 90f);
        //             // SET POS
        //             spear._lerpStartPos = spear.transform.position;
        //             spear._lerpEndPos = spear.transform.position + (toNextDir.normalized * spear._distanceToTarget);
        //         }

        //         return true;
        //     }

        //     return false;
        // }

        // private bool StabUltimate(Spear spear)
        // {
        //     spear._delta += Time.deltaTime;
        //     float percent = spear._delta / DESIRED_TIME_STAB_TO_REACH_TARGET;
        //     spear.transform.position = Vector3.Lerp(spear._lerpStartPos, spear._lerpEndPos, this.Curve(CurveType.Stab).Evaluate(percent));
        //     if (percent > 1f)
        //     {
        //         spear._lerpStartPos = spear.transform.position;
        //         spear.RigidBody.simulated = false;
        //         spear.HitCollider.enabled = false;
        //         spear._trail.enabled = false;

        //         ResetDelta(spear);

        //         return true;
        //     }

        //     return false;
        // }

        private bool ForceNextRotUltimate(Spear spear)
        {
            spear._delta += Time.deltaTime;
            float percent = spear._delta / (DESIRED_TIME_FORCE_ROTATE_TO_TARGET * 0.5f);
            spear.transform.rotation = Quaternion.Slerp(spear._lerpStartRot, spear._lerpEndRot, this.Curve(CurveType.Rotation).Evaluate(percent));
            if (percent > 1f)
            {
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
        #endregion


#region Methods
        private bool IsTargetInRange(Spear spear, float range)
        {
            if (spear._target.IsValid() == false)
            {
                ResetIdle(spear);
                return false;
            }

            float toTargetDistance = (spear._target.Center.transform.position - spear._head.transform.position).sqrMagnitude;
            if (toTargetDistance < range * range)
                return true;

            return false;
        }

        private bool IsOverCoolDown(Spear spear)
        {
            spear._delta += Time.deltaTime;
            spear._percent = spear._delta / spear.Data.CoolTime;
            if (spear._percent > 1f)
            {
                ResetDelta(spear);
                return true;
            }

            return false;
        }

        private void IdleMovement(Spear spear)
        {
            switch (spear._dir)
            {
                case (int)SpearDirection.Left:
                    {
                        if (spear._readyToBack)
                        {
                            spear._delta += Time.deltaTime;
                            spear._percent = spear._delta / DESIRED_TIME_RETURN_TO_OWNER;
                            spear.transform.position = Vector3.Lerp(spear._lockedPos, LEFT_IDLE_BOTTOM, this.Curve(CurveType.Back).Evaluate(spear._percent));
                            if (spear._percent > 1f)
                            {
                                spear._idleDirFlag = false;
                                spear._readyToBack = false;
                                ResetDelta(spear);
                            }

                            Vector3 toOwnerDir = (this.Owner.Center.transform.position - spear.transform.position).normalized;
                            float degrees = Mathf.Atan2(toOwnerDir.y, toOwnerDir.x) * Mathf.Rad2Deg;
                            spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, Quaternion.Euler(0, 0, degrees - 90f), Time.deltaTime * ROTATION_SPEED);
                        }
                        else
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
                    }
                    break;

                case (int)SpearDirection.Right:
                    {
                        if (spear._readyToBack)
                        {
                            spear._delta += Time.deltaTime;
                            spear._percent = spear._delta / DESIRED_TIME_RETURN_TO_OWNER;
                            spear.transform.position = Vector3.Lerp(spear._lockedPos, RIGHT_IDLE_BOTTOM, this.Curve(CurveType.Back).Evaluate(spear._percent));
                            if (spear._percent > 1f)
                            {
                                spear._idleDirFlag = false;
                                spear._readyToBack = false;
                                ResetDelta(spear);
                            }

                            Vector3 toOwnerDir = (this.Owner.Center.transform.position - spear.transform.position).normalized;
                            float degrees = Mathf.Atan2(toOwnerDir.y, toOwnerDir.x) * Mathf.Rad2Deg;
                            spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, Quaternion.Euler(0, 0, degrees - 90f), Time.deltaTime * ROTATION_SPEED);
                        }
                        else
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

        private void DoForceRotateToTarget(Spear spear)
        {
            // TODO : FIND OTHER CLOSEST TARGET // 
            spear._delta += Time.deltaTime;
            spear._percent = spear._delta / DESIRED_TIME_FORCE_ROTATE_TO_TARGET;
            spear.transform.rotation = Quaternion.Slerp(spear._lerpStartRot, spear._lerpEndRot, this.Curve(CurveType.Rotation).Evaluate(spear._percent));
            if (spear._percent > 1f)
            {
                spear._lockTargetToRot = true;
                ResetDelta(spear);
            }
        }

        private void SetStab(Spear spear)
        {
            spear._lerpStartPos = spear._head.transform.position; // HEAD로 해야되는거 확인함
            spear._lerpEndPos = spear._target.Center.position;

            spear._savingLerpStartPot = spear._head.transform.position;
            spear._distanceToTarget = (spear._target.Center.position - spear._head.transform.position).magnitude;
            if (spear._trail != null)
                spear._trail.enabled = true;

            spear.RigidBody.simulated = true;
            spear.HitCollider.enabled = true;
        }

        private bool Rotate(Spear spear, float desiredTime, bool isOnLockTarget = false)
        {
            spear._delta += Time.deltaTime;
            spear._percent = spear._delta / desiredTime;
            spear.transform.rotation = Quaternion.Slerp(spear._lerpStartRot, spear._lerpEndRot, this.Curve(CurveType.Rotation).Evaluate(spear._percent));
            if (spear._percent > 1f)
            {
                spear._lockTargetToRot = isOnLockTarget;
                ResetDelta(spear);
                return true;
            }

            return false;
        }

        private void RotateToTarget(Spear spear)
        {
            // +++ USE SPEAR._HEAD +++
            Vector3 toTargetDir = (spear._target.Center.position - spear._head.transform.position).normalized;
            float degrees = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
            float toTargetDist = (spear._target.Center.position - spear._head.transform.position).sqrMagnitude;
            if (toTargetDist > 1.5f)
                spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, Quaternion.Euler(0, 0, degrees - 90f), Time.deltaTime * ROTATION_SPEED);
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
            cc.GetComponent<MonsterController>().Stop();
        }
#endregion
    }
}

// -------------------------------------------------------------------------------------
// -------------------------------------------------------------------------------------
//          private bool IsReadyToStab(Spear spear)
// {
//     // percent에 맞춘 회전한 이후에도 실시간으로 타겟을 향한 회전을 지속해야하므로
//     // updateRotTarget을 실시간으로 받아 놓는다.
//     Vector3 toTargetDir = (spear._target.Center.position - spear.transform.position).normalized;
//     float degrees = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
//     // 이미 방향전환 자체는 완료가 된 상태이다. 하지만 발사되기 전 까진 Rot Target을 향해 방향을 틀어야한다.
//     Quaternion updateRotTarget = Quaternion.Euler(0, 0, degrees - 90f);

//     // 만약에 타겟이 멀어졌다가 다시 가까워지면 걍 한번에 휙 돌아버림.
//     // RotDelta를 리셋하지 않았기 때문. 그래서 이걸 나중에 처리해줘야하고
//     // 또한, 나중에 몬스터가 InValid 상태가 아닐때도 처리해줘야함
//     spear._delta += Time.deltaTime;
//     float percent = spear._delta / DESIRED_TIME_FORCE_ROTATE_TO_TARGET;
//     if (percent < 1f)
//         spear.transform.rotation = Quaternion.Slerp(spear._startStabRot, spear._endStabRot, this._stabCurve.Evaluate(percent));
//     if (percent > 1f)
//     {
//         spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, updateRotTarget, Time.deltaTime * ROTATION_SPEED);
//         float distToTarget = (spear._target.Center.position - spear.transform.position).sqrMagnitude;
//         // if (distToTarget < TARGET_IN_RANGE * TARGET_IN_RANGE)
//         // {
//         //     spear.HitCollider.enabled = true;
//         //     this.ResetDelta(spear);
//         //     return true;
//         // }
//     }

//     return false;
// }

// private IEnumerator CoIdle(GameObject go, int dir)
//         {
//             while (true)
//             {
//                 // LEFT SPEAR
//                 if (dir == LEFT)
//                 {
//                     // MOVEMENT
//                     _deltasForIdle[LEFT] += Time.deltaTime;
//                     float percent = _deltasForIdle[LEFT] / DESIRED_IDLE_DIR_SWITCH_TIME;

//                     if (_idleDirTargetSwitchers[LEFT] == false) // BOTTOM -> TOP
//                     {
//                         _goSpears[LEFT].transform.position = Vector3.Lerp(LEFT_IDLE_BOTTOM, LEFT_IDLE_TOP, _idleCurve.Evaluate(percent));
//                         if (percent > 1f)
//                         {
//                             _deltasForIdle[LEFT] = 0f;
//                             _idleDirTargetSwitchers[LEFT] = !_idleDirTargetSwitchers[LEFT];
//                         }
//                     }
//                     else // TOP -> BOTTOM
//                     {
//                         _goSpears[LEFT].transform.position = Vector3.Lerp(LEFT_IDLE_TOP, LEFT_IDLE_BOTTOM, _idleCurve.Evaluate(percent));
//                         if (percent > 1f)
//                         {
//                             _deltasForIdle[LEFT] = 0f;
//                             _idleDirTargetSwitchers[LEFT] = !_idleDirTargetSwitchers[LEFT];
//                         }
//                     }

//                     // ROTATE
//                     float degrees = Mathf.Atan2(this.Owner.ShootDir.y, this.Owner.ShootDir.x) * Mathf.Rad2Deg;
//                     _goSpears[LEFT].transform.rotation = Quaternion.Slerp(_goSpears[LEFT].transform.rotation, Quaternion.Euler(0, 0, degrees - 90f), Time.deltaTime * ROTATION_SPEED);
//                 }
//                 // RIGHT SPEAR
//                 else
//                 {
//                     // MOVEMENT
//                     _deltasForIdle[RIGHT] += Time.deltaTime;
//                     float percent = _deltasForIdle[RIGHT] / DESIRED_IDLE_DIR_SWITCH_TIME;

//                     if (_idleDirTargetSwitchers[RIGHT] == false) // BOTTOM -> TOP
//                     {
//                         _goSpears[RIGHT].transform.position = Vector3.Lerp(RIGHT_IDLE_BOTTOM, RIGHT_IDLE_TOP, _idleCurve.Evaluate(percent));
//                         if (percent > 1f)
//                         {
//                             _deltasForIdle[RIGHT] = 0f;
//                             _idleDirTargetSwitchers[RIGHT] = !_idleDirTargetSwitchers[RIGHT];
//                         }
//                     }
//                     else // TOP -> BOTTOM
//                     {
//                         _goSpears[RIGHT].transform.position = Vector3.Lerp(RIGHT_IDLE_TOP, RIGHT_IDLE_BOTTOM, _idleCurve.Evaluate(percent));
//                         if (percent > 1f)
//                         {
//                             _deltasForIdle[RIGHT] = 0f;
//                             _idleDirTargetSwitchers[RIGHT] = !_idleDirTargetSwitchers[RIGHT];
//                         }
//                     }

//                     // ROTATE
//                     float degrees = Mathf.Atan2(this.Owner.ShootDir.y, this.Owner.ShootDir.x) * Mathf.Rad2Deg;
//                     _goSpears[RIGHT].transform.rotation = Quaternion.Slerp(_goSpears[RIGHT].transform.rotation, Quaternion.Euler(0, 0, degrees - 90f), Time.deltaTime * ROTATION_SPEED);
//                 }

//                 yield return null;
//             }
//         }


// namespace STELLAREST_2D
// {
//     public class Spear : RepeatSkill
//     {
//         public enum Direction { Right, Left, Max }
//         private Collider2D _collider = null;
//         private SpriteTrail.SpriteTrail _spriteTrail = null;

//         private Spear[] _spears = new Spear[(int)Direction.Max];
//         private GameObject[] _targets = new GameObject[(int)Direction.Max];
//         private Vector2[] _spearStartStabPos = new Vector2[(int)Direction.Max];
//         private Vector2[] _targetsLastPos = new Vector2[(int)Direction.Max]; // 이 변수는 필요없어보임
//         private Vector2[] _targetExtendedLastPos = new Vector2[(int)Direction.Max];
//         private Vector2[] _spearEndStabPos = new Vector2[(int)Direction.Max];
//         private Quaternion[] _targetsRot = new Quaternion[(int)Direction.Max];
//         private float[] _deltas = new float[(int)Direction.Max];
//         private readonly float RotSpeed = 20f;

//         private Vector2 DefaultRightPosition => Owner.transform.position + new Vector3(4.5f, 0, 0);
//         private Vector2 DefaultLeftPosition => Owner.transform.position + new Vector3(-4.5f, 0, 0);

//         private int RIGHT => (int)Direction.Right;
//         private Vector2 RightSpearPos => _spears[RIGHT].transform.position;
//         private Vector2 RightTargetPos => _targets[RIGHT].transform.position;

//         private int LEFT => (int)Direction.Left;
//         private Vector2 LeftSpearPos => _spears[LEFT].transform.position;
//         private Vector2 LeftTargetPos => _targets[LEFT].transform.position;

//         private bool IsRightSpear(Spear spear) => _spears[RIGHT] == spear;

//         [SerializeField]
//         private AnimationCurve Curve;

//         public override void SetSkillInfo(CreatureController owner, int templateID)
//         {
//             base.SetSkillInfo(owner, templateID);
//             _collider = GetComponent<Collider2D>();
//             _collider.enabled = false;

//             Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
//         }

//         private void InitSpear()
//         {
//             for (int i = 0; i < _spears.Length; ++i)
//                 _spears[i] = null;

//             for (int i = 0; i < _targets.Length; ++i)
//                 _targets[i] = null;

//             for (int i = 0; i < _spearStartStabPos.Length; ++i)
//                 _spearStartStabPos[i] = Vector2.zero;

//             for (int i = 0; i < _targetsLastPos.Length; ++i)
//                 _targetsLastPos[i] = Vector2.zero;

//             for (int i = 0; i < _targetExtendedLastPos.Length; ++i)
//                 _targetExtendedLastPos[i] = Vector2.zero;

//             for (int i = 0; i < _spearEndStabPos.Length; ++i)
//                 _spearEndStabPos[i] = Vector2.zero;

//             for (int i = 0; i < _targetsRot.Length; ++i)
//                 _targetsRot[i] = Quaternion.identity;

//             for (int i = 0; i < _deltas.Length; ++i)
//                 _deltas[i] = 0;
//         }

//         protected override IEnumerator CoStartSkill()
//         {
//             InitSpear();

//             GameObject go = Managers.Resource.Instantiate(SkillData.PrimaryLabel, pooling: false);
//             _spears[RIGHT] = go.GetComponent<Spear>();
//             _spears[RIGHT].SetSkillInfo(Owner, SkillData.TemplateID);
//             _spears[RIGHT].gameObject.name += "_Right";
//             _spears[RIGHT].transform.localScale = Vector3.one * 1.5f;
//             _spears[RIGHT].transform.position = DefaultRightPosition;

//             go = Managers.Resource.Instantiate(SkillData.PrimaryLabel, pooling: false);
//             _spears[LEFT] = go.GetComponent<Spear>();
//             _spears[LEFT].SetSkillInfo(Owner, SkillData.TemplateID);
//             _spears[LEFT].gameObject.name += "_Left";
//             _spears[LEFT].transform.localScale = Vector3.one * 1.5f;
//             _spears[LEFT].transform.position = DefaultLeftPosition;

//             if (SkillData.InGameGrade == Define.InGameGrade.Legendary)
//             {
//                 _spears[RIGHT]._spriteTrail = _spears[RIGHT].GetComponent<SpriteTrail.SpriteTrail>();
//                 _spears[RIGHT]._spriteTrail.enabled = false;

//                 _spears[LEFT]._spriteTrail = _spears[LEFT].GetComponent<SpriteTrail.SpriteTrail>();
//                 _spears[LEFT]._spriteTrail.enabled = false;
//             }

//             DoSkillJob();
//             yield break;
//         }

//         protected override void DoSkillJob()
//         {
//             // +++++++++++++++++++++++++++++++++++++++++++++++++++++++
//             // +++++ SkillData.Duration is Scaler of toTargetDir +++++
//             // +++++++++++++++++++++++++++++++++++++++++++++++++++++++

//             // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//             // +++++ SkillData.ContinuousSpacing is WaitForSeconds between StartStab and EndStab +++++
//             // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//             StartCoroutine(DoStab(_spears[RIGHT]));
//             StartCoroutine(DoStab(_spears[LEFT]));
//         }

//         private float _totalDist = 0f;
//         private IEnumerator DoStab(Spear spear)
//         {
//             // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//             yield return null;
//             while (true)
//             {
//                 yield return new WaitUntil(() => SearchTarget(spear));
//                 if (IsRightSpear(spear))
//                 {
//                     // _targetsLastPos 위치는 한 번만 설정한다.
//                     if (_targets[RIGHT].IsValid())
//                     {
//                         _spearStartStabPos[RIGHT] = RightSpearPos;
//                         _targetsLastPos[RIGHT] = _targets[RIGHT].transform.position;

//                         Vector2 toTargetDir = (_targetsLastPos[RIGHT] - _spearStartStabPos[RIGHT]).normalized;
//                         _targetExtendedLastPos[RIGHT] = _targetsLastPos[RIGHT] + (toTargetDir * SkillData.Duration);

//                         // 이게 이동거리임
//                         // Debug.Log("DIST 1 : " + (_spearStartStabPos[RIGHT] - _targetExtendedLastPos[RIGHT]).magnitude);
//                         _totalDist = (_targetExtendedLastPos[RIGHT] - _spearStartStabPos[RIGHT]).magnitude;

//                         spear._collider.enabled = true;

//                         if (_spears[RIGHT]._spriteTrail != null)
//                             _spears[RIGHT]._spriteTrail.enabled = true;
//                     }
//                     else
//                         continue;
//                 }
//                 else
//                 {
//                     if (_targets[LEFT].IsValid())
//                     {
//                         _spearStartStabPos[LEFT] = LeftSpearPos;
//                         _targetsLastPos[LEFT] = _targets[LEFT].transform.position;

//                         Vector2 toTargetDir = (_targetsLastPos[LEFT] - _spearStartStabPos[LEFT]).normalized;
//                         _targetExtendedLastPos[LEFT] = _targetsLastPos[LEFT] + (toTargetDir * SkillData.Duration);
//                         spear._collider.enabled = true;

//                         if (_spears[LEFT]._spriteTrail != null)
//                             _spears[LEFT]._spriteTrail.enabled = true;
//                     }
//                     else
//                         continue;
//                 }

//                 yield return new WaitUntil(() => StartStab(spear));
//                 yield return new WaitForSeconds(SkillData.ContinuousSpacing);
//                 if (IsRightSpear(spear))
//                     _spearEndStabPos[RIGHT] = RightSpearPos;
//                 else
//                     _spearEndStabPos[LEFT] = LeftSpearPos;

//                 yield return new WaitUntil(() => EndStab(spear));

//                 if (SkillData.InGameGrade == Define.InGameGrade.Legendary)
//                 {
//                     // Vector2 upTargetPos = Quaternion.Euler(0, 0, 30f) * spear.transform.up;
//                     // Vector2 upTargetDir = RightSpearPos + upTargetPos.normalized * _targetExtendedLastPos[RIGHT].magnitude; // 어차피 갱신됨
//                     // //Debug.DrawLine(spear.transform.position, upTargetDir, Color.magenta, -1f);

//                     // Vector2 downTargetPos = Quaternion.Euler(0, 0, -30f) * spear.transform.up;
//                     // Vector2 downTargetDir = RightSpearPos + downTargetPos.normalized * _targetExtendedLastPos[RIGHT].magnitude;
//                     // Debug.DrawLine(spear.transform.position, downTargetDir, Color.blue, -1f);
//                     // Debug.Break();

//                     if (IsRightSpear(spear))
//                     {
//                         Vector2 upTargetPos = Quaternion.Euler(0, 0, 30f) * spear.transform.up;
//                         //Vector2 upTargetDir = RightSpearPos + (upTargetPos * _targetExtendedLastPos[RIGHT].magnitude); // 어차피 갱신됨
//                         Vector2 upTargetDir = RightSpearPos + upTargetPos.normalized * _totalDist; // 어차피 갱신됨
//                         // Debug.Log("<color=white> DIST 2 : " + (upTargetDir - RightSpearPos).magnitude + "</color>");

//                         // Vector2 upTargetDir = RightSpearPos * upTargetPos.normalized;
//                         // upTargetDir *= _targetExtendedLastPos[RIGHT].magnitude;
//                         // Debug.Log("MAG2 : " + upTargetDir.magnitude);

//                         Vector2 downTargetPos = Quaternion.Euler(0, 0, -30f) * spear.transform.up;
//                         //Vector2 downTargetDir = RightSpearPos + (downTargetPos * _targetExtendedLastPos[RIGHT].magnitude);
//                         Vector2 downTargetDir = RightSpearPos + downTargetPos.normalized * _totalDist;
//                         // Debug.Log("<color=yellow> DIST 3 : " + (downTargetDir - RightSpearPos).magnitude + "</color>");

//                         // Vector2 downTargetDir = RightSpearPos * downTargetPos.normalized;
//                         // upTargetDir *= _targetExtendedLastPos[RIGHT].magnitude;
//                         // Debug.Log("MAG3 : " + downTargetDir.magnitude);

//                         //Debug.DrawLine(spear.transform.position, downTargetDir, Color.blue, -1f);
//                         //Debug.Break();

//                         // First : Rot to upTarget
//                         Quaternion startRot = spear.transform.rotation;
//                         float angle = Mathf.Atan2(upTargetPos.y, upTargetPos.x) * Mathf.Rad2Deg;
//                         _targetsRot[RIGHT] = Quaternion.Euler(0, 0, angle - 90f);
//                         yield return new WaitUntil(() => SimpleRotToTarget(spear, startRot, _targetsRot[RIGHT]));

//                         // Clear !!
//                         // Debug.DrawLine(spear.transform.position, _targetExtendedLastPos[RIGHT], Color.blue, -1f);
//                         // Debug.DrawLine(spear.transform.position, downTargetDir, Color.green, -1f);
//                         // Debug.Break();

//                         // First : Go to target
//                         _spearStartStabPos[RIGHT] = RightSpearPos; // 이건 100% 맞음
//                         _targetExtendedLastPos[RIGHT] = upTargetDir;

//                         spear._collider.enabled = true;
//                         yield return new WaitUntil(() => StartStab(spear));

//                         // First : Return to pos
//                         // _spearEndStabPos[RIGHT] = RightSpearPos;
//                         Vector2 startPos = RightSpearPos;
//                         Vector2 endPos = _spearStartStabPos[RIGHT];
//                         yield return new WaitUntil(() => EndStab(spear, startPos, endPos));

//                         // Second : Rot to downTarget
//                         angle = Mathf.Atan2(downTargetPos.y, downTargetPos.x) * Mathf.Rad2Deg;
//                         _targetsRot[RIGHT] = Quaternion.Euler(0, 0, angle - 90f);
//                         yield return new WaitUntil(() => SimpleRotToTarget(spear, startRot, _targetsRot[RIGHT]));

//                         // Second : Go to target
//                         _spearStartStabPos[RIGHT] = RightSpearPos; // 이건 100% 맞음
//                         _targetExtendedLastPos[RIGHT] = downTargetDir;

//                         spear._collider.enabled = true;
//                         yield return new WaitUntil(() => StartStab(spear));

//                         // Second : Return to pos
//                         // _spearEndStabPos[RIGHT] = RightSpearPos;
//                         startPos = RightSpearPos;
//                         endPos = _spearStartStabPos[RIGHT];
//                         yield return new WaitUntil(() => EndStab(spear, startPos, endPos));
//                         _spears[RIGHT]._spriteTrail.enabled = false;

//                         //yield return new WaitForSeconds(5f);
//                     }
//                     else
//                     {
//                         Vector2 upTargetPos = Quaternion.Euler(0, 0, 30f) * spear.transform.up;
//                         //Vector2 upTargetDir = LeftSpearPos + upTargetPos.normalized * _targetExtendedLastPos[LEFT].magnitude; // 어차피 갱신됨
//                         Vector2 upTargetDir = LeftSpearPos + upTargetPos.normalized * _totalDist; // 어차피 갱신됨

//                         Vector2 downTargetPos = Quaternion.Euler(0, 0, -30f) * spear.transform.up;
//                         //Vector2 downTargetDir = LeftSpearPos + downTargetPos.normalized * _targetExtendedLastPos[LEFT].magnitude;
//                         Vector2 downTargetDir = LeftSpearPos + downTargetPos.normalized * _totalDist;

//                         // First : Rot to upTarget
//                         Quaternion startRot = spear.transform.rotation;
//                         float angle = Mathf.Atan2(downTargetPos.y, downTargetPos.x) * Mathf.Rad2Deg;
//                         _targetsRot[LEFT] = Quaternion.Euler(0, 0, angle - 90f);
//                         yield return new WaitUntil(() => SimpleRotToTarget(spear, startRot, _targetsRot[LEFT]));

//                         // First : Go to target
//                         _spearStartStabPos[LEFT] = LeftSpearPos; // 이건 100% 맞음
//                         _targetExtendedLastPos[LEFT] = downTargetDir;
//                         spear._collider.enabled = true;
//                         yield return new WaitUntil(() => StartStab(spear));

//                         // First : Return to pos
//                         // _spearEndStabPos[LEFT] = LeftSpearPos;
//                         Vector2 startPos = LeftSpearPos;
//                         Vector2 endPos = _spearStartStabPos[LEFT];
//                         yield return new WaitUntil(() => EndStab(spear, startPos, endPos));

//                         // Second : Rot to downTarget
//                         angle = Mathf.Atan2(upTargetPos.y, upTargetPos.x) * Mathf.Rad2Deg;
//                         _targetsRot[LEFT] = Quaternion.Euler(0, 0, angle - 90f);
//                         yield return new WaitUntil(() => SimpleRotToTarget(spear, startRot, _targetsRot[LEFT]));

//                         // Second : Go to target
//                         _spearStartStabPos[LEFT] = LeftSpearPos; // 이건 100% 맞음
//                         _targetExtendedLastPos[LEFT] = upTargetDir;
//                         spear._collider.enabled = true;
//                         yield return new WaitUntil(() => StartStab(spear));

//                         // Second : Return to pos
//                         // _spearEndStabPos[LEFT] = LeftSpearPos;
//                         startPos = LeftSpearPos;
//                         endPos = _spearStartStabPos[LEFT];
//                         yield return new WaitUntil(() => EndStab(spear, startPos, endPos));
//                         _spears[LEFT]._spriteTrail.enabled = false;
//                     }
//                 }
//             }
//         }

//         private bool SearchTarget(Spear spear)
//         {
//             if (IsRightSpear(spear))
//             {
//                 _targets[RIGHT] = Managers.Object.GetClosestTarget<MonsterController>(spear.transform.GetChild(0).gameObject, 6f);
//                 if (_targets[RIGHT] == null)
//                 {
//                     _deltas[RIGHT] = 0f;
//                     float angle = Mathf.Atan2(Managers.Game.Player.ShootDir.y, Managers.Game.Player.ShootDir.x) * Mathf.Rad2Deg;
//                     _targetsRot[RIGHT] = Quaternion.Euler(0, 0, angle - 90f);
//                     spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, _targetsRot[RIGHT], Time.deltaTime * RotSpeed);
//                 }
//                 else
//                 {
//                     Vector2 toTargetDir = (RightTargetPos - RightSpearPos).normalized;
//                     float angle = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
//                     _targetsRot[RIGHT] = Quaternion.Euler(0, 0, angle - 90f);

//                     _deltas[RIGHT] += Time.deltaTime;
//                     if (_deltas[RIGHT] > SkillData.CoolTime)
//                     {
//                         _deltas[RIGHT] = 0f;
//                         return true;
//                     }
//                 }

//                 spear.transform.position = DefaultRightPosition;
//                 spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, _targetsRot[RIGHT], Time.deltaTime * RotSpeed);
//             }
//             else
//             {
//                 _targets[LEFT] = Managers.Object.GetClosestTarget<MonsterController>(spear.transform.GetChild(0).gameObject, 6f);
//                 if (_targets[LEFT] == null)
//                 {
//                     float angle = Mathf.Atan2(Managers.Game.Player.ShootDir.y, Managers.Game.Player.ShootDir.x) * Mathf.Rad2Deg;
//                     _targetsRot[LEFT] = Quaternion.Euler(0, 0, angle - 90f);
//                 }
//                 else
//                 {
//                     Vector2 toTargetDir = (LeftTargetPos - LeftSpearPos).normalized;
//                     float angle = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
//                     _targetsRot[LEFT] = Quaternion.Euler(0, 0, angle - 90f);

//                     _deltas[LEFT] += Time.deltaTime;
//                     // 쿨타임 여기다가 넣어야 할 듯. 타겟을 조준한 상태에서 1초후 발사.
//                     if (_deltas[LEFT] > SkillData.CoolTime)
//                     {
//                         _deltas[LEFT] = 0f;
//                         return true;
//                     }
//                 }

//                 spear.transform.position = DefaultLeftPosition;
//                 spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, _targetsRot[LEFT], Time.deltaTime * RotSpeed);
//             }

//             return false;
//         }

//         private bool StartStab(Spear spear)
//         {
//             if (IsRightSpear(spear))
//             {
//                 // 거리에 따라서 조절할 수 있긴한데.. 일단 이렇게.
//                 _deltas[RIGHT] += Time.deltaTime;
//                 float percent = _deltas[RIGHT] / 0.1f;

//                 spear.transform.position = Vector2.Lerp(_spearStartStabPos[RIGHT], _targetExtendedLastPos[RIGHT], Curve.Evaluate(percent));
//                 if (percent >= 1f - Mathf.Epsilon)
//                 {
//                     spear._collider.enabled = false;
//                     _deltas[RIGHT] = 0f;
//                     return true;
//                 }
//             }
//             else
//             {
//                 _deltas[LEFT] += Time.deltaTime;
//                 float percent = _deltas[LEFT] / 0.1f;

//                 spear.transform.position = Vector2.Lerp(_spearStartStabPos[LEFT], _targetExtendedLastPos[LEFT], Curve.Evaluate(percent));
//                 if (percent >= 1f - Mathf.Epsilon)
//                 {
//                     spear._collider.enabled = false;
//                     _deltas[LEFT] = 0f;
//                     return true;
//                 }
//             }

//             return false;
//         }

//         private bool EndStab(Spear spear)
//         {
//             if (IsRightSpear(spear))
//             {
//                 // Last Movement
//                 _deltas[RIGHT] += Time.deltaTime;
//                 float percent = _deltas[RIGHT] / 0.25f;
//                 spear.transform.position = Vector2.Lerp(_spearEndStabPos[RIGHT], DefaultRightPosition, Curve.Evaluate(percent));

//                 // Last Rotation
//                 Vector2 toTargetDir = (_spearEndStabPos[RIGHT] - RightSpearPos).normalized;
//                 float angle = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
//                 _targetsRot[RIGHT] = Quaternion.Euler(0, 0, angle - 90f);
//                 spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, _targetsRot[RIGHT], Time.deltaTime * RotSpeed);

//                 if (percent >= 1f - Mathf.Epsilon)
//                 {
//                     _deltas[RIGHT] = 0f;
//                     return true;
//                 }
//             }
//             else
//             {
//                 // Last Movement
//                 _deltas[LEFT] += Time.deltaTime;
//                 float percent = _deltas[LEFT] / 0.25f;
//                 spear.transform.position = Vector2.Lerp(_spearEndStabPos[LEFT], DefaultLeftPosition, Curve.Evaluate(percent));

//                 // Last Rotation
//                 Vector2 toTargetDir = (_spearEndStabPos[LEFT] - LeftSpearPos).normalized;
//                 float angle = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
//                 _targetsRot[LEFT] = Quaternion.Euler(0, 0, angle - 90f);
//                 spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, _targetsRot[LEFT], Time.deltaTime * RotSpeed);

//                 if (percent >= 1f - Mathf.Epsilon)
//                 {
//                     _deltas[LEFT] = 0f;
//                     return true;
//                 }
//             }

//             return false;
//         }

//         private bool EndStab(Spear spear, Vector2 startPos, Vector2 endPos)
//         {
//             if (IsRightSpear(spear))
//             {
//                 _deltas[RIGHT] += Time.deltaTime;
//                 float percent = _deltas[RIGHT] / 0.25f;
//                 spear.transform.position = Vector2.Lerp(startPos, endPos, Curve.Evaluate(percent));

//                 Vector2 toTargetDir = (startPos - endPos).normalized;
//                 float angle = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
//                 _targetsRot[RIGHT] = Quaternion.Euler(0, 0, angle - 90f);
//                 spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, _targetsRot[RIGHT], Time.deltaTime * RotSpeed);

//                 if (percent >= 1f - Mathf.Epsilon)
//                 {
//                     _deltas[RIGHT] = 0f;
//                     return true;
//                 }
//             }
//             else
//             {
//                 _deltas[LEFT] += Time.deltaTime;
//                 float percent = _deltas[LEFT] / 0.25f;
//                 spear.transform.position = Vector2.Lerp(startPos, endPos, Curve.Evaluate(percent));

//                 Vector2 toTargetDir = (startPos - endPos).normalized;
//                 float angle = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
//                 _targetsRot[LEFT] = Quaternion.Euler(0, 0, angle - 90f);
//                 spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, _targetsRot[LEFT], Time.deltaTime * RotSpeed);

//                 if (percent >= 1f - Mathf.Epsilon)
//                 {
//                     _deltas[RIGHT] = 0f;
//                     return true;
//                 }
//             }

//             return false;
//         }

//         private bool SimpleRotToTarget(Spear spear, Quaternion startRot, Quaternion targetRot)
//         {
//             if (IsRightSpear(spear))
//             {
//                 _deltas[RIGHT] += Time.deltaTime * RotSpeed;
//                 float percent = _deltas[RIGHT];
//                 spear.transform.rotation = Quaternion.Slerp(startRot, targetRot, percent);
//                 if (percent >= 1f - Mathf.Epsilon)
//                 {
//                     _deltas[RIGHT] = 0f;
//                     return true;
//                 }
//             }
//             else
//             {
//                 _deltas[LEFT] += Time.deltaTime * RotSpeed;
//                 float percent = _deltas[LEFT];
//                 spear.transform.rotation = Quaternion.Slerp(startRot, targetRot, percent);
//                 if (percent >= 1f - Mathf.Epsilon)
//                 {
//                     _deltas[LEFT] = 0f;
//                     return true;
//                 }
//             }

//             return false;
//         }

//         private void OnTriggerEnter2D(Collider2D other)
//         {
//             MonsterController mc = other.GetComponent<MonsterController>();
//             if (mc.IsValid() == false)
//                 return;

//             if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.MonsterBody, other.gameObject.layer))
//             {
//                 mc.OnDamaged(Owner, this);
//             }
//         }

//         private void OnDisable()
//         {
//             if (_spears[(int)Direction.Right] != null)
//                 Managers.Resource.Destroy(_spears[(int)Direction.Right].gameObject);

//             if (_spears[(int)Direction.Left] != null)
//                 Managers.Resource.Destroy(_spears[(int)Direction.Left].gameObject);
//         }

//         public override void OnPreSpawned()
//         {
//             base.OnPreSpawned();
//             GetComponent<SpriteRenderer>().enabled = false;
//             GetComponent<Collider2D>().enabled = false;
//             if (GetComponent<SpriteTrail.SpriteTrail>() != null)
//                 GetComponent<SpriteTrail.SpriteTrail>().enabled = false;
//         }
//     }
// }
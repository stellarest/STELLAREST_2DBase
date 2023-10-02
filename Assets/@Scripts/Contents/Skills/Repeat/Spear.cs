using System.Collections;
using System.Linq;
using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // +++ Spear Skill Origin is actually like a commander for controlling spears[LEFT, RIGHT] +++
    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
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

        #region Fields && CONSTANT
        private Spear[] _spears = null;
        private int _dir = -1;
        private SpriteTrail.SpriteTrail _trail = null;

        // Ease Out
        // - Anim Loop
        // - Stab
        [SerializeField] private AnimationCurve _curveEaseOut = null;

        // Linear
        // - Rot
        // - Back
        [SerializeField] private AnimationCurve _curveLinear = null;
        private GameObject _head = null;

        private CreatureController _target = null;
        private Vector3 _lockedPos = Vector3.zero;
        private Vector3 _lerpStartPos = Vector3.zero;
        private Vector3 _lerpEndPos = Vector3.zero;

// // ULTIMATE TEST///////////////////////////////////////////////////////////////////////////
//         private Vector3 _lerpEndPos2 = Vector3.zero;
//         private Vector3 _lerpEndPos3 = Vector3.zero;
// ///////////////////////////////////////////////////////////////////////////////////////////

        private Quaternion _lerpStartRot = Quaternion.identity;
        private Quaternion _lerpEndRot = Quaternion.identity;

        private bool _idleDirFlag = false;
        private bool _lockTarget = false;
        private bool IsLockTarget(Spear spear) => spear._lockTarget;
        private void LockTarget(Spear spear)
        {
            // SET TARGET
            spear._lockTarget = true;

            // _lockPos 잡혔을때만,,
            spear._lockedPos = spear.transform.position; // 엉,,,?
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
            //spear._readyToBack = false; // nono // Init에서는 false 초기화를 해야할듯?. ㄹㅇ 침착함하나로 해결함.
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
        }
        #endregion

        #region Overrides
        // public override bool IsLast 
        // { 
        //     get => base.IsLast; 
        //     set => base.IsLast = value;
        // }

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

            //base.Deactivate(isPoolingClear);
            for (int i = 0; i < (int)SpearDirection.Max; ++i)
            {
                if (_spears[i].IsValid())
                {
                    Utils.Log($"{_spears[i].gameObject.name} is destroyed.");
                    Managers.Resource.Destroy(_spears[i].gameObject);
                }
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
                    yield return new WaitUntil(() => Stab(LEFT_SPEAR));
                    yield return new WaitUntil(() => Wait(LEFT_SPEAR));
                    yield return new WaitUntil(() => Back(LEFT_SPEAR));
                    yield return new WaitUntil(() => CanContinue(RIGHT_SPEAR));
                    yield return null;
                }
            }
            else
            {
                while (true)
                {
                    yield return null;
                    yield return new WaitUntil(() => Init(RIGHT_SPEAR));
                    yield return new WaitUntil(() => Idle(RIGHT_SPEAR));
                    yield return new WaitUntil(() => Stab(RIGHT_SPEAR));
                    yield return new WaitUntil(() => Wait(RIGHT_SPEAR));
                    yield return new WaitUntil(() => Back(RIGHT_SPEAR));
                    yield return new WaitUntil(() => CanContinue(RIGHT_SPEAR));
                    yield return null;
                }
            }
        }

        #region TEMPORARY CONSTANT OPTIONS
        private const float X_AXIS_INTERVAL_POSITION_FROM_OWNER = 4.5f;
        private const float Y_AXIS_INTERVAL_POSITION_FROM_OWNER = 0.25f;
        private const float IDLE_TOP_DOWN_POSITION_INTENSITY = 0.5f;
        private const float ROTATION_SPEED = 30f;
        private const float DESIRED_TIME_RETURN_TO_OWNER = DESIRED_STAB_REACH_TO_TARGET_TIME * 2;
        private const float DESIRED_TIME_IDLE_DIR_SWITCH = 2f;
        private const float DESIRED_TIME_FORCE_ROTATE_TO_TARGET = 0.15f;
        private const float SEARCH_TARGET_RANGE = 6f; // sqrMag : 25f
        private const float TARGET_IN_STAB_RANGE = 3f; // 9f
        private const float DESIRED_STAB_REACH_TO_TARGET_TIME = 0.15f;
        #endregion

        private bool Init(Spear spear)
        {
            ResetIdle(spear); // TEMP
            spear._readyToBack = false; // INIT에서는 해야할 것 같음.
            return true;
        }

        private bool Idle(Spear spear)
        {
            if (IsLockTarget(spear) == false)
            {
                spear._target = Utils.GetClosestCreatureTargetFromAndRange<CreatureController>(spear._head, spear.Owner, SEARCH_TARGET_RANGE);
                if (spear._target == null)
                    DoIdleMovement(spear);
                else if (spear._target.IsValid())
                {
                    LockTarget(spear);
                    ResetDelta(spear);
                }
            }
            else
            {
                if (spear._target.IsValid() == false)
                {
                    ResetIdle(spear);
                    return false;
                }

                // +++ FORCE ROTATE TO TARGET +++
                if (IsLockTargetToRot(spear) == false)
                    DoForceRotateToTarget(spear);

                else if (IsLockTargetToRot(spear))
                {
                    // +++ NEXT STEP +++
                    if (IsTargetInRange(spear, TARGET_IN_STAB_RANGE))
                    {
                        //spear._lerpStartPos = spear.transform.position;
                        // _head로 해야 잘 나올텐데??? _head로 해야함
                        // spear._lerpStartPos = spear._head.transform.position;
                        // spear._lerpEndPos = spear._target.Center.position;
                        // ============= TEST : 확인 완료 =============
                        // 방향 확인 완료
                        // Vector3 testDir = Quaternion.Euler(0, 0, 30) * spear._head.transform.up;
                        // spear._lerpEndPos = testDir.normalized * 5f;

                        // Debug.DrawRay(spear._lerpStartPos, testDir.normalized * 5f, Color.magenta, -1f);
                        // Utils.LogBreak("BREAK");

                        // Vector3 testDir2 = Quaternion.Euler(0, 0, -30) * spear._head.transform.up;
                        // Debug.DrawRay(spear._lerpStartPos, testDir2.normalized * 5f, Color.blue, -1f);
                        // Utils.LogBreak("BREAK2");

                        // spear.RigidBody.simulated = true;
                        // spear.HitCollider.enabled = true;
                        // // ============= ULTIMATE TEST // =============
                        // if (spear.Data.Grade == spear.Data.MaxGrade)
                        // {
                        //     if (spear._trail != null)
                        //         spear._trail.enabled = true;
                        // }

                        DoStab(spear);
                        ResetDelta(spear);
                        return true;
                    }
                    else if (IsTargetInRange(spear, SEARCH_TARGET_RANGE))
                    {
                        DoRotateToTarget(spear);
                    }

                    else if (IsTargetInRange(spear, SEARCH_TARGET_RANGE) == false)
                    {
                        Debug.Log("<color=yellow> === Out Of Range === </color>");
                        ResetIdle(spear);
                    }
                }
            }

            return false;
        }

        private bool Stab(Spear spear)
        {
            spear._delta += Time.deltaTime;
            float percent = spear._delta / DESIRED_STAB_REACH_TO_TARGET_TIME;
            spear.transform.position = Vector3.Lerp(spear._lerpStartPos, spear._lerpEndPos, _curveEaseOut.Evaluate(percent));
            if (percent > 1f)
            {
                spear._lerpStartPos = spear.transform.position;
                //spear._lerpEndPos = this.Owner.Center.transform.position; // nono
                ResetDelta(spear);

                spear.RigidBody.simulated = false;
                spear.HitCollider.enabled = false;
                return true;
            }

            return false;
        }

        private bool Wait(Spear spear) // WAIT FOR COOLTIME
        {
            spear._delta += Time.deltaTime;
            float percent = spear._delta / spear.Data.CoolTime;
            if (percent > 1f)
            {
                if (spear.Data.Grade == spear.Data.MaxGrade)
                {
                    if (spear._trail != null)
                        spear._trail.enabled = false;
                }

                ResetDelta(spear);
                return true;
            }

            return false;
        }

        private bool Back(Spear spear)
        {
            spear._delta += Time.deltaTime;
            float percent = spear._delta / DESIRED_TIME_RETURN_TO_OWNER;

            if (spear._dir == LEFT)
                spear.transform.position = Vector3.Lerp(spear._lerpStartPos, LEFT_IDLE_BOTTOM, _curveLinear.Evaluate(percent));
            else
                spear.transform.position = Vector3.Lerp(spear._lerpStartPos, RIGHT_IDLE_BOTTOM, _curveLinear.Evaluate(percent));

            Vector3 toOwnerDir = (this.Owner.Center.transform.position - spear.transform.position).normalized;
            float degrees = Mathf.Atan2(toOwnerDir.y, toOwnerDir.x) * Mathf.Rad2Deg;
            // TODO : 아래 개선?
            spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, Quaternion.Euler(0, 0, degrees - 90f), Time.deltaTime * ROTATION_SPEED);
            if (percent > 1f)
            {
                ResetDelta(spear);
                return true;
            }

            return false;
        }

        private bool CanContinue(Spear spear)
        {
            // TODO : CHECK OWNER IS DEAD OR IN CC STATE OR SOMETHING ELSE,,
            Utils.Log(nameof(CanContinue));
            return true;
        }

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

        private void DoIdleMovement(Spear spear)
        {
            switch (spear._dir)
            {
                case (int)SpearDirection.Left:
                    {
                        if (spear._readyToBack)
                        {
                            spear._delta += Time.deltaTime;
                            float percent = spear._delta / DESIRED_TIME_RETURN_TO_OWNER;
                            spear.transform.position = Vector3.Lerp(spear._lockedPos, LEFT_IDLE_BOTTOM, _curveLinear.Evaluate(percent));
                            if (percent > 1f)
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
                                {
                                    spear._idleDirFlag = (!spear._idleDirFlag);
                                }
                            }
                            else
                            {
                                if (LerpIdleAnimationLoop(spear, LEFT_IDLE_TOP, LEFT_IDLE_BOTTOM))
                                {
                                    spear._idleDirFlag = (!spear._idleDirFlag);
                                }
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
                            float percent = spear._delta / DESIRED_TIME_RETURN_TO_OWNER;
                            spear.transform.position = Vector3.Lerp(spear._lockedPos, RIGHT_IDLE_BOTTOM, _curveLinear.Evaluate(percent));
                            if (percent > 1f)
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
                                {
                                    spear._idleDirFlag = (!spear._idleDirFlag);
                                }
                            }
                            else
                            {
                                if (LerpIdleAnimationLoop(spear, RIGHT_IDLE_TOP, RIGHT_IDLE_BOTTOM))
                                {
                                    spear._idleDirFlag = (!spear._idleDirFlag);
                                }
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
            float percent = spear._delta / DESIRED_TIME_IDLE_DIR_SWITCH;
            spear.transform.position = Vector3.Lerp(startPos, endPos, this._curveEaseOut.Evaluate(percent));
            if (percent > 1f)
            {
                this.ResetDelta(spear);
                return true;
            }

            return false;
        }

        private void DoForceRotateToTarget(Spear spear)
        {
            spear._delta += Time.deltaTime;
            float percent = spear._delta / DESIRED_TIME_FORCE_ROTATE_TO_TARGET;
            spear.transform.rotation = Quaternion.Slerp(spear._lerpStartRot, spear._lerpEndRot, _curveLinear.Evaluate(percent));
            if (percent > 1f)
            {
                spear._lockTargetToRot = true;
                ResetDelta(spear);
            }
        }

        private void DoStab(Spear spear)
        {
            spear._lerpStartPos = spear._head.transform.position;
            spear._lerpEndPos = spear._target.Center.position;

            spear.RigidBody.simulated = true;
            spear.HitCollider.enabled = true;
        }

        private void DoRotateToTarget(Spear spear)
        {
            //Vector3 toTargetDir = (spear._target.Center.position - spear.transform.position).normalized;
            Vector3 toTargetDir = (spear._target.Center.position - spear._head.transform.position).normalized;
            float degrees = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
            spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, Quaternion.Euler(0, 0, degrees - 90f), Time.deltaTime * ROTATION_SPEED);
        }

        // -------------------------------------------------------------------------------------
        // -------------------------------------------------------------------------------------
        private void CreateSpears(int dir)
        {
            GameObject go = Managers.Resource.Instantiate(key: this.Data.PrimaryLabel, parent: null, pooling: false);
            if (dir == (int)SpearDirection.Left)
            {
                go.name = $"{this.Data.Name}_Left";
                go.transform.position = LEFT_IDLE_BOTTOM; // 무조건 BOTTOM부터. (_idleDirFlag == false)
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
            }

            if (this.Owner?.IsPlayer() == true)
                Managers.Collision.InitCollisionLayer(spear.gameObject, Define.CollisionLayers.PlayerAttack);
            else
                Managers.Collision.InitCollisionLayer(spear.gameObject, Define.CollisionLayers.MonsterAttack);

            _spears[dir] = spear;
            _spears[dir].SetSortingOrder();
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            cc.OnDamaged(attacker: this.Owner, from: this);
        }
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
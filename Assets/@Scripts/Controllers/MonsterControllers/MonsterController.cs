using System;
using System.Collections;
using STELLAREST_2D.UI;
using UnityEngine;
using UnityEngine.Rendering;

using static STELLAREST_2D.Define;
using CrowdControlType = STELLAREST_2D.Define.FixedValue.TemplateID.CrowdControl;

namespace STELLAREST_2D
{
    public class MonsterController : CreatureController, IEquatable<MonsterController> //, IHitStatus
    {
        public Define.MonsterType MonsterType { get; set; } = Define.MonsterType.None;
        public MonsterAnimationController MonsterAnimController { get; private set; } = null;
        public override Vector3 ShootDir => (MainTarget != null) ?
                                (MainTarget.Center.position - Center.position).normalized : this.transform.right;

        public override float SpeedModifier
        {
            get => base.SpeedModifier;
            set
            {
                base.SpeedModifier = value;
                //MonsterAnimController.SetAnimationSpeed(base.SpeedModifier);
            }
        }

        public override void ResetSpeedModifier()
        {
            base.ResetSpeedModifier();
            //MonsterAnimController.SetAnimationSpeed(base.SpeedModifier);
        }

        [SerializeField] private Sprite _defaultHead = null;
        public Sprite DefaultHead => _defaultHead;

        [SerializeField] private Sprite _angryHead = null;
        public Sprite AngryHead => _angryHead;

        [SerializeField] private Sprite _deadHead = null;
        public Sprite DeadHead => _deadHead;

        protected Coroutine _coMoveToRandomPoint = null;

        public override void Init(int templateID)
        {
            if (this.IsFirstPooling)
            {
                base.Init(templateID);
                LateInit();
                MonsterAnimController = CreatureAnimController as MonsterAnimationController;
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.MonsterBody);
                
                Managers.Game.OnPlayerIsDead += OnPlayerIsDeadHandler;
                Utils.Log("Add Event : OnPlayerIsDead += OnPlayerIsDeadHandler");

                Managers.Game.OnVFXEnvTarget += OnVFXEnvTargetHandler;
                Utils.Log("Add Event : OnVFXEnvTarget += OnVFXEnvTargetHandler");

                Managers.Game.OnStopAction += OnStopActionHandler;
                Utils.Log("Add Event : OnStopAction += OnStopActionHandler");

                this.IsFirstPooling = false;
            }

            EnterInGame(templateID);
        }

        protected override void InitChild(int templateID)
        {
            base.InitChild(templateID);
            Center = Utils.FindChild<Transform>(AnimTransform.gameObject, "Body", true);
        }

        protected override void EnterInGame(int templateID)
        {
            this.RendererController.EnterInGame();
            
            InitCreatureStat(templateID);
            ClearCrowdControlStates();
            ClearHitFrom();

            IsCompleteReadyToAction = false;
            ReadyToAction(false);

            // MainTarget은 중간에 바뀔수도 있긴함 (ex) Forest Guardian : Black Panther
            if (Managers.Game.Player != null)
                this.MainTarget = Managers.Game.Player;
        }

        private bool CanEnterRunState()
        {
            if (this.IsCompleteReadyToAction == false)
                return false;

            if (IsIdleState || IsSkillState || IsDeadState || this[CrowdControlType.Stun])
                return false;

            return true;
        }

        private void FixedUpdate()
        {
            if (this.CanEnterRunState() == false)
                return;

            // if (this.MainTarget != null)
            //     MoveToTarget(MainTarget, this.Stat.CollectRange * this.Stat.CollectRange);
        }

        public void StartMovementToRandomPoint()
        {
            this.SkillBook.DeactivateAll();
            if (this.IsValid() && _coMoveToRandomPoint == null)
                _coMoveToRandomPoint = StartCoroutine(CoMoveToRandomPoint());
        }

        private readonly float MIN_MOVE_TO_RANDOM_POINT_DELAY = 3f;
        private readonly float MAX_MOVE_TO_RANDOM_POINT_DELAY = 5f;

        private readonly float MIN_MOVE_TO_RANDOM_POINT_DISTANCE = 5f;
        private readonly float MAX_MOVE_TO_RANDOM_POINT_DISTANCE = 10f;

        protected IEnumerator CoMoveToRandomPoint()
        {
            while (true)
            {
                yield return null;
                this.CreatureState = Define.CreatureState.Idle;
                float waitDelay = UnityEngine.Random.Range(MIN_MOVE_TO_RANDOM_POINT_DELAY, MAX_MOVE_TO_RANDOM_POINT_DELAY);
                Vector3 randomPoint = Utils.GetRandomPosition(this.Center.transform.position,
                        MIN_MOVE_TO_RANDOM_POINT_DISTANCE, MAX_MOVE_TO_RANDOM_POINT_DISTANCE);

                yield return new WaitForSeconds(waitDelay);
                this.CreatureState = Define.CreatureState.Run;
                yield return new WaitUntil(() => MoveToTarget(randomPoint));
            }
        }

        protected bool MoveToTarget(CreatureController target, float minDistance = 1f)
        {
            Vector3 toTargetDir = (target.Center.transform.position - this.Center.transform.position);
            if (this.LockFlip == false)
                Flip(toTargetDir.x > 0 ? -1 : 1);

            Vector3 toTargetMovement = this.transform.position + (toTargetDir.normalized * (Stat.MovementSpeed * this.SpeedModifier) * Time.deltaTime);
            this.RigidBody.MovePosition(toTargetMovement);
            if (Utils.IsArriveToTarget(this.Center, this.MainTarget.transform, minDistance))
            {
                if (this[CrowdControlType.Slience] == false)
                    this.CreatureState = Define.CreatureState.Skill;
                else
                    this.CreatureState = Define.CreatureState.Idle;

                return true;
            }

            return false;
        }

        protected bool MoveToTarget(Vector3 targetPoint, float minDistance = 1f)
        {
            Vector3 toTargetPointDir = (targetPoint - this.Center.position);
            if (this.LockFlip == false)
                Flip(toTargetPointDir.x > 0 ? -1 : 1);

            Vector3 toTargetPointMovement = this.transform.position + (toTargetPointDir.normalized * Stat.MovementSpeed * Time.deltaTime);
            if (Utils.IsArriveToTarget(this.Center, targetPoint, minDistance))
                return true;
            else
            {
                this.transform.position = toTargetPointMovement;
                return false;
            }
        }

        protected override void UpdateAnimation()
        {
            switch (CreatureState)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;

                case CreatureState.Run:
                    StopIdleTick();
                    UpdateRun();
                    break;

                case CreatureState.Skill:
                    StopIdleTick();
                    UpdateSkill();
                    break;

                case CreatureState.Dead:
                    StopIdleTick();
                    OnDead();
                    break;
            }
        }

        protected Coroutine _coIdleTick = null;
        //private IEnumerator CoIdleTick() { yield return null; }
        protected void StopIdleTick()
        {
            if (_coIdleTick != null)
                StopCoroutine(_coIdleTick);

            _coIdleTick = null;
        }

        protected virtual IEnumerator CoIdleTick()
        {
            MonsterAnimController.Idle();
            //RendererController.MonsterHead.sprite = this.DefaultHead;
            Vector3 toTargetDir = Vector3.zero;
            while (true)
            {
                if (this.MainTarget != null)
                {
                    toTargetDir = (MainTarget.Center.transform.position - this.Center.transform.position);
                    if (this.LockFlip == false)
                        Flip(toTargetDir.x > 0 ? -1 : 1);
                }

                if (this[CrowdControlType.Slience])
                {
                    if (toTargetDir.sqrMagnitude > this.Stat.CollectRange * this.Stat.CollectRange)
                        this.CreatureState = Define.CreatureState.Run;
                }

                yield return null;
            }
        }


        public bool LockFlip { get; set; } = false;

        protected override void SetSortingOrder()
                => GetComponent<SortingGroup>().sortingOrder = (int)Define.SortingOrder.Monster;

        protected void Flip(float flipX)
                => transform.localScale = new Vector2(_baseRootLocalScale.x * flipX, _baseRootLocalScale.y);

        public void OnPlayerIsDeadHandler()
        {
            Utils.Log("Called::OnPlayerIsDeadHandler");
            MainTarget = null;
            StartMovementToRandomPoint();
        }

        public void OnStopActionHandler(bool isOnStop)
        {
            if (isOnStop)
            {
                MainTarget = null;
                this.CreatureState = Define.CreatureState.Idle;
            }
            else
            {
                MainTarget = Managers.Game.Player;
                this.CreatureState = Define.CreatureState.Run;
            }
        }

        public void OnVFXEnvTargetHandler(VFXEnvType vfxEnvType) => Managers.VFX.Environment(vfxEnvType, this);

        public override void OnDamaged(CreatureController attacker, SkillBase from)
        {
            // 한대 맞았을 때 바로 움직이게 했던 것임
            // if (this.OnStartAction == false)
            // {
            //     StopCoroutine(_coIdleToAction);
            //     _coIdleToAction = null;
            //     StartAction();
            // }

            base.OnDamaged(attacker, from);
        }

        // Move To CreatureState
        // public void SetDeadHead() => RendererController.MonsterHead.sprite = this.DeadHead;
        // public void SetDefaultHead() => RendererController.MonsterHead.sprite = this.DefaultHead;

        protected override void OnDead()
        {
            base.OnDead();
            //Managers.VFX.Environment(VFXEnv.Skull, this);
            //this.RendererController.MonsterHead.sprite = this.DeadHead;
            
            MonsterAnimController.Dead();
            this.RendererController.OnFaceDeadHandler();

            GemController spawnedGem = Managers.Object.Spawn<GemController>(this.Center.position,
                                            spawnObjectType: Define.ObjectType.Gem, isPooling: true);

            //StartCoroutine(CoDespawn());
            StartCoroutine(Managers.VFX.CoMatFadeOut(this, 
                        startCallback: () => Managers.VFX.Environment(VFXEnvType.Skull, this), 
                        endCallback: () => Managers.Object.Despawn(this)));
        }

        // protected virtual IEnumerator CoDespawn()
        // {
        //     //Managers.VFX.Material(Define.MaterialType.FadeOut, this);
        //     // StartCoroutine(Managers.VFX.CoFadeOut(this, 
        //     //             startCallback: null,
        //     //             endCallback: () => Managers.Object.Despawn(this)));
        //     //yield return new WaitForSeconds(Managers.VFX.DESIRED_TIME_FADE_OUT);
        //     //Utils.LogBreak("START FADE OUT !!");
        //     // yield return new WaitUntil(() => this.RendererController.IsChangingMaterial == false);
        //     // //Utils.LogBreak("BREAK");
        //     // Managers.Object.Despawn(this);
        //     yield return null;
        // }

        private void OnDestroy()
        {
            if (Managers.Game.OnPlayerIsDead != null)
            {
                Managers.Game.OnPlayerIsDead -= OnPlayerIsDeadHandler;
                Utils.Log("Release Event : OnPlayerIsDead -= OnPlayerIsDeadHandler");
            }

            if (Managers.Game.OnVFXEnvTarget != null)
            {
                Managers.Game.OnVFXEnvTarget -= OnVFXEnvTargetHandler;
                Utils.Log("Release Event : OnVFXEnvTarget -= OnVFXEnvTargetHandler");
            }

            if (Managers.Game.OnStopAction != null)
            {
                Managers.Game.OnStopAction -= OnStopActionHandler;
                Utils.Log("Release Event : OnStopAction -= OnStopActionHandler");
            }
        }

        public bool Equals(MonsterController other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other) == false)
                return false;

            if (this.GetType() != other.GetType())
                return false;

            return true;
        }

        public override bool Equals(object other) => base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
    }
}

// -------------------------------------------------------------------------------------
// -------------------------------------------------------------------------------------
// if (this.IsDeadState)
//     return;

// if (this.OnStartAction == false)
//     return;

// // TEMP : this.CreatureState != Define.CreatureState.CC_Slow
// if (this.CreatureState != Define.CreatureState.Run)
//     return;
// else if (this.MainTarget != null && this.MainTarget.IsPlayer())
//     MoveToTarget(MainTarget, this.Stat.CollectRange * this.Stat.CollectRange);

// public bool IsThrowingStarHit { get; set; } = false;
// public bool IsLazerBoltHit { get; set; } = false;
// Run State
// private void FixedUpdate()
// {
//     // if (CreatureState == Define.CreatureState.Death)
//     // {
//     //     return;
//     // }

//     // if (_ccStates[(int)Define.TemplateIDs.CCType.Stun])
//     // {
//     //     return;
//     // }

//     // if (_ccStates[(int)Define.TemplateIDs.CCType.KnockBack])
//     // {
//     //     return;
//     // }

//     // // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//     // // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//     // // if (CreatureState == Define.CreatureState.Death || CCStatus == Define.CCStatus.Stun)
//     // //     return;
//     // // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//     // // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//     // if (RigidBody.velocity != Vector2.zero)
//     //     MAC.Run();

//     // Vector2 predictedPos = new Vector2(transform.position.x + RigidBody.velocity.x,
//     //                                     transform.position.y + RigidBody.velocity.y);
//     // if (Managers.Stage.IsOutOfPos(predictedPos))
//     // {
//     //     BodyCollider.isTrigger = true;
//     //     RigidBody.velocity = Vector2.zero;
//     //     Managers.Stage.SetInLimitPos(this);
//     // }

//     // PlayerController pc = Managers.Game.Player;
//     // if (pc.IsValid() == false)
//     //     return;

//     // Vector3 toPlayer = pc.transform.position - transform.position;
//     // FlipX(toPlayer.x > 0 ? -1 : 1);
//     // if (CreatureState != Define.CreatureState.Run)
//     //     return;

//     // Vector3 newPos = transform.position + (toPlayer.normalized * Time.deltaTime * CreatureStat.MoveSpeed);
//     // //transform.position = newPos;
//     // RigidBody.MovePosition(newPos);

//     // float sqrDist = 2f * 2f;
//     // if (toPlayer.sqrMagnitude <= sqrDist)
//     //     CreatureState = Define.CreatureState.Skill;
// }

// switch (CreatureState)
// {
//     case Define.CreatureState.Idle:
//         {
//             MAC.Idle();
//         }
//         break;

//     case Define.CreatureState.Run:
//         {
//             MAC.Run();
//         }
//         break;

//     case Define.CreatureState.Attack:
//         {
//             MAC.Attack();
//         }
//         break;

//     case Define.CreatureState.Skill:
//         {
//             //SkillBook.StartNextSequenceSkill();
//         }
//         break;

//     case Define.CreatureState.Death:
//         {
//             //BodyCollider.isTrigger = true;
//             //StopCoroutine(_coReadyToAction);
//             //SkillBook.StopSkills();
//             RigidBody.simulated = false;
//             Managers.Sprite.SetMonsterFace(this, Define.MonsterFace.Death);
//             MAC.Death();
//             // if (this.GoCCEffect != null)
//             // {
//             //     Managers.Resource.Destroy(this.GoCCEffect);
//             //     GoCCEffect = null;
//             // }
//             // this.CoEffectFadeOut(0f, 1f, true);
//         }
//         break;
// }


// public bool IsSkillHittedStatus(Define.TemplateIDs.CreatureStatus.RepeatSkill skillType)
// {
//     switch (skillType)
//     {
//         case Define.TemplateIDs.CreatureStatus.RepeatSkill.None:
//             return true;

//         case Define.TemplateIDs.CreatureStatus.RepeatSkill.ThrowingStar:
//             return IsThrowingStarHit;

//         case Define.TemplateIDs.CreatureStatus.RepeatSkill.LazerBolt:
//             return IsLazerBoltHit;

//         default:
//             return false;
//     }
// }

// protected override IEnumerator CoReadyToAction(bool isOnActiveImmediately = false)
// {
//     // IsCompleteStartAction = false;
//     // CreatureState = Define.CreatureState.Idle;

//     // this.RigidBody.simulated = true;
//     // this.HitCollider.enabled = true;
//     // if (isOnActiveImmediately)
//     // {
//     //     //StartAction();
//     //     yield break;
//     // }

//     // float delta = 0f;
//     // float desiredTime = LoadIdleToActionTime();
//     // while (true)
//     // {
//     //     if (IsCCStates(CrowdControl.Stun))
//     //     {
//     //         //StartAction();
//     //         yield break;
//     //     }

//     //     delta += Time.deltaTime;
//     //     float percent = delta / desiredTime;
//     //     if (percent > 1f)
//     //     {
//     //         //StartAction();
//     //         yield break;
//     //     }

//     //     yield return null;
//     // }

//     yield return null;
// }
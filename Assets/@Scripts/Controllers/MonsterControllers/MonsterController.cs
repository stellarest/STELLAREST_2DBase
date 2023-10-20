using System;
using System.Collections;
using STELLAREST_2D.UI;
using UnityEngine;
using UnityEngine.Rendering;

using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

using VFXEnv = STELLAREST_2D.Define.TemplateIDs.VFX.Environment;
using UnityEditor.Build.Pipeline;

namespace STELLAREST_2D
{
    public class MonsterController : CreatureController //, IHitStatus
    {
        public Define.MonsterType MonsterType { get; set; } = Define.MonsterType.None;
        public MonsterAnimationController MonsterAnimController { get; private set; } = null;
        public bool Action { get; protected set; } = false;
        private Coroutine _coAction = null;

        [SerializeField] private Sprite _defaultHead = null;
        public Sprite DefaultHead => _defaultHead;

        [SerializeField] private Sprite _angryHead = null;
        public Sprite AngryHead => _angryHead;

        [SerializeField] private Sprite _deadHead = null;
        public Sprite DeadHead => _deadHead;

        public override void Init(int templateID)
        {
            if (this.IsFirstPooling)
            {
                base.Init(templateID);
                LateInit();
                MonsterAnimController = AnimController as MonsterAnimationController;
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.MonsterBody);
                Managers.Game.OnPlayerIsDead += OnPlayerIsDeadHandler;
                Utils.Log("Add Event : OnPlayerIsDeadHandler");
                this.IsFirstPooling = false;
            }
            
            StartGame(templateID);
        }

        protected override void InitChildObject()
        {
            base.InitChildObject();
            Center = Utils.FindChild<Transform>(AnimTransform.gameObject, "Body", true);
        }

        protected virtual void LateInit() { }

        protected override void StartGame(int templateID)
        {
            this.RendererController.StartGame();
            InitCreatureStat(templateID);
            ResetAllHitFrom();
            _coAction = StartCoroutine(CoStartAction());

            if (Managers.Game.Player != null)
            {
                // MainTarget은 중간에 바뀔수도 있긴함 (ex) Forest Guardian : Black Panther
                this.MainTarget = Managers.Game.Player;
                Utils.Log("Set MainTarget");
            }
            else
                Utils.Log("InValid MainTarget.");
        }

        protected virtual IEnumerator CoStartAction()
        {
            Action = false;
            CreatureState = Define.CreatureState.Idle;

            this.RigidBody.simulated = true;
            this.HitCollider.enabled = true;

            float delta = 0f;
            float desiredTime = LoadActionTime();
            while (true)
            {
                delta += Time.deltaTime;
                float percent = delta / desiredTime;
                if (percent > 1f)
                {
                    Action = true;
                    StartAction();
                    yield break;
                }

                yield return null;
            }
        }

        protected virtual void StartAction() { }

        // TEMP
        public void Stop()
        {
            this.CreatureState = Define.CreatureState.Idle;
            StartCoroutine(CoStartAction());
        }

        private void FixedUpdate()
        {
            if (this.MainTarget != null)
            {
                if (this.IsDeadState)
                    return;

                if (this.Action == false)
                    return;

                if (this.CreatureState != Define.CreatureState.Run)
                    return;
                else if (this.MainTarget != null && this.MainTarget.IsPlayer())
                    MoveToTarget(MainTarget, this.Stat.CollectRange * this.Stat.CollectRange);
            }
        }

        public void StartMovementToRandomPoint()
        {
            this.SkillBook.DeactivateAll();
            if (this.IsValid())
                StartCoroutine(CoMoveToRandomPoint());
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

            Vector3 toTargetMovement = this.transform.position + (toTargetDir.normalized * Stat.MovementSpeed * Time.deltaTime);
            this.RigidBody.MovePosition(toTargetMovement);
            if (Utils.IsArriveToTarget(this.Center, this.MainTarget.transform, minDistance))
            {
                this.CreatureState = Define.CreatureState.Skill;
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

        public override void UpdateAnimation()
        {
            switch (CreatureState)
            {
                case Define.CreatureState.Idle:
                    MonsterAnimController.Idle();
                    RendererController.MonsterHead.sprite = this.DefaultHead;
                    break;

                case Define.CreatureState.Run:
                    MonsterAnimController.Run();
                    break;

                case Define.CreatureState.Skill:
                    RunSkill();
                    break;

                case Define.CreatureState.Dead:
                    OnDead();
                    break;
            }
        }

        protected virtual float LoadActionTime() => -1f;

        public void CoStartReadyToAction(bool isSpawned = true)
                => StartCoroutine(CoReadyToAction(isSpawned));

        private IEnumerator CoReadyToAction(bool isSpawned = true)
        {
            if (CreatureState != Define.CreatureState.Idle)
                CreatureState = Define.CreatureState.Idle;

            float delta = 0f;
            float percent = 0f;
            //float desiredTime = Random.Range(CharaData.MinReadyToActionTime, CharaData.MaxReadyToActionTime);
            float desiredTime = UnityEngine.Random.Range(3f, 4f);

            if (isSpawned == false)
                desiredTime = 0.5f;

            while (percent < 1f)
            {
                delta += Time.deltaTime;
                percent = delta / desiredTime;
                yield return null;
            }

            CreatureState = Define.CreatureState.Run;
            //if (_ccStates[(int)Define.CCType.Stun] == false && _ccStates[(int)Define.CCType.None])
            // CreatureState = Define.CreatureState.Run;

            // LEGACY
            // if (CCStatus != Define.CCStatus.Stun)
            //     CreatureState = Define.CreatureState.Run;
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

        public override void OnDamaged(CreatureController attacker, SkillBase from)
        {
            if (this.Action == false)
            {
                StopCoroutine(_coAction);
                _coAction = null;
                StartAction();
            }

            base.OnDamaged(attacker, from);
        }

        protected override void OnDead()
        {
            base.OnDead();
            Managers.VFX.Environment(VFXEnv.Skull, this);
            this.RendererController.MonsterHead.sprite = this.DeadHead;
            MonsterAnimController.Dead();
            StartCoroutine(CoDespawn());
        }

        protected virtual IEnumerator CoDespawn()
        {
            Managers.VFX.Material(Define.MaterialType.FadeOut, this);
            yield return new WaitForSeconds(Managers.VFX.DESIRED_TIME_FADE_OUT);
            Managers.Object.Despawn(this);
        }

        private void OnDestroy()
        {
            if (Managers.Game.OnPlayerIsDead != null)
            {
                Utils.Log("Release Event : OnPlayerIsDeadHandler");
                Managers.Game.OnPlayerIsDead -= OnPlayerIsDeadHandler;
            }
        }
    }
}

// -------------------------------------------------------------------------------------
// -------------------------------------------------------------------------------------
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
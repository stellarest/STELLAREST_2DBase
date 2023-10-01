using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace STELLAREST_2D
{
    public class MonsterController : CreatureController //, IHitStatus
    {
        public Define.MonsterType MonsterType { get; set; } = Define.MonsterType.None;
        public MonsterAnimationController MonsterAnimController { get; private set; } = null;
        protected bool _action = false;

        public override void Init(int templateID)
        {
            if (this.IsFirstPooling)
            {
                base.Init(templateID);
                if (MonsterAnimController == null)
                    MonsterAnimController = AnimController as MonsterAnimationController;

                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.MonsterBody);
                this.IsFirstPooling = false;
            }
            else // Respawn from pooling
            {
                InitCreatureStat(templateID);
            }

            ResetAllHitFrom();
            _action = false;
            StartCoroutine(CoStartAction());
        }

        protected override void InitChildObject()
        {
            base.InitChildObject();
            Center = Utils.FindChild<Transform>(AnimTransform.gameObject, "Body", true);
        }

        private IEnumerator CoStartAction()
        {
            float delta = 0f;
            float desiredTime = LoadActionTime();

            while (true)
            {
                delta += Time.deltaTime;
                float percent = delta / desiredTime;
                if (percent > 1f)
                {
                    _action = true;
                    yield break;
                }

                yield return null;
            }
        }

        private void FixedUpdate()
        {
            PlayerController target = Managers.Game.Player;
            if (target.IsValid() == false)
                return;

            if (this.LockFlip == false)
            {
                Vector3 toTarget = (target.transform.position - transform.position);
                Flip(toTarget.x > 0 ? -1 : 1);
            }

            if (this._action == false)
                return;
        }

        public override void UpdateAnimation()
        {
            switch (CreatureState)
            {
                case Define.CreatureState.Idle:
                    MonsterAnimController.Idle();
                    break;
            }
        }

        private float LoadActionTime()
        {
            switch (MonsterType)
            {
                case Define.MonsterType.Chicken:
                    return UnityEngine.Random.Range(2f, 3f);
            }

            return 0f;
        }

        //private Coroutine _coReadyToAction;
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

        public override void OnDamaged(CreatureController attacker, SkillBase from)
        {
            base.OnDamaged(attacker, from);
            //Utils.Log($"Hit, from : {from.Data.Name}");
        }

        protected override void OnDead()
        {
            //base.OnDead();
            Utils.Log($"{Stat.Name} : I'am dead.");
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
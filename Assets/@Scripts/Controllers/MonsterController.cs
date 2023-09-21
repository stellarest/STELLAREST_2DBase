using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace STELLAREST_2D
{
    public interface ISkillHitStatus
    {
        public bool IsThrowingStarHit { get; set; }
        public bool IsLazerBoltHit { get; set; }
    }

    public class MonsterController : CreatureController, ISkillHitStatus
    {
        public Define.MonsterType MonsterType { get; protected set; } = Define.MonsterType.Monster;

        public bool LockFlipX { get; set; } = false;
        public MonsterAnimationController MAC { get; protected set; }
        public GameObject Body { get; protected set; }

        public bool IsThrowingStarHit { get; set; } = false;
        public bool IsLazerBoltHit { get; set; } = false;
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

        public override void UpdateAnimation()
        {
            switch (CreatureState)
            {
                case Define.CreatureState.Idle:
                    {
                        MAC.Idle();
                    }
                    break;

                case Define.CreatureState.Run:
                    {
                        MAC.Run();
                    }
                    break;

                case Define.CreatureState.Attack:
                    {
                        MAC.Attack();
                    }
                    break;

                case Define.CreatureState.Skill:
                    {
                        //SkillBook.StartNextSequenceSkill();
                    }
                    break;

                case Define.CreatureState.Death:
                    {
                        BodyCollider.isTrigger = true;
                        //StopCoroutine(_coReadyToAction);
                        //SkillBook.StopSkills();
                        RigidBody.simulated = false;
                        Managers.Sprite.SetMonsterFace(this, Define.MonsterFace.Death);
                        MAC.Death();
                        // if (this.GoCCEffect != null)
                        // {
                        //     Managers.Resource.Destroy(this.GoCCEffect);
                        //     GoCCEffect = null;
                        // }
                        // this.CoEffectFadeOut(0f, 1f, true);
                    }
                    break;
            }
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
            float desiredTime = Random.Range(3f, 4f);

            
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

        public override void Init(int templateID)
        {
            if (IsFirstPooling)
            {
                base.Init(templateID);
                InitMonster();
            }
            else
            {
                ResetMonster();
                /* DO SOMETHING : CC STATE RESET, ETC... */
            }
        }

        public virtual void InitMonster() 
        { 
            MAC = gameObject.GetOrAddComponent<MonsterAnimationController>();
            MAC.InitAnimationController(this);
            Body = Utils.FindChild(gameObject, "Body");
            Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.MonsterBody);
        }

        public virtual void ResetMonster()
        {
            CreatureStat.Hp = CreatureStat.MaxHp;
            // CreatureState = Define.CreatureState.Idle;
            // ResetCCStates();
            
            // if (GoCCEffect != null)
            // {
            //     Managers.Resource.Destroy(GoCCEffect);
            //     GoCCEffect = null;
            // }
        }

        protected override void SetRenderSorting()
                => GetComponent<SortingGroup>().sortingOrder = (int)Define.SortingOrder.Monster;

        // Run State
        private void FixedUpdate()
        {
            // if (CreatureState == Define.CreatureState.Death)
            // {
            //     return;
            // }

            // if (_ccStates[(int)Define.TemplateIDs.CCType.Stun])
            // {
            //     return;
            // }

            // if (_ccStates[(int)Define.TemplateIDs.CCType.KnockBack])
            // {
            //     return;
            // }

            // // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // // if (CreatureState == Define.CreatureState.Death || CCStatus == Define.CCStatus.Stun)
            // //     return;
            // // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            // if (RigidBody.velocity != Vector2.zero)
            //     MAC.Run();

            // Vector2 predictedPos = new Vector2(transform.position.x + RigidBody.velocity.x,
            //                                     transform.position.y + RigidBody.velocity.y);
            // if (Managers.Stage.IsOutOfPos(predictedPos))
            // {
            //     BodyCollider.isTrigger = true;
            //     RigidBody.velocity = Vector2.zero;
            //     Managers.Stage.SetInLimitPos(this);
            // }

            // PlayerController pc = Managers.Game.Player;
            // if (pc.IsValid() == false)
            //     return;

            // Vector3 toPlayer = pc.transform.position - transform.position;
            // FlipX(toPlayer.x > 0 ? -1 : 1);
            // if (CreatureState != Define.CreatureState.Run)
            //     return;

            // Vector3 newPos = transform.position + (toPlayer.normalized * Time.deltaTime * CreatureStat.MoveSpeed);
            // //transform.position = newPos;
            // RigidBody.MovePosition(newPos);

            // float sqrDist = 2f * 2f;
            // if (toPlayer.sqrMagnitude <= sqrDist)
            //     CreatureState = Define.CreatureState.Skill;
        }

        private void FlipX(float flipX)
                => transform.localScale = new Vector2(_initialLocalScale.x * flipX, _initialLocalScale.y);

        public override void OnDamaged(BaseController attacker, SkillBase skill)
                => base.OnDamaged(attacker, skill);
    }
}

using System.Collections;
using DG.Tweening;
using UnityEngine;

using static STELLAREST_2D.Define;

namespace STELLAREST_2D
{
    public class ChickenController : MonsterController
    {
        public override bool this[CrowdControlType crowdControlType]
        {
            get => base[crowdControlType];
            set
            {
                base[crowdControlType] = value;
                switch (crowdControlType)
                {
                    case CrowdControlType.Stun:
                        {
                            if (this[CrowdControlType.Stun])
                            {
                                this.SetDeadHead();
                                this.SkillBook.DeactivateAll();
                                this.CreatureState = Define.CreatureState.Idle;

                                transform.DOShakePosition(FixedValue.Numeric.STANDARD_CREATURE_SHAKE_DURATION, FixedValue.Numeric.STANDARD_CREATURE_SHAKE_POWER);
                                transform.DOShakeScale(FixedValue.Numeric.STANDARD_CREATURE_SHAKE_DURATION, FixedValue.Numeric.STANDARD_CREATURE_SHAKE_POWER);
                            }
                            else if (this[CrowdControlType.Stun] == false)
                            {
                                this.SetDefaultHead();
                                ReadyToAction(onStartImmediately: true); // true or false로 기믹을 줘도될것같긴한데 일단 그냥 즉시 움직이도록
                            }
                        }
                        break;

                    case CrowdControlType.Silence:
                        {
                            if (this[CrowdControlType.Silence] == false)
                            {
                                if (this.CreatureState == Define.CreatureState.Idle)
                                {
                                    Utils.Log("##### RUN AGAIN #####");
                                    this.CreatureState = Define.CreatureState.Run;
                                }
                            }
                        }
                        break;
                }
            }
        }

        public override void Init(int templateID) => base.Init(templateID);

        protected override void LateInit()
        {
            SkillBook.LevelUp(FixedValue.TemplateID.Skill.Monster_Unique_BodyAttack);
            //SkillBook.LevelUp(SkillTemplate.ThrowingStar);
        }

        protected override IEnumerator CoReadyToAction(bool onStartImmediately = false) // 파라미터로 NextState를 넣는다면? 괜찮을듯
        {
            this.CreatureState = Define.CreatureState.Idle;
            this.RendererController.OnFaceDefaultHandler();

            this.RigidBody.simulated = true;
            this.HitCollider.enabled = true;

            if (onStartImmediately)
            {
                IsCompleteReadyToAction = true;
                if (this[CrowdControlType.Stun]) // CHECK THIS
                    this.CreatureState = Define.CreatureState.Idle;
                else
                    this.CreatureState = Define.CreatureState.Run;

                yield break;
            }

            float delta = 0f;
            float desiredTime = this.ReadyToActionCompleteTime();
            float percent = 0f;
            while (percent < 1f)
            {
                delta += Time.deltaTime;
                percent = delta / desiredTime;
                yield return null;
            }

            IsCompleteReadyToAction = true;
            if (this[CrowdControlType.Stun])
                this.CreatureState = Define.CreatureState.Idle;
            else
                this.CreatureState = Define.CreatureState.Run;
        }

        protected override float ReadyToActionCompleteTime() => UnityEngine.Random.Range(2f, 3f);

        protected override void UpdateIdle()
        {
            MonsterAnimController.Idle();
            RigidBody.velocity = Vector2.zero;

            // if (_coIdleTick != null)
            //     StopCoroutine(_coIdleTick);

            // 이미 Idle 틱이 재생중일 때는 다시 한번 탈 수 없음
            if (_coIdleTick != null)
                return;

            if (this.IsValid())
                _coIdleTick = StartCoroutine(CoIdleTick());
        }

        protected override IEnumerator CoIdleTick()
        {
            Vector3 toTargetDir = Vector3.zero;
            while (true)
            {
                RigidBody.velocity = Vector2.zero;
                // if (this.IsCCStates(CrowdControl.Stun))
                //     continue;

                if (this.MainTarget != null)
                {
                    toTargetDir = (MainTarget.Center.position - this.Center.position);
                    if (this[CrowdControlType.Stun] == false && this.LockFlip == false)
                        Flip(toTargetDir.x > 0 ? -1 : 1);
                }

                if (this[CrowdControlType.Silence])
                {
                    if (toTargetDir.sqrMagnitude > this.Stat.CollectRange * this.Stat.CollectRange)
                        this.CreatureState = Define.CreatureState.Run;
                }

                yield return null;
            }
        }

        protected override void UpdateRun() => MonsterAnimController.Run();
        protected override void UpdateSkill()
        {
            if (this.SkillBook != null)
            {
                this.MonsterAnimController.Attack();
                this.SkillBook.Activate(FixedValue.TemplateID.Skill.Monster_Unique_BodyAttack);
            }
        }

        public override Vector3 LoadVFXEnvSpawnScale(VFXEnvType vfxEnvType)
        {
            switch (vfxEnvType)
            {
                case VFXEnvType.Skull:
                    return Vector3.one * 2f;

                case VFXEnvType.Stun:
                    return Vector3.one * 2.5f;

                case VFXEnvType.Slow:
                    return new Vector3(1.85f, 1f, 1f);

                case VFXEnvType.QuestionMark:
                    return Vector3.one * 2.5f;

                default:
                    return base.LoadVFXEnvSpawnScale(vfxEnvType);
            }
        }

        public override Vector3 LoadVFXEnvSpawnPos(VFXEnvType vfxEnvType)
        {
            switch (vfxEnvType)
            {
                case VFXEnvType.Spawn:
                    return (transform.position + (Vector3.up * 2.5f)) + new Vector3(0.1f, 1.2f, 1f);

                case VFXEnvType.Poison:
                case VFXEnvType.Damage:
                    return (transform.position + (Vector3.up * 2.5f)) - Vector3.up;

                case VFXEnvType.Dust:
                    return transform.position;

                case VFXEnvType.Stun:
                    return (transform.position + (Vector3.up * 1.83f));

                case VFXEnvType.Slow:
                    return (transform.position + new Vector3(0f, -0.5f, 0f));

                case VFXEnvType.Silence:
                    return (Center.position + new Vector3(-1f, 1.05f, 0f));

                case VFXEnvType.Targeted:
                    return Center.position;

                case VFXEnvType.QuestionMark:
                    return Center.position + (Vector3.up * 2f);

                default:
                    return base.LoadVFXEnvSpawnPos(vfxEnvType);
            }
        }
    }
}

// -------------------------------------------------------------------------------------
// -------------------------------------------------------------------------------------
// MonsterType = Define.MonsterType.Chicken;
// ResetCCStates();
// if (GoCCEffect != null)
// {
//     Managers.Resource.Destroy(GoCCEffect);
//     GoCCEffect = null;
// }
// Managers.Sprite.SetMonsterFace(this, Define.MonsterFace.Normal);
// RigidBody.simulated = true;
// RigidBody.velocity = Vector2.zero;
// SkillBook.Stopped = false;
// BodyCol.isTrigger = false;
// IsThrowingStarHit = false;
// IsLazerBoltHit = false;

// //CreatureType = Define.CreatureType.Monster;
// if (SkillBook.SequenceSkills.Count != 0)
//     SkillBook.SequenceSkills[(int)Define.InGameGrade.Normal - 1].gameObject.SetActive(true);
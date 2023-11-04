using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using VFXEnv = STELLAREST_2D.Define.TemplateIDs.VFX.Environment;
using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;
using CrowdControl = STELLAREST_2D.Define.TemplateIDs.CrowdControl;

namespace STELLAREST_2D
{
    public class ChickenController : MonsterController
    {
        public override bool this[CrowdControl crowdControlType]
        {
            get => base[crowdControlType];
            set
            {
                base[crowdControlType] = value;
                switch (crowdControlType)
                {
                    case CrowdControl.Stun:
                        {
                            if (this[CrowdControl.Stun])
                            {
                                this.SetDeadHead();
                                this.SkillBook.DeactivateAll();
                                this.CreatureState = Define.CreatureState.Idle;

                                transform.DOShakePosition(Utils.CREATURES_FIXED_SHAKE_DURATION, Utils.CREATURES_FIXED_SHAKE_POWER);
                                transform.DOShakeScale(Utils.CREATURES_FIXED_SHAKE_DURATION, Utils.CREATURES_FIXED_SHAKE_POWER);
                            }
                            else if (this[CrowdControl.Stun] == false)
                            {
                                this.SetDefaultHead();
                                ReadyToAction(onStartImmediately: true); // true or false로 기믹을 줘도될것같긴한데 일단 그냥 즉시 움직이도록
                            }
                        }
                        break;

                    case CrowdControl.Slience:
                        {
                            if (IsCCStates(CrowdControl.Slience) == false)
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

                // if (IsCCStates(CrowdControl.Stun))
                // {
                //     Utils.Log("STUN IN !!");
                //     this.SkillBook.DeactivateAll();
                //     MonsterAnimController.AnimController.StopPlayback();
                //     MonsterAnimController.Idle();
                // }
                // else if (IsCCStates(CrowdControl.Stun) == false)
                // {
                //     Utils.LogBreak("STUN OUT");
                // }
            }
        }

        public override void Init(int templateID) => base.Init(templateID);

        protected override void LateInit()
        {
            SkillBook.LevelUp(SkillTemplate.BodyAttack);
            //SkillBook.LevelUp(SkillTemplate.ThrowingStar);
        }

        protected override IEnumerator CoReadyToAction(bool onStartImmediately = false) // 파라미터로 NextState를 넣는다면? 괜찮을듯
        {
            this.CreatureState = Define.CreatureState.Idle;
            this.RigidBody.simulated = true;
            this.HitCollider.enabled = true;

            if (onStartImmediately)
            {
                IsCompleteReadyToAction = true;
                if (this.IsCCStates(CrowdControl.Stun)) // CHECK THIS
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
            if (this.IsCCStates(CrowdControl.Stun))
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
                    if (this.IsCCStates(CrowdControl.Stun) == false && this.LockFlip == false)
                        Flip(toTargetDir.x > 0 ? -1 : 1);
                }

                if (this.IsCCStates(CrowdControl.Slience))
                {
                    if (toTargetDir.sqrMagnitude > this.Stat.CollectRange * this.Stat.CollectRange)
                        this.CreatureState = Define.CreatureState.Run;
                }

                yield return null;
            }
        }

        protected override void UpdateRun()
        {
            MonsterAnimController.Run();
        }

        protected override void UpdateSkill()
        {
            if (this.SkillBook != null)
            {
                this.MonsterAnimController.Attack();
                this.SkillBook.Activate(SkillTemplate.BodyAttack);
            }
        }

        public override Vector3 LoadVFXEnvSpawnScale(VFXEnv templateOrigin)
        {
            switch (templateOrigin)
            {
                case VFXEnv.Skull:
                    return Vector3.one * 2f;

                case VFXEnv.Stun:
                    return Vector3.one * 2.5f;

                case VFXEnv.Slow:
                    return new Vector3(1.85f, 1f, 1f);

                default:
                    return base.LoadVFXEnvSpawnScale(templateOrigin);
            }
        }

        public override Vector3 LoadVFXEnvSpawnPos(VFXEnv templateOrigin)
        {
            switch (templateOrigin)
            {
                case VFXEnv.Spawn:
                    return (transform.position + (Vector3.up * 2.5f)) + new Vector3(0.1f, 1.2f, 1f);

                case VFXEnv.Damage:
                    return (transform.position + (Vector3.up * 2.5f)) - Vector3.up;

                case VFXEnv.Dust:
                    return transform.position;

                case VFXEnv.Stun:
                    return (transform.position + (Vector3.up * 1.83f));

                case VFXEnv.Slow:
                    return (transform.position + new Vector3(0f, -0.5f, 0f));

                case VFXEnv.Silence:
                    return (Center.position + new Vector3(-1f, 1.05f, 0f));

                default:
                    return base.LoadVFXEnvSpawnPos(templateOrigin);
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
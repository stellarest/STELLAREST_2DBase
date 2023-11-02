using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using VFXEnv = STELLAREST_2D.Define.TemplateIDs.VFX.Environment;
using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;
using CrowdControl = STELLAREST_2D.Define.TemplateIDs.CrowdControl;
using UnityEditor;

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
                if (IsCCStates(CrowdControl.Slience) == false)
                {
                    if (this.CreatureState == Define.CreatureState.Idle)
                    {
                        Utils.Log("##### RUN #####");
                        this.CreatureState = Define.CreatureState.Run;
                    }
                }

                if (IsCCStates(CrowdControl.Stun))
                {
                    this.SkillBook.DeactivateAll();
                    MonsterAnimController.AnimController.StopPlayback();
                    MonsterAnimController.Idle();
                }
            }  
        }

        public override void Init(int templateID)
        {
            base.Init(templateID);
        }

        protected override void LateInit()
        {
            SkillBook.LevelUp(SkillTemplate.BodyAttack);
            //SkillBook.LevelUp(SkillTemplate.ThrowingStar);
        }

        protected override void StartAction()
        {
            this.OnStartAction = true;

            if (IsCCStates(CrowdControl.Stun))
                return;
            
            this.CreatureState = Define.CreatureState.Run;
            //this.SkillBook.Activate(SkillTemplate.ThrowingStar);
        }

        protected override float LoadIdleToActionTime() => UnityEngine.Random.Range(2f, 3f);

        protected override void RunSkill()
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
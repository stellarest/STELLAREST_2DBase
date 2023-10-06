using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using EnvTemplate = STELLAREST_2D.Define.TemplateIDs.VFX.Environment;

namespace STELLAREST_2D
{
    public class ChickenController : MonsterController
    {
        public override void Init(int templateID)
        {
            base.Init(templateID);
            CreatureState = Define.CreatureState.Idle;
        }

        public override Vector3 LoadVFXEnvSpawnPos(EnvTemplate templateOrigin)
        {
            switch (templateOrigin)
            {
                case EnvTemplate.Spawn:
                    return (transform.position + (Vector3.up * 2.5f)) + new Vector3(0.1f, 1.2f, 1f);

                case EnvTemplate.DamageFont:
                    return (transform.position + (Vector3.up * 2.5f)) - Vector3.up;

                default:
                    return base.LoadVFXEnvSpawnPos(templateOrigin);
            }
        }

        protected override float LoadActionTime() => UnityEngine.Random.Range(2f, 3f);
        protected override void OnDead() { }
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
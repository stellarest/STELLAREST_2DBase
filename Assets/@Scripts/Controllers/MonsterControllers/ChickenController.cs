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

        /*
                    private const float DEFAULT_DMG_SPAWN_HEIGHT = 2.5f;
        private Vector3 GetSpawnPosForDamageFont(CreatureController cc)
        {
            Vector3 spawnPos = cc.transform.position + (Vector3.up * DEFAULT_DMG_SPAWN_HEIGHT);
            if (cc?.IsPlayer() == false)
            {
                MonsterController mc = cc.GetComponent<MonsterController>();
                switch (mc.MonsterType)
                {
                    case Define.MonsterType.Chicken:
                        return (spawnPos -= Vector3.up);
                }
            }

            return spawnPos;
        }
        */

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

        // public override float ADDITIONAL_SPAWN_WIDTH => 0.1f;
        // public override float ADDITIONAL_SPAWN_HEIGHT => 1.2f;
        // public override Vector3 LoadSpawnPos() => _defaultSpawnPos;
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
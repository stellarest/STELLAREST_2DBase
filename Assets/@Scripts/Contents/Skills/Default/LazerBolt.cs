// using System.Collections;
// using UnityEngine;

using System.Collections;
using STELLAREST_2D.Data;
using UnityEngine;
using UnityEngine.Rendering;

namespace STELLAREST_2D
{
    public class LazerBolt : DefaultSkill
    {
        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            GetComponent<Rigidbody2D>().simulated = false;
            GetComponent<Collider2D>().enabled = false;
            foreach (var particle in GetComponentsInChildren<ParticleSystem>())
            {
                var emission = particle.emission;
                emission.enabled = false;
            }
        }

        public override void InitClone(CreatureController ownerFromOrigin, SkillData dataFromOrigin)
        {
            if (this.IsFirstPooling)
            {
                RigidBody = GetComponent<Rigidbody2D>();
                HitCollider = GetComponent<Collider2D>();

                base.InitClone(ownerFromOrigin, dataFromOrigin);
                this.IsFirstPooling = false;
            }
        }

        protected override void DoSkillJob()
        {
            StartCoroutine(CoGenerateLazerBolt());
        }

        private const float FROM_MIN_RANGE = 1f;
        private const float FROM_MAX_RANGE = 30;
        private IEnumerator CoGenerateLazerBolt()
        {
            Vector3 prevCheckPosition = Utils.GetRandomTargetPosition<MonsterController>(this.Owner.transform.position,
                                            FROM_MIN_RANGE, FROM_MAX_RANGE, Define.HitFromType.LazerBolt);
            if (prevCheckPosition == Vector3.zero)
                yield break;

            Vector3 spawnPos = Vector3.zero;
            for (int i = 0; i < this.Data.ContinuousCount; ++i)
            {
                spawnPos = Utils.GetRandomTargetPosition<MonsterController>(this.Owner.transform.position,
                                            FROM_MIN_RANGE, FROM_MAX_RANGE, Define.HitFromType.LazerBolt);

                SkillBase clone = Managers.Object.Spawn<SkillBase>(spawnPos: Vector3.zero, templateID: this.Data.TemplateID,
                        spawnObjectType: Define.ObjectType.Skill, isPooling: true);
                clone.InitClone(this.Owner, this.Data);
                if (spawnPos == Vector3.zero)
                    spawnPos = Utils.GetRandomPosition(this.Owner.transform.position);
                clone.transform.position = spawnPos;

                yield return new WaitForSeconds(this.Data.ContinuousSpacing);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            cc.OnDamaged(attacker: this.Owner, from: this);
            cc.IsHitFrom_LazerBolt = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            cc.ResetHitFrom(Define.HitFromType.LazerBolt, 0.1f);
        }

        protected override void SetSortingOrder() 
            => GetComponent<SortingGroup>().sortingOrder = (int)Define.SortingOrder.Skill;
    }
}

// namespace STELLAREST_2D
// {
//     public class LazerBolt : RepeatSkill
//     {
//         private Collider2D _collider = null;
//         private ParticleSystem[] _particles = null;

//         public override void SetSkillInfo(CreatureController owner, int templateID)
//         {
//             base.SetSkillInfo(owner, templateID);
//             SkillType = Define.TemplateIDs.SkillType.LazerBolt;
//             _collider = GetComponent<Collider2D>();
//             _collider.enabled = false;

//             _particles = GetComponentsInChildren<ParticleSystem>();
//             for (int i = 0; i < _particles.Length; ++i)
//                 _particles[i].GetComponent<ParticleSystemRenderer>().sortingOrder = (int)Define.SortingOrder.ParticleEffect;

//             if (owner?.IsPlayer() == true)
//                 Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
//         }

//         protected override void DoSkillJob()
//         {
//             StartCoroutine(GenerateLazerBolt());
//         }

//         private IEnumerator GenerateLazerBolt()
//         {
//             for (int i = 0; i < SkillData.ContinuousCount; ++i)
//             {
//                 GameObject go = Managers.Resource.Instantiate(SkillData.PrimaryLabel, pooling: true);
//                 LazerBolt lazerBolt = go.GetComponent<LazerBolt>();
//                 lazerBolt.SetSkillInfo(this.Owner, SkillData.TemplateID);

//                 lazerBolt._collider.enabled = true;
//                 lazerBolt.transform.position = Managers.Object.GetRandomTargetingPosition<MonsterController>(Owner.gameObject,
//                                                     skillHitStatus: this.SkillType);

//                 StartCoroutine(CoLifeTime(lazerBolt));
//                 yield return new WaitForSeconds(SkillData.ContinuousSpacing);
//             }
//         }

//         private IEnumerator CoLifeTime(LazerBolt lazerBolt)
//         {
//             while (true)
//             {
//                 if (lazerBolt._particles[0].isPlaying == false)
//                 {
//                     Managers.Object.ResetSkillHittedStatus(Define.TemplateIDs.SkillType.LazerBolt);
//                     Managers.Resource.Destroy(lazerBolt.gameObject);
//                     yield break;
//                 }

//                 yield return null;
//             }
//         }

//         private void OnTriggerEnter2D(Collider2D other)
//         {
//             MonsterController mc = other.GetComponent<MonsterController>();
//             if (mc.IsValid() == false)
//                 return;

//             if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.MonsterBody, other.gameObject.layer))
//             {
//                 mc.IsLazerBoltHit = true;
//                 mc.OnDamaged(Owner, this);
//             }
//         }

//         public override void OnPreSpawned()
//         {
//             base.OnPreSpawned();
//             foreach (var particle in GetComponentsInChildren<ParticleSystem>())
//             {
//                 var emission = particle.emission;
//                 emission.enabled = false;
//             }
//         }
//     }
// }

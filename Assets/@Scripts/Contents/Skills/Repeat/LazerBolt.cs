// using System.Collections;
// using UnityEngine;

using UnityEngine;

namespace STELLAREST_2D
{
    public class LazerBolt : RepeatSkill
    {

        // public override void SetParticleInfo(Vector3 startAngle, Define.LookAtDirection lookAtDir, float continuousAngle, float continuousFlipX, float continuousFlipY)
        // {
        // }

        protected override void DoSkillJob()
        {
        }
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

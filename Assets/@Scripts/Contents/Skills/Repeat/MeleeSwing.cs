using System.Collections;
using System.Collections.Generic;
using STELLAREST_2D.Data;
using UnityEngine;
using UnityEngine.Rendering;

namespace STELLAREST_2D
{
    public class MeleeSwing : RepeatSkill
    {
        private ParticleSystem[] _particles = null;
        private ParticleSystemRenderer[] _particleRenderers = null;

        // +++++
        // Projectile must be skill.
        // 프로젝타일은 스킬에서 딸려나갈 수 있는 것.
        // But, Skill is not projectile sometimes. Init pos is here.
        // public override void Init(CreatureController owner, SkillData data)
        // {
        //     base.Init(owner, data);
        //     if (this.IsFirstPooling)
        //     {
        //         _particles = GetComponents<ParticleSystem>();
        //         _particleRenderers = GetComponents<ParticleSystemRenderer>();
        //         SetSortingGroup();
        //         Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
        //         IsFirstPooling = false;
        //     }
        // }

        protected override void SetSortingGroup() 
                => GetComponent<SortingGroup>().sortingOrder = (int)Define.SortingOrder.Skill;

        // public override void SetParticleInfo(Vector3 startAngle, Define.LookAtDirection lookAtDir, 
        //                                     float continuousAngle, float continuousFlipX, float continuousFlipY)
        // {
        //     for (int i = 0; i < _particles.Length; ++i)
        //     {
        //         Vector3 tempAngle = startAngle;
        //         tempAngle.z += continuousAngle;
        //         //tempAngle.z += TestParticleAngle;
        //         transform.rotation = Quaternion.Euler(tempAngle);

        //         var main = _particles[i].main;
        //         main.startRotation = Mathf.Deg2Rad * tempAngle.z * -1f;
        //         main.flipRotation = (int)lookAtDir;
        //         _particleRenderers[i].flip = new Vector3(continuousFlipX, continuousFlipY, 0);
        //     }
        // }

        protected override void DoSkillJob() 
            => Owner.CreatureState = Define.CreatureState.Attack; // Event Handler로 대체

        // public override void OnPreSpawned()
        // {
        //     base.OnPreSpawned();
        //     foreach (var particle in GetComponentsInChildren<ParticleSystem>())
        //     {
        //         var emission = particle.emission;
        //         emission.enabled = false;
        //     }

        //     foreach (var col in GetComponents<Collider2D>())
        //         col.enabled = false;
        // }
    }
}


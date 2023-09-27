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
        private ParticleSystemRenderer[] _particleSystemRenderers = null;

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            foreach (var particle in GetComponentsInChildren<ParticleSystem>())
            {
                var emission = particle.emission;
                emission.enabled = false;
            }

            GetComponent<Rigidbody2D>().simulated = false;
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SortingGroup>().enabled = false;
        }

        public override void InitClone(CreatureController ownerFromOrigin, SkillData dataFromOrigin)
        {
            if (this.IsFirstPooling)
            {
                _particles = GetComponents<ParticleSystem>();
                _particleSystemRenderers = GetComponents<ParticleSystemRenderer>();

                RigidBody = GetComponent<Rigidbody2D>();
                HitCollider = GetComponent<Collider2D>();

                base.InitClone(ownerFromOrigin, dataFromOrigin);
                this.PC.OnSetParticleInfo += this.OnSetSwingParticleInfoHandler;
                this.IsFirstPooling = false;
            }
        }

        protected override void SetSortingGroup()
                => GetComponent<SortingGroup>().sortingOrder = (int)Define.SortingOrder.Skill;

        protected override void DoSkillJob()
        {
            Owner.CreatureState = Define.CreatureState.Attack;
        }

        protected override IEnumerator CoCloneSkill() => base.CoCloneSkill();

        public void OnSetSwingParticleInfoHandler(Vector3 indicatorAngle, Define.LookAtDirection lookAtDir, float continuousAngle, float continuousFlipX, float continuousFlipY)
        {
            for (int i = 0; i < _particles.Length; ++i)
            {
                Vector3 angle = indicatorAngle;
                angle.z += continuousAngle;
                transform.rotation = Quaternion.Euler(angle);

                var main = _particles[i].main;
                main.startRotation = Mathf.Deg2Rad * angle.z * -1;
                main.flipRotation = (int)lookAtDir;
                _particleSystemRenderers[i].flip = new Vector3(continuousFlipX, continuousFlipY);
            }
        }

        private void OnDestroy()
        {
            if (this.PC != null && this.PC.OnSetParticleInfo != null)
            {
                this.PC.OnSetParticleInfo -= OnSetSwingParticleInfoHandler;
                //this.PC.Owner.AnimCallback.OnCloneExclusiveSkill -= OnCloneExclusiveSkillHandler;
            }
        }

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

        // protected override void DoSkillJob() 
        //     => Owner.CreatureState = Define.CreatureState.Attack; // Event Handler로 대체

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


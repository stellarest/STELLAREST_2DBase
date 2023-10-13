using System.Collections;
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
                RigidBody.simulated = true;

                HitCollider = GetComponent<Collider2D>();
                HitCollider.isTrigger = true;

                base.InitClone(ownerFromOrigin, dataFromOrigin);
                this.PC.OnSetParticleInfo += this.OnSetSwingParticleInfoHandler;
                this.IsFirstPooling = false;
            }
        }

        protected override void SetSortingOrder()
                => GetComponent<SortingGroup>().sortingOrder = (int)Define.SortingOrder.Skill;

        protected override void DoSkillJob()
        {
            Owner.CreatureState = Define.CreatureState.Skill;
        }

        protected override IEnumerator CoCloneSkill() => base.CoCloneSkill();

        public void OnSetSwingParticleInfoHandler(Vector3 indicatorAngle, Define.LookAtDirection lookAtDir, float continuousAngle, float continuousFlipX, float continuousFlipY)
        {
            for (int i = 0; i < _particles.Length; ++i)
            {
                Vector3 angle = indicatorAngle;
                //angle.z += continuousAngle;
                angle.z -= continuousAngle;
                transform.rotation = Quaternion.Euler(angle);

                var main = _particles[i].main;
                main.startRotation = Mathf.Deg2Rad * angle.z * -1;
                main.flipRotation = (int)lookAtDir;
                _particleSystemRenderers[i].flip = new Vector3(continuousFlipX, continuousFlipY);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            cc.OnDamaged(attacker: this.Owner, from: this);
        }

        private void OnDestroy()
        {
            if (this.PC != null && this.PC.OnSetParticleInfo != null)
            {
                this.PC.OnSetParticleInfo -= OnSetSwingParticleInfoHandler;
            }
        }
    }
}


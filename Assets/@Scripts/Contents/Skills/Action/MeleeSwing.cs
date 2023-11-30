using System.Collections;
using STELLAREST_2D.Data;
using UnityEngine;
using UnityEngine.Rendering;

namespace STELLAREST_2D
{
    public class MeleeSwing : ActionSkill
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

            if (GetComponent<Rigidbody2D>() != null)
                GetComponent<Rigidbody2D>().simulated = false;
            
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SortingGroup>().enabled = false;
        }

        public override void InitClone(CreatureController ownerFromOrigin, SkillData dataFromOrigin)
        {
            if (this.IsFirstPooling)
            {
                // _particles = GetComponents<ParticleSystem>();
                // _particleSystemRenderers = GetComponents<ParticleSystemRenderer>();
                _particles = GetComponentsInChildren<ParticleSystem>();
                _particleSystemRenderers = GetComponentsInChildren<ParticleSystemRenderer>();

                if (GetComponent<Rigidbody2D>() != null)
                {
                    RigidBody = GetComponent<Rigidbody2D>();
                    RigidBody.simulated = true;
                }

                HitCollider = GetComponent<Collider2D>();
                HitCollider.isTrigger = true;

                base.InitClone(ownerFromOrigin, dataFromOrigin);

                if (dataFromOrigin.UsePresetParticleInfo == false)
                    this.PC.OnSetParticleInfo += this.OnSetSwingParticleInfoHandler;
                    
                this.IsFirstPooling = false;
            }
        }

        protected override void SetSortingOrder()
                => GetComponent<SortingGroup>().sortingOrder = (int)Define.SortingOrder.Skill;

        protected override IEnumerator CoStartSkill()
        {
            WaitForSeconds wait = new WaitForSeconds(this.Data.CoolTime);
            while (true)
            {
                DoSkillJob();
                yield return wait;
            }
        }

        protected override void DoSkillJob(System.Action callback = null)
        {
            Owner.ReserveSkillAnimationType(this.Data.AnimationType);
            Owner.CreatureState = Define.CreatureState.Skill;
        }

        public override void OnActiveMasteryActionHandler()
        {
            if (IsStopped)
                return;

            Owner.AttackStartPoint = transform.position;
            StartCoroutine(this.CoGenerateProjectile());
        }

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

            HitPoint = other.ClosestPoint(this.transform.position);
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


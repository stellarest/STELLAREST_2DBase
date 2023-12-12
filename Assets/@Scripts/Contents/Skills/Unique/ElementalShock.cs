using System;
using System.Collections;
using UnityEngine;

using static STELLAREST_2D.Define;
using STELLAREST_2D.Data;

namespace STELLAREST_2D
{
    public class ElementalShock : UniqueSkill
    {
        private ParticleSystem[] _particles = null;
        private ParticleSystem _burst = null; // TEMP
        private ReinaController _ownerController = null;
        private CircleCollider2D _hitCollider = null;

        private float _startHitColliderRadius = 0f;
        private const float FIXED_HIT_COLLIDER_RADIUS = 0f;

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            _particles = GetComponentsInChildren<ParticleSystem>();
            _burst = transform.GetChild(0).GetComponent<ParticleSystem>(); // TEMP
            _ownerController = owner.GetComponent<ReinaController>();

            HitCollider = GetComponent<CircleCollider2D>();
            _hitCollider = HitCollider as CircleCollider2D;
            _startHitColliderRadius = _hitCollider.radius;
     
            HitCollider.isTrigger = true;
            HitCollider.enabled = false;

            if (owner?.IsPlayer == true)
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);

            this.transform.localScale = Vector3.one * 2.5f;
        }

        protected override IEnumerator CoStartSkill()
        {
            WaitForSeconds wait = new WaitForSeconds(this.Data.Cooldown);
            while (true)
            {
                DoSkillJob();
                yield return wait;
            }
        }

        protected override void DoSkillJob(Action callback = null)
        {
            _ownerController.PlayerAnimController.EnterNextState(false);
            _ownerController.LockHandle = true;

            this.Owner.SkillBook.Deactivate(FixedValue.TemplateID.Skill.ElementalArcherMastery);
            this.Owner.CreatureSkillAnimType = this.Data.AnimationType;
            this.Owner.CreatureState = CreatureState.Skill;
            callback?.Invoke();

            EnableParticles(_particles, true);
            StartCoroutine(CoElementalShock());
        }

        private const float FIXED_WAIT_TIME_AFTER_SKILL = 1.25f;
        private IEnumerator CoElementalShock()
        {
            _hitCollider.enabled = true;
            
            float delta = 0f;
            float percent = 0f;
            float desiredDuration = 0.5f;
            while (percent < 1f)
            {
                delta += Time.deltaTime;
                percent = delta / desiredDuration;
                _hitCollider.radius = Mathf.Lerp(_startHitColliderRadius, FIXED_HIT_COLLIDER_RADIUS, percent);
                yield return null;
            }
            _hitCollider.enabled = false;
            _hitCollider.radius = _startHitColliderRadius;

            yield return new WaitForSeconds(FIXED_WAIT_TIME_AFTER_SKILL);
            _ownerController.LockHandle = false;
            this.Owner.SkillBook.Activate(FixedValue.TemplateID.Skill.ElementalArcherMastery);
            EnableParticles(_particles, false);
            _ownerController.PlayerAnimController.EnterNextState(true);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            HitPoint = other.ClosestPoint(this.transform.position);
            cc.OnDamaged(attacker: this.Owner, from: this);
        }
    }
}

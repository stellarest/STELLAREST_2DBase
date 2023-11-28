using System;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_2D.Data;
using UnityEngine;

using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

namespace STELLAREST_2D
{
    /*
        [ Ability Info : Elemental Shock (Elite Action, Elemental Archer) ]
        (Currently Temped Set : lv.3)

        // TODO : 개선 사항 
        lv.1 피해를 입을 때, 10%의 확률로 Elemental Shock를 발동시키고, 동시에 받게 되는 피해량 또한 10% 감소한다. (쿨타임 12초) -- 쿨타임도 있어야함
        Elemental Shock는 주변의 적에게 [n]만큼의 데미지를 입히고, 1초 동안 기절과 함께 4초 동안 30% 감속시킨다.

        lv.2 피해를 입을 때, 15%의 확률로 Elemental Shock를 발동시키고, 동시에 받게 되는 피해량 또한 15% 감소한다. (쿨타임 12초)
        Elemental Shock는 주변의 적에게 [n + (n * ratio)]만큼의 데미지를 입히고, 2초 동안 기절과 함께 4초 동안 40% 감속시킨다.

        lv.3 피해를 입을 때, 30%의 확률로 Elemental Shock를 발동시키고, 동시에 받게 되는 피해량 또한 30% 감소한다. (쿨타임 12초)
        Elemental Shock는 주변의 적에게 [n + (n * ratio)]만큼의 데미지를 입히고, 3초 동안 기절과 함께 7초 동안 70% 감속시킨다.
        
        lv.1 : 주변의 적에게 [n]만큼의 데미지를 입히고, 1초 동안 기절과 함께 4초 동안 30% 감속시킨다.
        lv.2 : 주변의 적에게 [n + (n * ratio)]만큼의 데미지를 입히고, 2초 동안 기절과 함께 4초 동안 40% 감속시킨다.
        lv.3 : 주변의 적에게 [n + (n * ratio)]만큼의 데미지를 입히고, 3초 동안 기절과 함께 7초 동안 70% 감속시킨다.
    */

    public class ElementalShock : ActionSkill
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
            WaitForSeconds wait = new WaitForSeconds(this.Data.CoolTime);
            while (true)
            {
                DoSkillJob();
                yield return wait;
            }
        }

        protected override void DoSkillJob(Action callback = null)
        {
            _ownerController.PlayerAnimController.SetCanEnterNextState(false);
            _ownerController.LockHandle = true;

            this.Owner.SkillBook.Deactivate(SkillTemplate.ElementalArcherMastery);
            this.Owner.ReserveSkillAnimationType(this.Data.AnimationType);
            Owner.CreatureState = Define.CreatureState.Skill;

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
            this.Owner.SkillBook.Activate(SkillTemplate.ElementalArcherMastery);
            EnableParticles(_particles, false);
            _ownerController.PlayerAnimController.SetCanEnterNextState(true);
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

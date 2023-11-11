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

        lv.1 : 
        lv.2 :
        lv.3 : 
    */

    public class ElementalShock : ActionSkill
    {
        private ParticleSystem[] _particles = null;

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            _particles = GetComponentsInChildren<ParticleSystem>();
            HitCollider = GetComponent<CircleCollider2D>();
            HitCollider.isTrigger = true;
            HitCollider.enabled = false;

            if (owner?.IsPlayer() == true)
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
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
            this.Owner.SkillBook.Deactivate(SkillTemplate.ElementalArcherMastery);
            this.Owner.ReserveSkillAnimationType(this.Data.AnimationType);
            Owner.CreatureState = Define.CreatureState.Skill;
        }

        public override void OnActiveEliteActionHandler() => StartCoroutine(CoElementalShock());

        private IEnumerator CoElementalShock()
        {
            yield return new WaitForSeconds(1f);
            ElementalArcherAnimationController anim = this.Owner.AnimController.GetComponent<ElementalArcherAnimationController>();
            anim.EnterNextState();
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

using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    public class ThrowingStar : DefaultSkill
    {
        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Rigidbody2D>().simulated = false;
            GetComponent<Collider2D>().enabled = false;
        }

        public override void InitClone(CreatureController owner, SkillData data)
        {
            if (this.IsFirstPooling)
            {
                SR = GetComponent<SpriteRenderer>();
                RigidBody = GetComponent<Rigidbody2D>();
                HitCollider = GetComponent<Collider2D>();

                base.InitClone(owner, data);
                this.IsFirstPooling = false;
            }
        }

        protected override void SetSortingOrder() 
            => SR.sortingOrder = (int)Define.SortingOrder.Skill;

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            cc.OnDamaged(attacker: this.Owner, from: this);
            cc.IsHitFrom_ThrowingStar = true;
        }

        private readonly float RESET_HIT_FROM_DELAY = 0.5f;
        private void OnTriggerExit2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            if (cc.IsPlayer() == false)
                cc.ResetHitFrom(Define.HitFromType.ThrowingStar, RESET_HIT_FROM_DELAY);
        }
    }
}
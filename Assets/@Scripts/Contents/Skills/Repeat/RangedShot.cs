// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    public class RangedShot : RepeatSkill
    {
        public SpriteTrail.SpriteTrail Trail { get; private set; }

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            GetComponent<Rigidbody2D>().simulated = false;
            GetComponent<Collider2D>().enabled = false;
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            if (GetComponentInChildren<SpriteTrail.SpriteTrail>() != null)
                GetComponentInChildren<SpriteTrail.SpriteTrail>().enabled = false;
        }

        public override void InitClone(CreatureController ownerFromOrigin, SkillData dataFromOrigin)
        {
            if (this.IsFirstPooling)
            {
                SR = GetComponentInChildren<SpriteRenderer>();
                RigidBody = GetComponent<Rigidbody2D>();
                HitCollider = GetComponent<Collider2D>();
                base.InitClone(ownerFromOrigin, dataFromOrigin);
                if (this.Data.Grade == this.Data.MaxGrade)
                    Trail = GetComponentInChildren<SpriteTrail.SpriteTrail>();
                this.IsFirstPooling = true;
            }
        }

        protected override void DoSkillJob()
                => Owner.CreatureState = Define.CreatureState.Skill;

        protected override void SetSortingOrder()
        {
            SR.sortingOrder = (int)Define.SortingOrder.Skill;
            if (Trail != null)
                Trail.m_SortingLayerID = (int)Define.SortingOrder.Skill;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            cc.OnDamaged(this.Owner, this);
        }
    }
}

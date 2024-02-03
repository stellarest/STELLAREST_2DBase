using System.Collections;
using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    public class Boomerang : PublicSkill
    {
        [SerializeField] private AnimationCurve _curve = null;
        public AnimationCurve Curve => _curve;
        public SpriteTrail.SpriteTrail Trail { get; private set; }

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            if (this.Data.Grade < this.Data.MaxGrade)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Rigidbody2D>().simulated = false;
                GetComponent<Collider2D>().enabled = false;
            }
            else
            {
                transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                transform.GetChild(0).GetComponent<Rigidbody2D>().simulated = false;
                transform.GetChild(0).GetComponent<Collider2D>().enabled = false;
                transform.GetChild(0).GetComponent<SpriteTrail.SpriteTrail>().enabled = false;
            }
        }

        public override void InitClone(CreatureController ownerFromOrigin, SkillData dataFromOrigin)
        {
            if (this.IsFirstPooling)
            {
                if (dataFromOrigin.Grade < dataFromOrigin.MaxGrade)
                {
                    SR = GetComponent<SpriteRenderer>();
                    RigidBody = GetComponent<Rigidbody2D>();
                    HitCollider = GetComponent<Collider2D>();
                }
                else if (dataFromOrigin.Grade == dataFromOrigin.MaxGrade)
                {
                    BoomerangChild child = transform.GetChild(0).GetComponent<BoomerangChild>();
                    child.Init(ownerFromOrigin, dataFromOrigin);

                    SR = child.GetComponent<SpriteRenderer>();
                    RigidBody = child.GetComponent<Rigidbody2D>();
                    HitCollider = child.GetComponent<Collider2D>();
                    Trail = child.GetComponent<SpriteTrail.SpriteTrail>();
                    Trail.m_OrderInSortingLayer = (int)Define.SortingOrder.Skill;
                    Trail.enabled = false;
                }

                base.InitClone(ownerFromOrigin, dataFromOrigin);
                this.IsFirstPooling = false;
            }
            else if (this.Data.Grade == this.Data.MaxGrade)
            {
                this.SR.enabled = true;
                this.RigidBody.simulated = true;
                this.HitCollider.enabled = true;
            }
        }

        protected override IEnumerator CoStartSkill()
        {
            if (this.Data.Grade < this.Data.MaxGrade)
                yield return base.CoStartSkill(); // JUST RUN EVERY COOLTIME.
            else
            {
                base.DoSkillJob(); // ONLY ONCE RUN
                yield return null;
            }
        }

        protected override IEnumerator CoDoSkillJobManually(SkillBase caller, float delay)
        {
            // DEACTIVE
            this.SR.enabled = false;
            this.RigidBody.simulated = false;
            this.HitCollider.enabled = false;

            yield return new WaitForSeconds(delay);
            this.DoSkillJob();
            Managers.Object.Despawn<SkillBase>(caller);
        }

        protected override void SetSortingOrder() 
            => SR.sortingOrder = (int)Define.SortingOrder.Skill;

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            cc.OnDamaged(attacker: this.Owner, from: this);
        }
    }
}
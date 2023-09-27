// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

using System.Collections;
using Assets.FantasyMonsters.Scripts.Utils;
using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    public class Boomerang : RepeatSkill
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
                    SR = transform.GetChild(0).GetComponent<SpriteRenderer>();
                    RigidBody = transform.GetChild(0).GetComponent<Rigidbody2D>();
                    HitCollider = transform.GetChild(0).GetComponent<Collider2D>();
                    Trail = transform.GetChild(0).GetComponent<SpriteTrail.SpriteTrail>();
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

        // 여기 건드려야함.
        protected override IEnumerator CoStartSkill()
        {
            if (this.Data.Grade < this.Data.MaxGrade)
                yield return base.CoStartSkill();
            else
            {
                base.DoSkillJob(); // ONCE
                yield return null;
            }
        }

        protected override IEnumerator Delay(SkillBase caller, float delay)
        {
            this.SR.enabled = false;
            this.RigidBody.simulated = false;
            this.HitCollider.enabled = false;

            Utils.Log($"{this.Data.Name} !! In Delay !!");
            yield return new WaitForSeconds(delay);
            this.DoSkillJob();
            Utils.Log($"{this.Data.Name} !! Start !!");
            Managers.Object.Despawn<SkillBase>(caller);
        }

        protected override void SetSortingGroup() 
            => SR.sortingOrder = (int)Define.SortingOrder.Skill;
    }
}
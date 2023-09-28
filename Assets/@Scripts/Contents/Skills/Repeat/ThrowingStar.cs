using System.Collections;
using System.Linq;
using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    public class ThrowingStar : RepeatSkill
    {
        public int CurrentBounceCount { get; private set; } = 0;
        public int MaxBounceCount { get; private set; } = 0;

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

                MaxBounceCount = data.BounceCount;

                base.InitClone(owner, data);
                this.IsFirstPooling = false;
            }

            CurrentBounceCount = 0;
        }

        protected override void SetSortingGroup() 
            => SR.sortingOrder = (int)Define.SortingOrder.Skill;

        public bool CanStillBounce => (CurrentBounceCount++ < MaxBounceCount);

        // 그리고 왠지 바운스 로직을 프로젝타일로 옮겨야할것같기도하고
        public Vector3 NextBounceDir { get; private set; } = Vector3.zero;

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            if (cc?.IsPlayer() == false)
            {
                if (Managers.Collision.IsCorrectTarget(Define.CollisionLayers.MonsterBody, cc.gameObject.layer))
                {
                    cc.OnDamaged(attacker: this.Owner, from: this);
                    cc.IsHitFrom_ThrowingStar = true;
                    MonsterController nextMC = Managers.Object.GetClosestNextMonsterTarget(cc, Define.HitFromType.ThrowingStar);
                    if (nextMC.IsValid())
                        NextBounceDir = (nextMC.transform.position - this.transform.position).normalized;
                    else
                        NextBounceDir = Vector3.zero;
                }
            }
            else if (Managers.Collision.IsCorrectTarget(Define.CollisionLayers.PlayerBody, cc.gameObject.layer))
                cc.OnDamaged(attacker: this.Owner, from: this);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            cc.ResetHitFrom(Define.HitFromType.ThrowingStar, 0.25f);
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

using static STELLAREST_2D.Define;
using STELLAREST_2D.Data;

namespace STELLAREST_2D
{
    public class LazerBolt : PublicSkill
    {
        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            GetComponent<Rigidbody2D>().simulated = false;
            GetComponent<Collider2D>().enabled = false;
            foreach (var particle in GetComponentsInChildren<ParticleSystem>())
            {
                var emission = particle.emission;
                emission.enabled = false;
            }
        }

        public override void InitClone(CreatureController ownerFromOrigin, SkillData dataFromOrigin)
        {
            if (this.IsFirstPooling)
            {
                RigidBody = GetComponent<Rigidbody2D>();
                HitCollider = GetComponent<Collider2D>();

                base.InitClone(ownerFromOrigin, dataFromOrigin);
                this.IsFirstPooling = false;
            }
        }

        protected override void DoSkillJob() => StartCoroutine(CoGenerateLazerBolt());

        private const float FROM_TARGET_POS_MIN_RANGE = 1f;
        private const float FROM_TARGET_POS_MAX_RANGE = 30;
        private IEnumerator CoGenerateLazerBolt()
        {
            Vector3 prevCheckPosition = Utils.GetRandomTargetPosition<MonsterController>(this.Owner.transform.position,
                                            FROM_TARGET_POS_MIN_RANGE, FROM_TARGET_POS_MAX_RANGE, HitFromType.LazerBolt);
            if (prevCheckPosition == Vector3.zero)
                yield break;

            Vector3 spawnPos = Vector3.zero;

            // ContinuousCount -> Count
            for (int i = 0; i < this.Data.Count; ++i)
            {
                spawnPos = Utils.GetRandomTargetPosition<MonsterController>(this.Owner.transform.position,
                                            FROM_TARGET_POS_MIN_RANGE, FROM_TARGET_POS_MAX_RANGE, HitFromType.LazerBolt);

                SkillBase clone = Managers.Object.Spawn<SkillBase>(spawnPos: Vector3.zero, templateID: this.Data.TemplateID,
                        spawnObjectType: ObjectType.Skill, isPooling: true);
                clone.InitClone(this.Owner, this.Data);
                if (spawnPos == Vector3.zero)
                    spawnPos = Utils.GetRandomPosition(this.Owner.transform.position);
                clone.transform.position = spawnPos;

                // ContinuousSpacing -> Spacing
                yield return new WaitForSeconds(this.Data.Spacing);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            cc.OnDamaged(attacker: this.Owner, from: this);
            cc.IsHitFrom_LazerBolt = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            cc.ResetHitFrom(HitFromType.LazerBolt, 0.1f);
        }

        protected override void SetSortingOrder() 
            => GetComponent<SortingGroup>().sortingOrder = (int)SortingOrder.Skill;
    }
}

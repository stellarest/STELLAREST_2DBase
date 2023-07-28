using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class ThrowingStar : RepeatSkill
    {
        public override void SetSkillInfo(CreatureController owner, int templateID)
        {
            base.SetSkillInfo(owner, templateID);
            GetComponent<SpriteRenderer>().sortingOrder = (int)Define.SortingOrder.Skill;

            if (owner?.IsPlayer() == true)
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
        }

        protected override void DoSkillJob()
        {
            StartCoroutine(GenerateThrowingStart());
        }

        private IEnumerator GenerateThrowingStart()
        {
            for (int i = 0; i < SkillData.ContinuousCount; ++i)
            {
                ProjectileController pc = Managers.Object.Spawn<ProjectileController>(Owner.transform.position,
                                        SkillData.TemplateID);
                pc.CurrentSkill = this;
                pc.SetProjectileInfo(Owner, this.SkillData, Managers.Game.Player.ShootDir, 0, Vector3.zero, transform.position,
                            transform.localScale);
                yield return new WaitForSeconds(SkillData.ContinuousSpacing);
            }
        }

        public override void OnPreSpawned()
        {
            base.OnPreSpawned();
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
        }
    }
}


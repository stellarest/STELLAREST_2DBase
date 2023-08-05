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
            SkillType = Define.TemplateIDs.SkillType.ThrowingStar;

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
                
                // TODO : 개선 필요
                pc.GetComponent<ThrowingStar>().SetSkillInfo(Owner, SkillData.TemplateID);
                pc.SetProjectileInfo(this.Owner, this, Managers.Game.Player.ShootDir, 
                    Owner.transform.position, pc.transform.localScale, Vector3.zero);

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


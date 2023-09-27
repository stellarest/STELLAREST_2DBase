// using System.Collections;
// using System.Collections.Generic;
using System;
using System.Collections;
using STELLAREST_2D.Data;
using Unity.VisualScripting;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Rendering;

namespace STELLAREST_2D
{
    public class ThrowingStar : RepeatSkill
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
                base.InitClone(owner, data);
                this.IsFirstPooling = false;
            }
        }

        protected override void SetSortingGroup()
            => GetComponent<SpriteRenderer>().sortingOrder = (int)Define.SortingOrder.Skill;
    }
}

// namespace STELLAREST_2D
// {
//     public class ThrowingStar : RepeatSkill
//     {
//         public override void SetSkillInfo(CreatureController owner, int templateID)
//         {
//             base.SetSkillInfo(owner, templateID);
//             GetComponent<SpriteRenderer>().sortingOrder = (int)Define.SortingOrder.Skill;
//             SkillType = Define.TemplateIDs.SkillType.ThrowingStar;

//             if (owner?.IsPlayer() == true)
//                 Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
//         }

//         protected override void DoSkillJob()
//         {
//             StartCoroutine(GenerateThrowingStart());
//         }

//         private IEnumerator GenerateThrowingStart()
//         {
//             for (int i = 0; i < SkillData.ContinuousCount; ++i)
//             {
//                 ProjectileController pc = Managers.Object.Spawn<ProjectileController>(Owner.transform.position,
//                                         SkillData.TemplateID);

//                 pc.GetComponent<ThrowingStar>().SetSkillInfo(Owner, SkillData.TemplateID);
//                 pc.SetProjectileInfo(this.Owner, this, Managers.Game.Player.ShootDir, 
//                     Owner.transform.position, pc.transform.localScale, Vector3.zero);

//                 yield return new WaitForSeconds(SkillData.ContinuousSpacing);
//             }
//         }

//         public override void OnPreSpawned()
//         {
//             base.OnPreSpawned();
//             GetComponent<SpriteRenderer>().enabled = false;
//             GetComponent<Collider2D>().enabled = false;
//         }
//     }
// }


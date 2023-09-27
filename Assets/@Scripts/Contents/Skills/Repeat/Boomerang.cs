// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

using Assets.FantasyMonsters.Scripts.Utils;
using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    public class Boomerang : RepeatSkill
    {
        [SerializeField] private AnimationCurve _curve = null;
        public AnimationCurve Curve => _curve;

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
                base.InitClone(ownerFromOrigin, dataFromOrigin);
                if (this.Data.Grade == this.Data.MaxGrade)
                    transform.GetChild(0).GetComponent<SpriteTrail.SpriteTrail>().enabled = false;

                this.IsFirstPooling = false;
            }
        }

        protected override void SetSortingGroup()
        {
            if (GetComponent<SpriteRenderer>() != null)
                GetComponent<SpriteRenderer>().sortingOrder = (int)Define.SortingOrder.Skill;
            else
            {
                foreach( var SR in GetComponentsInChildren<SpriteRenderer>())
                {
                    if (SR != null)
                    {
                        SR.sortingOrder = (int)Define.SortingOrder.Skill;
                        return;
                    }
                }

                Utils.LogCritical(nameof(Boomerang), nameof(SetSortingGroup), "Failed to SetSortingGroup.");
            }
        }
    }
}


// namespace STELLAREST_2D
// {
//     public class Boomerang : RepeatSkill
//     {
//         public override void SetSkillInfo(CreatureController owner, int templateID)
//         {
//             base.SetSkillInfo(owner, templateID);
//             SkillType = Define.TemplateIDs.SkillType.Boomerang;

//             if (GetComponent<SpriteRenderer>() != null)
//                 GetComponent<SpriteRenderer>().sortingOrder = (int)Define.SortingOrder.Skill;
//             else
//                 GetComponentInChildren<SpriteRenderer>().sortingOrder = (int)Define.SortingOrder.Skill;

//             if (owner?.IsPlayer() == true)
//             {
//                 if (SkillData.InGameGrade != Define.InGameGrade.Legendary)
//                     Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
//                 else
//                 {
//                     GameObject go = GetComponentInChildren<Collider2D>().gameObject;
//                     Managers.Collision.InitCollisionLayer(go, Define.CollisionLayers.PlayerAttack);
//                 }
//             }
//         }

//         protected override void DoSkillJob()
//         {
//             StartCoroutine(GenerateBoomerang());
//         }

//         private IEnumerator GenerateBoomerang()
//         {
//             for (int i = 0; i < SkillData.ContinuousCount; ++i)
//             {
//                 ProjectileController pc = Managers.Object.Spawn<ProjectileController>(Owner.transform.position,
//                         SkillData.TemplateID);
                        
//                 // pc.SetSkillInfo(Owner, this);
//                 // TODO : 개선 필요
//                 pc.GetComponent<Boomerang>().SetSkillInfo(Owner, SkillData.TemplateID);
                
//                 pc.SetProjectileInfo(this.Owner, this, Managers.Game.Player.ShootDir,
//                     Owner.transform.position, pc.transform.localScale, Vector3.zero);

//                 yield return new WaitForSeconds(SkillData.ContinuousSpacing);
//             }
//         }

//         public override void OnPreSpawned()
//         {
//             base.OnPreSpawned();

//             if (GetComponent<SpriteRenderer>() != null)
//                 GetComponent<SpriteRenderer>().enabled = false;
//             else
//             {
//                 GetComponentInChildren<SpriteRenderer>().enabled = false;
//                 GetComponentInChildren<SpriteTrail.SpriteTrail>().enabled = false;
//             }

//             if (GetComponent<Collider2D>() != null)
//                 GetComponent<Collider2D>().enabled = false;
//             else
//             {
//                 GetComponentInChildren<Collider2D>().enabled = false;
//                 GetComponentInChildren<SpriteTrail.SpriteTrail>().enabled = false;
//             }
//         }
//     }
// }

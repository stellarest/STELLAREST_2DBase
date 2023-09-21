// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

using UnityEngine;

namespace STELLAREST_2D
{
    public class RangedShot : RepeatSkill
    {
        public override void InitRepeatSkill(RepeatSkill originRepeatSkill)
        {
            throw new System.NotImplementedException();
        }

        public override void SetParticleInfo(Vector3 startAngle, Define.LookAtDirection lookAtDir, float continuousAngle, float continuousFlipX, float continuousFlipY)
        {
        }

        protected override void DoSkillJob()
        {
        }
    }
}

// namespace STELLAREST_2D
// {
//     public class RangedShot : RepeatSkill
//     {
//         public override void SetSkillInfo(CreatureController owner, int templateID)
//         {
//             base.SetSkillInfo(owner, templateID);
//             if (owner?.IsPlayer() == true)
//                 Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
//         }

//         protected override void DoSkillJob()
//         {
//             Managers.Game.Player.CreatureState = Define.CreatureState.Attack;
//         }
  
//         public override void OnPreSpawned()
//         {
//             base.OnPreSpawned();

//             GetComponent<Rigidbody2D>().simulated = false;
//             GetComponent<Collider2D>().enabled = false;
//             GetComponentInChildren<SpriteRenderer>().enabled = false;
//         }
//     }
// }

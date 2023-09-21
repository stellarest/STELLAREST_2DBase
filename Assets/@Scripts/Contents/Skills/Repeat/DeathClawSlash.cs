// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

using UnityEngine;

namespace STELLAREST_2D
{
    public class DeathClawSlash : MonoBehaviour
    {
    }
}

// namespace STELLAREST_2D
// {
//     public class DeathClawSlash : MonoBehaviour
//     {
//         private DeathClaw _parent = null;
//         public DeathClaw Parent
//         {
//             get => _parent;
//             set
//             {
//                 if (_parent == null)
//                 {
//                     _parent = value;
//                     if (_parent.Owner?.IsPlayer() == true)
//                         Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
//                 }
//             }
//         }

//         private void OnTriggerEnter2D(Collider2D other)
//         {
//             MonsterController mc = other.GetComponent<MonsterController>();
//             if (mc.IsValid() == false)
//                 return;

//             if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.MonsterBody, other.gameObject.layer))
//             {
//                 mc.OnDamaged(Parent.Owner, Parent);
//             }
//         }

//         public void OnParticleSystemStopped()
//                 => Managers.Resource.Destroy(gameObject);
//     }
// }

// using System.Collections;
// using System.Collections.Generic;
// using System.Net.NetworkInformation;
// using UnityEngine;
// using UnityEngine.Animations;

using UnityEngine;

namespace STELLAREST_2D
{
    public class BombTrapLegendaryChild : MonoBehaviour
    {
    }
}

// namespace STELLAREST_2D
// {
//     public class BombTrapLegendaryChild : MonoBehaviour
//     {
//         private BombTrap _parent = null;
//         private Collider2D _explosionCol = null;

//         private void OnEnable()
//         {
//             if (_parent == null)
//             {
//                 _parent = transform.parent.GetComponent<BombTrap>();
//                 if (_parent.Owner?.IsPlayer() == true)
//                     Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
//             }

//             if (_explosionCol == null)
//                 _explosionCol = GetComponent<Collider2D>();
//             _explosionCol.enabled = true;

//             StartCoroutine(CoDisableCollider());
//         }

//         private IEnumerator CoDisableCollider()
//         {
//             float t = 0f;
//             while (true)
//             {
//                 t += Time.deltaTime;
//                 if (t > 1f)
//                     _explosionCol.enabled = false;

//                 yield return null;
//             }
//         }


//         public void OnParticleSystemStopped()
//         {
//             _parent.Generator.Count--;
//             _parent.IsOnStepped = false;
//             Managers.Resource.Destroy(_parent.gameObject);
//         }

//         private void OnTriggerEnter2D(Collider2D other)
//         {
//             MonsterController mc = other.GetComponent<MonsterController>();
//             if (mc.IsValid() == false)
//                 return;

//             // if (mc[Define.CCType.Stun] && mc.GoCCEffect != null)
//             //     return;

//             if (mc.CreatureData.Hp > 0)
//             {
//                 if (_parent.SkillData.HasCC)
//                 {
//                     if (Random.Range(0f, 1f) <= _parent.SkillData.CCRate)
//                         Managers.CC.ApplyCC<MonsterController>(mc, _parent.SkillData);
//                 }

//                 // Managers.CC.ApplyCC<MonsterController>(mc, Define.CCState.Stun, 3f);
//                 // Managers.CC.ApplyStun<MonsterController>(mc, 3f);
//                 // Define.CCType ccType = _parent.SkillData.CCType;
//                 // float ccRate = _parent.SkillData.CCRate;
//                 // float duration = _parent.SkillData.CCDuration;
//                 // if (Random.Range(0f, 1f) <= ccRate)
//                 //     Managers.CC.ApplyCC<MonsterController>(mc, ccType, duration, Vector3.zero);
//             }
//         }
//     }
// }

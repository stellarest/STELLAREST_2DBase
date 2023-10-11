// using System.Collections;
// using System.Collections.Generic;
// using STELLAREST_2D.Data;
// using UnityEditor.Animations;
// using UnityEngine;

using Unity.Collections;
using UnityEngine;

namespace STELLAREST_2D
{
    public class BombTrapChild : BombTrap
    {
        public void Init(CreatureController owner, Data.SkillData data)
        {
            this.Owner = owner;
            this.Data = data;

            RigidBody = GetComponent<Rigidbody2D>();
            RigidBody.simulated = false;

            HitCollider = GetComponent<Collider2D>();
            HitCollider.isTrigger = true;
            HitCollider.enabled = false;

            base.SetClonedRootTargetOnParticleStopped();
            GetComponent<ParticleSystemRenderer>().sortingOrder = (int)Define.SortingOrder.EnvEffect;
            if (this.Owner?.IsPlayer() == true)
                Managers.Collision.InitCollisionLayer(this.gameObject, Define.CollisionLayers.PlayerAttack);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            cc?.OnDamaged(this.Owner, this);
        }
    }
}

// namespace STELLAREST_2D
// {
//     public class BombTrapChild : MonoBehaviour
//     {
//         private BombTrap _parent = null;
//         private Collider2D _explosionCol = null;
//         private GameObject _redSmokeExplosion = null;

//         private void OnEnable()
//         {
//             if (_explosionCol == null)
//                 _explosionCol = GetComponent<Collider2D>();
//             _explosionCol.enabled = true;

//             if (_parent == null)
//                 _parent = transform.parent.GetComponent<BombTrap>();

//             if (_parent.SkillData.InGameGrade == Define.InGameGrade.Legendary)
//             {
//                 if (_redSmokeExplosion == null)
//                     _redSmokeExplosion = _parent.transform.GetChild((int)BombTrap.Child.Explosion + 1).gameObject;
//                 _redSmokeExplosion.SetActive(true);
//             }

//             if (_parent.Owner?.IsPlayer() == true)
//                 Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);

//             StartCoroutine(CoDisableCollider());
//         }

//         private IEnumerator CoDisableCollider()
//         {
//             float t = 0f;
//             while (true)
//             {
//                 t += Time.deltaTime;
//                 if (t > 0.1f)
//                     _explosionCol.enabled = false;

//                 yield return null;
//             }
//         }

//         public void OnParticleSystemStopped()
//         {
//             if (_parent.SkillData.InGameGrade != Define.InGameGrade.Legendary)
//             {
//                 _parent.Generator.Count--;
//                 _parent.IsOnStepped = false;
//                 Managers.Resource.Destroy(_parent.gameObject);
//             }
//         }

//         private void OnTriggerEnter2D(Collider2D other)
//         {
//             MonsterController mc = other.GetComponent<MonsterController>();
//             if (mc.IsValid() == false)
//                 return;

//             if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.MonsterBody, other.gameObject.layer))
//             {
//                 mc.OnDamaged(_parent.Owner, _parent);
//                 // +++ CC 적용시 주의 사항은, 데미지를 먼저 전달해야 한다 +++
//                 // if (_parent.SkillData.InGameGrade == Define.InGameGrade.Legendary)
//                 // {
//                 //     if (mc.CharaData.Hp > 0)
//                 //         Managers.CC.ApplyCC(mc, Define.CCStatus.Stun, 3f);
//                 // }
//             }
//         }
//     }
// }

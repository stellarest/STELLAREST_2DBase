// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

using System;
using DG.Tweening;
using UnityEngine;

namespace STELLAREST_2D
{
    public class BodyAttack : SequenceSkill
    {
        public override void DoSkill(Action callback = null)
        {
        }
    }
}

// namespace STELLAREST_2D
// {
//     public class BodyAttack : SequenceSkill
//     {
//         private Coroutine _coroutine;
//         private Vector3 _startAttackPoint;
//         private CircleCollider2D _attackCol;

//         public override void SetSkillInfo(CreatureController owner, int templateID)
//         {
//             base.SetSkillInfo(owner, templateID);
//             transform.localPosition = Vector3.zero;
//             transform.localScale = Vector3.one;

//             _attackCol = GetComponent<CircleCollider2D>();
//             _attackCol.enabled = false;

//             if (Owner?.IsMonster() == true)
//                 Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.MonsterAttack);
//         }

//         public override void DoSkill(System.Action callback = null)
//         {
//             if (_coroutine != null)
//                 StopCoroutine(_coroutine);

//             _coroutine = StartCoroutine(CoBodyAttack(callback));
//         }

//         private bool _returnBody = false;
//         private IEnumerator CoBodyAttack(System.Action callback)
//         {
//             _startAttackPoint = Owner.transform.position;
//             Vector3 target = Managers.Game.Player.transform.position;

//             float elapsedTime = 0f;
//             float desiredReachTime = 0.15f;
//             float percent = 0f;
//             //float endWait = 1f;
//             float endWait = 0.5f;

//             Managers.Sprite.SetMonsterFace(Owner as MonsterController, Define.MonsterFace.Angry);
//             mc.CreatureState = Define.CreatureState.Attack;
//             _attackCol.enabled = true;
//             while (percent < 1f)
//             {
//                 elapsedTime += Time.deltaTime;
//                 percent = elapsedTime / desiredReachTime;
//                 if (percent < 1f)
//                     Owner.transform.position = Vector3.Lerp(_startAttackPoint, target, percent);
//                 else
//                 {
//                     _attackCol.enabled = false;
//                     yield return CoBodyReturn(callback, _startAttackPoint, target);
//                     yield return new WaitUntil(() => _returnBody);
//                     mc.CreatureState = Define.CreatureState.Idle;
//                     yield return new WaitForSeconds(endWait);
//                     Managers.Sprite.SetMonsterFace(Owner as MonsterController, Define.MonsterFace.Normal);
//                     mc.CreatureState = Define.CreatureState.Run;
//                     yield break;
//                 }

//                 yield return null;
//             }
//         }

//         private IEnumerator CoBodyReturn(System.Action callback, Vector3 initPos, Vector3 target)
//         {
//             float elapsedTime = 0f;
//             float desiredReturnTime = 0.25f;
//             float percent = 0f;
//             while (percent < 1f)
//             {
//                 elapsedTime += Time.deltaTime;
//                 percent = elapsedTime / desiredReturnTime;
//                 Owner.transform.position = Vector3.Lerp(target, initPos, percent);
//                 yield return null;
//             }
//             _returnBody = true;
//             // Owner.BodyCol.enabled = true;
//         }

//         private void OnTriggerEnter2D(Collider2D other)
//         {
//             if (Owner.IsValid() == false || Managers.Game.Player.IsValid() == false)
//                 return;

//             if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.PlayerBody, other.gameObject.layer))
//             {
//                 Managers.Game.Player.OnDamaged(Owner, this);
//             }
//         }

//         public override void OnPreSpawned() => base.OnPreSpawned();
//     }
// }

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    // 몬스터에서만 사용하는 MeleeAttack
    // None Prefab
    public class BodyAttack : SequenceSkill
    {
        private Coroutine _coroutine;
        private Vector3 _initPos;

        public override void DoSkill(System.Action callback = null)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(CoBodyAttack(callback));
        }

        private bool _returnBody = false;
        private IEnumerator CoBodyAttack(System.Action callback)
        {
            _initPos = transform.position;
            Vector3 target = Managers.Game.Player.transform.position;

            float elapsedTime = 0f;
            float desiredReachTime = 0.15f;
            float percent = 0f;

            float endWait = 1f;

            mac.AngryFace();
            mc.MonsterState = Define.MonsterState.Attack;
            Owner.AttackCol.enabled = true;
            while (percent < 1f)
            {
                elapsedTime += Time.deltaTime;
                percent = elapsedTime / desiredReachTime;
                if (percent < 1f)
                    transform.position = Vector3.Lerp(_initPos, target, percent);
                else
                {
                    yield return CoBodyReturn(callback, _initPos, target);
                    yield return new WaitUntil(() => _returnBody);
                    mc.MonsterState = Define.MonsterState.Idle;
                    yield return new WaitForSeconds(endWait);
                    mac.DefaultFace();
                    mc.MonsterState = Define.MonsterState.Run;
                    yield break;
                }

                yield return null;
            }
        }

        private IEnumerator CoBodyReturn(System.Action callback, Vector3 initPos, Vector3 target)
        {
            float elapsedTime = 0f;
            float desiredReturnTime = 0.25f;
            float percent = 0f;
            while (percent < 1f)
            {
                elapsedTime += Time.deltaTime;
                percent = elapsedTime / desiredReturnTime;
                transform.position = Vector3.Lerp(target, initPos, percent);
                yield return null;
            }
            _returnBody = true;
        }
    }


      // TEMP
        // private IEnumerator CoBodyAttack_PingPong(System.Action callback)
        // {
        //     _initPos = transform.position;
        //     Vector2 target = Managers.Game.Player.transform.position;

        //     _curve.AddKey(0, 0);
        //     _curve.AddKey(0.25f, 1);
        //     _curve.preWrapMode = WrapMode.PingPong;
        //     _curve.postWrapMode = WrapMode.PingPong;
            
        //     float elapsedTime = 0f;
        //     float desiredTime = 1f;
        //     float percent = 0f;
        //     while (percent < 1f)
        //     {
        //         elapsedTime += Time.deltaTime;
        //         percent = elapsedTime / desiredTime;
        //         transform.position = Vector3.Lerp(_initPos, target, _curve.Evaluate(percent));
        //         yield return null;
        //     }

        //     mc.MonsterState = Define.MonsterState.Idle;
        //     yield return new WaitForSeconds(3f);
        //     mc.MonsterState = Define.MonsterState.Run;

        //     yield return null;
        // }
}

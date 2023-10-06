using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class SkillMove : SequenceSkill
    {
        private Rigidbody2D _rigidBody;
        private Coroutine _corouine;

        private void Awake()
        {
        }

        public override void DoSkillJob(Action callback = null)
        {
            if (_corouine != null)
                StopCoroutine(_corouine);
            
            _corouine = StartCoroutine(CoMove(callback));
        }

        // public float Speed { get; } = 2.0f;
        public string AnimationName { get; } = "Moving";

        private IEnumerator CoMove(Action callback = null)
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            GetComponent<Animator>().Play(AnimationName);
            float elapsed = 0f;

            while (true)
            {
                elapsed += Time.deltaTime;
                if (elapsed > 5f) // 데이터 시트로 빼야함. 5초동안 뚜벅뚜벅 플레이어에게 걸어감
                    break; // 5f말고 데이터 시트로 랜덤하게 설정해주는게 조금 더 이쁘긴 할것임
                
                Vector3 dir = ((Vector2)Managers.Game.Player.transform.position - _rigidBody.position).normalized;
                Vector2 targetPosition = Managers.Game.Player.transform.position + dir * UnityEngine.Random.Range(1, 4);

                if (Vector3.Distance(_rigidBody.position, targetPosition) <= 0.2f)
                    continue;

                Vector2 dirVec = targetPosition - _rigidBody.position;
                Vector2 nextVec = dirVec.normalized * 1f * Time.fixedDeltaTime; // Speed가 제대로 설정되어있지 않아서 1f 일단 때려박음
                _rigidBody.MovePosition(_rigidBody.position + nextVec);

                yield return null;
            }

            callback?.Invoke();
        }
    }
}

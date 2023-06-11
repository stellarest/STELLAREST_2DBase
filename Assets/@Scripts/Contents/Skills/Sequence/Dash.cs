using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class Dash : SequenceSkill
    {
        private Rigidbody2D _rigidBody;
        private Coroutine _coroutine;

        // 데이터시트 당연히 연동해야함. 하지만 처음부터 너무 데이터 시트 연동을 설계하기보다는
        // 스킬을 만들다보면 들어가는 정보들이 엄청 많아지는데 그것들을 언젠가 통합해서
        // 위로 올려서 데이터를 통합하면서 관리하거나. 천천히
        // 애니메이션 네임, 사운드, 카메라가 흔들리는 강도 이런것들도 데이터시트화 해야됨 어차피 나중에
        public float WaitTime { get; } = 1f;
        public float DashSpeed { get; } = 10f;
        public string AnimationName { get; } = "Charge";

        public override void DoSkill(Action callback = null)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            
            _coroutine = StartCoroutine(CoDash(callback));
        }

        private IEnumerator CoDash(Action callback)
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            yield return new WaitForSeconds(2f); // 처음에 멍때리다가 돌진할 수 있도록
            GetComponent<Animator>().Play(AnimationName);

            Vector3 dir = ((Vector2)Managers.Game.Player.transform.position - _rigidBody.position).normalized;
            // 플레이어의 위치보다 조금 더 가도록  // + (2f) 플레이어의 충돌박스 만큼으로 나중에 설정
            Vector2 targetPos = Managers.Game.Player.transform.position + dir * UnityEngine.Random.Range(1, 5);

            // Vector3.Distance(_rigidBody.position, targetPos) > 0.2f루트계산까지 하기 때문에 바꿔야함
            // dir.sqrMagnitude > 0.04f
            while (Vector3.Distance(_rigidBody.position, targetPos) > 0.2f) // Dash가 끝날때까지 체크
            {
                Vector2 dirVec = targetPos - _rigidBody.position;
                Vector2 nextVec = dirVec.normalized * 5f * Time.fixedDeltaTime; // Speed가 제대로 설정되어있지 않아서 5f 일단 때려박음
                _rigidBody.MovePosition(_rigidBody.position + nextVec);

                yield return null;
            }

            callback?.Invoke();
        }
    }
}

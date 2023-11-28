using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Jobs;

namespace STELLAREST_2D
{
    public class Dash : ActionSkill
    {
#region Dash Sample
        float dashDistance = 5f;
        private void DashSample()
        {
            // 로그함수를 통해 점차 자연스럽게 멈추게 됨
            // 이 공식을 사용하는 이유는 현재 설정된 리지드바디의 저항값을 로그 함수화 시켜서
            // 점차적으로 느려지는 것을 볼 수 있게 Dash에 대한 Velocity를 계산한 것.
            Vector3 dashVelocity = Vector3.Scale(transform.forward, 
                dashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * RigidBody.drag + 1))  / -Time.deltaTime), 
                0, 
                (Mathf.Log(1f / (Time.deltaTime * RigidBody.drag + 1))  / -Time.deltaTime)));
            RigidBody.AddForce(dashVelocity, ForceMode2D.Force); // ForceMode.VelocityChange

            /*
              RigidBody.velocity = Vector3.Scale(DashToDir.normailzed, 
                dashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * RigidBody.drag + 1))  / -Time.deltaTime), 
                (Mathf.Log(1f / (Time.deltaTime * RigidBody.drag + 1))  / -Time.deltaTime)), 
                0);
                
            */
        }

#endregion

        private Rigidbody2D _rigidBody;
        private Coroutine _coroutine;

        // 데이터시트 당연히 연동해야함. 하지만 처음부터 너무 데이터 시트 연동을 설계하기보다는
        // 스킬을 만들다보면 들어가는 정보들이 엄청 많아지는데 그것들을 언젠가 통합해서
        // 위로 올려서 데이터를 통합하면서 관리하거나. 천천히
        // 애니메이션 네임, 사운드, 카메라가 흔들리는 강도 이런것들도 데이터시트화 해야됨 어차피 나중에
        public float WaitTime { get; } = 1f;
        public float DashSpeed { get; } = 10f;
        public string AnimationName { get; } = "Charge";

        protected override IEnumerator CoStartSkill()
        {
            DoSkillJob();
            yield return null;
        }

        protected override void DoSkillJob(Action callback = null)
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

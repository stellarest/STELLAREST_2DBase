using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class BossController : MonsterController
    {
        public override bool Init()
        {
            base.Init();

            //CreatureState = Define.GameData.CreatureState.Moving;

            // 무조건 일단 Skill로 시작 가능
            CreatureState = Define.CreatureState.Skill;
            // Skills.AddSkill<BossMove>(transform.position);
            // Skills.AddSkill<Dash>(transform.position);
            // // 만약에 3단 대쉬를 만든다면? 
            // // Dash 내부적으로 수정해주거나
            // Skills.AddSkill<Dash>(transform.position);
            // Skills.AddSkill<Dash>(transform.position); // 이런식으로 대쉬를 두번 더 추가해주면 됨
            SkillBook.StartNextSequenceSkill();

            // 그래서 보스는 SequenceSkill 방식으로 만들고 일반 쫄몹은 원래 썼던 State Pattern이나
            // 그냥 플레이어만 졸졸졸 쫓아다니는 방식으로 만들어도 되고 진짜로 진짜로 마음대로 만들어주면 됨

            return true;
        }
        
        public override void UpdateAnimation()
        {
            // base.UpdateAnimation(); 필요없
            switch (CreatureState)
            {
                case Define.CreatureState.Idle:
                    _animator.Play("Idle");
                    break;

                case Define.CreatureState.Moving:
                    _animator.Play("Moving");
                    break;

                case Define.CreatureState.Skill:
                    //_animator.Play("Attack"); // 일단 Attack
                    // 어차피 Skill별로 애니메이션이 재생이 될것이므로 (탕탕이니까 가능)
                    break;

                case Define.CreatureState.Dead:
                    _animator.Play("Death");
                    break;
            }
        }

        // 아래와 같이 UpdateState가 없어도 이제 항상 스킬 상태로 모든 것을 관리할 수 있게 되었다 (탕탕이니까 가능한 것)
        // // Boss Collider의 영역 + Player Collider의 영역으로 정하던지 데이터 시트로 빼서 정하던지
        // private float _range = 2f;
        // // 필요한건 재정의하고 필요없는건 걍 안만들고 냅둠. Idle은 사용 안할 예정
        // protected override void UpdateMoving()
        // {
        //     // base.UpdateMoving(); 필요없
        //     PlayerController pc = Managers.Object.Player;
        //     if (pc.IsValid() == false)
        //         return;

        //     Vector3 dir = pc.transform.position - transform.position;
        //     if (dir.magnitude < _range)
        //     {
        //         // 평타 또는 돌진 스킬 등을 랜덤으로 날림
        //         CreatureState = Define.GameData.CreatureState.Skill;

        //         // _animator.runtimeAnimatorController.animationClips; // 이런식으로 얻어올 수 있다고 함
        //         // 아니면 상수로 때려 박던지
        //         float animLength = 0.41f;
        //         Wait(animLength);
        //     }
        // }

        // // deltaTime이용해서 일정 시간 지나면 다시 Moving으로 돌아오게하던지. 코루틴 쓰던지
        // protected override void UpdateSkill()
        // {
        //     GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        //     if (_coWait == null)
        //         CreatureState = Define.GameData.CreatureState.Moving;
        // }

        protected override void UpdateDead()
        {
            Debug.Log("##### UPDATE DEAD #####");
            if (_coWait == null)
            {
                Managers.Object.Despawn(this);
                Debug.Log("<color=red> DESPAWN BOSS </color>");
            }
        }

        #region Wait Coroutine

        private Coroutine _coWait;
        private void Wait(float waitSeconds)
        {
            if (_coWait != null)
                StopCoroutine(_coWait);

            _coWait = StartCoroutine(CoStartWait(waitSeconds));
        }

        private IEnumerator CoStartWait(float waitSeconds)
        {
            yield return new WaitForSeconds(waitSeconds);
            _coWait = null; // CoRoutine Null 체크를 통해서 기다리는 것이 끝났는지 여부를 판단할 수 있다.
        }

        #endregion

        public override void OnDamaged(BaseController attacker, SkillBase skill, int damage)
        {
            base.OnDamaged(attacker, skill, damage);
        }

        protected override void OnDead()
        {
            CreatureState = Define.CreatureState.Dead; // 애니메이션부터 틀어주고
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Debug.Log("### PLAY BOSS DEATH ANIM ###");
            Wait(2.0f);
        }
    }
}

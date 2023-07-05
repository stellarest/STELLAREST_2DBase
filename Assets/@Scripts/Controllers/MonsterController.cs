using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace STELLAREST_2D
{
    public class MonsterController : CreatureController
    {
        #region State Pattern
        private Define.CreatureState _creatureState = Define.CreatureState.Moving;
        public virtual Define.CreatureState CreatureState
        {
            get => _creatureState;
            set
            {
                _creatureState = value;
                UpdateAnimation();
            }
        }

        private Define.MonsterState _monsterState = Define.MonsterState.Run;
        public Define.MonsterState MonsterState
        {
            get => _monsterState;
            set
            {
                _monsterState = value;
                UpdateMonsterAnimation();
            }
        }

        public bool LockFlipX { get; set; } = false;

        // 몬스터마다 재정의 가능
        protected Animator _animator;
        public virtual void UpdateAnimation()
        {
        }

        public virtual void UpdateMonsterAnimation()
        {
            switch (_monsterState)
            {
                case Define.MonsterState.Idle:
                    MAC.Idle();
                    break;

                case Define.MonsterState.Run:
                    MAC.Run();
                    break;

                case Define.MonsterState.Skill:
                    SkillBook.StartNextSequenceSkill();
                    break;

                case Define.MonsterState.Attack:
                    MAC.Attack();
                    break;
            }
        }

        // 일반 몬스터도 나중에 이거에 맞게 바꿔야함
        // 필요한 부분들만 쏙쏙 골라서 오버라이드해서 고치면 됨
        // public override void UpdateController()
        // {
        //     // base.UpdateController();

        //     switch (CreatureState)
        //     {
        //         case Define.CreatureState.Idle:
        //             {
        //                 UpdateIdle(); // 무조건 딴건 안하고 UpdateIdle만 실행
        //             }
        //             break;

        //         case Define.CreatureState.Moving:
        //             {
        //                 UpdateMoving();
        //             }
        //             break;

        //         case Define.CreatureState.Skill:
        //             {
        //                 UpdateSkill();
        //             }
        //             break;

        //         case Define.CreatureState.Dead:
        //             {
        //                 UpdateDead();
        //             }
        //             break;
        //     }
        // }

        protected virtual void UpdateIdle() { }
        protected virtual void UpdateMoving() { }
        protected virtual void UpdateSkill() { }
        protected virtual void UpdateDead() { }

        #endregion

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = Define.ObjectType.Monster;
            Debug.Log("### MC INIT ###");

            // _animator = GetComponent<Animator>();
            // ObjectType = Define.ObjectType.Monster;
            // CreatureState = Define.CreatureState.Moving;
            MAC = gameObject.GetOrAddComponent<MonsterAnimationController>();
            //_monsterState = Define.MonsterState.Run;

            return true;
        }

        public virtual void InitMonsterSkill() { }

        public override void SetInfo(int templateID)
        {
            base.SetInfo(templateID);
            //Managers.Effect.Initi
        }

        protected override void SetSortingGroup()
        {
            GetComponent<SortingGroup>().sortingOrder = (int)Define.SortingOrder.Monster;
        }

        private void FixedUpdate()
        {
            // 물리 기반(FixedUpdate) 이동이라 이 부분은 옮기기 싫다고 한다면 Moving 상태일때만 이 코드가 실행 되도록
            // 이런식으로 편하게 유동적으로 코드를 작성하면됨
            // if (CreatureState != Define.CreatureState.Moving)
            //     return;

            // 일단 Idle 상태에서도 시선은 계속 플레이어를 향하도록..
            // 나중에 스턴 먹으면 그때는 전환이 안되도록
            PlayerController pc = Managers.Game.Player;
            Vector3 toPlayer = pc.transform.position - transform.position;
            FlipX(toPlayer.x > 0 ? -1 : 1);

            if (MonsterState != Define.MonsterState.Run)
                return;

            if (pc.IsValid() == false)
                return;

            Vector3 newPos = transform.position + (toPlayer.normalized * Time.deltaTime * MoveSpeed);
            //transform.position = newPos;
            RigidBody.MovePosition(newPos);

            float sqrDist = _attackRange * _attackRange;
            if (toPlayer.sqrMagnitude <= sqrDist)
                MonsterState = Define.MonsterState.Skill;
        }

        private void FlipX(float flipX)
        {
            transform.localScale = new Vector2(_initScale.x * flipX, _initScale.y);
        }

        public override void OnDamaged(BaseController attacker, SkillBase skill, int damage)
        {
            base.OnDamaged(attacker, skill, damage);
            //Managers.Effect.HitEffect(gameObject);
        }       

        private Coroutine _coBodyAttack;
        // private void OnCollisionEnter2D(Collision2D other)
        // {
        // }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (this.IsValid() == false) // 풀링은 되어 있지만 이미 꺼져있을 경우
                return;

            PlayerController target = other.gameObject.GetComponent<PlayerController>();
            if (target.IsValid() == false)
                return;

            // if (_coBodyAttack != null)
            //     StopCoroutine(_coBodyAttack);

            //_coBodyAttack = StartCoroutine(CoBodyAttack(target));
            target.OnDamaged(this, null, Random.Range(1, 6));
        }

        // private void OnTriggerExit2D(Collider2D other)
        // {
        //     if (this.IsValid() == false) // 풀링은 되어 있지만 이미 꺼져있을 경우
        //         return;

        //     PlayerController target = other.gameObject.GetComponent<PlayerController>();
        //     if (target.IsValid() == false)
        //         return;

        //     if (this.IsValid() == false)
        //         return;

        //     if (_coBodyAttack != null)
        //         StopCoroutine(_coBodyAttack);

        //     _coBodyAttack = null;
        // }

        // private IEnumerator CoBodyAttack(PlayerController target)
        // {
        //     while (true)
        //     {
        //         // *****
        //         // 플레이어는 최소 0.5초에 한 번 도트 데미지를 받는다.
        //         // (브로 포테토 같은 경우 1초정도 되는 것 같긴함)
        //         // 그리고, 플레이어의 이펙트는 0.1초만에 재생됨
        //         target.OnDamaged(this, null, Random.Range(1, 11));
        //         yield return new WaitForSeconds(10f);
        //     }
        // }

        // private void OnCollisionExit2D(Collision2D other)
        // {
        // }

        // private Coroutine _coDotDamage;
        // public IEnumerator CoStartDotDamage(PlayerController target)
        // {
        //     while (true)
        //     {
        //         // *** 데미지는 무조건 피해자쪽에서 처리하는것이 좋다 ***
        //         target.OnDamaged(this, 2); // 도트 데미지 예시..
        //         yield return new WaitForSeconds(0.1f);
        //     }
        // }

        // protected override void OnDead()
        // {
        //     // 보스는 여기 안탐
        //     base.OnDead();
        //     Managers.Game.KillCount++;

        //     if (_coDotDamage != null)
        //         StopCoroutine(_coDotDamage);
        //     _coDotDamage = null;

        //     GemController gc = Managers.Object.Spawn<GemController>(transform.position);

        //     //Managers.Object.Despawn<MonsterController>(this);
        //     Managers.Object.Despawn(this);
        // }
    }
}

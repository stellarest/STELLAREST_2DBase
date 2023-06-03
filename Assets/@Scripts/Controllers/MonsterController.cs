using System.Collections;
using UnityEngine;

namespace STELLAREST_2D
{
    public class MonsterController : CreatureController
    {
        #region State Pattern

        private Define.GameData.CreatureState _creatureState = Define.GameData.CreatureState.Moving;
        public virtual Define.GameData.CreatureState CreatureState
        {
            get => _creatureState;
            set
            {
                _creatureState = value;
                UpdateAnimation();
            }
        }

        // 몬스터마다 재정의 가능
        protected Animator _animator;
        public virtual void UpdateAnimation()
        {
        }

        // 일반 몬스터도 나중에 이거에 맞게 바꿔야함
        // 필요한 부분들만 쏙쏙 골라서 오버라이드해서 고치면 됨
        public override void UpdateController()
        {
            base.UpdateController();

            switch (CreatureState)
            {
                case Define.GameData.CreatureState.Idle:
                    {
                        UpdateIdle(); // 무조건 딴건 안하고 UpdateIdle만 실행
                    }
                    break;

                case Define.GameData.CreatureState.Moving:
                    {
                        UpdateMoving();
                    }
                    break;

                case Define.GameData.CreatureState.Skill:
                    {
                        UpdateSkill();
                    }
                    break;

                case Define.GameData.CreatureState.Dead:
                    {
                        UpdateDead();
                    }
                    break;
            }
        }

        protected virtual void UpdateIdle() { }
        protected virtual void UpdateMoving() { }
        protected virtual void UpdateSkill() { }
        protected virtual void UpdateDead() { }

        #endregion

        private Data.MonsterData _monsterData;
        public Data.MonsterData MonsterData
        {
            get => _monsterData;
            set
            {
                _monsterData = value; // 고유의 몬스터 데이터 전부
                // 크리처 공용 데이터
                this.MaxHp = _monsterData.maxHp;
                this.Hp = this.MaxHp;
                this.MoveSpeed = _monsterData.moveSpeed;
            }
        }

        public override bool Init()
        {
            base.Init();

            _animator = GetComponent<Animator>();
            ObjectType = Define.GameData.ObjectType.Monster;
            CreatureState = Define.GameData.CreatureState.Moving;

            return true;
        }

        private void FixedUpdate()
        {
            // 물리 기반(FixedUpdate) 이동이라 이 부분은 옮기기 싫다고 한다면 Moving 상태일때만 이 코드가 실행 되도록
            // 이런식으로 편하게 유동적으로 코드를 작성하면됨
            if (CreatureState != Define.GameData.CreatureState.Moving)
                return;

            PlayerController pc = Managers.Game.Player;
            if (pc == null)
                return;

            Vector3 toPlayer = pc.transform.position - transform.position;
            Vector3 newPos = transform.position + (toPlayer.normalized * Time.deltaTime * MoveSpeed);
            //transform.position = newPos;
            GetComponent<Rigidbody2D>().MovePosition(newPos);
            GetComponent<SpriteRenderer>().flipX = toPlayer.x > 0;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            PlayerController target = other.gameObject.GetComponent<PlayerController>();
            if (target.IsValid() == false)
                return;
            if (this.IsValid() == false) // 풀링은 되어 있지만 이미 꺼져있을 경우
                return;

            if (_coDotDamage != null)
                StopCoroutine(_coDotDamage);

            _coDotDamage = StartCoroutine(CoStartDotDamage(target));
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            PlayerController target = other.gameObject.GetComponent<PlayerController>();
            if (target.IsValid() == false)
                return;
            if (this.IsValid() == false) // 풀링된(InActive) 상태에서 StartCoroutine 호출하면 안됨
                return;

            if (_coDotDamage != null)
                StopCoroutine(_coDotDamage);
            _coDotDamage = null;
        }

        private Coroutine _coDotDamage;
        public IEnumerator CoStartDotDamage(PlayerController target)
        {
            while (true)
            {
                // *** 데미지는 무조건 피해자쪽에서 처리하는것이 좋다 ***
                target.OnDamaged(this, 2); // 도트 데미지 예시..
                yield return new WaitForSeconds(0.1f);
            }
        }

        protected override void OnDead()
        {
            // 보스는 여기 안탐
            base.OnDead();
            Managers.Game.KillCount++;

            if (_coDotDamage != null)
                StopCoroutine(_coDotDamage);
            _coDotDamage = null;

            GemController gc = Managers.Object.Spawn<GemController>(transform.position);

            //Managers.Object.Despawn<MonsterController>(this);
            Managers.Object.Despawn(this);
        }
    }
}

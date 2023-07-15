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

            // _animator = GetComponent<Animator>();
            // ObjectType = Define.ObjectType.Monster;
            // CreatureState = Define.CreatureState.Moving;
            //_monsterState = Define.MonsterState.Run;
            MAC = gameObject.GetOrAddComponent<MonsterAnimationController>();
            MAC.Owner = this;

            Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.MonsterBody);

            return true;
        }

        public override void SetInfo(int templateID)
        {
            base.SetInfo(templateID);
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

            // 일단 Idle 상태에서도 시선은 계속 플레이어를 향하도록.. 나중에 스턴 먹으면 그때는 전환이 안되도록
            PlayerController pc = Managers.Game.Player;
            if (pc.IsValid() == false)
                return;

            Vector3 toPlayer = pc.transform.position - transform.position;
            FlipX(toPlayer.x > 0 ? -1 : 1);
            if (MonsterState != Define.MonsterState.Run)
                return;

            // Vector3 newPos = transform.position + (toPlayer.normalized * Time.deltaTime * MoveSpeed);
            // //transform.position = newPos;
            // RigidBody.MovePosition(newPos);

            // float sqrDist = Range * Range;
            // if (toPlayer.sqrMagnitude <= sqrDist)
            //     MonsterState = Define.MonsterState.Skill;
        }

        private void FlipX(float flipX)
        {
            transform.localScale = new Vector2(_initScale.x * flipX, _initScale.y);
        }

        public override void OnDamaged(BaseController attacker, SkillBase skill, float damage)
        {
            base.OnDamaged(attacker, skill, damage);
        }       

        // private void OnCollisionEnter2D(Collision2D other)
        // {
        // }

        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     if (this.IsValid() == false) // 풀링은 되어 있지만 이미 꺼져있을 경우
        //         return;

        //     PlayerController target = other.gameObject.GetComponent<PlayerController>();
        //     if (target.IsValid() == false)
        //         return;
                
        //     if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.PlayerBody, other.gameObject.layer))
        //     {
        //         // 일단 SkillData null로.. 처리할게 많네
        //         target.OnDamaged(this, null, 3f);
        //     }
        // }
    }
}

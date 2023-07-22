using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace STELLAREST_2D
{
    public class MonsterController : CreatureController
    {
        private Define.MonsterState _monsterState = Define.MonsterState.Idle;
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

        protected virtual void UpdateIdle() { }
        protected virtual void UpdateMoving() { }
        protected virtual void UpdateSkill() { }
        protected virtual void UpdateDead() { }


        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = Define.ObjectType.Monster;
            MAC = gameObject.GetOrAddComponent<MonsterAnimationController>();
            MAC.Owner = this;

            Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.MonsterBody);

            return true;
        }

        public void CoStartReadyToAction() 
                    => StartCoroutine(CoReadyToAction());

        private IEnumerator CoReadyToAction()
        {
            float delta = 0f;
            float percent = 0f;
            float desiredTime = Random.Range(CharaData.MinReadyToActionTime, CharaData.MaxReadyToActionTime);
            Utils.Log("READY TO ACTION TIME : " + desiredTime.ToString());
            while (percent < 1f)
            {
                delta += Time.deltaTime;
                percent = delta / desiredTime;

                yield return null;
            }

            MonsterState = Define.MonsterState.Run;
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
            PlayerController pc = Managers.Game.Player;
            if (pc.IsValid() == false)
                return;

            Vector3 toPlayer = pc.transform.position - transform.position;
            FlipX(toPlayer.x > 0 ? -1 : 1);
            if (MonsterState != Define.MonsterState.Run)
                return;

            Vector3 newPos = transform.position + (toPlayer.normalized * Time.deltaTime * CharaData.MoveSpeed);
            //transform.position = newPos;
            RigidBody.MovePosition(newPos);

            float sqrDist = 2f * 2f;
            if (toPlayer.sqrMagnitude <= sqrDist)
                MonsterState = Define.MonsterState.Skill;
        }

        private void FlipX(float flipX)
        {
            transform.localScale = new Vector2(_initScale.x * flipX, _initScale.y);
        }

        public override void OnDamaged(BaseController attacker, SkillBase skill)
        {
            base.OnDamaged(attacker, skill);
        }       
    }
}

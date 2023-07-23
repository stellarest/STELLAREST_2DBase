using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace STELLAREST_2D
{
    public class MonsterController : CreatureController
    {
        public bool LockFlipX { get; set; } = false;
        public MonsterAnimationController MAC { get; protected set; }
        public override void UpdateAnimation()
        {
            switch (_cretureState)
            {
                case Define.CreatureState.Idle:
                    MAC.Idle();
                    break;

                case Define.CreatureState.Run:
                    MAC.Run();
                    break;

                case Define.CreatureState.Attack:
                    MAC.Attack();
                    break;

                case Define.CreatureState.Skill:
                    SkillBook.StartNextSequenceSkill();
                    break;

                case Define.CreatureState.Death:
                    MAC.Death();
                    break;
            }
        }

        public void CoStartReadyToAction() 
                => StartCoroutine(CoReadyToAction());
        private IEnumerator CoReadyToAction()
        {
            float delta = 0f;
            float percent = 0f;
            float desiredTime = Random.Range(CharaData.MinReadyToActionTime, CharaData.MaxReadyToActionTime);
            while (percent < 1f)
            {
                delta += Time.deltaTime;
                percent = delta / desiredTime;
                yield return null;
            }

            // CreatureState = Define.CreatureState.Run;
            CreatureState = Define.CreatureState.Death;
        }

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

        public override void SetInfo(int templateID) => base.SetInfo(templateID);
        protected override void SetSortingGroup() 
                => GetComponent<SortingGroup>().sortingOrder = (int)Define.SortingOrder.Monster;

        private void FixedUpdate()
        {
            PlayerController pc = Managers.Game.Player;
            if (pc.IsValid() == false)
                return;

            Vector3 toPlayer = pc.transform.position - transform.position;
            FlipX(toPlayer.x > 0 ? -1 : 1);
            if (CreatureState != Define.CreatureState.Run)
                return;

            Vector3 newPos = transform.position + (toPlayer.normalized * Time.deltaTime * CharaData.MoveSpeed);
            //transform.position = newPos;
            RigidBody.MovePosition(newPos);

            float sqrDist = 2f * 2f;
            if (toPlayer.sqrMagnitude <= sqrDist)
                CreatureState = Define.CreatureState.Skill;
        }

        private void FlipX(float flipX) 
                => transform.localScale = new Vector2(_initScale.x * flipX, _initScale.y);
        

        public override void OnDamaged(BaseController attacker, SkillBase skill) 
                => base.OnDamaged(attacker, skill);
    }
}

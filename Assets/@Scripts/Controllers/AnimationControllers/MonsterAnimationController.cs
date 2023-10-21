using UnityEngine;

namespace STELLAREST_2D
{
    public class MonsterAnimationController : BaseAnimationController
    {
        public override void Init(CreatureController owner)
        {
            base.Init(owner);
        }

        private readonly int ANIM_SPEED = Animator.StringToHash("AnimSpeed");
        private readonly int IDLE = Animator.StringToHash("Idle");
        private readonly int RUN = Animator.StringToHash("Run");
        private readonly int ATTACK = Animator.StringToHash("Attack");
        private readonly int DEAD = Animator.StringToHash("Dead");

        public void SetAnimationSpeed(float speed) => AnimController.SetFloat(ANIM_SPEED, speed);
        public void Idle() => AnimController.Play(IDLE);
        public void Run() => AnimController.Play(RUN);
        public void Attack() => AnimController.Play(ATTACK);
        public void Dead() => AnimController.Play(DEAD);
    }
}


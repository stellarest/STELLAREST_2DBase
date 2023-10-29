using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class NinjaAnimationController : PlayerAnimationController
    {
        private readonly int UPPER_READY = Animator.StringToHash("ReadyMelee1H");
        private readonly int UPPER_ATTACK = Animator.StringToHash("ThrowKunai");
        private readonly int UPPER_ATTACK_ULTIMATE = Animator.StringToHash("ThrowKunai_Ninja_Ultimate");
        private readonly int LOWER_NINJA_RUN = Animator.StringToHash("NinjaRun");
        public override void Init(CreatureController owner) 
        {
            base.Init(owner);
            SetMovementSpeed(2f);
        }

        public override void Run() => AnimController.Play(LOWER_NINJA_RUN);
        public override void Ready() => AnimController.Play(UPPER_READY);
        public override void Attack()
        {
            if (this.Owner.SkillBook.GetFirstSkillGrade() < Define.InGameGrade.Ultimate)
                AnimController.Play(UPPER_ATTACK);
            else
                AnimController.Play(UPPER_ATTACK_ULTIMATE);
        }
    }
}

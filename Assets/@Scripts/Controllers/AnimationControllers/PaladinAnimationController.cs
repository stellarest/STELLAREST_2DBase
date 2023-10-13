using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace STELLAREST_2D
{
    public class PaladinAnimationController : PlayerAnimationController
    {
        private readonly int UPPER_READY = Animator.StringToHash("ReadyMelee1H");
        private readonly int UPPER_RELEASE = Animator.StringToHash("IdleMelee");
        private readonly int UPPER_ATTACK = Animator.StringToHash("SlashMelee1H");

        private readonly int LOWER_DEATH_BACK = Animator.StringToHash("DeathBack");
        private readonly int LOWER_DEATH_FRONT = Animator.StringToHash("DeathFront");

        
        public override void Init(CreatureController owner)
        {
            base.Init(owner);
        }

        public override void Ready() => AnimController.Play(UPPER_READY);
        public override void Release() => AnimController.Play(UPPER_RELEASE);
        public override void Attack() => AnimController.Play(UPPER_ATTACK);
    }
}

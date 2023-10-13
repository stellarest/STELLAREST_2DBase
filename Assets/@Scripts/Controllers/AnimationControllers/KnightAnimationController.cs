using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class KnightAnimationController : PlayerAnimationController
    {
        private readonly int UPPER_READY = Animator.StringToHash("ReadyMelee2H");
        private readonly int UPPER_RELEASE = Animator.StringToHash("IdleMelee");
        private readonly int UPPER_ATTACK = Animator.StringToHash("SlashMelee2H");

        public override void Init(CreatureController owner)
        {
            base.Init(owner);
        }

        public override void Ready() => AnimController.Play(UPPER_READY);
        public override void Release() => AnimController.Play(UPPER_RELEASE);
        public override void Attack() => AnimController.Play(UPPER_ATTACK);
    }
}


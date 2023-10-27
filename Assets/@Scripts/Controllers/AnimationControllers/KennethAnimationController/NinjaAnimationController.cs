using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class NinjaAnimationController : PlayerAnimationController
    {
        private readonly int UPPER_READY = Animator.StringToHash("ReadyMelee1H");
        private readonly int UPPER_ATTACK = Animator.StringToHash("Throw_Kunai");
        public override void Init(CreatureController owner) => base.Init(owner);
        public override void Ready() => AnimController.Play(UPPER_READY);
        public override void Attack() => AnimController.Play(UPPER_ATTACK);
    }
}

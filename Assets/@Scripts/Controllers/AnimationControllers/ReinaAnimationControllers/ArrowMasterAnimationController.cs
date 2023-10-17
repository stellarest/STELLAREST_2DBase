using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class ArrowMasterAnimationController : PlayerAnimationController
    {
        private readonly int UPPER_READY = Animator.StringToHash("ReadyBow");
        private readonly int UPPER_ATTACK = Animator.StringToHash("RangedShot");
        public override void Init(CreatureController owner) => base.Init(owner);
        public override void Idle() => AnimController.Play(UPPER_READY);
        public override void Attack() => AnimController.Play(UPPER_ATTACK);
    }
}


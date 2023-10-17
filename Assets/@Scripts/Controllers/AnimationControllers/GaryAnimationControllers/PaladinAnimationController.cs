using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace STELLAREST_2D
{
    public class PaladinAnimationController : PlayerAnimationController
    {
        private readonly int UPPER_READY = Animator.StringToHash("ReadyMelee1H");
        private readonly int UPPER_ATTACK = Animator.StringToHash("SlashMelee1H");
        public override void Init(CreatureController owner) => base.Init(owner);
        public override void Idle() => AnimController.Play(UPPER_READY);
        public override void Attack() => AnimController.Play(UPPER_ATTACK);
    }
}

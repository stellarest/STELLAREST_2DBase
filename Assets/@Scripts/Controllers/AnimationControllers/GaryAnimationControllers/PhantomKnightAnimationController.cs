using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class PhantomKnightAnimationController : PlayerAnimationController
    {
        private readonly int UPPER_READY = Animator.StringToHash("ReadyMelee1H");
        private readonly int UPPER_RELEASE = Animator.StringToHash("IdleMelee");
        private readonly int UPPER_ATTACK = Animator.StringToHash("SlashMelee1H");
        
        public override void Init(CreatureController owner) => base.Init(owner);

        public override void Ready()
        {
            if (this.IsOnReady == false)
            {
                Utils.Log("ON READY IN ANIM CONTROLLER !!");
                AnimController.Play(UPPER_READY);
                this.IsOnReady = true;
            }
        }
        
        public override void Release() => AnimController.Play(UPPER_RELEASE);
        public override void Attack() => AnimController.Play(UPPER_ATTACK);
    }
}

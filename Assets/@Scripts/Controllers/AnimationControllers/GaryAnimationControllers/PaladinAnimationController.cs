using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_2D.Define;

namespace STELLAREST_2D
{
    public class PaladinAnimationController : PlayerAnimationController
    {
        private readonly int UPPER_READY = Animator.StringToHash("ReadyMelee1H");
        private readonly int UPPER_ATTACK = Animator.StringToHash("SlashMelee1H");
        private readonly int UPPER_ELITE_SEQUENCE = Animator.StringToHash("UseShield");

        public override void Init(CreatureController owner) => base.Init(owner);
        public override void Ready() => AnimController.Play(UPPER_READY);
        public override void RunSkill()
        {
            switch (this.Owner.SkillAnimationType)
            {
                case SkillAnimationType.Attack:
                    AnimController.Play(UPPER_ATTACK);
                    break;

                case SkillAnimationType.ElitePlus:
                    {
                        AnimController.StopPlayback(); // FORCE DEACTIVATE ANIMATION AGAIN.
                        AnimController.Play(UPPER_ELITE_SEQUENCE);
                    }
                    break;

                case SkillAnimationType.UltimatePlus:
                    break;
            }
        }
    }
}

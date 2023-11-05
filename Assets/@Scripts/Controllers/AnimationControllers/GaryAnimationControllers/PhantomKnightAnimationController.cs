using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class PhantomKnightAnimationController : PlayerAnimationController
    {
        private readonly int UPPER_READY = Animator.StringToHash("ReadyMelee1H");
        private readonly int UPPER_ATTACK = Animator.StringToHash("SlashMelee1H");
        private readonly int UPPER_ELITE_SEQUENCE = Animator.StringToHash("UsePhantomSoul");

        public override void Init(CreatureController owner) => base.Init(owner);
        public override void Ready() => AnimController.Play(UPPER_READY);
                public override void RunSkill()
        {
            switch (this.Owner.SkillAnimationType)
            {
                case Define.SkillAnimationType.ExclusiveRepeat:
                    AnimController.Play(UPPER_ATTACK);
                    break;

                case Define.SkillAnimationType.EliteSequence:
                    {
                        AnimController.StopPlayback();
                        AnimController.Play(UPPER_ELITE_SEQUENCE);
                    }
                    break;

                case Define.SkillAnimationType.UltimateSequence:
                    break;
            }
        }
    }
}

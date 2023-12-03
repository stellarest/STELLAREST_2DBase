using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_2D.Define;

namespace STELLAREST_2D
{
    public class NinjaAnimationController : PlayerAnimationController
    {
        private readonly int UPPER_READY = Animator.StringToHash("ReadyMelee1H");
        private readonly int UPPER_ATTACK = Animator.StringToHash("ThrowKunai");
        private readonly int UPPER_ATTACK_ELITE = Animator.StringToHash("ThrowKunai_Elite");
        private readonly int UPPER_ATTACK_ULTIMATE = Animator.StringToHash("ThrowKunai_Ultimate");
        
        private readonly int UPPER_ELITE_ACTION = Animator.StringToHash("UseCloak");
        private readonly int UPPER_CONTINUOUS_ELITE_ACTION = Animator.StringToHash("NinjaSlash");
        private readonly int LOWER_NINJA_RUN = Animator.StringToHash("NinjaRun");
        public override void Init(CreatureController owner)
        {
            base.Init(owner);
            SetMovementSpeed(2f);
        }

        public override void Run() => AnimController.Play(LOWER_NINJA_RUN);
        public override void Ready() => AnimController.Play(UPPER_READY);
        public override void RunSkill()
        {
            switch (this.Owner.SkillAnimationType)
            {
                case SkillAnimationType.Attack:
                    {
                        if (this.Owner.SkillBook.GetCurrentSkillGrade(FixedValue.TemplateID.Skill.NinjaMastery) == InGameGrade.Default)
                            AnimController.Play(UPPER_ATTACK);
                        else if (this.Owner.SkillBook.GetCurrentSkillGrade(FixedValue.TemplateID.Skill.NinjaMastery) == InGameGrade.Elite)
                            AnimController.Play(UPPER_ATTACK_ELITE);
                        else
                            AnimController.Play(UPPER_ATTACK_ULTIMATE);
                    }
                    break;

                case SkillAnimationType.ElitePlus:
                    {
                        AnimController.StopPlayback();
                        AnimController.Play(UPPER_ELITE_ACTION);
                    }
                    break;

                case SkillAnimationType.C1ElitePlus:
                    {
                        AnimController.StopPlayback();
                        AnimController.Play(UPPER_CONTINUOUS_ELITE_ACTION);
                    }
                    break;

                case SkillAnimationType.UltimatePlus:
                    break;
            }
        }
    }
}

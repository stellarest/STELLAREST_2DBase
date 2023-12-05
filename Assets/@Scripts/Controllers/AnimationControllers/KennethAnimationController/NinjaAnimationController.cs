using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_2D.Define;

namespace STELLAREST_2D
{
    public class NinjaAnimationController : PlayerAnimationController
    {
        private readonly int UPPER_ATTACK_ELITE = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_THROW_KUNAI_ELITE);
        private readonly int UPPER_ATTACK_ULTIMATE = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_THROW_KUNAI_ULTIMATE);
        private readonly int UPPER_MASTERY_ELITE_PLUS_C1 = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_NINJA_SLASH);
        //private readonly int LOWER_PLAYER_NINJA_RUN = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_NINJA_RUN);
        
        public override void Init(CreatureController owner)
        {
            base.Init(owner);
            this.UPPER_READY = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_READY_MELEE_1H);
            this.UPPER_ATTACK = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_THROW_KUNAI);
            this.UPPER_MASTERY_ELITE_PLUS = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_CLOAK);
            this.UPPER_MASTERY_ULTIMATE_PLUS = Animator.StringToHash("");
        }

        //public override void Run() => CreatureAnimator.Play(LOWER_PLAYER_NINJA_RUN);
        public override void Skill(FixedValue.TemplateID.SkillAnimation skillAnimType = FixedValue.TemplateID.SkillAnimation.None)
        {
            switch (skillAnimType)
            {
                case FixedValue.TemplateID.SkillAnimation.Mastery:
                    {
                        if (this.Owner.SkillBook.GetCurrentSkillGrade(FixedValue.TemplateID.Skill.NinjaMastery) < InGameGrade.Ultimate)
                            CreatureAnimator.Play(UPPER_ATTACK);
                        else if (this.Owner.SkillBook.GetCurrentSkillGrade(FixedValue.TemplateID.Skill.NinjaMastery) == InGameGrade.Elite)
                            CreatureAnimator.Play(UPPER_ATTACK_ELITE);
                        else
                            CreatureAnimator.Play(UPPER_ATTACK_ULTIMATE);
                    }
                    break;

                case FixedValue.TemplateID.SkillAnimation.Unlock_Mastery_Elite:
                    CreatureAnimator.StopPlayback();
                    CreatureAnimator.Play(UPPER_MASTERY_ELITE_PLUS);
                    break;

                case FixedValue.TemplateID.SkillAnimation.Unlock_Mastery_Elite_C1:
                    CreatureAnimator.StopPlayback();
                    CreatureAnimator.Play(UPPER_MASTERY_ELITE_PLUS_C1);
                    break;

                case FixedValue.TemplateID.SkillAnimation.Unlock_Mastery_Ultimate:
                    break;
            }
        }
    }
}

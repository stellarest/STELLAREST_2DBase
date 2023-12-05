using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_2D.Define;

namespace STELLAREST_2D
{
    public class AssassinAnimationController : PlayerAnimationController
    {
        private readonly int UPPER_ATTACK_ULTIMATE = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_JAB_MELEE_PAIRED_ULTIMATE);

        public override void Init(CreatureController owner)
        {
            base.Init(owner);
            this.UPPER_READY = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_READY_MELEE_1H);
            this.UPPER_ATTACK = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_JAB_MELEE_1H);
            this.UPPER_MASTERY_ELITE_PLUS = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_POISON_DAGGER);
            this.UPPER_MASTERY_ULTIMATE_PLUS = Animator.StringToHash("");
        }

        public override void Skill(FixedValue.TemplateID.SkillAnimation skillAnimType = FixedValue.TemplateID.SkillAnimation.None)
        {
            switch (skillAnimType)
            {
                case FixedValue.TemplateID.SkillAnimation.Mastery:
                    {
                        if (this.Owner.SkillBook.GetCurrentSkillGrade(FixedValue.TemplateID.Skill.AssassinMastery) < InGameGrade.Ultimate)
                            CreatureAnimator.Play(UPPER_ATTACK);
                        else
                            CreatureAnimator.Play(UPPER_ATTACK_ULTIMATE);
                    }
                    break;

                case FixedValue.TemplateID.SkillAnimation.Unlock_Mastery_Elite:
                    CreatureAnimator.StopPlayback();
                    CreatureAnimator.Play(UPPER_MASTERY_ELITE_PLUS);
                    break;

                case FixedValue.TemplateID.SkillAnimation.Unlock_Mastery_Ultimate:
                    break;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_2D.Define;

namespace STELLAREST_2D
{
    public class ElementalArcherAnimationController : PlayerAnimationController
    {
        private readonly int UPPER_READY = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_READY_BOW);
        private readonly int UPPER_ATTACK = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_RANGED_ARROW_SHOT);
        private readonly int UPPER_MASTERY_ELITE_PLUS = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_USE_ELEMENTAL_SHOCK);
        private readonly int UPPER_MASTERY_ULTIMATE_PLUS = Animator.StringToHash("");

        public override void Init(CreatureController owner) => base.Init(owner);
        public override void Ready() => CreatureAnimator.Play(UPPER_READY);
        public override void Skill(FixedValue.TemplateID.SkillAnimation skillAnimType = FixedValue.TemplateID.SkillAnimation.None)
        {
            switch (skillAnimType)
            {
                case FixedValue.TemplateID.SkillAnimation.MasteryAttack:
                    CreatureAnimator.Play(UPPER_ATTACK);
                    break;

                case FixedValue.TemplateID.SkillAnimation.MasteryElitePlus:
                    CreatureAnimator.StopPlayback();
                    CreatureAnimator.Play(UPPER_MASTERY_ELITE_PLUS);
                    break;

                case FixedValue.TemplateID.SkillAnimation.MasteryUltimatePlus:
                    break;
            }
        }
    }
}

using UnityEngine;

using static STELLAREST_2D.Define;

namespace STELLAREST_2D
{
    public class PlayerAnimationController : CreatureAnimationController
    {
        protected int UPPER_READY = -1;
        protected int UPPER_ATTACK = -1;
        protected int UPPER_MASTERY_ELITE_PLUS = -1;
        protected int UPPER_MASTERY_ULTIMATE_PLUS = -1;
        
        public override void Init(CreatureController owner) => base.Init(owner);
        public void Ready() => CreatureAnimator.Play(UPPER_READY);
        public virtual void Skill(FixedValue.TemplateID.SkillAnimation skillAnimType = FixedValue.TemplateID.SkillAnimation.None)
        {
            switch (skillAnimType)
            {
                case FixedValue.TemplateID.SkillAnimation.Mastery:
                    CreatureAnimator.Play(UPPER_ATTACK);
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


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_2D.Define;

namespace STELLAREST_2D
{
    public class AssassinAnimationController : PlayerAnimationController
    {
        private readonly int UPPER_READY = Animator.StringToHash("ReadyMelee1H");
        private readonly int UPPER_ATTACK = Animator.StringToHash("JabMelee1H");
        private readonly int UPPER_ATTACK_ULTIMATE = Animator.StringToHash("JabMelee_Paired_Ultimate");
        private readonly int UPPER_ELITE_SEQUENCE = Animator.StringToHash("UsePoisonDagger");

        public override void Init(CreatureController owner) => base.Init(owner);
        public override void Ready() => AnimController.Play(UPPER_READY);
        public override void RunSkill()
        {
            switch (this.Owner.SkillAnimationType)
            {
                case SkillAnimationType.Attack:
                    {
                        if (this.Owner.SkillBook.GetCurrentSkillGrade(FixedValue.TemplateID.Skill.AssassinMastery) < InGameGrade.Ultimate)
                            AnimController.Play(UPPER_ATTACK);
                        else
                            AnimController.Play(UPPER_ATTACK_ULTIMATE);
                    }
                    break;

                case SkillAnimationType.ElitePlus:
                    {
                        AnimController.StopPlayback();
                        AnimController.Play(UPPER_ELITE_SEQUENCE);
                    }
                    break;

                case SkillAnimationType.UltimatePlus:
                    break;
            }
        }
    }
}

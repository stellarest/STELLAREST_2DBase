using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

namespace STELLAREST_2D
{
    public class NinjaAnimationController : PlayerAnimationController
    {
        private readonly int UPPER_READY = Animator.StringToHash("ReadyMelee1H");
        private readonly int UPPER_ATTACK = Animator.StringToHash("ThrowKunai");
        private readonly int UPPER_ATTACK_ELITE = Animator.StringToHash("ThrowKunai_Elite");
        private readonly int UPPER_ATTACK_ULTIMATE = Animator.StringToHash("ThrowKunai_Ultimate");
        private readonly int UPPER_ELITE_SEQUENCE = Animator.StringToHash("UseCloak");


        private readonly int UPPER_ATTACK_MELEE = Animator.StringToHash("NinjaMelee2H");
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
                case Define.SkillAnimationType.MasteryAction:
                    {
                        if (this.Owner.SkillBook.GetCurrentSkillGrade(SkillTemplate.NinjaMastery) == Define.InGameGrade.Default)
                            AnimController.Play(UPPER_ATTACK);
                        else if (this.Owner.SkillBook.GetCurrentSkillGrade(SkillTemplate.NinjaMastery) == Define.InGameGrade.Elite)
                            AnimController.Play(UPPER_ATTACK_ELITE);
                        else
                            AnimController.Play(UPPER_ATTACK_ULTIMATE);
                    }
                    break;

                case Define.SkillAnimationType.EliteAction:
                    {
                        AnimController.StopPlayback();
                        AnimController.Play(UPPER_ELITE_SEQUENCE);
                    }
                    break;

                case Define.SkillAnimationType.UltimateAction:
                    break;
            }
        }
    }
}

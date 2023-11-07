using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

namespace STELLAREST_2D
{
    public class KnightAnimationController : PlayerAnimationController
    {
        private readonly int UPPER_READY = Animator.StringToHash("ReadyMelee2H");
        private readonly int UPPER_ATTACK = Animator.StringToHash("SlashMelee2H");
        private readonly int UPPER_ELITE_SEQUENCE = Animator.StringToHash("UseSecondWind"); // SECOND WIND

        public override void Init(CreatureController owner) => base.Init(owner);
        public override void Ready() => AnimController.Play(UPPER_READY);
        public override void RunSkill() 
        {
            switch (this.Owner.SkillAnimationType)
            {
                case Define.SkillAnimationType.MasteryAction:
                    {
                        if (this.Owner.SkillBook.GetCanActiveSkillMember(SkillTemplate.KnightMastery))
                            AnimController.Play(UPPER_ATTACK);
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


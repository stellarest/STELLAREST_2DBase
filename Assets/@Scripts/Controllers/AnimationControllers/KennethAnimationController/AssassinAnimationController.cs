using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class AssassinAnimationController : PlayerAnimationController
    {
        private readonly int UPPER_READY = Animator.StringToHash("ReadyMelee1H");
        private readonly int UPPER_ATTACK = Animator.StringToHash("JabMelee1H");
        private readonly int UPPER_ATTACK_ULTIMATE = Animator.StringToHash("JabMelee_Paired_Ultimate");
        public override void Init(CreatureController owner) => base.Init(owner);
        public override void Ready() => AnimController.Play(UPPER_READY);
        public override void RunSkill()
        {
            if (this.Owner.SkillBook.GetFirstSkillGrade() < Define.InGameGrade.Ultimate)
                AnimController.Play(UPPER_ATTACK);
            else
                AnimController.Play(UPPER_ATTACK_ULTIMATE);
        }
    }
}

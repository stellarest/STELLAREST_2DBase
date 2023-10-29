using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace STELLAREST_2D
{
    public class NinjaAnimationController : PlayerAnimationController
    {
        private readonly int UPPER_READY = Animator.StringToHash("ReadyMelee1H");
        private readonly int UPPER_ATTACK = Animator.StringToHash("ThrowKunai");
        private readonly int UPPER_ATTACK_ELITE = Animator.StringToHash("ThrowKunai_Elite");
        private readonly int UPPER_ATTACK_ULTIMATE = Animator.StringToHash("ThrowKunai_Ultimate");

        private readonly int UPPER_ATTACK_MELEE_SWING = Animator.StringToHash("SlashMelee2H");

        private readonly int LOWER_NINJA_RUN = Animator.StringToHash("NinjaRun");
        public override void Init(CreatureController owner)
        {
            base.Init(owner);
            SetMovementSpeed(2f);
        }

        public override void Run() => AnimController.Play(LOWER_NINJA_RUN);
        public override void Ready() => AnimController.Play(UPPER_READY);
        public override void Attack()
        {
            // CreatureController cc = Utils.GetClosestCreatureTargetFromAndRange<MonsterController>
            //     (this.Owner.gameObject, this.Owner, this.Owner.Stat.CollectRange);
            // if (cc != null)
            // {
            //     Utils.Log("CC IS VALID !!");
            //     AnimController.Play(UPPER_ATTACK_MELEE_SWING);
            //     return;
            // }
            // else
            //     Utils.Log("NULL");

            switch (this.Owner.SkillBook.GetFirstSkillGrade())
            {
                case Define.InGameGrade.Default:
                    AnimController.Play(UPPER_ATTACK);
                    break;

                case Define.InGameGrade.Elite:
                    AnimController.Play(UPPER_ATTACK_ELITE);
                    break;

                case Define.InGameGrade.Ultimate:
                    AnimController.Play(UPPER_ATTACK_ULTIMATE);
                    break;
            }
        }
    }
}

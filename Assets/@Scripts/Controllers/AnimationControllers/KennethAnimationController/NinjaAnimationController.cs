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

        private readonly int UPPER_ATTACK_MELEE = Animator.StringToHash("NinjaMelee2H");

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
            // if (this.Owner.SkillBook.GetFirstSkillGrade() > Define.InGameGrade.Default)
            // {
            //     CreatureController cc = Utils.GetClosestCreatureTargetFromAndRange<MonsterController>
            //         (this.Owner.gameObject, this.Owner, this.Owner.Stat.CollectRange * 2);

            //     if (cc != null)
            //     {
            //         AnimController.Play(UPPER_ATTACK_MELEE);
            //         return;
            //     }
            // }

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

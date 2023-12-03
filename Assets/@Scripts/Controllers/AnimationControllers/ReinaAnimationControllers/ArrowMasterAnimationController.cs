using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 26 RIO
// 31 RIO (last henshin)
// 57 RIO
using static STELLAREST_2D.Define;

namespace STELLAREST_2D
{
    public class ArrowMasterAnimationController : PlayerAnimationController
    {
        private readonly int UPPER_READY = Animator.StringToHash("ReadyBow");
        private readonly int UPPER_ATTACK = Animator.StringToHash("RangedShot");
        private readonly int UPPER_ELITE_ACTION = Animator.StringToHash("UseConcentration");

        public override void Init(CreatureController owner) => base.Init(owner);
        public override void Ready() => AnimController.Play(UPPER_READY);
        public override void RunSkill()
        {
            switch (this.Owner.SkillAnimationType)
            {
                case SkillAnimationType.Attack:
                    AnimController.Play(UPPER_ATTACK);
                    break;

                case SkillAnimationType.ElitePlus:
                    {
                        AnimController.StopPlayback();
                        AnimController.Play(UPPER_ELITE_ACTION);
                    }
                    break;

                case SkillAnimationType.UltimatePlus:
                    break;
            }
        }
    }
}


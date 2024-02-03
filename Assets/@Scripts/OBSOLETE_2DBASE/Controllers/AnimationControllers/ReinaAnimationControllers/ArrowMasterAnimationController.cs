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
        public override void Init(CreatureController owner)
        {
            base.Init(owner);
            this.UPPER_READY = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_READY_BOW);
            this.UPPER_ATTACK = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_RANGED_ARROW_SHOT);
            this.UPPER_MASTERY_ELITE_PLUS = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_ELEMENTAL_SHOCK);
            this.UPPER_MASTERY_ULTIMATE_PLUS = Animator.StringToHash("");
        }
    }
}


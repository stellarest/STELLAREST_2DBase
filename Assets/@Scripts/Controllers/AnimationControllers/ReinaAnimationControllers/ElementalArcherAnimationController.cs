using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_2D.Define;

namespace STELLAREST_2D
{
    public class ElementalArcherAnimationController : PlayerAnimationController
    {
        public override void Init(CreatureController owner)
        {
            base.Init(owner);
            this.UPPER_READY = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_READY_BOW);
            this.UPPER_ATTACK = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_RANGED_ARROW_SHOT);
            this.UPPER_MASTERY_ELITE_PLUS = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_PLAYER_CONCENTRATION);
            this.UPPER_MASTERY_ULTIMATE_PLUS = Animator.StringToHash("");
        }
    }
}

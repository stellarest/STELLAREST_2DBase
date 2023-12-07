using UnityEngine;

using static STELLAREST_2D.Define;

namespace STELLAREST_2D
{
    public class MonsterAnimationController : CreatureAnimationController
    {
        private readonly int MONSTER_ATTACK = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_MONSTER_ATTACK);
        
        public override void Init(CreatureController owner) => base.Init(owner);
        public override void Stun() => this.Idle();
        public void Attack() => CreatureAnimator.Play(MONSTER_ATTACK);
    }
}


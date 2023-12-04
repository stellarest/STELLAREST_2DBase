using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_2D.Define;

namespace STELLAREST_2D
{
    public class CreatureAnimationController : BaseController
    {
        public CreatureController Owner { get; private set; } = null;
        public Animator CreatureAnimator { get; private set; } = null;

        private readonly int CREATURE_IDLE = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_CREATURE_IDLE);
        private readonly int CREATURE_RUN = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_CREATURE_RUN);
        private readonly int CREATURE_STUN = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_CREATURE_STUN);
        private readonly int CREATURE_DEAD = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_CREATURE_DEAD);

        private readonly int CREATURE_ANIM_SPEED = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_CREATURE_ANIM_SPEED);
        public void SetAnimationSpeed(float animSpeed) => CreatureAnimator.SetFloat(CREATURE_ANIM_SPEED, animSpeed);

        private readonly int CREATURE_MOVEMENT_SPEED = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_CREATURE_MOVEMENT_SPEED);
        public void SetMovementSpeed(float movementSpeed) => CreatureAnimator.SetFloat(CREATURE_MOVEMENT_SPEED, movementSpeed);

        private readonly int CREATURE_ENTER_NEXT_STATE_TRIGGER = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_CREATURE_ENTER_NEXT_STATE_TRIGGER);
        public void EnterNextState() => CreatureAnimator.SetTrigger(CREATURE_ENTER_NEXT_STATE_TRIGGER);

        private readonly int CREATURE_ENTER_NEXT_STATE_BOOLEAN = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_CREATURE_ENTER_NEXT_STATE_BOOLEAN);
        public void EnterNextState(bool canEnter) => CreatureAnimator.SetBool(CREATURE_ENTER_NEXT_STATE_BOOLEAN, canEnter);

        public virtual void Init(CreatureController owner)
        {
            Owner = owner;
            CreatureAnimator = GetComponent<Animator>();
        }

        public void Idle() => CreatureAnimator.Play(CREATURE_IDLE);
        public virtual void Run() => CreatureAnimator.Play(CREATURE_RUN);
        public virtual void Stun() => CreatureAnimator.Play(CREATURE_STUN);
        public void Dead() => CreatureAnimator.Play(CREATURE_DEAD);
    }
}


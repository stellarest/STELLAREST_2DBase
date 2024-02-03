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
        public void SetAnimationSpeed(float addSpeedRatio) => CreatureAnimator.SetFloat(CREATURE_ANIM_SPEED, addSpeedRatio);

        protected readonly int CREATURE_ATTACK_SPEED = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_CREATURE_ATTACK_SPEED);
        public virtual void SetAttackSpeed(float addSpeedRatio) => CreatureAnimator.SetFloat(CREATURE_ATTACK_SPEED, addSpeedRatio);
        public float GetAttackSpeed() => CreatureAnimator.GetFloat(FixedValue.Find.ANIM_PARAM_CREATURE_ATTACK_SPEED);

        private readonly int CREATURE_MOVEMENT_SPEED = Animator.StringToHash(FixedValue.Find.ANIM_PARAM_CREATURE_MOVEMENT_SPEED);
        public void SetMovementSpeed(float addSpeedRatio)
        {
            float movementSpeedMultiplierResult = Mathf.Clamp01(addSpeedRatio / FixedValue.Numeric.CREATURE_MAX_MOVEMENT_SPEED) 
                                                    * FixedValue.Numeric.CREATURE_MAX_MOVEMENT_SPEED_ANIM_MULTIPLIER;

            CreatureAnimator.SetFloat(CREATURE_MOVEMENT_SPEED, movementSpeedMultiplierResult);
        }

        public virtual void Ready() { }
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


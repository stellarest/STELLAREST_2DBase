using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace STELLAREST_2D
{
    public class BaseAnimationController : BaseController
    {
        private const string FIXED_ANIM_PARAM_MOVEMENT_SPEED = "MovementSpeed";
        protected readonly int MOVEMENT_SPEED = Animator.StringToHash(FIXED_ANIM_PARAM_MOVEMENT_SPEED);
        public void SetMovementSpeed(float movementSpeed)
        {
            this.AnimController.SetFloat(MOVEMENT_SPEED, 1f);
        }

        public CreatureController Owner { get; protected set; } = null;
        public Animator AnimController { get; protected set; } = null;

        public virtual void Init(CreatureController owner)
        {
            Owner = owner;
            AnimController = GetComponent<Animator>();
        }
    }
}


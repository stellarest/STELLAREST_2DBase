using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Animations;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class BaseAnimation : BaseObject
    {
        public Animator BaseAnimator { get; private set; } = null;
        public Creature Owner { get; set; } = null;

        private readonly int IDLE = Animator.StringToHash(FixedValue.String.ANIM_PARAM_IDLE);
        private readonly int MOVE = Animator.StringToHash(FixedValue.String.ANIM_PARAM_MOVE);

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            BaseAnimator = GetComponent<Animator>();

            InitLog(typeof(BaseAnimation));
            return true;
        }

        private void Start() // TEMP
        {
            RigidBody.simulated = false;
        }

        public virtual void SetInfo(Creature owner, int animDataID)
        {
            this.Owner = owner;
        }

        public virtual void Idle()
        {
            BaseAnimator.Play(IDLE, layer: 0);
            BaseAnimator.Play(IDLE, layer: 1);
        }

        public virtual void Move()
        {
            BaseAnimator.Play(IDLE, layer: 0);
            BaseAnimator.Play(MOVE, layer: 1);
        }
    }
}

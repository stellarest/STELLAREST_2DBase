using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class AnimationController : BaseController
    {
        public CreatureController Owner { get; set; }
        
        protected Animator _animController;
        public Animator AnimController => _animController;

        public override bool Init()
        {
            base.Init();
            _animController = gameObject.GetComponentInChildren<Animator>();

            return true;
        }

        public virtual void Idle() { }
        public virtual void Ready() { }

        public virtual void Walk() { }
        public virtual void Run() { }
        public virtual void Stun() { }

        public virtual void Attack() { }
        public virtual void Slash1H() { }
        public virtual void Jab1H() { }

        public virtual void DefaultFace() { }
        public virtual void AngryFace() { }
        public virtual void DeadFace() { }
    }
}


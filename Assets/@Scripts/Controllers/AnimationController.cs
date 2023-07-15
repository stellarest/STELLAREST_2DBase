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
            //_animController.SetBool("Ready", true);

            return true;
        }

        public virtual void Idle() { }
        public virtual void Ready() { }
        public virtual void Walk() { }
        public virtual void Run(float speed = 1f) { }
        public virtual void Stun() { }
        public virtual void Attack() { }
        public virtual void MeleeSlash(float speed = 1f) { }
        public virtual void MeleeJab(float speed = 1f) { }

        public virtual void DefaultFace() { }
        public virtual void AngryFace() { }
        public virtual void DeadFace() { }
    }
}


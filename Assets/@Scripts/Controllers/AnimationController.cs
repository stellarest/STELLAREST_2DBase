using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class AnimationController : BaseController
    {
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
        public virtual void Run() { }
        public virtual void Stun() { }
        public virtual void MeleeSlash(float upperBodySpeed = 1f) { }
        public virtual void MeleeJab(float upperBodySpeed = 1f) { }
    }
}


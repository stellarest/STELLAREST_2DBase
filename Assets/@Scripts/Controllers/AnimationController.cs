using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class AnimationController : BaseController
    {
        public CreatureController Owner { get; set; }
        public Animator AnimController { get; protected set; }

        public override bool Init()
        {
            base.Init();
            AnimController = gameObject.GetComponentInChildren<Animator>();
            return true;
        }
    }
}


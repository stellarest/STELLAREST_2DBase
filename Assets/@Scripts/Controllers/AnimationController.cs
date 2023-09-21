using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class AnimationController : BaseController
    {
        public CreatureController Owner { get; protected set; }
        public Animator AnimController { get; protected set; }

        public void InitAnimationController(CreatureController owner)
        {
            Owner = owner;
            AnimController = GetComponentInChildren<Animator>();
        }
    }
}


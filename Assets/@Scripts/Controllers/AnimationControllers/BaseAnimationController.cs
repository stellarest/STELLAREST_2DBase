using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace STELLAREST_2D
{
    public class BaseAnimationController : BaseController
    {
        public CreatureController Owner { get; protected set; } = null;
        public Animator AnimController { get; protected set; } = null;

        public virtual void Init(CreatureController owner)
        {
            Owner = owner;
            AnimController = GetComponentInChildren<Animator>();
        }
    }
}


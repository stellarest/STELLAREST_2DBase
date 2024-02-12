using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class AnimationEvent : InitBase
    {
        public event Action OnDustVFX = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public void OnDustVFXHandler()
            => OnDustVFX?.Invoke();
    }
}

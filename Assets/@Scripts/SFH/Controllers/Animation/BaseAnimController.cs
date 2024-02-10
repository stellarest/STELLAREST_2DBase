using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class BaseAnimController : InitBase
    {
        public BaseObject Owner { get; protected set; } = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }
    }
}


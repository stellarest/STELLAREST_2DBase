using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class BaseObject : InitBase
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

namespace STELLAREST_SFH
{
    public class HeroCamp : BaseObject
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }
    }
}

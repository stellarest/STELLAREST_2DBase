using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_SFH
{
    public class Projectile : BaseObject
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }
    }
}

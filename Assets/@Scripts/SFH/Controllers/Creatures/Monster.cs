using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_SFH
{
    public class Monster : Creature
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }
    }
}

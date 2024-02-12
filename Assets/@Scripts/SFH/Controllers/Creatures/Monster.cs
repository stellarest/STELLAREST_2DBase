using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class Monster : Creature
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            CreatureType = ECreatureType.Monster;

            return true;
        }
    }
}

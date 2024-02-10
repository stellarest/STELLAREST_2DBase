using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class Creature : BaseObject
    {
        public float Speed { get; protected set; } = 1f;
        public ECreatureType CreatureType { get; protected set; } = ECreatureType.None;
        
        protected ECreatureState _creatureState = ECreatureState.None;
        

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = EObjectType.Creature;

            return true;
        }
    }
}

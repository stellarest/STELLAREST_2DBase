using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class EventManager
    {
        public List<System.Action> ActionList { get; private set; } = new List<System.Action>();
    }
}

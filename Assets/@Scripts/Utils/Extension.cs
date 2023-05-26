using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public static class Extension
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
        {
            return Utils.GetOrAddComponent<T>(go);
        }
    }
}

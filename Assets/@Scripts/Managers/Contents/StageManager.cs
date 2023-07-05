using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class StageManager
    {
        public Vector3 LeftBottom { get; set; }
        public Vector3 RightTop { get; set; }

        public void Init()
        {
            GameObject go = UnityEngine.GameObject.Find("Map_01_Forest");
            LeftBottom = Utils.FindChild(go, "LeftBottom").transform.position;
            RightTop = Utils.FindChild(go, "RightTop").transform.position;
        }
    }
}

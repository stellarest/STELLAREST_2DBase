using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public abstract class BaseScene : InitBase
    {
        public EScene SceneType { get; protected set; } = EScene.Unknown;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            UnityEngine.Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
            if (obj == null)
            {
                GameObject go = new GameObject { name = FixedValue.String.EVENT_SYSTEM };
                go.AddComponent<EventSystem>();
                go.AddComponent<StandaloneInputModule>();
            }

            return true;
        }

        public abstract void Clear();
    }
}


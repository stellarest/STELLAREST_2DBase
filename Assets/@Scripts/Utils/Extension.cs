using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STELLAREST_2D.UI;

namespace STELLAREST_2D
{
    // 그 애니메이션 에셋 중에서 Animator Extension 했던거 기억나는데 나중에 뭐였는지 찾아볼것
    public static class Extension
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
        {
            return Utils.GetOrAddComponent<T>(go);
        }

        public static void BindEvent(this GameObject go, Action action = null, Action<UnityEngine.EventSystems.BaseEventData> dragAction = null, Define.UIEvent evtType = Define.UIEvent.Click)
        {
            UI_Base.BindEvent(go, action, dragAction, evtType);
        }

        public static bool IsValid(this GameObject go)
        {
            return go != null && go.activeSelf;
        }

        public static bool IsValid(this BaseController bc)
        {
            return bc != null && bc.isActiveAndEnabled;
        }

        public static bool IsCreatureDead(this GameObject go) 
                => go.GetComponent<CreatureController>().CreatureState == Define.CreatureState.Death;
    }
}

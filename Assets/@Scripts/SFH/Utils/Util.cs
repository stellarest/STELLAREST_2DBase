using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
//using Unity.Mathematics;
using Debug = UnityEngine.Debug;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using System;

namespace STELLAREST_SFH
{
    public static class Util
    {
        public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
        {
            T comp = go.GetComponent<T>();
            if (comp == null)
                comp = go.AddComponent<T>();

            return comp;
        }

        public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
        {
            Transform tr = FindChild<Transform>(go, name, recursive);
            if (tr == null)
                return null;

            return tr.gameObject;
        }

        public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
        {
            if (go == null)
                return null;

            if (recursive == false)
            {
                for (int i = 0; i < go.transform.childCount; ++i)
                {
                    Transform tr = go.transform.GetChild(i);
                    if (string.IsNullOrEmpty(name) || tr.name == name)
                        return tr.GetComponent<T>() != null ? tr.GetComponent<T>() : null;
                }
            }
            else
            {
                foreach (T comp in go.GetComponentsInChildren<T>(includeInactive: true))
                {
                    if (string.IsNullOrEmpty(name) || comp.name == name)
                        return comp;
                }
            }

            return default(T);
        }

        public static T ParseEnum<T>(string value)
            => (T)System.Enum.Parse(typeof(T), value, true);

#if UNITY_EDITOR
        #region Dev Logger
        [Conditional("UNITY_EDITOR")]
        public static void Log(object message, bool onBreak = false)
        {
            if (onBreak)
            {
                Debug.Log($"<color=white>[BREAK]</color>: {message}");
                Debug.Break();
            }
            else
                Debug.Log($"{message}");
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogCritical(object message)
        {
            Debug.LogError($"<color=red>[BREAK]</color>: {message}");
            Debug.Break();
        }

        [Conditional("UNITY_EDITOR")]
        public static void ClearLog()
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
            Debug.Log("##### [[[ Debug Message ]]] #####");
        }
        #endregion
#endif
    }
}
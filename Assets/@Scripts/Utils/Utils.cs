using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

using Debug = UnityEngine.Debug;

namespace STELLAREST_2D
{
    public class Utils
    {
        public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
        {
            T component = go.GetComponent<T>();
            if (component == null)
                component = go.AddComponent<T>();
            return component;
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
                    {
                        T comp = tr.GetComponent<T>();
                        if (comp != null)
                            return comp;
                    }
                }
            }
            else
            {
                foreach (T comp in go.GetComponentsInChildren<T>())
                {
                    if (string.IsNullOrEmpty(name) || comp.name == name)
                        return comp;
                }
            }

            return null;
        }

        public static Vector2 GetRandomPosition(Vector2 fromPos, float fromMinDistance = 10.0f, float fromMaxDistance = 20.0f)
        {
            int maxAttempts = 100;
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
                float distance = Random.Range(fromMinDistance, fromMaxDistance);

                float xDist = Mathf.Cos(angle) * distance;
                float yDist = Mathf.Sin(angle) * distance;

                Vector2 spawnPosition = fromPos + new Vector2(xDist, yDist);
                if (spawnPosition.x > Managers.Stage.MinimumPosition.x && spawnPosition.x < Managers.Stage.MaximumPosition.x)
                {
                    if (spawnPosition.y > Managers.Stage.MinimumPosition.y && spawnPosition.y < Managers.Stage.MaximumPosition.y)
                        return spawnPosition;
                }

                ++attempts;
            }

            return fromPos;
        }

        [Conditional("UNITY_EDITOR")]
        public static void Log(string message)
        {
            Debug.Log($"<color=white>{message}</color>");
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogStrong(string message)
        {
            Debug.Log($"<color=magenta>{message}</color>");
        }
    }
}

using UnityEngine;

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
    }
}

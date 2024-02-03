using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class Managers : MonoBehaviour
    {
        private static bool s_isApplicationQuitting = false;
        private static Managers s_instance = null;
        public static Managers Instance
        {
            get
            {
                if (s_isApplicationQuitting == false && s_instance == null)
                {
                    GameObject go = GameObject.Find(FixedValue.String.MANAGERS);
                    if (go == null)
                    {
                        go = new GameObject { name = FixedValue.String.MANAGERS };
                        s_instance = go.AddComponent<Managers>();
                    }

                    s_instance = go.GetComponent<Managers>();
                    DontDestroyOnLoad(go);
                }

                return s_instance;
            }
        }

        private DataManager _data = new DataManager();
        public static DataManager Data => Instance?._data;

        private PoolManager _pool = new PoolManager();
        public static PoolManager Pool => Instance?._pool;

        private ResourceManager _resource = new ResourceManager();
        public static ResourceManager Resource => Instance?._resource;

        private SceneManagerEx _scene = new SceneManagerEx();
        public static SceneManagerEx Scene => Instance?._scene;

        private UIManager _ui = new UIManager();
        public static UIManager UI => Instance?._ui;

        private void OnApplicationQuit() => s_isApplicationQuitting = true;
    }
}


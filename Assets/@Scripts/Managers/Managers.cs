using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class Managers : MonoBehaviour
    {
        private static Managers _instance = null;
        public static Managers Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = GameObject.Find("@Managers");
                    if (go == null)
                    {
                        go = new GameObject() { name = "@Managers" };
                        go.AddComponent<Managers>();
                    }

                    DontDestroyOnLoad(go);
                    _instance = go.GetComponent<Managers>();
                }

                return _instance;
            }
        }

        #region Contents
        private GameManager _gameManager = new GameManager();
        public static GameManager GameManager => Instance?._gameManager;

        private ObjectManager _objectManager = new ObjectManager();
        public static ObjectManager ObjectManager => Instance?._objectManager;

        private PoolManager _poolManager = new PoolManager();
        public static PoolManager PoolManager => Instance?._poolManager;
        #endregion

        #region Core
        private DataManager _dataManager = new DataManager();
        public static DataManager DataManager => Instance?._dataManager;

        private ResourceManager _resourceManager = new ResourceManager();
        public static ResourceManager ResourceManager => Instance?._resourceManager;

        private SceneManagerEx _sceneManagerEx = new SceneManagerEx();
        public static SceneManagerEx SceneManagerEx => Instance?._sceneManagerEx;

        private SoundManager _soundManager = new SoundManager();
        public static SoundManager SoundManager => Instance?._soundManager;

        private UIManager _uiManager = new UIManager();
        public static UIManager UIManager => Instance?._uiManager;
        #endregion
    }
}

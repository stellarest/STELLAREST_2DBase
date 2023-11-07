using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class Managers : MonoBehaviour
    {
        private static bool isApplicationQuitting = false;
        private static Managers _instance = null;
        public static Managers Instance
        {
            get
            {
                if (isApplicationQuitting == false && _instance == null)
                {
                    GameObject go = GameObject.Find("@Managers");
                    if (go == null)
                    {
                        go = new GameObject() { name = "@Managers" };
                        go.AddComponent<Managers>();
                    }

                    DontDestroyOnLoad(go);
                    _instance = go.GetComponent<Managers>();

                    Object.Init();
                    Stage.Init();
                }

                return _instance;
            }
        }

        #region Contents
        private CrowdControlManager _crowdControlManager = new CrowdControlManager();
        public static CrowdControlManager CrowdControl => Instance?._crowdControlManager;

        private GameManager _gameManager = new GameManager();
        public static GameManager Game => Instance?._gameManager;

        private ObjectManager _objectManager = new ObjectManager();
        public static ObjectManager Object => Instance?._objectManager;

        private PoolManager _poolManager = new PoolManager();
        public static PoolManager Pool => Instance?._poolManager;

        private SpriteManager _spriteManager = new SpriteManager();
        public static SpriteManager Sprite => Instance?._spriteManager;

        private StageManager _stageManager = new StageManager();
        public static StageManager Stage => Instance?._stageManager;

        private VFXManager _vfxManager = new VFXManager();
        public static VFXManager VFX => Instance?._vfxManager;

        #endregion

        #region Core
        private CollisionManager _collisionManager = new CollisionManager();
        public static CollisionManager Collision => Instance?._collisionManager;

        private DataManager _dataManager = new DataManager();
        public static DataManager Data => Instance?._dataManager;

        private ResourceManager _resourceManager = new ResourceManager();
        public static ResourceManager Resource => Instance?._resourceManager;

        private SceneManagerEx _sceneManagerEx = new SceneManagerEx();
        public static SceneManagerEx Scene => Instance?._sceneManagerEx;

        private SoundManager _soundManager = new SoundManager();
        public static SoundManager Sound => Instance?._soundManager;

        private UIManager _uiManager = new UIManager();
        public static UIManager UI => Instance?._uiManager;
        #endregion

        private void OnApplicationQuit()
        {
            isApplicationQuitting = true;
        }
    }
}

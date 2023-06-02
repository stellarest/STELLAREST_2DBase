using UnityEngine;
using Newtonsoft.Json.Linq;

namespace STELLAREST_2D
{
    public class GameScene : MonoBehaviour
    {
        private void Start()
        {
            Managers.Resource.LoadAllAsync<Object>("PreLoad", (delegate (string key, int count, int totalCount)
            {
                // 굳이 count / totalCount를 적어놓은 이유는 처음에 로딩화면, 로딩바에서 1,2,3,4,5... 이를 이용해 퍼센테이지로 표현 가능
                // 이런 프로세스바를 나중에 구현하기 위함
                Debug.Log($"Key : {key}, Count : {count} / TotalCount : {totalCount}");
                if (count == totalCount)
                    StartLoaded();
            }));
        }

        private SpawningPool _spawningPool;
        private void StartLoaded()
        {
            Managers.Data.Init();
            Managers.UI.ShowFixedSceneUI<UI_GameScene>();

            Managers.Game.OnKillCountChanged -= OnKillCountChangedHandler;
            Managers.Game.OnKillCountChanged += OnKillCountChangedHandler;

            Managers.Game.OnGemCountChanged -= OnGemCountChangedHandler;
            Managers.Game.OnGemCountChanged += OnGemCountChangedHandler;

            _spawningPool = gameObject.AddComponent<SpawningPool>();
            Debug.Log("##### PC SPAWN IN GAMESCENE #####");
            var player = Managers.Object.Spawn<PlayerController>
                    (Vector3.zero, Define.PlayerData.INITIAL_SPAWN_TEMPLATE_ID);

            var joystick = Managers.Resource.Instantiate(Define.UIData.Prefabs.JOYSTICK);
            joystick.name = "@UI_Joystick";

            var map = Managers.Resource.Instantiate(Define.GameData.Prefabs.MAP_01);
            map.name = "@Map";
            Camera.main.GetComponent<CameraController>().Target = player.gameObject;
        
            // DATA TEST
            // TextAsset playerData = Managers.Resource.Load<TextAsset>("PlayerData.json");
            // JObject json = JObject.Parse(playerData.text); // Parse로 해야함            
            //Debug.Log(json.ToString());

            // Debug.Log("=============================");
            // Managers.Data.Init();
            // foreach (var playerData in Managers.Data.PlayerDict.Values)
            // {
            //     Debug.Log($"Lv1 : {playerData.level}, HP : {playerData.maxHp}");
            // }
            // Debug.Log("=============================");
        }

        public void OnKillCountChangedHandler(int killCount)
        {
            Managers.UI.GetFixedSceneUI<UI_GameScene>().SetKillCount(killCount);

            if (killCount == 5)
            {
                Debug.Log("BOSS MON INCOMING");
            }
        }

        private int _collectedGemCount = 0;
        private int _remainingTotalGemCount = 10;
        public void OnGemCountChangedHandler(int gemCount)
        {
            _collectedGemCount++;
            if (_collectedGemCount == _remainingTotalGemCount)
            {
                Managers.UI.ShowPopup<UI_SkillSelectPopup>();
                _collectedGemCount = 0;
                _remainingTotalGemCount *= 2;
            }

            // *** 인자 둘 중 하나는 무조건 float
            Managers.UI.GetFixedSceneUI<UI_GameScene>().SetGemCountRatio(_collectedGemCount / (float)_remainingTotalGemCount);
        }

        private void OnDestroy()
        {
            if (Managers.Game != null)
                Managers.Game.OnGemCountChanged -= OnGemCountChangedHandler;
        }

        private void StartLoaded2() // LEGACY
        {
            // Managers.ResourceManager.LoadAsync<GameObjecT>("Snake_01", (go =>
            // {
            // }));
            // Managers.ResourceManager.LoadAsync<GameObject>("Snake_01", (delegate(GameObject go)
            // {
            //     // TODO
            // }));
            // GameObject go = new GameObject() { name = "@Monsters" };

            // var player = Managers.Resource.Instantiate("Slime_01.prefab");
            // player.AddComponent<PlayerController>();

            // var snake = Managers.Resource.Instantiate("Snake_01.prefab");
            // var goblin = Managers.Resource.Instantiate("Goblin_01.prefab");
            // var joystick = Managers.Resource.Instantiate("UI_Joystick.prefab");
            // joystick.name = "@UI_Joystick";

            // var map = Managers.Resource.Instantiate("Map.prefab");
            // map.name = "@Map";
            // Camera.main.GetComponent<CameraController>().Target = player;
        }
    }
}
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

            _spawningPool = gameObject.AddComponent<SpawningPool>();
            var player = Managers.Object.Spawn<PlayerController>(Vector3.zero, 1);

            var joystick = Managers.Resource.Instantiate(Define.LOAD_JOYSTICK_PREFAB);
            joystick.name = "@UI_Joystick";

            var map = Managers.Resource.Instantiate(Define.LOAD_MAP_PREFAB);
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
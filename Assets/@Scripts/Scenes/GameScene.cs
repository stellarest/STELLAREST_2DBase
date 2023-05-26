using UnityEngine;

namespace STELLAREST_2D
{
    public class GameScene : MonoBehaviour
    {
        private void Start()
        {
            Managers.Resource.LoadAllAsync<GameObject>("Prefabs", (delegate (string key, int count, int totalCount)
            {
                Debug.Log($"Key : {key}, Count : {count} / TotalCount : {totalCount}");

                if (count == totalCount)
                    StartLoaded();
            }));
        }

        private SpawningPool _spawningPool;
        private void StartLoaded()
        {
            _spawningPool = gameObject.AddComponent<SpawningPool>();
            var player = Managers.Object.Spawn<PlayerController>();
            
            // for (int i = 0; i < 10; ++i)
            // {
            //     MonsterController mc = Managers.Object.Spawn<MonsterController>(Random.Range(0, 2));
            //     mc.transform.position = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
            // }

            var joystick = Managers.Resource.Instantiate("UI_Joystick.prefab");
            joystick.name = "@UI_Joystick";

            var map = Managers.Resource.Instantiate("Map.prefab");
            map.name = "@Map";
            Camera.main.GetComponent<CameraController>().Target = player.gameObject;
        }

        private void StartLoaded2()
        {
            // Managers.ResourceManager.LoadAsync<GameObjecT>("Snake_01", (go =>
            // {
            // }));
            // Managers.ResourceManager.LoadAsync<GameObject>("Snake_01", (delegate(GameObject go)
            // {
            //     // TODO
            // }));
            // GameObject go = new GameObject() { name = "@Monsters" };



            var player = Managers.Resource.Instantiate("Slime_01.prefab");
            player.AddComponent<PlayerController>();

            var snake = Managers.Resource.Instantiate("Snake_01.prefab");
            var goblin = Managers.Resource.Instantiate("Goblin_01.prefab");
            var joystick = Managers.Resource.Instantiate("UI_Joystick.prefab");
            joystick.name = "@UI_Joystick";

            var map = Managers.Resource.Instantiate("Map.prefab");
            map.name = "@Map";
            Camera.main.GetComponent<CameraController>().Target = player;
        }
    }
}
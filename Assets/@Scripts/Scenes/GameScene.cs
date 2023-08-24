using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Cinemachine;

using STELLAREST_2D.UI; // 이 스크립트에서 UI 스크립트 사용중을 간편하게 확인할 수 있어서 .UI의 네임을 추가해주었다
using DG.Tweening;


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
        private Define.StageType _stageType;
        public Define.StageType StageType
        {
            get => _stageType;
            set
            {
                _stageType = value;
                if (_spawningPool != null)
                {
                    switch (value)
                    {
                        case Define.StageType.Normal:
                            {
                                _spawningPool.Stopped = false;
                            }
                            break;

                        case Define.StageType.Boss:
                            {
                                _spawningPool.Stopped = true;
                            }
                            break;
                    }
                }
            }
        }

        private void StartLoaded()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            
            Managers.Data.Init();
            
            //Managers.UI.ShowFixedSceneUI<UI_GameScene>();

            Managers.Game.OnKillCountChanged -= OnKillCountChangedHandler;
            Managers.Game.OnKillCountChanged += OnKillCountChangedHandler;

            Managers.Game.OnGemCountChanged -= OnGemCountChangedHandler;
            Managers.Game.OnGemCountChanged += OnGemCountChangedHandler;
            DOTween.SetTweensCapacity(200, 200);

            // Spawn Test Map
            //var testMap = Managers.Resource.Instantiate(Define.PrefabLabels.TEST_MAP);
            // var testMap = Managers.Resource.Instantiate(TempPrefabKeyLoader.TEST_MAP);
            // testMap.name = "@TestMap";

            // Spawn Player
            // +++ GARY +++
            // var player = Managers.Object.Spawn<PlayerController>(Vector3.zero, (int)Define.TemplateIDs.Player.Gary_Paladin);
            // var player = Managers.Object.Spawn<PlayerController>(Vector3.zero, (int)Define.TemplateIDs.Player.Gary_Knight);
            // var player = Managers.Object.Spawn<PlayerController>(Vector3.zero, (int)Define.TemplateIDs.Player.Gary_PhantomKnight);

            // +++ REINA +++
            var player = Managers.Object.Spawn<PlayerController>(Vector3.zero, (int)Define.TemplateIDs.Player.Reina_ArrowMaster);

            // +++ Kenneth +++
            // var player = Managers.Object.Spawn<PlayerController>(Vector3.zero, (int)Define.TemplateIDs.Player.Kenneth_Assassin);
            // var player = Managers.Object.Spawn<PlayerController>(Vector3.zero, (int)Define.TemplateIDs.Player.Kenneth_Thief);

            // +++ LIONEL +++
            // var player = Managers.Object.Spawn<PlayerController>(Vector3.zero, (int)Define.TemplateIDs.Player.Lionel_Warrior);
            // var player = Managers.Object.Spawn<PlayerController>(Vector3.zero, (int)Define.TemplateIDs.Player.Lionel_Berserker);

            // +++ STIGMA +++
            // var player = Managers.Object.Spawn<PlayerController>(Vector3.zero, (int)Define.TemplateIDs.Player.Stigma_SkeletonKing);
            // var player = Managers.Object.Spawn<PlayerController>(Vector3.zero, (int)Define.TemplateIDs.Player.Stigma_Pirate);
            // var player = Managers.Object.Spawn<PlayerController>(Vector3.zero, (int)Define.TemplateIDs.Player.Stigma_Mutant);

            // Camera.main.GetComponent<CameraController>().Target = player.gameObject;
            // GameObject.Find("CMcam").GetComponent<CameraController>().SetTarget(player.gameObject);

            var CMcam = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject;
            CMcam.GetComponent<CameraController>().SetTarget(player.gameObject);

            // Spawn Joystick
            var joystick = Managers.Resource.Instantiate(Define.PrefabLabels.JOYSTICK);
            joystick.name = "@Joystick"; // UI_Joystick라고 하기엔 좀 애매함

            // Create Spawning Pool
            // _spawningPool = gameObject.AddComponent<SpawningPool>();
            
            // TEST
            // Managers.Object.Spawn<Chicken>(new Vector3(3, 3, 1), (int)Define.TemplateIDs.Monster.Chicken);
            // Managers.Object.Spawn<Chicken>(new Vector3(8, 3, 1), (int)Define.TemplateIDs.Monster.Chicken);
            // Managers.Object.Spawn<Chicken>(new Vector3(5, -6, 1), (int)Define.TemplateIDs.Monster.Chicken);
            
            // Init Collision Layer
            Managers.Collision.SetCollisionLayers(Define.CollisionLayers.PlayerAttack, Define.CollisionLayers.MonsterBody, true);
            Managers.Collision.SetCollisionLayers(Define.CollisionLayers.PlayerBody, Define.CollisionLayers.MonsterAttack, true);
            Managers.Collision.SetCollisionLayers(Define.CollisionLayers.MonsterBody, Define.CollisionLayers.MonsterBody, true);

            // Camera Target
            // Test Gem Spawn
            // for (int i = 0; i < 30; ++i)
            // {
            //     Vector3 randPos = Utils.GenerateMonsterSpawnPosition(Managers.Game.Player.transform.position, 10f, 20f);
            //     GemController gc = Managers.Object.Spawn<GemController>(randPos);
            //     gc.GemSize = Random.Range(0, 2) == 0 ? gc.GemSize = GemSize.Normal : gc.GemSize = GemSize.Large;
            // }
        }

        public void OnKillCountChangedHandler(int killCount)
        {
            Managers.UI.GetFixedSceneUI<UI_GameScene>().SetKillCount(killCount);

            // 마찬가지로 스테이지 데이터로 관리
            if (killCount == 5)
            {
                // Debug.Log("BOSS MON INCOMING");
                StageType = Define.StageType.Boss;
                StartCoroutine(CoIncomingBoss());
            }
        }

        private IEnumerator CoIncomingBoss()
        {
            yield return new WaitForSeconds(1.0f);
            Managers.Object.DespawnAllMonsters(); // 죽이자마자 전부 DespawnAllMonster해서 IsVaild() == false에 걸렸었던것
            Vector2 spawnPos = Utils.GetRandomPosition(Managers.Object.Player.transform.position, 5f, 10f);
            //Managers.Object.Spawn<MonsterController>(spawnPos, (int)Define.TemplateIDs.Boss.Gnoll); // 3 is BOSS_ID
            //Managers.Object.Spawn<BossController>(spawnPos, 3);
        }

        private int _collectedGemCount = 0;
        // private int _remainingTotalGemCount = 10;
        public void OnGemCountChangedHandler(int gemCount)
        {
            //_collectedGemCount++;
            _collectedGemCount += gemCount;

            // if (_collectedGemCount == _remainingTotalGemCount)
            // {
            //     // Managers.UI.ShowPopup<UI_SkillSelectPopup>();
            //     _collectedGemCount = 0;
            //     _remainingTotalGemCount *= 2;
            // }

            Utils.Log("MY GEM COUNT : " + _collectedGemCount.ToString());

            // *** 인자 둘 중 하나는 무조건 float
            // Managers.UI.GetFixedSceneUI<UI_GameScene>().SetGemCountRatio(_collectedGemCount / (float)_remainingTotalGemCount);
        }

        private void OnDestroy()
        {
            if (Managers.Game != null)
                Managers.Game.OnGemCountChanged -= OnGemCountChangedHandler;
        }

        // private void StartLoaded2() // LEGACY
        // {
        //     // Managers.ResourceManager.LoadAsync<GameObjecT>("Snake_01", (go =>
        //     // {
        //     // }));
        //     // Managers.ResourceManager.LoadAsync<GameObject>("Snake_01", (delegate(GameObject go)
        //     // {
        //     //     // TODO
        //     // }));
        //     // GameObject go = new GameObject() { name = "@Monsters" };

        //     // var player = Managers.Resource.Instantiate("Slime_01.prefab");
        //     // player.AddComponent<PlayerController>();

        //     // var snake = Managers.Resource.Instantiate("Snake_01.prefab");
        //     // var goblin = Managers.Resource.Instantiate("Goblin_01.prefab");
        //     // var joystick = Managers.Resource.Instantiate("UI_Joystick.prefab");
        //     // joystick.name = "@UI_Joystick";

        //     // var map = Managers.Resource.Instantiate("Map.prefab");
        //     // map.name = "@Map";
        //     // Camera.main.GetComponent<CameraController>().Target = player;
        // }
    }
}
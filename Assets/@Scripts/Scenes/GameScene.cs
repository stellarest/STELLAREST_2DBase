using Cinemachine;
using DG.Tweening;
using UnityEngine;

using static STELLAREST_2D.Define;
using STELLAREST_2D.UI;

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
        private StageType _stageType;
        public StageType StageType
        {
            get => _stageType;
            set
            {
                _stageType = value;
                if (_spawningPool != null)
                {
                    switch (value)
                    {
                        case StageType.Normal:
                            {
                                _spawningPool.Stopped = false;
                            }
                            break;

                        case StageType.Boss:
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
            DOTween.SetTweensCapacity(200, 200);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            
            Managers.Data.Init();
            Managers.VFX.Init();
            Managers.Collision.Init();

            PlayerController player = this.SpawnPlayer(FixedValue.TemplateID.Player.Gary_Knight);
            //Managers.Game.OnGameStart += player.OnGameStartHandler;
            //Utils.Log("Add Event : Managers.Game.OnGameStart += player.OnGameStartHandler");

            var CMcam = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject;
            CMcam.GetComponent<CameraController>().SetTarget(player.gameObject);

            // Joystick
            var joystick = Managers.Resource.Instantiate(FixedValue.Load.UI_JOYSTICK);
            joystick.name = FixedValue.Find.JOYSTICK; // UI_Joystick라고 하기엔 좀 애매함

            // Spawning Pool
            _spawningPool = gameObject.AddComponent<SpawningPool>();
            Managers.Game.OnKillCountChanged -= this.OnKillCountChangedHandler;
            Managers.Game.OnGemCountChanged += this.OnGemCountChangedHandler;

            // Managers.UI.ShowFixedSceneUI<UI_GameScene>();
            // Managers.Game.OnKillCountChanged -= OnKillCountChangedHandler;
            // Managers.Game.OnKillCountChanged += OnKillCountChangedHandler;
            // Managers.Game.OnGemCountChanged -= OnGemCountChangedHandler;
            // Managers.Game.OnGemCountChanged += OnGemCountChangedHandler;
            // Test Gem Spawn
            // for (int i = 0; i < 30; ++i)
            // {
            //     Vector3 randPos = Utils.GenerateMonsterSpawnPosition(Managers.Game.Player.transform.position, 10f, 20f);
            //     GemController gc = Managers.Object.Spawn<GemController>(randPos);
            //     gc.GemSize = Random.Range(0, 2) == 0 ? gc.GemSize = GemSize.Normal : gc.GemSize = GemSize.Large;
            // }

            Managers.Game.GAME_START();
        }

        public void OnKillCountChangedHandler(int killCount)
        {
            Managers.UI.GetFixedSceneUI<UI_GameScene>().SetKillCount(killCount);

            // 마찬가지로 스테이지 데이터로 관리
            if (killCount == 5)
            {
                // Debug.Log("BOSS MON INCOMING");
                StageType = Define.StageType.Boss;
                // StartCoroutine(CoIncomingBoss());
            }
        }

        private int _collectedGemCount = 0;
        // private int _remainingTotalGemCount = 10;
        public void OnGemCountChangedHandler(int gemCount)
        {
            //_collectedGemCount++;
            _collectedGemCount = gemCount;
            Utils.Log("MY GEM COUNT : " + _collectedGemCount.ToString());
            // if (_collectedGemCount == _remainingTotalGemCount)
            // {
            //     // Managers.UI.ShowPopup<UI_SkillSelectPopup>();
            //     _collectedGemCount = 0;
            //     _remainingTotalGemCount *= 2;
            // }

            // *** 인자 둘 중 하나는 무조건 float
            // Managers.UI.GetFixedSceneUI<UI_GameScene>().SetGemCountRatio(_collectedGemCount / (float)_remainingTotalGemCount);
        }

        private PlayerController SpawnPlayer(FixedValue.TemplateID.Player playerTemplateID) 
                =>  Managers.Object.Spawn<PlayerController>(Vector3.zero, (int)playerTemplateID, ObjectType.Player);

        private void OnDestroy()
        {
            if (Managers.Game != null)
            {
                Managers.Game.OnGemCountChanged -= OnGemCountChangedHandler;
            }
        }
    }
}

// ==============================================================================================================
// private IEnumerator CoIncomingBoss()
// {
//     yield return new WaitForSeconds(1.0f);
//     Managers.Object.DespawnAllMonsters(); // 죽이자마자 전부 DespawnAllMonster해서 IsVaild() == false에 걸렸었던것
//     Vector2 spawnPos = Utils.GetRandomPosition(Managers.Object.Player.transform.position, 5f, 10f);
//     //Managers.Object.Spawn<MonsterController>(spawnPos, (int)Define.TemplateIDs.Boss.Gnoll); // 3 is BOSS_ID
//     //Managers.Object.Spawn<BossController>(spawnPos, 3);
// }
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace STELLAREST_2D
{
    public class  SpawningPool : MonoBehaviour
    {
        private float _spawnInterval = 1.25f; // 이것도 나중에 데이터 시트로 뺴야함. 무조건.
        private const int MAX_MONSTER_COUNT = 1; // 나중에 데이터 시트로 빼야함

        private Coroutine _coUpdateSpawningPool;
        public bool Stopped { get; set; } = false;
        private void Start()
        {
            _coUpdateSpawningPool = StartCoroutine(UpdateSpawningPool());
        }

        private IEnumerator UpdateSpawningPool()
        {
            while (true)
            {
                TrySpawn();
                yield return new WaitForSeconds(_spawnInterval);
            }
        }

        private void TrySpawn()
        {
            if (Stopped)
                return;

            if (Managers.Game.Player == null)
                return;

            int monsterCount = Managers.Object.Monsters.Count;
            if (monsterCount >= MAX_MONSTER_COUNT)
                return;

            // TEMP : DataID for spawning pos
            // Vector3 randPos = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
            // Vector3 randPos = Utils.GenerateMonsterSpawnPosition(Managers.Game.Player.transform.position, 10f, 15f);
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 1. Stage Data를 가져왔다고 가정.
            // 2. Stage Data에 있는 크리쳐 정보를 가져왔다고 가정.
            // 3. 크리쳐 정보 내에 있는 SpawnMinDistanceFromPlayer, SpawnMaxDistanceFromPlayer 정보를 가져왔다고 가정
            // 4. 또한 CreatureData, CreatureStat, Stage, 또는 Wave 데이터에서 IsPooling에 대한 정보를 가져와서, 이를 바탕으로 오브젝트를 생성해야함
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            
            Vector3 randPos = Utils.GetRandomPosition(Managers.Game.Player.transform.position, 5f, 20f);
            MonsterController mc = Managers.Object.Spawn<MonsterController>(randPos, (int)Define.TemplateIDs.Creatures.Monster.Chicken, Define.ObjectType.Monster, true);
            //MonsterController mc = Managers.Object.Spawn<MonsterController>(randPos, (int)Define.TemplateIDs.Monster.Chicken);
            // 개선 요망
            // randPos = new Vector3(15, 2, 0);
            // randPos = new Vector3(4.5f, 5f, 0);
            // randPos = new Vector3(6 + i, 0, 0);
            // i += 6;
            // Chicken chicken = Managers.Object.Spawn<Chicken>(randPos, (int)Define.TemplateIDs.Creatures.Monster.Chicken);
        }
    }
}

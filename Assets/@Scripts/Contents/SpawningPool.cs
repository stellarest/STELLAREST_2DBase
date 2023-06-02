using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class SpawningPool : MonoBehaviour
    {
        private float _spawnInterval = 0.1f; // 이것도 나중에 데이터 시트로 뺴야함
        private const int MaxMonsterCount = 100; // 나중에 데이터 시트로 빼야함
        private Coroutine _coUpdateSpawningPool;

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
            int monsterCount = Managers.Object.Monsters.Count;
            if (monsterCount >= MaxMonsterCount)
                return;

            // TEMP : DataID for spawning pos
            //Vector3 randPos = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
            Vector3 randPos = Utils.GenerateMonsterSpawnPosition(Managers.Game.Player.transform.position, 10f, 15f);
            MonsterController mc = Managers.Object.Spawn<MonsterController>(randPos, 
                        Random.Range((int)Define.MonsterData.MinMaxTemplateIDs.Min, 
                        (int)Define.MonsterData.MinMaxTemplateIDs.Max));
        }
    }
}

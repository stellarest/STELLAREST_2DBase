using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class SpawningPool : MonoBehaviour
    {
        private float _spawnInterval = 1f;
        private const int MaxMonsterCount = 100;
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

            Vector3 randPos = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
            MonsterController mc = Managers.Object.Spawn<MonsterController>(randPos, Random.Range(0, 2));
        }
    }
}

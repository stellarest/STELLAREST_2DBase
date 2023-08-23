using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class SpawningPool : MonoBehaviour
    {
        private float _spawnInterval = 0.5f; // 이것도 나중에 데이터 시트로 뺴야함
        private const int MaxMonsterCount = 2; // 나중에 데이터 시트로 빼야함

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

int i = 0;
        private void TrySpawn()
        {
            if (Stopped)
                return;

            int monsterCount = Managers.Object.Monsters.Count;
            if (monsterCount >= MaxMonsterCount)
                return;

            // TEMP : DataID for spawning pos
            //Vector3 randPos = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
            // Vector3 randPos = Utils.GenerateMonsterSpawnPosition(Managers.Game.Player.transform.position, 10f, 15f);
            Vector3 randPos = Utils.GetRandomPosition(Managers.Game.Player.transform.position, 5f, 20f);

            // MonsterController mc = Managers.Object.Spawn<MonsterController>(randPos, 
            //             Random.Range((int)Define.TemplateIDs.Monster.RedChicken, 
            //             (int)Define.TemplateIDs.Monster.Chicken));

            //MonsterController mc = Managers.Object.Spawn<MonsterController>(randPos, (int)Define.TemplateIDs.Monster.Chicken);
            // 개선 요망
            // randPos = new Vector3(15, 2, 0);
            // randPos = new Vector3(4.5f, 5f, 0);
            randPos = new Vector3(6 + i, 0, 0);
            i += 6;
            Chicken chicken = Managers.Object.Spawn<Chicken>(randPos, (int)Define.TemplateIDs.Monster.Chicken);
        }
    }
}

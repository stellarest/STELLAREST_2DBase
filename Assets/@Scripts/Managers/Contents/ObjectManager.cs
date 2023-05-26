using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class ObjectManager
    {
        public PlayerController Player { get; private set; }
        public HashSet<MonsterController> Monsters { get; } = new HashSet<MonsterController>();
        public HashSet<ProjectileController> Projectiles { get; } = new HashSet<ProjectileController>();
        // Env는 나중에 추가하던지

        public T Spawn<T>(int templateID = 0) where T : BaseController
        {
            System.Type type = typeof(T);
            if (type == typeof(PlayerController))
            {
                // TODO : use DataSheet
                GameObject go = Managers.Resource.Instantiate("Slime_01.prefab");
                go.name = "Player";

                PlayerController playerController = go.GetOrAddComponent<PlayerController>();
                Player = playerController;

                return playerController as T;
            }
            else if (type == typeof(MonsterController))
            {
                // TODO : use DataSheet
                string name = (templateID == 0 ? "Goblin_01" : "Snake_01");
                GameObject go = Managers.Resource.Instantiate(name + ".prefab", pooling: true);

                MonsterController monsterController = go.GetOrAddComponent<MonsterController>();
                Monsters.Add(monsterController);

                return monsterController as T;
            }

            return null;
        }

        public void Despawn<T>(T obj) where T : BaseController
        {
            System.Type type = typeof(T);
            if (type == typeof(PlayerController))
            {
            }
            else if (type == typeof(MonsterController))
            {
                Monsters.Remove(obj as MonsterController);
                Managers.Resource.Destroy(obj.gameObject);
            }
            else if (type == typeof(ProjectileController))
            {
                Projectiles.Remove(obj as ProjectileController);
                Managers.Resource.Destroy(obj.gameObject);
            }
        }
    }
}

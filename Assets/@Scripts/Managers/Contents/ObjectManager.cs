using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace STELLAREST_2D
{
    public class ObjectManager
    {
        public PlayerController Player { get; private set; }
        public HashSet<MonsterController> Monsters { get; } = new HashSet<MonsterController>();
        public HashSet<ProjectileController> Projectiles { get; } = new HashSet<ProjectileController>();
        public HashSet<GemController> Gems { get; } = new HashSet<GemController>();
        // EnvCont는 나중에 추가하던지
        public GridController GridController { get; private set; }

        public void Init()
        {
            GridController = UnityEngine.GameObject.Find("@Grid").GetComponent<GridController>();
        }

        public T Spawn<T>(Vector3 position, int templateID = -1) where T : BaseController
        {
            System.Type type = typeof(T);
            if (typeof(T).IsSubclassOf(typeof(CreatureController)))
            {
                if (type == typeof(PlayerController))
                {
                    GameObject go = Managers.Resource.Instantiate(Managers.Data.CreatureDict[templateID].PrefabLabel, pooling: false);
                    go.transform.position = position;
                    PlayerController pc = go.GetOrAddComponent<PlayerController>();
                    pc.SetInfo(templateID);
                    Player = pc;

                    return pc as T;
                }

                if (type == typeof(MonsterController))
                {
                    GameObject go = Managers.Resource.Instantiate(Managers.Data.CreatureDict[templateID].PrefabLabel, pooling: true);
                    go.transform.position = position;
                    MonsterController mc = go.GetOrAddComponent<MonsterController>();
                    mc.SetInfo(templateID);
                    Monsters.Add(mc);

                    return mc as T;
                }
            }
            else if (type == typeof(GemController))
            {
                GameObject go = Managers.Resource.Instantiate(Define.PrefabLabels.EXP_GEM, pooling: true);
                go.transform.position = position;

                GemController gc = go.GetOrAddComponent<GemController>();
                bool changeSprite = Random.Range(0, 2) == 0 ? true : false;
                string spriteKey = "";
                if (changeSprite)
                {
                    spriteKey = Random.Range(0, 2) == 0 ?
                                Define.SpriteLabels.EXP_GEM_BLUE : Define.SpriteLabels.EXP_GEM_YELLOW;

                    Sprite yellowOrBlue = Managers.Resource.Load<Sprite>(spriteKey);
                    if (yellowOrBlue != null)
                        gc.GetComponent<SpriteRenderer>().sprite = yellowOrBlue;

                }
                GridController.Add(go);
                Gems.Add(gc);

                return gc as T;
            }
            else if (type == typeof(ProjectileController))
            {
                // GameObject go = Managers.Resource.Instantiate(skillData.prefab, pooling: true);
                // go.transform.position = position;

                // ProjectileController pc = go.GetOrAddComponent<ProjectileController>();
                // Projectiles.Add(pc);
                // return pc as T;
            }

            return null;
        }


        public void Despawn<T>(T obj) where T : BaseController
        {
            if (obj.IsValid() == false)
            {
                Debug.Log("<color=magenta>##### Already despawned #####</color>"); // 단순 체크용
                return;
            }

            System.Type type = typeof(T);
            if (type == typeof(PlayerController))
            {
                // TODO
            }
            else if (typeof(T).IsSubclassOf(typeof(CreatureController)))
            {
                Monsters.Remove(obj as MonsterController);
                Managers.Resource.Destroy(obj.gameObject);
            }
            else if (type == typeof(GemController))
            {
                Gems.Remove(obj as GemController);
                Managers.Resource.Destroy(obj.gameObject);
                GridController.Remove(obj.gameObject);
            }
            else if (type == typeof(ProjectileController))
            {
                Projectiles.Remove(obj as ProjectileController);
                Managers.Resource.Destroy(obj.gameObject);
            }
        }

        public void DespawnAllMonsters()
        {
            var monsters = Monsters.ToList();
            foreach (var monster in monsters)
            {
                if (monster.IsValid() == false)
                    continue;

                Despawn<MonsterController>(monster);
            }
        }


        ///// LEGACY
        // public T Spawn3<T>(Vector3 position, int templateID = 0) where T : BaseController
        // {
        //     // templateID : 단순히 데이터가 있고 없고의 체크용으로만 사용하고
        //     // 직접적인 데이터는 모두 Init에서 가져온다.
        //     System.Type type = typeof(T);
        //     if (type == typeof(PlayerController))
        //     {
        //         if (Managers.Data.PlayerStatDict.TryGetValue(templateID, out Data.PlayerStatData playerData) == false)
        //         {
        //             Debug.LogError($"@@@@@ PlayerDict load failed, templateID : {templateID} @@@@@");
        //             return null;
        //         }

        //         // 나중에 캐릭터별로 프리팹 네임 필요할수도있음
        //         GameObject go = Managers.Resource.Instantiate(Define.PrefabLabels.NONE);
        //         go.name = "Player";
        //         go.transform.position = position;

        //         PlayerController pc = go.GetOrAddComponent<PlayerController>();
        //         //pc.PlayerData = playerData;
        //         Player = pc;
        //         // pc.Init();

        //         return pc as T;
        //     }
        //     else if (type == typeof(GemController)) // 현재 Gem과 관련된 아이템 데이터 시트가 없으므로 임시 코드
        //     {
        //         GameObject go = Managers.Resource.Instantiate(Define.PrefabLabels.EXP_GEM, pooling: true);
        //         go.transform.position = position;

        //         GemController gc = go.GetOrAddComponent<GemController>();

        //         bool changeSprite = Random.Range(0, 2) == 0 ? true : false;
        //         string spriteKey = "";
        //         if (changeSprite)
        //         {
        //             spriteKey = Random.Range(0, 2) == 0 ?
        //                         Define.SpriteLabels.EXP_GEM_BLUE : Define.SpriteLabels.EXP_GEM_BLUE;

        //             Sprite yellowOrBlue = Managers.Resource.Load<Sprite>(spriteKey);
        //             if (yellowOrBlue != null)
        //                 gc.GetComponent<SpriteRenderer>().sprite = yellowOrBlue;

        //         }
        //         //gc.Init();
        //         GridController.Add(go);
        //         Gems.Add(gc);

        //         return gc as T;
        //     }
        //     else if (typeof(T).IsSubclassOf(typeof(CreatureController))) // 일단 모든 몬스터로 퉁침
        //     {
        //         if (Managers.Data.MonsterDict.TryGetValue(templateID, out Data.MonsterData monsterData) == false)
        //         {
        //             Debug.LogError($"@@@@@ MonsterDict load failed, templateID : {templateID} @@@@@");
        //             return null;
        //         }

        //         GameObject go = Managers.Resource.Instantiate(monsterData.prefab, pooling: true);
        //         go.transform.position = position;

        //         if (monsterData.type == Define.MonsterData.Type.Normal)
        //         {
        //             MonsterController mc = go.GetOrAddComponent<MonsterController>();
        //             mc.MonsterData = monsterData; // 여기서 다시 에너지가 채워지네 ;; 풀에서 꺼내면서 // //mc.Init();
        //             Monsters.Add(mc);
        //             return mc as T;
        //         }
        //         else if (monsterData.type == Define.MonsterData.Type.Boss)
        //         {
        //             BossController bc = go.GetOrAddComponent<BossController>();
        //             bc.MonsterData = monsterData;
        //             Monsters.Add(bc);
        //             return bc as T;
        //         }
        //     }
        //     else if (type == typeof(ProjectileController))
        //     {
        //         if (Managers.Data.SkillDict.TryGetValue(templateID, out Data.SkillData skillData) == false)
        //         {
        //             Debug.LogError($"@@@@@ SkillDict load failed, templateID : {templateID} @@@@@");
        //             return null;
        //         }

        //         GameObject go = Managers.Resource.Instantiate(skillData.prefab, pooling: true);
        //         go.transform.position = position;

        //         ProjectileController pc = go.GetOrAddComponent<ProjectileController>();
        //         pc.SkillData = skillData;
        //         Projectiles.Add(pc);
        //         return pc as T;
        //     }
        //     else if (typeof(T).IsSubclassOf(typeof(SkillBase)))
        //     {
        //         if (Managers.Data.SkillDict.TryGetValue(templateID, out Data.SkillData skillData) == false)
        //         {
        //             Debug.LogError($"@@@@@ SkillDict load failed, templateID : {templateID} @@@@@");
        //             return null;
        //         }

        //         GameObject go = Managers.Resource.Instantiate(skillData.prefab, pooling: true);
        //         go.transform.position = position;

        //         T t = go.GetOrAddComponent<T>();
        //         t.Init();

        //         return t;
        //     }

        //     return null;
        // }
    }
}

using System.Collections.Generic;
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

        public T Spawn<T>(Vector3 position, int templateID = 0) where T : BaseController
        {
            System.Type type = typeof(T);
            if (type == typeof(PlayerController))
            {
                if (Managers.Data.PlayerDict.TryGetValue(templateID, out Data.PlayerData playerData) == false)
                {
                    Debug.LogError($"@@@@@ PlayerDict load failed, templateID : {templateID} @@@@@");
                    return null;
                }

                // ***** PlayerPrefab은 PlayerJsonData 파일에 없으므로 일단 Define.LOAD_PLAYER_PREFAB으로 호출
                // 나중에 캐릭터별로 프리팹 네임 필요할수도있음
                GameObject go = Managers.Resource.Instantiate(Define.LOAD_PLAYER_PREFAB);
                go.name = "Player";
                go.transform.position = position;

                PlayerController pc = go.GetOrAddComponent<PlayerController>();
                pc.PlayerData = playerData;
                Player = pc;
                // pc.Init();

                return pc as T;
            }
            else if (type == typeof(GemController)) // 현재 Gem과 관련된 아이템 데이터 시트가 없으므로 임시 코드
            {
                GameObject go = Managers.Resource.Instantiate(Define.LOAD_EXP_GEM_PREFAB, pooling: true);
                go.transform.position = position;

                GemController gc = go.GetOrAddComponent<GemController>();

                bool changeSprite = Random.Range(0, 2) == 0 ? true : false;
                string spriteKey = "";
                if (changeSprite)
                {
                    spriteKey = Random.Range(0, 2) == 0 ?
                                Define.LOAD_EXP_GEM_YELLOW_SPRITE : Define.LOAD_EXP_GEM_BLUE_SPRITE;

                    Sprite yellowOrBlue = Managers.Resource.Load<Sprite>(spriteKey);
                    if (yellowOrBlue != null)
                        gc.GetComponent<SpriteRenderer>().sprite = yellowOrBlue;

                }
                //gc.Init();
                GridController.Add(go);
                Gems.Add(gc);

                return gc as T;
            }
            else if (typeof(T).IsSubclassOf(typeof(CreatureController))) // 일단 모든 몬스터로 퉁침
            {
                if (Managers.Data.MonsterDict.TryGetValue(templateID, out Data.MonsterData monsterData) == false)
                {
                    Debug.LogError($"@@@@@ MonsterDict load failed, templateID : {templateID} @@@@@");
                    return null;
                }

                GameObject go = Managers.Resource.Instantiate(monsterData.prefab, pooling: true);
                go.transform.position = position;

                MonsterController mc = go.GetOrAddComponent<MonsterController>();
                mc.MonsterData = monsterData;
                //mc.Init();
                Monsters.Add(mc);

                return mc as T;
            }
            else if (typeof(T).IsSubclassOf(typeof(SkillController)))
            {
                if (Managers.Data.SkillDict.TryGetValue(templateID, out Data.SkillData skillData) == false)
                {
                    Debug.LogError($"@@@@@ SkillDict load failed, templateID : {templateID} @@@@@");
                    return null;
                }

                GameObject go = Managers.Resource.Instantiate(skillData.prefab, pooling: true);
                go.transform.position = position;

                if (type == typeof(ProjectileController))
                {
                    ProjectileController pc = go.GetOrAddComponent<ProjectileController>();
                    pc.SkillData = skillData; // 데미지를 미리 여기서 설정
                    Projectiles.Add(pc);
                    return pc as T;
                }

                if (type == typeof(EgoSwordController))
                {
                    // TODO...
                }
            }

            return null;
        }


        public T Spawn2<T>(Vector3 position, int templateID = 0) where T : BaseController
        {
            // TemplateID가 0이 아닌것은 모두 체크
            System.Type type = typeof(T);
            if (type == typeof(PlayerController))
            {
                // TODO : use DataSheet
                GameObject go = Managers.Resource.Instantiate(Define.LOAD_PLAYER_PREFAB);
                go.name = "Player";
                go.transform.position = position;

                PlayerController pc = go.GetOrAddComponent<PlayerController>();
                //pc.Init();
                Player = pc;

                return pc as T;
            }
            else if (type == typeof(MonsterController))
            {
                // TODO : use DataSheet
                string name = (templateID == 0 ? Define.LOAD_GOBLIN_PREFAB : Define.LOAD_SNAKE_PREFAB);
                GameObject go = Managers.Resource.Instantiate(name, pooling: true);
                go.transform.position = position;

                MonsterController mc = go.GetOrAddComponent<MonsterController>();
                //mc.Init();
                Monsters.Add(mc);

                return mc as T;
            }
            else if (type == typeof(GemController))
            {
                GameObject go = Managers.Resource.Instantiate(Define.LOAD_EXP_GEM_PREFAB, pooling: true);
                go.transform.position = position;

                GemController gc = go.GetOrAddComponent<GemController>();

                bool changeSprite = Random.Range(0, 2) == 0 ? true : false;
                string spriteKey = "";
                if (changeSprite)
                {
                    spriteKey = Random.Range(0, 2) == 0 ?
                                Define.LOAD_EXP_GEM_YELLOW_SPRITE : Define.LOAD_EXP_GEM_BLUE_SPRITE;

                    Sprite yellowOrBlue = Managers.Resource.Load<Sprite>(spriteKey);
                    if (yellowOrBlue != null)
                        gc.GetComponent<SpriteRenderer>().sprite = yellowOrBlue;

                }
                //gc.Init();
                GridController.Add(go);
                Gems.Add(gc);

                return gc as T;
            }
            else if (typeof(T).IsSubclassOf(typeof(ProjectileController))) // 미친 씨샵 개좋음
            {
                // 나중에 데이터는 위에다가 몰빵해야함
                if (Managers.Data.SkillDict.TryGetValue(templateID, out Data.SkillData skillData) == false)
                {
                    Debug.LogError($"Projectile Spawn Failed !! templateID : {templateID}");
                    return null;
                }

                // *** 현재 인자로 받는 templateID는 아직 제대로 활용 안하는중. 데이터 정리할 때 같이 하면 됨.
                GameObject go = Managers.Resource.Instantiate(skillData.prefab, pooling: true);
                go.transform.position = position;

                ProjectileController pc = go.GetOrAddComponent<ProjectileController>();
                Projectiles.Add(pc); // 안해도됨
                // pc.Init();

                return pc as T;
            }
            else if (type == typeof(EgoSwordController) || typeof(T).IsSubclassOf(typeof(SkillController)))
            {
                if (Managers.Data.SkillDict.TryGetValue(templateID, out Data.SkillData skillData) == false)
                {
                    Debug.LogError("@@@ Spawn Skill Failed @@@");
                    return null;
                }

                GameObject go = Managers.Resource.Instantiate(skillData.prefab, pooling: true);
                go.transform.position = position;

                T t = go.GetOrAddComponent<T>();
                t.Init();

                return t;
            }

            return null;
        }

        public void Despawn<T>(T obj) where T : BaseController
        {
            if (obj.IsValid() == false)
            {
                // 혹시라도 디스폰을 한번 더 한다면 체크
                Debug.Log("<color=magenta>##### Why are you despawning again ???? #####</color>");
            }

            System.Type type = typeof(T);
            if (type == typeof(PlayerController))
            {
            }
            else if (type == typeof(MonsterController))
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
            else if (type == typeof(ProjectileController) || typeof(T).IsSubclassOf(typeof(ProjectileController)))
            {
                Projectiles.Remove(obj as ProjectileController);
                Managers.Resource.Destroy(obj.gameObject);
            }
        }
    }
}

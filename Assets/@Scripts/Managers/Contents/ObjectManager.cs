using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_2D.Define;

namespace STELLAREST_2D
{
    public class ObjectManager
    {
        public PlayerController Player { get; private set; } = null;
        public HashSet<MonsterController> Monsters { get; } = new HashSet<MonsterController>();
        public HashSet<SkillBase> Skills { get; } = new HashSet<SkillBase>();
        public HashSet<GemController> Gems { get; } = new HashSet<GemController>();
        // EnvController는 필요하게 되면 추가할 것.
        public GridController GridController { get; private set; } = null;
        public void OnPlayerDeadHandler() 
        {
            if (Managers.Game != null)
            {
                Managers.Game.OnMoveDirChanged -= this.Player.OnMoveDirChangedHandler;
                Utils.Log("Release Event : Managers.Game.OnGameStart -= Player.OnMoveDirChangedHandler");

                Managers.Game.OnGameStart -= this.Player.OnGameStartHandler;
                Utils.Log("Release Event : Managers.Game.OnGameStart -= Player.OnGameStartHandler");
            }

            this.Player = null;
            Managers.Game.GAME_OVER();
        }

        public void Init()
        {
            GridController = UnityEngine.GameObject.Find("@Grid").GetComponent<GridController>();
            Managers.Game.OnPlayerIsDead += OnPlayerDeadHandler;
        }

        public T Spawn<T>(Vector3 spawnPos, int templateID = -1, Define.ObjectType spawnObjectType = Define.ObjectType.None, bool isPooling = false) where T : BaseController
        {
            switch (spawnObjectType)
            {
                case Define.ObjectType.Player:
                    {
                        GameObject go = Managers.Resource.Instantiate(Managers.Data.CreaturesDict[templateID].PrimaryLabels[0], pooling: isPooling);
                        go.transform.position = spawnPos;

                        PlayerController pc = go.GetComponent<PlayerController>();
                        pc.ObjectType = spawnObjectType;

                        pc.Init(templateID);
                        this.Player = pc;

                        return pc as T;
                    }

                case Define.ObjectType.Monster:
                    {
                        switch (templateID)
                        {
                            case (int)FixedValue.TemplateID.Monster.Chicken:
                                {
                                    GameObject go = Managers.Resource.Instantiate(Managers.Data.CreaturesDict[templateID].PrimaryLabels[0], pooling: isPooling);
                                    go.transform.position = spawnPos;

                                    for (int i = 0; i < go.transform.childCount; ++i)
                                    {
                                        go.transform.GetChild(i).transform.localRotation = Quaternion.identity;
                                        go.transform.GetChild(i).transform.rotation = Quaternion.identity;
                                    }

                                    ChickenController chicken = go.GetComponent<ChickenController>();
                                    chicken.ObjectType = spawnObjectType;
                                    chicken.MonsterType = Define.MonsterType.Chicken;

                                    chicken.Init(templateID);
                                    Monsters.Add(chicken);
                                    Managers.VFX.Environment(VFXEnvType.Spawn, chicken);

                                    return chicken as T;
                                }

                            default:
                                return null;
                        }
                }

                // ---------------------------------------------------------------------------
                // Projectiles are must be skills. But, sometimes skills are not projectiles.
                // ---------------------------------------------------------------------------
                case Define.ObjectType.Skill:
                    {
                        GameObject go = Managers.Resource.Instantiate(Managers.Data.SkillsDict[templateID].PrimaryLabel, pooling: isPooling);
                        go.transform.position = spawnPos;

                        SkillBase skill = go.GetComponent<SkillBase>();
                        skill.ObjectType = spawnObjectType;

                        Skills.Add(skill);
                        return skill as T;
                    }

                case Define.ObjectType.Gem:
                    {
                        GameObject go = Managers.Resource.Instantiate(FixedValue.Load.ENV_GEM, pooling: isPooling);
                        go.transform.position = spawnPos;

                        GemController gem = go.GetComponent<GemController>();
                        gem.ObjectType = spawnObjectType;
                        gem.Init();

                        Gems.Add(gem);
                        GridController.Add(spawnObjectType, gem.gameObject);

                        return gem as T;
                    }

                case Define.ObjectType.Soul:
                    {
                        GameObject go = Managers.Resource.Instantiate(FixedValue.Load.ENV_SOUL, pooling: isPooling);
                        go.transform.position = spawnPos;

                        SoulController soul = go.GetComponent<SoulController>();
                        soul.ObjectType = spawnObjectType;
                        soul.Init();

                        return soul as T;
                    }
            }

            return null;
        }

        public void Despawn<T>(T obj) where T : BaseController
        {
            if (obj.IsValid() == false)
                return;

            System.Type type = typeof(T);
            if (type == typeof(PlayerController))
            {
            }
            else if (typeof(T).IsSubclassOf(typeof(CreatureController)))
            {
                Monsters.Remove(obj.GetComponent<MonsterController>());
                Managers.Resource.Destroy(obj.gameObject);
            }
            else if (type == typeof(GemController))
            {
                Gems.Remove(obj as GemController);
                Managers.Resource.Destroy(obj.gameObject);
                GridController.Remove(Define.ObjectType.Gem, obj.gameObject);
            }
            else if (type == typeof(SkillBase) || type == typeof(ProjectileController))
            {
                // as로 형변환하면 버그 있음. (제거가 될때도있고 안될때도 있음)
                //Skills.Remove(obj as SkillBase);
                Skills.Remove(obj.GetComponent<SkillBase>());
                Managers.Resource.Destroy(obj.gameObject);
            }
        }

        public void DestroySpawnedObject<T>(T obj) where T : BaseController
        {
            System.Type type = typeof(T);
            if (type == typeof(MonsterController))
            {
                Managers.Pool.ClearPool<MonsterController>(obj.gameObject);
                Monsters.Clear();
            }
            else if (type == typeof(SkillBase))
            {
                Managers.Pool.ClearPool<SkillBase>(obj.gameObject);
                Skills.Clear();
            }
        }

        private void OnDestroy()
        {
            if (Managers.Game.OnPlayerIsDead != null)
            {
                Utils.Log("Release Event : OnPlayerDeadHandler");
                Managers.Game.OnPlayerIsDead -= OnPlayerDeadHandler;
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        // public void DespawnAllMonsters()
        // {
        //     var monsters = Monsters.ToList();
        //     foreach (var monster in monsters)
        //     {
        //         if (monster.IsValid() == false)
        //             continue;

        //         Despawn<MonsterController>(monster);
        //         // Managers.Pool.ClearPool<MonsterController> --> Test
        //     }
        // }

        // 해당 dist 안에 가장 몬스터가 밀집되어 있는 곳을 골라서 리턴해준다.
        // public GameObject GetClosestTarget<T>(GameObject from, float fromRange, GameObject priority, float priorityRange) where T : BaseController
        // {
        //     System.Type type = typeof(T);
        //     Vector3 fromPos = from.transform.position;

        //     GameObject priorityTarget = null;
        //     Vector3 priorityPos = Vector3.zero;

        //     if (type == typeof(MonsterController))
        //     {
        //         List<MonsterController> toMonsters = this.Monsters.ToList();
        //         // GameObject fromTarget = toMonsters
        //         //                     .Where(m => m.IsValid() && m.IsCreatureDead() == false)
        //         //                     .Where(m => (m.transform.position - fromPos).sqrMagnitude < fromRange * fromRange)
        //         //                     .OrderBy(m => (m.transform.position - fromPos).sqrMagnitude)
        //         //                     .FirstOrDefault()?
        //         //                     .Body;
        //         GameObject fromTarget = toMonsters
        //                             .Where(m => m.IsValid())
        //                             .Where(m => (m.transform.position - fromPos).sqrMagnitude < fromRange * fromRange)
        //                             .OrderBy(m => (m.transform.position - fromPos).sqrMagnitude)
        //                             .FirstOrDefault()?
        //                             .gameObject;


        //         if (priority != null)
        //         {
        //             priorityPos = priority.transform.position;
        //             // priorityTarget = toMonsters
        //             //             .Where(m => m.IsValid() && m.IsCreatureDead() == false)
        //             //             .Where(m => (m.transform.position - priorityPos).sqrMagnitude < priorityRange * priorityRange)
        //             //             .OrderBy(m => (m.transform.position - priorityPos).sqrMagnitude)
        //             //             .FirstOrDefault()?
        //             //             .Body;

        //             priorityTarget = toMonsters
        //                          .Where(m => m.IsValid())
        //                          .Where(m => (m.transform.position - priorityPos).sqrMagnitude < priorityRange * priorityRange)
        //                          .OrderBy(m => (m.transform.position - priorityPos).sqrMagnitude)
        //                          .FirstOrDefault()?
        //                          .gameObject;
        //         }

        //         return priorityTarget != null ? priorityTarget : fromTarget;
        //     }

        //     return null;
        // }

        // public GameObject GetClosestTarget<T>(GameObject from, float range) where T : BaseController
        // {
        //     System.Type type = typeof(T);
        //     Vector3 fromPos = from.transform.position;
        //     if (type == typeof(MonsterController))
        //     {
        //         List<MonsterController> toMonsters = this.Monsters.ToList();
        //         // GameObject target = toMonsters
        //         //                     .Where(m => m.IsValid() && m.IsCreatureDead() == false)
        //         //                     .Where(m => (m.transform.position - from.transform.position).sqrMagnitude < range * range)
        //         //                     .OrderBy(m => (m.transform.position - fromPos).sqrMagnitude)
        //         //                     .FirstOrDefault()?
        //         //                     .Body;

        //         GameObject target = toMonsters
        //                         .Where(m => m.IsValid())
        //                         .Where(m => (m.transform.position - from.transform.position).sqrMagnitude < range * range)
        //                         .OrderBy(m => (m.transform.position - fromPos).sqrMagnitude)
        //                         .FirstOrDefault()?
        //                         .gameObject;

        //         return target;
        //     }

        //     return null;
        // }

        // public Vector2 GetRandomTargetingPosition<T>(GameObject from, float fromMinDistance = 1f, float fromMaxDistance = 22f,
        //                    Define.TemplateIDs.CreatureStatus.RepeatSkill skillHitStatus = Define.TemplateIDs.CreatureStatus.RepeatSkill.None) where T : BaseController
        // {
        //     System.Type type = typeof(T);
        //     Vector2 fromPos = from.transform.position;
        //     if (type == typeof(MonsterController))
        //     {
        //         List<MonsterController> toMonsters = this.Monsters.ToList();
        //         if (toMonsters.Count != 0)
        //         {
        //             List<MonsterController> toFilteredMonsters = toMonsters
        //                     .Where(m => m.IsValid() && m.IsCreatureDead() == false && m.IsSkillHittedStatus(skillHitStatus) == false)
        //                     .Where(m => (m.transform.position - from.transform.position).sqrMagnitude >= fromMinDistance && (m.transform.position - from.transform.position).sqrMagnitude <= fromMaxDistance * fromMaxDistance)
        //                     .ToList();

        //             if (toFilteredMonsters.Count > 0)
        //             {
        //                 int randIdx = Random.Range(0, toFilteredMonsters.Count);
        //                 return toFilteredMonsters[randIdx].transform.position;
        //             }
        //             else
        //                 return Utils.GetRandomPosition(fromPos);
        //         }
        //     }
        //     return Utils.GetRandomPosition(fromPos);
        // }

        // // +++ 개선 필요 +++
        // public GameObject GetNextTarget(GameObject from, Define.TemplateIDs.CreatureStatus.RepeatSkill checkBounceHitSkillType = Define.TemplateIDs.CreatureStatus.RepeatSkill.None)
        // {
        //     GameObject target = null;
        //     List<MonsterController> toMonsters = this.Monsters.ToList();
        //     target = toMonsters
        //             .Where(m => m.IsSkillHittedStatus(checkBounceHitSkillType) == false)
        //             .OrderBy(m => (m.transform.position - from.transform.position).sqrMagnitude)
        //             .FirstOrDefault()?
        //             .gameObject;

        //     return target;
        // }

        // public void ResetSkillHittedStatus(Define.TemplateIDs.CreatureStatus.RepeatSkill skillType)
        // {
        //     switch (skillType)
        //     {
        //         case Define.TemplateIDs.CreatureStatus.RepeatSkill.ThrowingStar:
        //             {
        //                 foreach (var monster in Monsters.ToList())
        //                     monster.IsThrowingStarHit = false;
        //             }
        //             break;

        //         case Define.TemplateIDs.CreatureStatus.RepeatSkill.LazerBolt:
        //             {
        //                 foreach (var monster in Monsters.ToList())
        //                     monster.IsLazerBoltHit = false;
        //             }
        //             break;
        //     }
        // }
    }
}

// float closestDist = float.MaxValue;
// List<MonsterController> toMonsters = new List<MonsterController>();
// foreach (var mon in this.Monsters)
//     toMonsters.Add(mon);

// foreach (var mon in toMonsters)
// {
//     if(mon.transform == from || mon.IsBounceHitStatus(checkBounceHitSkillType) == false)
//         continue;

//     float sqrMag = (mon.transform.position - tr.position).sqrMagnitude;
//     if (sqrMag < closestDist)
//     {
//         closestDist = sqrMag;
//         target = mon.gameObject;
//     }
// }
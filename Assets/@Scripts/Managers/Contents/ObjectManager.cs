
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering; // Sorting Group

// using DamageNumbersPro;
namespace STELLAREST_2D
{
    public class ObjectManager
    {
        public PlayerController Player { get; private set; }
        public HashSet<MonsterController> Monsters { get; } = new HashSet<MonsterController>();
        public HashSet<ProjectileController> Projectiles { get; } = new HashSet<ProjectileController>();
        public HashSet<SkillBase> Skills { get; } = new HashSet<SkillBase>();
        public HashSet<GemController> Gems { get; } = new HashSet<GemController>();
        // EnvCont는 나중에 추가하던지
        public GridController GridController { get; private set; }

        public void Init()
        {
            GridController = UnityEngine.GameObject.Find("@Grid").GetComponent<GridController>();
        }

        public T Spawn<T>(Vector3 spawnPos, int templateID, Define.ObjectType spawnObjectType, bool isPooling = false) where T : BaseController
        {
            switch (spawnObjectType)
            {
                case Define.ObjectType.Player:
                    {
                        GameObject go = Managers.Resource.Instantiate(Managers.Data.CreaturesDict[templateID].PrimaryLabel, pooling: isPooling);
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
                            case (int)Define.TemplateIDs.Creatures.Monster.Chicken:
                                {
                                    GameObject go = Managers.Resource.Instantiate(Managers.Data.CreaturesDict[templateID].PrimaryLabel, pooling: isPooling);
                                    go.transform.position = spawnPos;

                                    ChickenController chicken = go.GetComponent<ChickenController>();
                                    chicken.ObjectType = spawnObjectType;
                                    chicken.MonsterType = Define.MonsterType.Chicken;

                                    chicken.Init(templateID);
                                    Monsters.Add(chicken);

                                    return chicken as T;
                                }
                            default:
                                return Managers.Resource.Instantiate(Managers.Data.CreaturesDict[templateID].PrimaryLabel, pooling: isPooling) as T;
                        }
                }

                // --------------------------------------------------------------------------------------------------
                // ***** Projectile is must be skill. But, skill is not sometimes projectil. It's just a skill. *****
                // --------------------------------------------------------------------------------------------------
                case Define.ObjectType.Skill:
                    {
                        GameObject go = Managers.Resource.Instantiate(Managers.Data.SkillsDict[templateID].PrimaryLabel, pooling: isPooling);
                        go.transform.position = spawnPos;

                        SkillBase skill = go.GetComponent<SkillBase>();
                        skill.ObjectType = spawnObjectType;

                        Skills.Add(skill);
                        return skill as T;
                    }

                // case Define.ObjectType.Projectile:
                // {
                //         GameObject go = Managers.Resource.Instantiate(Managers.Data.SkillsDict[templateID].PrimaryLabel, pooling: isPooling);
                //         go.transform.position = spawnPos;

                //         ProjectileController pc = go.GetOrAddComponent<ProjectileController>();
                //         pc.ObjectType = spawnObjectType;

                //         // pc.Init(templateID);
                //         Projectiles.Add(pc);

                //         return pc as T;
                // }
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
            }
            else if (typeof(T).IsSubclassOf(typeof(CreatureController)))
            {
                //Utils.LogStrong("DESPAWN MONSTER...2 !!!");
                Monsters.Remove(obj as MonsterController);
                Managers.Resource.Destroy(obj.gameObject);
            }
            else if (type == typeof(GemController))
            {
                Gems.Remove(obj as GemController);
                Managers.Resource.Destroy(obj.gameObject);
                GridController.Remove(obj.gameObject);
            }
            else if (type == typeof(SkillBase))
            {
                Skills.Remove(obj as SkillBase);
                Managers.Resource.Destroy(obj.gameObject);
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

        public MonsterController GetClosestNextMonsterTarget(CreatureController from, Define.HitFromType hitFromStatusFilter = Define.HitFromType.None)
        {
            List<MonsterController> toList = new List<MonsterController>();
            foreach (var mon in Monsters)
            {
                if (mon.IsValid())
                    toList.Add(mon);
            }

            Vector3 fromPos = from.transform.position;
            MonsterController nextTarget = null;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < toList.Count; ++i)
            {
                if (toList[i] == from)
                    continue;

                // 그 사이에 죽을수도 있다.
                if (toList[i].IsValid() == false)
                    continue;

                if (hitFromStatusFilter != Define.HitFromType.None)
                {
                    if (hitFromStatusFilter == Define.HitFromType.ThrowingStar)
                    {
                        if (toList[i].IsHitFrom_ThrowingStar)
                            continue;
                    }
                }

                float fromNextDist = (toList[i].transform.position - fromPos).sqrMagnitude;
                if (fromNextDist < closestDistance)
                {
                    closestDistance = fromNextDist;
                    nextTarget = toList[i];
                }
            }

            return nextTarget;
        }
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

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
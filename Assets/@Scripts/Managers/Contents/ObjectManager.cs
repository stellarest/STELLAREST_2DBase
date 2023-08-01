
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                    GameObject go = Managers.Resource.Instantiate(Managers.Data.CreatureDict[templateID].PrimaryLabel, pooling: false);
                    PlayerController pc = go.GetOrAddComponent<PlayerController>();
                    Player = pc;
                    pc.SetInfo(templateID);

                    Managers.Effect.Init();
                    Managers.Effect.AddCreatureMaterials(pc);
                    Managers.Sprite.InitPlayerSprite(pc);

                    return pc as T;
                }

                if (type == typeof(Chicken))
                {
                    GameObject go = Managers.Resource.Instantiate(Managers.Data.CreatureDict[templateID].PrimaryLabel, pooling: true);
                    go.transform.position = position;
                    Vector3 spawnEffectPos = new Vector3(go.transform.position.x, go.transform.position.y + 3.8f, go.transform.position.z);
                    Managers.Effect.ShowSpawnEffect(Define.PrefabLabels.SPAWN_EFFECT, spawnEffectPos);

                    Chicken mc = go.GetOrAddComponent<Chicken>();
                    mc.SetInfo(templateID);
                    mc.Init();
                    Monsters.Add(mc);

                    Managers.Effect.AddCreatureMaterials(mc);
                    Managers.Effect.SetDefaultMaterials(mc);

                    // mc.CoEffectFade();
                    // mc.CoEffectFadeIn(2f);
                    mc.CoStartReadyToAction();

                    return mc as T;
                }
            }
            else if (type == typeof(GemController))
            {
                //GameObject go = Managers.Resource.Instantiate(Define.PrefabLabels.EXP_GEM, pooling: true);
                GameObject go = Managers.Resource.Instantiate(Define.PrefabLabels.GEM, pooling: true);
                go.transform.position = position;

                GemController gc = go.GetOrAddComponent<GemController>();

                // gc.Init();
                // bool changeSprite = Random.Range(0, 2) == 0 ? true : false;
                // string spriteKey = "";
                // if (changeSprite)
                // {
                //     spriteKey = Random.Range(0, 2) == 0 ?
                //                 Define.SpriteLabels.EXP_GEM_BLUE : Define.SpriteLabels.EXP_GEM_YELLOW;

                //     Sprite yellowOrBlue = Managers.Resource.Load<Sprite>(spriteKey);
                //     if (yellowOrBlue != null)
                //         gc.GetComponent<SpriteRenderer>().sprite = yellowOrBlue;
                // }

                GridController.Add(go);
                Gems.Add(gc);

                return gc as T;
            }
            else if (type == typeof(ProjectileController))
            {
                GameObject go = Managers.Resource.Instantiate(Managers.Data.SkillDict[templateID].PrimaryLabel, pooling: true);
                go.transform.position = position;

                ProjectileController pc = go.GetOrAddComponent<ProjectileController>();
                Projectiles.Add(pc);

                return pc as T;
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
                Utils.LogStrong("DESPAWN MONSTER...2 !!!");
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

        public MonsterController GetRandomTarget(CreatureController cc, float distanceFromPlayer = 20f)
        {
            MonsterController target = null;
            List<MonsterController> toMonsters = this.Monsters.ToList();
            if (toMonsters.Count != 0)
            {
                for (int i = 0; i < toMonsters.Count; ++i)
                {
                    if ((toMonsters[i].transform.position - cc.transform.position).sqrMagnitude < distanceFromPlayer * distanceFromPlayer)
                        target = toMonsters[i];
                }
            }

            return target;
        }

        public GameObject GetContinuousTarget()
        {
            GameObject target = null;
            List<MonsterController> toMonsters = this.Monsters.ToList();

            return target;
        }

        public bool IsAllMonsterBounceHitStatus(Define.TemplateIDs.SkillType checkBounceHitSkillType)
        {
            List<MonsterController> toMonsters = Monsters.ToList();
            return toMonsters.All(m => m.IsContinuousHitStatus(checkBounceHitSkillType));
        }

        // +++ 개선 필요 +++
        public GameObject GetNextTarget(Transform from, Define.TemplateIDs.SkillType checkBounceHitSkillType = Define.TemplateIDs.SkillType.None)
        {
            // m => m.transform != from && 
            // .Where(m => m.transform != tr && m.IsThrowingStarBounceHit == false)
            GameObject target = null; 
#if USE_LINQ
            List<MonsterController> toMonsters = this.Monsters.ToList();
            target = toMonsters
                    .Where(m => m.IsContinuousHitStatus(checkBounceHitSkillType) == false)
                    .OrderBy(m => (m.transform.position - from.position).sqrMagnitude)
                    .FirstOrDefault()?
                    .gameObject;
#else
#endif
            return target;
        }

        public void ResetBounceHits(Define.TemplateIDs.SkillType bounceHitSkillType)
        {
#if USE_LINQ
            switch (bounceHitSkillType)
            {
                case Define.TemplateIDs.SkillType.ThrowingStar:
                    {
                        foreach (var monster in Monsters.ToList())
                            monster.IsThrowingStarHit = false;
                    }
                    break;

                case Define.TemplateIDs.SkillType.LazerBolt:
                    {
                        foreach (var monster in Monsters.ToList())
                            monster.IsLazerBoltHit = false;
                    }
                    break;
            }
#else
#endif
        }
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
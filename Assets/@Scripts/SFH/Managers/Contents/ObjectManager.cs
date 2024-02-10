using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class ObjectManager
    {
        public HashSet<Hero> Heroes = new HashSet<Hero>();
        public HashSet<Monster> Monsters = new HashSet<Monster>();

        public Transform GetRootTransform(string name)
        {
            GameObject root = GameObject.Find(name);
            if (root == null)
                root = new GameObject { name = name };
            
            return root.transform;
        }

        public Transform HeroRoot => GetRootTransform(FixedValue.String.ROOT_HERO);
        public Transform MonsterRoot => GetRootTransform(FixedValue.String.ROOT_MONSTER);
        public Transform ProjectileRoot => GetRootTransform(FixedValue.String.ROOT_PROJECTILE);
        public Transform EnvRoot => GetRootTransform(FixedValue.String.ROOT_ENV);

        public T Spawn<T>(Vector3 position, int templateID) where T : BaseObject
        {
            string prefabName = typeof(T).Name;

            GameObject go = Managers.Resource.Instantiate(prefabName);
            go.name = prefabName;
            go.transform.position = position;

            BaseObject baseObj = go.GetComponent<BaseObject>();
            if (baseObj.ObjectType == EObjectType.Creature)
            {
                Creature creature = go.GetComponent<Creature>();
                switch (creature.CreatureType)
                {
                    case ECreatureType.Hero:
                        baseObj.transform.SetParent(HeroRoot);
                        Hero hero = creature as Hero;
                        this.Heroes.Add(hero);
                        break;

                    case ECreatureType.Monster:
                        baseObj.transform.SetParent(MonsterRoot);
                        Monster monster = creature as Monster;
                        this.Monsters.Add(monster);
                        break;
                }
            }
            else if (baseObj.ObjectType == EObjectType.Projectile)
            {
            }
            else if (baseObj.ObjectType == EObjectType.Env)
            {
            }
            else if (baseObj.ObjectType == EObjectType.HeroCamp)
            {
            }

            return baseObj as T;
        }
    }
}

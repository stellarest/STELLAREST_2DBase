using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class Utils
    {
        public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
        {
            T component = go.GetComponent<T>();
            if (component == null)
                component = go.AddComponent<T>();
            return component;
        }

        public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
        {
            Transform tr = FindChild<Transform>(go, name, recursive);
            if (tr == null)
                return null;

            return tr.gameObject;
        }

        public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
        {
            if (go == null)
                return null;

            if (recursive == false)
            {
                for (int i = 0; i < go.transform.childCount; ++i)
                {
                    Transform tr = go.transform.GetChild(i);
                    if (string.IsNullOrEmpty(name) || tr.name == name)
                    {
                        T comp = tr.GetComponent<T>();
                        if (comp != null)
                            return comp;
                    }
                }
            }
            else
            {
                foreach (T comp in go.GetComponentsInChildren<T>())
                {
                    if (string.IsNullOrEmpty(name) || comp.name == name)
                        return comp;
                }
            }

            return null;
        }

        public static Vector2 GenerateMonsterSpawnPosition(Vector2 characterPosition, float minDistance = 10.0f, float maxDistance = 20.0f)
        {
            float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
            float distance = Random.Range(minDistance, maxDistance);

            float xDist = Mathf.Cos(angle) * distance;
            float yDist = Mathf.Sin(angle) * distance;

            // charactet 중심으로 원형의 범위 내에서 랜덤하게 스폰
            Vector2 spawnPosition = characterPosition + new Vector2(xDist, yDist);
            
            return spawnPosition;
        }
    }

    public static class TempPrefabKeyLoader
    {
        public static readonly string SLIME;
        public static readonly string SNAKE;
        public static readonly string GOBLIN;
        public static readonly string TEST_MAP;
        public static readonly string EXP_GEM;
        public static readonly string BOSS;

        static TempPrefabKeyLoader()
        {
            SLIME = Managers.Data.GetPrefabKey("Slime");
            SNAKE = Managers.Data.GetPrefabKey("Snake");
            GOBLIN = Managers.Data.GetPrefabKey("Goblin");
            TEST_MAP = Managers.Data.GetPrefabKey("TestMap");
            EXP_GEM = Managers.Data.GetPrefabKey("EXPGem");
            BOSS = Managers.Data.GetPrefabKey("Boss");
        }
    }
}

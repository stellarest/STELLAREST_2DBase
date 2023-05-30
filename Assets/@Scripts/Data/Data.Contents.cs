using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace STELLAREST_2D.Data
{
    [System.Serializable]
    public class PlayerData
    {
        public int level;
        public int maxHp;
        public int attack; // Skill의 Attack으로 뺴야할듯. 그러나 캐릭터마다 특성을 부여하려면 있어야될수도. 일단 냅두자.
        public float moveSpeed;
        public int totalExp;
    }

    [System.Serializable]
    public class SkillData
    {
        public int templateID;
        public string name;
        //public string type;
        //[JsonConverter(typeof(StringEnumConverter))] --> Serialize 할 때 쓰는 것임. enumType 제이슨 가독성을 위해서.
        public Define.SkillType type = Define.SkillType.None; // enum type이 안들어와지면 string으로 임시적으로 받아와서 넣어주면 된다고함 
        public string prefab;
        public int damage;
        public float speed;
    }

    [System.Serializable]
    public class MonsterData
    {
        public int templateID;
        public string name;
        public Define.MonsterType type = Define.MonsterType.None;
        public string prefab;
        public int maxHp;
        public int attack;
        public float moveSpeed;
        public int exp;
    }

    [System.Serializable]
    public class PlayerDataLoader : ILoader<int, PlayerData>
    {
        public List<PlayerData> stats = new List<PlayerData>();

        public Dictionary<int, PlayerData> MakeDict()
        {
            if (stats.Count == 0)
            {
                Debug.LogError("@@@@@ Load failed PlayerData.json !! @@@@@");
                return null;
            }
            else
                Debug.Log("<color=cyan> Load success PlayerData.json </color>");

            Dictionary<int, PlayerData> dict = new Dictionary<int, PlayerData>();
            foreach (PlayerData stat in stats)
                dict.Add(stat.level, stat);

            return dict;
        }
    }

    [System.Serializable]
    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> skills = new List<SkillData>();

        public Dictionary<int, SkillData> MakeDict()
        {
            if (skills.Count == 0)
            {
                Debug.LogError("@@@@@ Load failed SkillData.json !! @@@@@");
                return null;
            }
            else
                Debug.Log("<color=cyan> Load success SkillData.json </color>");

            Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
            foreach (SkillData skill in skills)
                dict.Add(skill.templateID, skill);

            return dict;
        }
    }

    [SerializeField]
    public class MonsterDataLoader : ILoader<int, MonsterData>
    {
        public List<MonsterData> monsters = new List<MonsterData>();

        public Dictionary<int, MonsterData> MakeDict()
        {
            if (monsters.Count == 0)
            {
                Debug.LogError("@@@@@ Load failed MonsterData.json !! @@@@@");
                return null;
            }
            else
                Debug.Log("<color=cyan> Load success MonsterData.json </color>");

            Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
            foreach (MonsterData monster in monsters)
                dict.Add(monster.templateID, monster);

            return dict;
        }
    }
}

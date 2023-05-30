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
        public int attack;
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

        /*
          "templateID" : "1",
          "name" : "FireBall",
          "type" : "Projectile",
          "prefab" : "FireProjectile.prefab",
          "damage" : "1000",
          "speed" : "3f"
        */
    }

    [System.Serializable]
    public class PlayerDataLoader : ILoader<int, PlayerData>
    {
        public List<PlayerData> stats = new List<PlayerData>();

        public Dictionary<int, PlayerData> MakeDict()
        {
            if (stats.Count == 0)
            {
                Debug.LogWarning("Load failed PlayerData.json !!");
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
        //public List<SkillData> skills = new List<SkillData>();
        public SkillData skillDataSingle = new SkillData();

        public Dictionary<int, SkillData> MakeDict()
        {
            Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();

            // foreach (SkillData skill in skills)
            //     dict.Add(skill.templateID, skill);
            if (skillDataSingle == null)
                Debug.LogWarning("Load failed SkillData.json");
            else
                Debug.Log("<color=cyan> Load success SkillData.json </color>");

            dict.Add(skillDataSingle.templateID, skillDataSingle);
            return dict;
        }
    }
}

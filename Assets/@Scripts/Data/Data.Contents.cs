using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

// ENV
// - EXP GEM
// - SOUL
namespace STELLAREST_2D.Data
{
    [Serializable]
    public class CreatureData
    {
        /*
            TemplateID
            101000 ~ ... : Player Characters
            201000 ~ ... : Normal Monsters
            301000 ~ ... : Middle Boss Monsters
            401000 ~ ... : Boss Monsters
        */

        public int TemplateID;
        public string Name;
        public string PrefabLabel;
        public Define.TemplateIDs.SkillType DefaultSkillType;
        public int MaxHp;
        public int Strength; // 힘
        public float MoveSpeed;
        public float Luck;
        public Define.WeaponType WeaponType;
        public string IconLabel;
    }

    [Serializable]
    public class CreatureDataLoader : ILoader<int, CreatureData>
    {
        public List<CreatureData> creatures = new List<CreatureData>();
        public Dictionary<int, CreatureData> MakeDict()
        {
            if (creatures.Count == 0)
            {
                Debug.LogAssertion("!!! Failed to load CreatureData.json !!!");
                Debug.Break();
            }

            Dictionary<int, CreatureData> dict = new Dictionary<int, CreatureData>();
            foreach (var creature in creatures)
                dict.Add(creature.TemplateID, creature);

            Debug.Log("<color=cyan>### Success to load CreatureData.json</color> ###");
            return dict;
        }
    }

    #region TEMP SkillData
    [Serializable]
    public class SkillData
    {
        // Skill
        // - InGame Acquired
        // ---> Sequence
        // ---> Repeated

        public int TemplateID;
        public string Name;
        public string PrefabLabel;
        public int Damage;
        public float ProjectileSpeed; // Projectile
        public float CoolTime;
    }

    [Serializable]
    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> skills = new List<SkillData>();

        public Dictionary<int, SkillData> MakeDict()
        {
            if (skills.Count == 0)
            {
                Debug.LogError("!!! Failed to load SkillData.json !!!");
                Debug.Break();
            }

            Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
            foreach (SkillData skill in skills)
                dict.Add(skill.TemplateID, skill);

            Debug.Log("<color=cyan>### Success to load SkillData.json</color> ###");
            return dict;
        }
    }
    #endregion
}

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
        public int TemplateID;
        public string Name;
        public string PrimaryLabel;
        public int InGameGrade;
        public float MaxHp;
        public float MaxHpUpRate;
        public float Power;
        public float PowerUpRate;
        public float Armor;
        public float ArmorUpRate;
        public float MoveSpeed;
        public float MoveSpeedUpRate;
        public float Range;
        public float RangeUpRate;
        public float Agility;
        public float AgilityUpRate;
        public float Critical;
        public float CriticaUpRate;
        public float RepeatAttackAnimSpeed;
        public float RepeatAttackAnimSpeedUpRate;
        public float RepeatAttackCoolTime;
        public float RepeatAttackCoolTimeUpRate;
        public float Luck;
        public float LuckUpRate;
        public float TotalExp;
        public string IconLabel;
        public List<int> InGameSkillList;
    }

    [Serializable]
    public class CreatureDataLoader : ILoader<int, CreatureData>
    {
        public List<CreatureData> creatures = new List<CreatureData>();
        public Dictionary<int, CreatureData> MakeDict()
        {
            if (creatures.Count == 0)
            {
                Utils.LogError("Failed to load CreatureData.json");
                Debug.Break();
            }

            Dictionary<int, CreatureData> dict = new Dictionary<int, CreatureData>();
            foreach (var creature in creatures)
                dict.Add(creature.TemplateID, creature);

            return dict;
        }
    }

    [Serializable]
    public class SkillData
    {
        public int TemplateID;
        public int OriginTemplateID;
        public string Name;
        public string PrimaryLabel;
        public Define.InGameGrade InGameGrade;
        public float DamageUpMultiplier;
        public bool IsPlayerDefaultAttack;
        public float Speed;
        public float Duration;
        public int ContinuousCount;
        public float ContinuousSpacing;
        public int BounceCount;
        public int PenetrationCount;
        public int HitEffectTemplateID;
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
                Utils.LogError("Failed to load SkillData.json");
                Debug.Break();
            }

            Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
            foreach (SkillData skill in skills)
                dict.Add(skill.TemplateID, skill);

            return dict;
        }
    }

    [System.Serializable]
    public class WaveData
    {
    }
}

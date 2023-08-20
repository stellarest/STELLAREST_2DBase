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
        public float MaxHp;
        public float MaxHpUp;
        public float HpRegen;
        public float LifeStealChance;
        public float DamageUp;
        public float CriticalChance;
        public float CoolTimeDown;
        public float Armor;
        public float ArmorUp;
        public float DodgeChance;
        public float Resistance;
        public float MoveSpeed;
        public float MoveSpeedUp;
        public float CollectRange;
        public float Luck;
        public float TotalExp;
        public float MinReadyToActionTime;
        public float MaxReadyToActionTime;
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
                Debug.LogError("Failed to load CreatureData.json");
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
        public string ModelingLabel;
        public bool IsPlayerDefaultAttack;
        public bool IsOnFireSocket;
        public Define.InGameGrade InGameGrade;
        public float MinDamage;
        public float MaxDamage;
        public float Speed;
        public float AnimationSpeed;
        public float Duration;
        public int ContinuousCount;
        public float ContinuousSpacing;
        public float[] ContinuousSpeedRatios;
        public float[] ContinuousAngles;
        public float[] ContinuousFlipXs;
        public float[] ContinuousFlipYs;
        public float[] ShootDirectionIntensities;
        public Vector2[] InterpolateTargetScales;
        public float CollisionKeepingRatio;
        public int BounceCount;
        public float SelfRotationZSpeed;
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
                Debug.LogError("Failed to load SkillData.json");
                Debug.Break();
            }

            Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
            foreach (SkillData skill in skills)
                dict.Add(skill.TemplateID, skill);

            return dict;
        }
    }

    [Serializable]
    public class PassiveSkillData
    {
        public int TemplateID;
        public string Name;
        public string Description;
        public Define.InGameGrade InGameGrade;
        public float MaxHpUp;
        public float HpRegenUp;
        public float LifeStealChanceUp;
        public float DamageUp;
        public float CriticalChanceUp;
        public float CoolTimeDown;
        public float ArmorUp;
        public float DodgeChanceUp;
        public float ResistanceUp;
        public float MoveSpeedUp;
        public float LuckUp;
        public float ExpUp;
    }

    [Serializable]
    public class PassiveSkillDataLoader : ILoader<int, PassiveSkillData>
    {
        public List<PassiveSkillData> passiveSkills = new List<PassiveSkillData>();

        public Dictionary<int, PassiveSkillData> MakeDict()
        {
            if (passiveSkills.Count == 0)
            {
                Debug.LogError("Failed to load PassiveSkillData.json");
                Debug.Break();
            }

            Dictionary<int, PassiveSkillData> dict = new Dictionary<int, PassiveSkillData>();
            foreach (PassiveSkillData passiveSkill in passiveSkills)
                dict.Add(passiveSkill.TemplateID, passiveSkill);

            return dict;
        }
    }

    [System.Serializable]
    public class WaveData
    {
    }
}

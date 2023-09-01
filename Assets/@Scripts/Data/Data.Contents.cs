using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace STELLAREST_2D.Data
{
    [Serializable]
    public class CreatureData
    {
        public int TemplateID;
        public string Name;
        public string Description;
        public string PrimaryLabel;
        public float MaxHp;
        public float Damage;
        public float Critical;
        public float AttackSpeed;
        public float CoolDown;
        public float Armor;
        public float Dodge;
        public float MoveSpeed;
        public float CollectRange;
        public float Luck;
        public float TotalExp;
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
        public bool IsOnlyFixedRotation;
        public bool[] IsOnHits;
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
        public float[] ContinuousFixedRotations;
        public float[] ContinuousFlipXs;
        public float[] ContinuousFlipYs;
        public float[] ContinuousPowers;
        public Vector2[] InterpolateTargetScales;
        public float CollisionKeepingRatio;
        public int BounceCount;
        public float SelfRotationZSpeed;
        public int PenetrationCount;
        public Define.TemplateIDs.BonusStatType BonusStatTemplateID;
        public Define.TemplateIDs.BonusBuffType BonusBuffTemplateID;
        public Define.TemplateIDs.HitEffectType HitEffectTemplateID;
        public Define.TemplateIDs.UltimateSequenceType UltimateSequenceTemplateID;
        public bool HasCC;
        public Define.CCType CCType;
        public float CCRate;
        public float CCDuration;
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
    public class BonusStatData
    {
        public int TemplateID;
        public string Name;
        public string Description;
        Define.InGameGrade InGameGrade;
        public float MaxHpUp;
        public float DamageUp;
        public float CriticalUp;
        public float AttackSpeedUp;
        public float CoolDownUp;
        public float ArmorUp;
        public float DodgeUp;
        public float MoveSpeedUp;
        public float CollectRangeUp;
        public float LuckUp;
    }

    [Serializable]
    public class BonusStatDataLoader : ILoader<int, BonusStatData>
    {
        public List<BonusStatData> bonusStats = new List<BonusStatData>();

        public Dictionary<int, BonusStatData> MakeDict()
        {
            if (bonusStats.Count == 0)
            {
                Debug.LogError("Failed to load PassiveSkillData.json");
                Debug.Break();
            }

            Dictionary<int, BonusStatData> dict = new Dictionary<int, BonusStatData>();
            foreach (BonusStatData bonusStat in bonusStats)
                dict.Add(bonusStat.TemplateID, bonusStat);

            return dict;
        }
    }

    [Serializable]
    public class BonusBuffData
    {
        public int TemplateID;
        public string Name;
        public string Description;
        public string PrimaryLabel;
        public Define.InGameGrade InGameGrade;
        public float Duration;
        public float CoolTime;
        public bool IsOnParent;
        public bool IsLoopType;
        public BuffBase.BuffType BuffType;
    }

    [Serializable]
    public class BonusBuffDataLoader : ILoader<int, BonusBuffData>
    {
        public List<BonusBuffData> bonusBuffs = new List<BonusBuffData>();

        public Dictionary<int, BonusBuffData> MakeDict()
        {
            if (bonusBuffs.Count == 0)
            {
                Debug.LogError("Failed to load PassiveSkillData.json");
                Debug.Break();
            }

            Dictionary<int, BonusBuffData> dict = new Dictionary<int, BonusBuffData>();
            foreach (BonusBuffData buff in bonusBuffs)
                dict.Add(buff.TemplateID, buff);

            return dict;
        }
    }

    [System.Serializable]
    public class WaveData
    {
    }
}

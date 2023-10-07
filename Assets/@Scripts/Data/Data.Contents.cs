using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace STELLAREST_2D.Data
{
    [Serializable]
    public class InitialCreatureData
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
        public float MovementSpeed;
        public float CollectRange;
        public float Luck;
        public float TotalExp;
        public int OtherExclusiveSkillSocket;
        public List<int> RepeatSkillList;
        public List<int> SequenceSkillList;
        public FaceExpressionLoader[] FaceExpressionsLoader;
    }

    [Serializable]
    public class InitialCreatureDataLoader : ILoader<int, InitialCreatureData>
    {
        public List<InitialCreatureData> initialCreatures = new List<InitialCreatureData>();
        public Dictionary<int, InitialCreatureData> MakeDict()
        {
            if (initialCreatures.Count == 0)
            {
                Debug.LogError("Failed to load CreatureData.json");
                Debug.Break();
            }

            Dictionary<int, InitialCreatureData> dict = new Dictionary<int, InitialCreatureData>();
            foreach (var creature in initialCreatures)
                dict.Add(creature.TemplateID, creature);

            return dict;
        }
    }

    [Serializable]
    public class CreatureStatData
    {
        public int TemplateID;
        public string Name;
        public string Description;
        Define.InGameGrade InGameGrade;
        public float MaxHpUp;
        public float DamageUp; // 전체 데미지 증가량 (적게 증가함)
        public float CriticalUp;
        public float AttackSpeedUp;
        public float CoolDownUp;
        public float ArmorUp;
        public float DodgeUp;
        public float MovementSpeedUp;
        public float CollectRangeUp;
        public float LuckUp;
    }

    [Serializable]
    public class CreatureStatDataLoader : ILoader<int, CreatureStatData>
    {
        public List<CreatureStatData> creatureStats = new List<CreatureStatData>();

        public Dictionary<int, CreatureStatData> MakeDict()
        {
            if (creatureStats.Count == 0)
            {
                Debug.LogError("Failed to load CreatureStatData.json");
                Debug.Break();
            }

            Dictionary<int, CreatureStatData> dict = new Dictionary<int, CreatureStatData>();
            foreach (CreatureStatData stat in creatureStats)
                dict.Add(stat.TemplateID, stat);

            return dict;
        }
    }

    [Serializable]
    public class SkillData
    {
        public int TemplateID;
        public Define.TemplateIDs.Status.Skill OriginalTemplate;
        public Define.SkillType SkillType;
        public string Name;
        public string Description;
        public string PrimaryLabel;
        public string ModelingLabel;
        public bool HasEventHandler;
        public bool IsProjectile;
        public bool IsOnFireSocket;
        public Define.InGameGrade Grade;
        public Define.InGameGrade MaxGrade;
        public float MinDamage;
        public float MaxDamage;
        public float MovementSpeed;
        public float RotationSpeed;
        public float Duration;
        public int ContinuousCount;
        public float ContinuousSpacing;
        public float[] ContinuousSpeedRatios;
        public float[] ContinuousAngles;
        public float[] ContinuousFixedRotations;
        public float[] ContinuousFlipXs;
        public float[] ContinuousFlipYs;
        public Vector3[] AdditionalLocalPositions;
        public Vector3[] ScaleInterpolations;
        public bool[] IsOnlyVisibles;
        public float ColliderPreDisableLifeRatio;
        public int MaxBounceCount;
        public int MaxPenetrationCount;
        public Define.TemplateIDs.Status.AfterEffect AfterEffectType;
        public Define.TemplateIDs.VFX.Muzzle VFX_Muzzle;
        public Define.TemplateIDs.VFX.Impact VFX_Impact;
        public bool IsImpactPointOnTarget;
        // public Define.TemplateIDs.VFX.Environment VFX_Environment; // VFX_Env는 데이터에서 일단 제외
        public Define.TemplateIDs.Status.Skill UnlockSkillTemplate;
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
    public class BuffSkillData
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
    public class BuffSkillDataLoader : ILoader<int, BuffSkillData>
    {
        public List<BuffSkillData> bonusBuffs = new List<BuffSkillData>();

        public Dictionary<int, BuffSkillData> MakeDict()
        {
            if (bonusBuffs.Count == 0)
            {
                Debug.LogError("Failed to load PassiveSkillData.json");
                Debug.Break();
            }

            Dictionary<int, BuffSkillData> dict = new Dictionary<int, BuffSkillData>();
            foreach (BuffSkillData buff in bonusBuffs)
                dict.Add(buff.TemplateID, buff);

            return dict;
        }
    }

    [System.Serializable]
    public class WaveData
    {
    }
}

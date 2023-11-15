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
        public List<int> ActionSkillList;
        public List<int> DefaultSkillList;
        public FaceContainerLoader[] FaceContainerLoaders;
    }

    [Serializable]
    public class InitialCreatureDataLoader : ILoader<int, InitialCreatureData>
    {
        public List<InitialCreatureData> initialCreatures = new List<InitialCreatureData>();
        public Dictionary<int, InitialCreatureData> MakeDict()
        {
            if (initialCreatures.Count == 0)
                Utils.LogCritical(nameof(InitialCreatureDataLoader), nameof(MakeDict), "Failed to load InitialCreatureData.json.json");

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
                Utils.LogCritical(nameof(CreatureStatDataLoader), nameof(MakeDict), "Failed to load CreatureStatData.json");

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
        public float[] ContinuousFlipXs;
        public float[] ContinuousFlipYs;
        //public Vector3[] AdditionalLocalPositions;
        public Vector3[] ScaleInterpolations;
        public bool[] IsOnlyVisibles;
        public bool IsColliderHalfRatio;
        public int MaxBounceCount;
        public int MaxPenetrationCount;
        public Define.TemplateIDs.VFX.ImpactHit VFX_ImpactHit;
        public bool IsImpactPointOnTarget;
        //public Define.TemplateIDs.Status.Skill UnlockSkillTemplate;
        public Define.SkillAnimationType AnimationType;
        public Define.TemplateIDs.CrowdControl CrowdControlType;
        public float CrowdControlRatio;
        public float CrowdControlDuration;
        public float CrowdControlIntensity;

        public Define.TemplateIDs.CrowdControl ContinuousCrowdControlType;
        public Define.TemplateIDs.VFX.ImpactHit VFX_ImpactHit_ForContinuousCrowdControl;
        public float ContinuousCrowdControlRatio;
        public float ContinuousCrowdControlDuration;
        public float ContinuousCrowdControlIntensity;

        public float CoolTime;
    }

    [Serializable]
    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> skills = new List<SkillData>();

        public Dictionary<int, SkillData> MakeDict()
        {
            if (skills.Count == 0)
                Utils.LogCritical(nameof(SkillDataLoader), nameof(MakeDict), "Failed to load SkillData.json");

            Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
            foreach (SkillData skill in skills)
                dict.Add(skill.TemplateID, skill);

            return dict;
        }
    }

    // =============================================================================================
    // =============================================================================================
    // [Serializable]
    // public class CrowdControlData
    // {
    //     public int TemplateID;
    //     public string Name;
    //     public string Description;

    // }

    // [Serializable]
    // public class CrowdControlDataLoader : ILoader<int, CrowdControlData>
    // {
    //     public List<CrowdControlData> crowdControls = new List<CrowdControlData>();

    //     public Dictionary<int, CrowdControlData> MakeDict()
    //     {
    //         if (crowdControls.Count == 0)
    //             Utils.LogCritical(nameof(CrowdControlDataLoader), nameof(MakeDict), "Failed to load CrowdControlData.json");

    //         Dictionary<int, CrowdControlData> dict = new Dictionary<int, CrowdControlData>();
    //         foreach (CrowdControlData crowdControl in crowdControls)
    //             dict.Add(crowdControl.TemplateID, crowdControl);

    //         return dict;
    //     }
    // }

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

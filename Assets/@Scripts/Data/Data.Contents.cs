using System;
using System.Collections;
using System.Collections.Generic;
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

        public float InitialMaxHP;
        public Define.InitialStatDescGrade InitialStatDescGrade_Power;
        public Define.InitialStatDescGrade InitialStatDescGrade_AttackSpeed;
        public Define.InitialStatDescGrade InitialStatDescGrade_Armor;
        public Define.InitialStatDescGrade InitialStatDescGrade_MovementSpeed;

        public Sprite InitialSkillDesc_MasteryUniqueSkillIcon;
        public string InitialSkillDesc_MasteryUniqueSkillDescription;
        public Sprite InitialSkillDesc_EliteUniqueSkillIcon;
        public string InitialSkillDesc_EliteUniqueSkillDescription;
        public Sprite InitialSkillDesc_UltimateUniqueSkillIcon;
        public string InitialSkillDesc_UltimateUniqueSkillDescription;

        public List<int> UniqueSkills;
        public List<int> PublicSkills;
        public FaceContainerLoader[] FaceContainerLoaders;
    }

    [Serializable]
    public class CreatureDataLoader : ILoader<int, CreatureData>
    {
        public List<CreatureData> Creatures = new List<CreatureData>();
        public Dictionary<int, CreatureData> MakeDict()
        {
            if (Creatures.Count == 0)
                Utils.LogCritical(nameof(CreatureDataLoader), nameof(MakeDict));
                
            Dictionary<int, CreatureData> dict = new Dictionary<int, CreatureData>();
            foreach (var creature in Creatures)
                dict.Add(creature.TemplateID, creature);

            return dict;
        }
    }

    [Serializable]
    public class SkillData
    {
        public int TemplateID;
        public Define.FixedValue.TemplateID.Skill FirstTemplateID;
        public Define.SkillType SkillType;
        public string Name;
        public string Description;
        public string PrimaryLabel;
        public string ModelingLabel;
        public bool HasEventHandler;
        public bool IsProjectile;
        public bool IsOnFireSocket;
        public bool UsePresetLocalScale;
        public bool UsePresetParticleInfo;
        public float AddtionalSpawnHeightRatio;
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
        public Vector3[] ScaleInterpolations;
        public bool[] IsOnlyVisibles;
        public bool IsColliderHalfRatio;
        public int MaxBounceCount;
        public int MaxPenetrationCount;
        public Define.FixedValue.TemplateID.VFX.ImpactHit VFX_ImpactHit_TemplateID;
        public bool IsImpactPointOnTarget;
        //public Define.SkillAnimationType AnimationType;
        public Define.FixedValue.TemplateID.SkillAnimation SkillAnimationTemplateID;
        public Define.FixedValue.TemplateID.CrowdControl CrowdControlTemplateID;
        public float CrowdControlChance;
        public float CrowdControlDuration;
        public float CrowdControlIntensity;
        public Define.FixedValue.TemplateID.CrowdControl ContinuousCrowdControlTemplateID;
        public Define.FixedValue.TemplateID.VFX.ImpactHit ContinuousCrowdControl_VFX_ImpactHit_TemplateID; // 사실 필요한지 모르겠...
        public float ContinuousCrowdControlChance;
        public float ContinuousCrowdControlDuration;
        public float ContinuousCrowdControlIntensity;
        public float Cooldown;
    }

    [Serializable]
    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> skills = new List<SkillData>();

        public Dictionary<int, SkillData> MakeDict()
        {
            if (skills.Count == 0)
                Utils.LogCritical(nameof(SkillDataLoader), nameof(MakeDict));

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

// -------------------------------------------------------------------------------------
// -------------------------------------------------------------------------------------
// [Serializable]
// public class CreatureStatData
// {
//     public int TemplateID;
//     public string Name;
//     public string Description;
//     Define.InGameGrade InGameGrade;
//     public float MaxHpUp;
//     public float DamageUp;
//     public float CriticalUp;
//     public float AttackSpeedUp;
//     public float CoolDownUp;
//     public float ArmorUp;
//     public float DodgeUp;
//     public float MovementSpeedUp;
//     public float CollectRangeUp;
//     public float LuckUp;
// }

// [Serializable]
// public class CreatureStatDataLoader : ILoader<int, CreatureStatData>
// {
//     public List<CreatureStatData> CreatureStats = new List<CreatureStatData>();

//     public Dictionary<int, CreatureStatData> MakeDict()
//     {
//         if (CreatureStats.Count == 0)
//             Utils.LogCritical(nameof(CreatureStatDataLoader), nameof(MakeDict));

//         Dictionary<int, CreatureStatData> dict = new Dictionary<int, CreatureStatData>();
//         foreach (CreatureStatData creatureStat in CreatureStats)
//             dict.Add(creatureStat.TemplateID, creatureStat);

//         return dict;
//     }
// }
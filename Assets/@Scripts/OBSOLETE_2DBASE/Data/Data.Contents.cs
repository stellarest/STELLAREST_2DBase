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
        public Sprite Icon;
        public string Description;
        public string[] PrimaryLabels;

        public string StatGradeDesc_Power;
        public string StatGradeDesc_AttackSpeed;
        public string StatGradeDesc_Armor;
        public string StatGradeDesc_MovementSpeed;

        public float InitialMaxHP;
        public float InitialArmor;
        public float InitialMovementSpeed;

        public Sprite Icon_MasterySkill;
        public string Desc_MasterySkill;
        public Sprite Icon_UniqueEliteSkill;
        public string Desc_UniqueEliteSkill;
        public Sprite Icon_UniqueUltimateSkill;
        public string Desc_UniqueUltimateSkill;

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
        public Define.FixedValue.TemplateID.Skill TemplateOrigin;

        public Define.InGameGrade Grade;
        public Define.InGameGrade MaxGrade;
        public int GradeCount;

        public string Name;
        public Sprite Icon;
        public string Description;
        public string PrimaryLabel;

        public Define.SkillType Type;
        public Define.SkillAnimationType OwnerAnimationType;
        public Define.VFXImpactHitType VFXImpactHitType;

        public int Count;
        public float Spacing;
        public float Duration;
        
        public float MinDamage;
        public float MaxDamage;
        public float MovementSpeed;
        public float RotationSpeed;
        public int MaxBounceCount;
        public int MaxPenetrationCount;
        public SkillCustomValue CustomValue;

        public float[] ContinuousAngles;
        public float[] ContinuousFlipXs;
        public float[] ContinuousFlipYs;
        public float[] AddContinuousMovementSpeedRatios;
        public float[] AddContinuousRotationSpeedRatios;
        public Vector3[] TargetScaleInterpolations;

        public bool IsProjectile;
        public bool IsOnFromEventHandler;
        public bool IsOnFromFireSocket;
        public bool IsVFXImpactPointOnTarget;
        public bool UsePresetLocalScale;
        public bool UsePresetParticleInfo;

        public int CrowdControlCount;
        public Define.CrowdControlType[] CrowdControlTypes;
        public float[] CrowdControlChances;
        public float[] CrowdControlDurations;
        public float[] CrowdControlIntensities;

        public float LevelUpCost;            
        public float Cooldown;
    }

    [Serializable]
    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> Skills = new List<SkillData>();

        public Dictionary<int, SkillData> MakeDict()
        {
            if (Skills.Count == 0)
                Utils.LogCritical(nameof(SkillDataLoader), nameof(MakeDict));

            Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
            foreach (SkillData skill in Skills)
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
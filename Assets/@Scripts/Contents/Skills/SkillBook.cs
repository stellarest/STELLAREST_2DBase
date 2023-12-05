using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static STELLAREST_2D.Define;
using STELLAREST_2D.Data;

// default skill, action skill
namespace STELLAREST_2D
{
    [System.Serializable]
    public class SkillMember
    {
        public SkillMember(SkillBase skillOrigin)
        {
            // INIT ORIGIN -> DEACTIVE
            skillOrigin.Deactivate();
            this.SkillOrigin = skillOrigin;
            this.Name = skillOrigin.Data.Name;
            this.Grade = skillOrigin.Data.Grade.ToString();
            this.IsLearned = skillOrigin.IsLearned;
            this.IsLast = skillOrigin.IsLast;
            this.IsActive = skillOrigin.gameObject.activeSelf;
        }

        public SkillBase Unlock()
        {
            this.SkillOrigin.IsLearned = true;
            this.IsLearned = true;

            this.SkillOrigin.IsLast = true;
            this.IsLast = true;

            return SkillOrigin;
        }

        public void SetActiveLog(bool isOn) => IsActive = isOn;

        public SkillBase DeactiveForce()
        {
            this.SkillOrigin.IsLast = false;
            this.IsActive = false;
            this.IsLast = false;

            return this.SkillOrigin;
        }

        [field: SerializeField] public SkillBase SkillOrigin { get; private set; } = null;
        [field: SerializeField, ShowOnly] public string Name { get; private set; } = string.Empty;
        [field: SerializeField, ShowOnly] public string Grade { get; private set; } = string.Empty;
        [field: SerializeField, ShowOnly] public bool IsActive { get; private set; } = false;
        [field: SerializeField, ShowOnly] public bool IsLearned { get; private set; } = false;
        [field: SerializeField, ShowOnly] public bool IsLast { get; private set; } = false;
    }

    [System.Serializable]
    public class SkillGroup : IGroupDictionary
    {
        public SkillGroup(SkillBase skillOrigin)
        {
            this._skillOrigin = skillOrigin;
        }

        public void InitLeader(object groupLeader)
        {
            SkillGroup leader = groupLeader as SkillGroup;
            if (leader != null)
            {
                Tag = $"Group : {leader._skillOrigin.Data.Name}";
                SkillType = leader._skillOrigin.Data.SkillType;
                Members = new List<SkillMember>();
                Members.Add(new SkillMember(leader._skillOrigin));
                ++MemberCount;
            }
        }

        public void AddMember(object groupLeader, object groupMember)
        {
            SkillGroup leader = groupLeader as SkillGroup;
            SkillGroup member = groupMember as SkillGroup;
            if (leader != null && member != null)
            {
                leader.Members.Add(new SkillMember(member._skillOrigin));
                ++MemberCount;
            }
        }

        public SkillBase Unlock() // JUST UNLOCK
        {
            for (int i = 0; i < MemberCount; ++i)
            {
                if (Members[i].IsLearned == false)
                {
                    if (i == 0) // FIRST SKILL
                    {
                        SkillBase unlockSkill = Members[0].Unlock();
                        if (unlockSkill.Data.HasEventHandler && unlockSkill.Data.SkillType == Define.SkillType.Unique)
                        {
                            if (unlockSkill.Data.FirstTemplateID == Book.MasteryAttackTemplate)
                            {
                                unlockSkill.Owner.AnimCallback.OnActiveMasteryAction -= unlockSkill.GetComponent<UniqueSkill>().OnActiveMasteryActionHandler;
                                unlockSkill.Owner.AnimCallback.OnActiveMasteryAction += unlockSkill.GetComponent<UniqueSkill>().OnActiveMasteryActionHandler;
                                Utils.Log($"Add Event : AnimCallback.OnActiveMasteryAction += {unlockSkill.Data.Name}");
                            }
                            else if (unlockSkill.Data.FirstTemplateID == Book.EliteActionTemplate)
                            {
                                unlockSkill.Owner.AnimCallback.OnActiveEliteAction += unlockSkill.GetComponent<UniqueSkill>().OnActiveEliteActionHandler;
                                Utils.Log($"Add Event : AnimCallback.OnActiveEliteAction += {unlockSkill.Data.Name}");
                            }
                            else if (unlockSkill.Data.FirstTemplateID == FixedValue.TemplateID.Skill.Unlock_AssassinMastery_Elite_C1)
                            {
                                unlockSkill.Owner.AnimCallback.OnActiveMasteryAction -= unlockSkill.GetComponent<UniqueSkill>().OnActiveMasteryActionHandler;
                                unlockSkill.Owner.AnimCallback.OnActiveMasteryAction += unlockSkill.GetComponent<UniqueSkill>().OnActiveMasteryActionHandler;
                                Utils.Log($"Add Event : AnimCallback.OnActiveMasteryAction += {unlockSkill.Data.Name}");
                            }
                            else if (unlockSkill.Data.FirstTemplateID == FixedValue.TemplateID.Skill.Unlock_NinjaMastery_Elite_C1)
                            {
                                unlockSkill.Owner.AnimCallback.OnActiveMasteryAction -= unlockSkill.GetComponent<UniqueSkill>().OnActiveMasteryActionHandler;
                                unlockSkill.Owner.AnimCallback.OnActiveMasteryAction += unlockSkill.GetComponent<UniqueSkill>().OnActiveMasteryActionHandler;
                                Utils.Log($"Add Event : AnimCallback.OnActiveMasteryAction += {unlockSkill.Data.Name}");
                            }
                        }

                        return unlockSkill;
                    }
                    else // REST OF SKILLS
                    {
                        // RELEASE EVENT
                        SkillBase deactiveSkill = Members[i - 1].DeactiveForce();
                        if (deactiveSkill.Data.HasEventHandler && deactiveSkill.Data.SkillType == Define.SkillType.Unique)
                        {
                            if (deactiveSkill.Data.FirstTemplateID == Book.MasteryAttackTemplate)
                            {
                                deactiveSkill.Owner.AnimCallback.OnActiveMasteryAction -= deactiveSkill.GetComponent<UniqueSkill>().OnActiveMasteryActionHandler;
                                Utils.Log($"Release Event : AnimCallback.OnActiveMasteryAction -= {deactiveSkill.Data.Name}");
                            }
                            else if (deactiveSkill.Data.FirstTemplateID == Book.EliteActionTemplate)
                            {
                                deactiveSkill.Owner.AnimCallback.OnActiveEliteAction -= deactiveSkill.GetComponent<UniqueSkill>().OnActiveEliteActionHandler;
                                Utils.Log($"Release Event : AnimCallback.OnActiveEliteAction -= {deactiveSkill.Data.Name}");
                            }
                        }

                        // ADD EVENT
                        SkillBase unlockSkill = Members[i].Unlock();
                        if (unlockSkill.Data.HasEventHandler && unlockSkill.Data.SkillType == Define.SkillType.Unique)
                        {
                            if (unlockSkill.Data.FirstTemplateID == Book.MasteryAttackTemplate)
                            {
                                unlockSkill.Owner.AnimCallback.OnActiveMasteryAction += unlockSkill.GetComponent<UniqueSkill>().OnActiveMasteryActionHandler;
                                Utils.Log($"Add Event : AnimCallback.OnActiveMasteryAction += {unlockSkill.Data.Name}");
                            }
                            else if (unlockSkill.Data.FirstTemplateID == Book.EliteActionTemplate)
                            {
                                unlockSkill.Owner.AnimCallback.OnActiveEliteAction += unlockSkill.GetComponent<UniqueSkill>().OnActiveEliteActionHandler;
                                Utils.Log($"Add Event : AnimCallback.OnActiveEliteAction += {unlockSkill.Data.Name}");
                            }
                        }

                        return unlockSkill;
                    }
                }
                else if (i == MemberCount - 1)
                {
                    // MAX LEVEL
                    Utils.Log(nameof(SkillGroup), nameof(Unlock), Members[i].Name, "is <color=yellow>MAX LEVEL</color> in this group.");
                    return Members[i].SkillOrigin;
                }
            }

            return null;
        }

        public void Activate() // JUST ACTIVATE
        {
            for (int i = 0; i < MemberCount; ++i)
            {
                if (Members[i].IsActive)
                {
                    Utils.Log("IS ALREADY ACTIVE T_T");
                    continue;
                }

                if (Members[i].IsLearned == false)
                    continue;

                if (Members[i].IsLast == false)
                    continue;

                if (Members[i].IsActive == false && Members[i].IsLearned && Members[i].IsLast)
                {
                    Members[i].SetActiveLog(true);
                    Members[i].SkillOrigin.Activate();
                }
            }
        }

        public void Deactivate() // JUST DEACTIVATE
        {
            for (int i = 0; i < MemberCount; ++i)
            {
                if (Members[i].IsActive == false)
                    continue;

                if (Members[i].IsLearned == false)
                    continue;

                if (Members[i].IsLast == false)
                    continue;

                if (Members[i].IsActive && Members[i].IsLearned && Members[i].IsLast)
                {
                    Members[i].SetActiveLog(false);
                    Members[i].SkillOrigin.Deactivate();
                }
            }
        }

        public SkillBase GetLastLearnedSkillMember()
        {
            for (int i = 0; i < MemberCount; ++i)
            {
                if (Members[i].IsLearned == false)
                    continue;

                if (Members[i].IsLast == false)
                    continue;

                if (Members[i].IsLearned && Members[i].IsLast)
                    return Members[i].SkillOrigin;
            }

            return null;
        }

        private SkillBase _skillOrigin = null;
        [field: SerializeField, ShowOnly] public string Tag { get; private set; } = string.Empty;
        [field: SerializeField] public Define.SkillType SkillType { get; private set; } = Define.SkillType.None;
        [field: SerializeField, ShowOnly] public int MemberCount { get; private set; } = 0;
        [field: SerializeField] public List<SkillMember> Members { get; private set; } = null;
        public SkillBook Book { get; set; } = null;
    }

    [System.Serializable]
    public class SkillBook : MonoBehaviour
    {
        public CreatureController Owner { get; set; }

        public FixedValue.TemplateID.Skill MasteryAttackTemplate { get; private set; } = FixedValue.TemplateID.Skill.None;
        public FixedValue.TemplateID.Skill EliteActionTemplate { get; private set; } = FixedValue.TemplateID.Skill.None;
        public FixedValue.TemplateID.Skill UltimateActionTemplate { get; private set; } = FixedValue.TemplateID.Skill.None;

        // +++ Key(int) : Skill Origin Template, Value : SkillGroup +++
        [System.Serializable] public class SkillGroupDictionary : SerializableGroupDictionary<int, SkillGroup> { }
        [field: SerializeField] public SkillGroupDictionary SkillGroupsDict { get; private set; } = new SkillGroupDictionary();

        public int SequenceIdx { get; set; } = 0;
        [field: SerializeField] public List<UniqueSkill> ActionSkills { get; private set; } = new List<UniqueSkill>();

        public Shield CachedShield { get; private set; } = null;
        public SecondWind CachedSecondWind { get; private set; } = null;
        public ForestBarrier CachedForestBarrier { get; private set; } = null;

        public void LateInit()
        {
            //this.SetFirstSkill();
            foreach (var group in SkillGroupsDict)
            {
                group.Value.Book = this;
                for (int i = 0; i < group.Value.MemberCount; ++i)
                {
                    SkillBase skill = group.Value.Members[i].SkillOrigin;
                    if (skill.Data.SkillType == Define.SkillType.Unique)
                    {
                        switch (skill.Data.FirstTemplateID)
                        {
                            case FixedValue.TemplateID.Skill.PaladinMastery:
                            case FixedValue.TemplateID.Skill.KnightMastery:
                            case FixedValue.TemplateID.Skill.PhantomKnightMastery:

                            case FixedValue.TemplateID.Skill.ArrowMasterMastery:
                            case FixedValue.TemplateID.Skill.ElementalArcherMastery:
                            case FixedValue.TemplateID.Skill.ForestGuardianMastery:

                            case FixedValue.TemplateID.Skill.AssassinMastery:
                            case FixedValue.TemplateID.Skill.NinjaMastery:
                                this.MasteryAttackTemplate = skill.Data.FirstTemplateID;
                                break;

                            case FixedValue.TemplateID.Skill.Unlock_PaladinMastery_Elite:
                            case FixedValue.TemplateID.Skill.Unlock_KnightMastery_Elite:
                            case FixedValue.TemplateID.Skill.Unlock_PhantomKnightMastery_Elite_C1:

                            case FixedValue.TemplateID.Skill.Unlock_ArrowMasterMastery_Elite:
                            case FixedValue.TemplateID.Skill.Unlock_ElementalArcherMastery_Elite:
                            case FixedValue.TemplateID.Skill.Unlock_ForestGuardian_Elite:

                            case FixedValue.TemplateID.Skill.Unlock_AssassinMastery_Elite:
                            case FixedValue.TemplateID.Skill.Unlock_NinjaMastery_Elite:
                                {
                                    if (skill.Data.FirstTemplateID == FixedValue.TemplateID.Skill.Unlock_PaladinMastery_Elite)
                                        this.CachedShield = skill.GetComponent<Shield>();
                                    else if (skill.Data.FirstTemplateID == FixedValue.TemplateID.Skill.Unlock_KnightMastery_Elite)
                                        this.CachedSecondWind = skill.GetComponent<SecondWind>();
                                    else if (skill.Data.FirstTemplateID == FixedValue.TemplateID.Skill.Unlock_ForestGuardian_Elite)
                                        this.CachedForestBarrier = skill.GetComponent<ForestBarrier>();

                                    this.EliteActionTemplate = skill.Data.FirstTemplateID;
                                }
                                break;


                            case FixedValue.TemplateID.Skill.Unlock_PaladinMastery_Ultimate:
                            case FixedValue.TemplateID.Skill.Unlock_KnightMastery_Ultimate:
                            case FixedValue.TemplateID.Skill.Unlock_PhantomKnightMastery_Ultimate:

                            case FixedValue.TemplateID.Skill.Unlock_ArrowMasterMastery_Ultimate:
                            case FixedValue.TemplateID.Skill.Unlock_ElementalArcherMasstery_Ultimate:
                            case FixedValue.TemplateID.Skill.Unlock_ForestGuardian_Ultimate:

                            case FixedValue.TemplateID.Skill.Unlock_AssassinMastery_Ultimate:
                            case FixedValue.TemplateID.Skill.Unlock_NinjaMastery_Ultimate:
                                this.UltimateActionTemplate = skill.Data.FirstTemplateID;
                                break;
                        }

                        ActionSkills.Add(skill as UniqueSkill);
                    }
                }
            }

            this.Owner.Stat.OnAddSkillCooldownRatio += OnAddSkillCooldownRatioHandler;
            Utils.LogAddEvent(nameof(this.Owner.Stat.OnAddSkillCooldownRatio), nameof(OnAddSkillCooldownRatioHandler));

            this.Owner.Stat.OnResetSkillCooldown += OnResetSkillCooldownHandler;
            Utils.LogAddEvent(nameof(this.Owner.Stat.OnResetSkillCooldown), nameof(OnResetSkillCooldownHandler));
        }

        public void OnAddSkillCooldownRatioHandler(FixedValue.TemplateID.Skill skillTemplateID, float addRatio)
        {
            SkillBase currentSkill = GetLastLearnedSkillMember(skillTemplateID);
            if (currentSkill == null)
                return;
            
            float cooldown = currentSkill.Data.Cooldown;
            currentSkill.Data.Cooldown = cooldown + (cooldown * addRatio);

            Deactivate(skillTemplateID);
            Activate(skillTemplateID);

            Utils.Log($"Apply Cooldown : {currentSkill.Data.Name}");
        }

        public void OnResetSkillCooldownHandler(FixedValue.TemplateID.Skill skillTemplateID)
        {
            SkillBase currentSkill = GetLastLearnedSkillMember(skillTemplateID);
            if (currentSkill == null)
                return;

            currentSkill.Data.Cooldown = currentSkill.InitialCooldown;

            Deactivate(skillTemplateID);
            Activate(skillTemplateID);

            Utils.Log($"Reset Cooldown : {currentSkill.Data.Name}");
        }

        public void ReserveNextSequence(FixedValue.TemplateID.Skill currentEnd)
        {
        }

        public Define.InGameGrade GetCurrentSkillGrade(FixedValue.TemplateID.Skill skillType)
        {
            if (SkillGroupsDict.TryGetValue((int)skillType, out SkillGroup group) == false)
                Utils.LogCritical(nameof(SkillBook), nameof(GetCurrentSkillGrade), $"Input : {skillType}");

            return group.GetLastLearnedSkillMember().Data.Grade;
        }

        public void LevelUp(FixedValue.TemplateID.Skill skillTemplateID)
        {
            SkillBase newSkill = Unlock(skillTemplateID);
            /*
                DO SOMETHING BELOW
            */
        }

        private SkillBase Unlock(FixedValue.TemplateID.Skill skillTemplateID)
        {
            if (SkillGroupsDict.TryGetValue((int)skillTemplateID, out SkillGroup group) == false)
                Utils.LogCritical(nameof(SkillBook), nameof(Unlock), $"Input : {skillTemplateID}");

            return group.Unlock();
        }

        public void Activate(FixedValue.TemplateID.Skill skillTemplateID)
        {
            if (SkillGroupsDict.TryGetValue((int)skillTemplateID, out SkillGroup group) == false)
                Utils.LogCritical(nameof(SkillBook), nameof(Activate), $"Input : {skillTemplateID}");

            group.Activate();
        }

        public void ActivateAll()
        {
            foreach (var group in SkillGroupsDict)
                group.Value.Activate();
        }

        public void Deactivate(FixedValue.TemplateID.Skill skillTemplateID)
        {
            if (SkillGroupsDict.TryGetValue((int)skillTemplateID, out SkillGroup group) == false)
                Utils.LogCritical(nameof(SkillBook), nameof(Deactivate), $"Input : {skillTemplateID}");

            group.Deactivate();
        }

        public void DeactivateAll()
        {
            foreach (var group in SkillGroupsDict)
                group.Value.Deactivate();
        }

        public SkillBase GetLastLearnedSkillMember(FixedValue.TemplateID.Skill skillTemplateID)
        {
            if (SkillGroupsDict.TryGetValue((int)skillTemplateID, out SkillGroup group) == false)
                Utils.LogCritical(nameof(SkillBook), nameof(GetLastLearnedSkillMember), $"Input : {skillTemplateID}");

            SkillBase skill = group.GetLastLearnedSkillMember();
            if (skill == null)
                Utils.LogStrong(nameof(SkillGroup), nameof(GetLastLearnedSkillMember), "Please Unlock First Skill in advance.");

            return skill;
        }

        public SkillBase ForceGetSkillMember(FixedValue.TemplateID.Skill skillTemplateID, int idxOfGroupMember)
        {
            if (SkillGroupsDict.TryGetValue((int)skillTemplateID, out SkillGroup group) == false)
                Utils.LogCritical(nameof(SkillBook), nameof(ForceGetSkillMember), $"Input : {skillTemplateID}");

            for (int i = 0; i < group.MemberCount; ++i)
            {
                if (idxOfGroupMember == i)
                    return group.Members[i].SkillOrigin;
            }

            return null;
        }

        // callback 전용 메서드 정의
        public void RandomizeSequenceGroup(FixedValue.TemplateID.Skill prevTemplateOrigin)
        {
            this.Deactivate(prevTemplateOrigin); // End Prev Sequence Skill Template
            //Utils.Log("Randomize Sequence Something if you want to.");
        }

        public bool IsOnShield => (this.CachedShield != null) ? this.CachedShield.IsOnShield : false;
        public void HitShield() => this.CachedShield.Hit();
        public void OffSheild() => this.CachedShield.IsOnShield = false;
        
        public bool IsOnBarrier => (this.CachedForestBarrier != null) ? this.CachedForestBarrier.IsOnBarrier : false;
        public void HitBarrier() => this.CachedForestBarrier.BarrierCount -= 1;

        public bool IsReadySecondWind => (this.CachedSecondWind != null) ? this.CachedSecondWind.IsReady : false;
        public void OnSecondWind() => this.CachedSecondWind.On();

        public T CachedSkill<T>() where T : SkillBase
        {
            System.Type type = typeof(T);
            if (type == typeof(Shield))
            {
                if (CachedShield != null)
                    return CachedShield as T;
            }
            else if (type == typeof(SecondWind))
            {
                if (CachedSecondWind != null)
                    return CachedSecondWind as T;
            }

            return null;
        }

        private void OnDestroy()
        {
            foreach (KeyValuePair<int, SkillGroup> pair in SkillGroupsDict)
            {
                for (int i = 0; i < pair.Value.MemberCount; ++i)
                {
                    SkillBase skillOrigin = pair.Value.Members[i].SkillOrigin;
                    if (skillOrigin.Data.HasEventHandler)
                    {
                        if (skillOrigin.Data.SkillType == Define.SkillType.Unique)
                        {
                            skillOrigin.Owner.AnimCallback.OnActiveMasteryAction -= skillOrigin.GetComponent<UniqueSkill>().OnActiveMasteryActionHandler;
                            Utils.Log($"Release Event : AnimCallback.OnActiveMasteryAction -= {skillOrigin.Data.Name}");

                            skillOrigin.Owner.AnimCallback.OnActiveEliteAction -= skillOrigin.GetComponent<UniqueSkill>().OnActiveEliteActionHandler;
                            Utils.Log($"Release Event : AnimCallback.OnActiveEliteAction -= {skillOrigin.Data.Name}");
                        }
                    }
                }
            }

            if (this.Owner.Stat.OnAddSkillCooldownRatio != null)
            {
                this.Owner.Stat.OnAddSkillCooldownRatio -= OnAddSkillCooldownRatioHandler;
                Utils.LogReleaseEvent(nameof(this.Owner.Stat.OnAddSkillCooldownRatio), nameof(OnAddSkillCooldownRatioHandler));
            }

            if (this.Owner.Stat.OnResetSkillCooldown != null)
            {

                this.Owner.Stat.OnResetSkillCooldown -= OnResetSkillCooldownHandler;
                Utils.LogReleaseEvent(nameof(this.Owner.Stat.OnResetSkillCooldown), nameof(OnResetSkillCooldownHandler));
            }
        }
    }
}

// -------------------------------------------------------------------------------------
// -------------------------------------------------------------------------------------
// public void ActivateSequence(SkillTemplate templateOrigin)
// {
//     if (SkillGroupsDict.TryGetValue((int)templateOrigin, out SkillGroup group) == false)
//         Utils.LogCritical(nameof(SkillBook), nameof(Acquire), $"Check TemplateID : {templateOrigin}");

//     for (int i = 0; i < SequenceSkills.Count; ++i)
//     {
//         if (SequenceSkills[i].Data.OriginalTemplate == templateOrigin)
//             SequenceSkills[i].Activate();
//     }
// }

// UNLOCK IN SKILL GROUP
// public SkillBase Unlock() // JUST UNLOCK
//         {
//             for (int i = 0; i < MemberCount; ++i)
//             {
//                 if (Members[i].IsLearned == false)
//                 {
//                     if (i == 0)
//                     {
//                         Utils.Log("!!!!!");
//                         SkillBase unlockSkill = Members[0].Unlock();
//                         if (unlockSkill.Data.HasEventHandler) // HasEventHandler : Exclusive Skill
//                         {
//                             if (unlockSkill.Data.SkillType == Define.SkillType.Repeat)
//                             {
//                                 unlockSkill.Owner.AnimCallback.OnActiveRepeatSkill += unlockSkill.GetComponent<RepeatSkill>().OnActiveRepeatSkillHandler;
//                                 Utils.Log($"Add Event(OnActiveRepeatSkill) : {unlockSkill.Data.Name}");
//                             }
//                             else if (unlockSkill.Data.SkillType == Define.SkillType.Sequence)
//                             {
//                                 unlockSkill.Owner.AnimCallback.OnActiveSequenceSkill += unlockSkill.GetComponent<SequenceSkill>().OnActiveSequenceSkillHandler;
//                                 Utils.Log($"Release Event(OnActiveSequenceSkill) : {unlockSkill.Data.Name}");
//                             }
//                         }

//                         return unlockSkill;
//                     }
//                     else // REST OF SKILLS
//                     {
//                         // RELEASE EVENT
//                         SkillBase deactiveSkill = Members[i - 1].DeactiveForce();
//                         if (deactiveSkill.Data.HasEventHandler)
//                         {
//                             if (deactiveSkill.Data.SkillType == Define.SkillType.Repeat)
//                             {
//                                 deactiveSkill.Owner.AnimCallback.OnActiveRepeatSkill -= deactiveSkill.GetComponent<RepeatSkill>().OnActiveRepeatSkillHandler;
//                                 Utils.Log($"Release Event(OnActiveRepeatSkill) : {deactiveSkill.Data.Name}");
//                             }
//                             else if (deactiveSkill.Data.SkillType == Define.SkillType.Sequence)
//                             {
//                                 deactiveSkill.Owner.AnimCallback.OnActiveSequenceSkill -= deactiveSkill.GetComponent<SequenceSkill>().OnActiveSequenceSkillHandler;
//                                 Utils.Log($"Release Event(OnActiveSequenceSkill) : {deactiveSkill.Data.Name}");
//                             }
//                         }

//                         SkillBase unlockSkill = Members[i].Unlock();
//                         if (unlockSkill.Data.HasEventHandler)
//                         {
//                             if (unlockSkill.Data.SkillType == Define.SkillType.Repeat)
//                             {
//                                 unlockSkill.Owner.AnimCallback.OnActiveRepeatSkill += unlockSkill.GetComponent<RepeatSkill>().OnActiveRepeatSkillHandler;
//                                 Utils.Log($"Add Event(OnActiveRepeatSkill) : {unlockSkill.Data.Name}");
//                             }
//                             else if (unlockSkill.Data.SkillType == Define.SkillType.Sequence)
//                             {
//                                 unlockSkill.Owner.AnimCallback.OnActiveSequenceSkill += unlockSkill.GetComponent<SequenceSkill>().OnActiveSequenceSkillHandler;
//                                 Utils.Log($"Add Event(OnActiveSequenceSkill) : {unlockSkill.Data.Name}");
//                             }
//                         }

//                         return unlockSkill;
//                     }
//                 }
//                 else if (i == MemberCount - 1)
//                 {
//                     // MAX LEVEL
//                     Utils.Log(nameof(SkillGroup), nameof(Unlock), Members[i].Name, "is <color=yellow>MAX LEVEL</color> in this group.");
//                     return Members[i].SkillOrigin;
//                 }
//             }

//             return null;
//         }

// private T GetCachedSkill<T>(SkillTemplate templateOrigin) where T : SkillBase
// {
//     switch (templateOrigin)
//     {
//         case SkillTemplate.Shield:
//             return CachedShield as T;
//     }

//     return null;
// }

// public void ActivateAll()
// {
//     foreach (KeyValuePair<int, SkillGroup> pair in SkillGroupsDict)
//     {
//         for (int i = 0; i < pair.Value.MemberCount; ++i)
//         {
//             SkillBase skillOrigin = pair.Value.Members[i].SkillOrigin;
//             if (skillOrigin.IsLearned && skillOrigin.IsLast)
//                 skillOrigin.Activate();
//         }
//     }
// }

// public void DeactivateAll()
// {
//     foreach (KeyValuePair<int, SkillGroup> pair in SkillGroupsDict)
//     {
//         for (int i = 0; i < pair.Value.MemberCount; ++i)
//         {
//             SkillBase skillOrigin = pair.Value.Members[i].SkillOrigin;
//             if (skillOrigin.IsLearned && skillOrigin.IsLast)
//                 skillOrigin.Deactivate();
//         }
//     }
// }

// private void InitActionSkills() // TEMP
// {
//     for (int i = 0; i < ActionSkills.Count; ++i)
//     {
//         if (ActionSkills[i].Data.OriginalTemplate == SkillTemplate.Shield_Elite_Solo)
//         {
//             //SecondSequenceSkill = SequenceSkills[i].Data.OriginalTemplate;
//             if (this.CachedShield == null)
//                 this.CachedShield = ActionSkills[i].GetComponent<Shield>();
//         }

//         if (ActionSkills[i].Data.OriginalTemplate == SkillTemplate.SecondWind_Elite)
//         {
//             //SecondSequenceSkill = SequenceSkills[i].Data.OriginalTemplate;
//             if (this.CachedSecondWind == null)
//                 this.CachedSecondWind = ActionSkills[i].GetComponent<SecondWind>();
//         }

//         // if (SequenceSkills[i].Data.OriginalTemplate == SkillTemplate.PhantomSoul_Elite)
//         //       SecondSequenceSkill = SequenceSkills[i].Data.OriginalTemplate;
//     }
// }

// case SkillTemplate.Shield_Elite_Solo:
//     {
//         if (this.CachedShield == null)
//             this.CachedShield = skill.GetComponent<Shield>();

//         this.EliteActionTemplate = skill.Data.OriginalTemplate;
//     }
//     break;

// case SkillTemplate.SecondWind_Elite_Solo:
//     {
//         if (this.CachedSecondWind == null)
//             this.CachedSecondWind = skill.GetComponent<SecondWind>();

//         this.EliteActionTemplate = skill.Data.OriginalTemplate;
//     }
//     break;

// case SkillTemplate.PhantomSoul_Elite_Solo:
//     this.EliteActionTemplate = skill.Data.OriginalTemplate;
//     break;
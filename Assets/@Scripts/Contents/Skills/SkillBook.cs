using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using STELLAREST_2D.Data;
using Unity.VisualScripting;
using UnityEngine;
using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

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

        // FIX
        public SkillBase Unlock() // JUST UNLOCK
        {
            for (int i = 0; i < MemberCount; ++i)
            {
                if (Members[i].IsLearned == false)
                {
                    if (i == 0) // 있어야하는게 맞음
                    {
                        SkillBase unlockSkill = Members[0].Unlock();
                        if (unlockSkill.Data.HasEventHandler && unlockSkill.Data.SkillType == Define.SkillType.Action)
                        {
                            if (unlockSkill.Data.OriginalTemplate == Book.MasteryActionTemplate)
                            {
                                unlockSkill.Owner.AnimCallback.OnActiveMasteryAction += unlockSkill.GetComponent<ActionSkill>().OnActiveMasteryActionHandler;
                                Utils.Log($"Add Event(OnActiveMasteryAction) : {unlockSkill.Data.Name}");
                            }
                            else if (unlockSkill.Data.OriginalTemplate == Book.EliteActionTemplate)
                            {
                                unlockSkill.Owner.AnimCallback.OnActiveEliteAction += unlockSkill.GetComponent<ActionSkill>().OnActiveEliteActionHandler;
                                Utils.Log($"Add Event(OnActiveEliteAction) : {unlockSkill.Data.Name}");
                            }
                        }

                        return unlockSkill;
                    }
                    else // REST OF SKILLS
                    {
                        // RELEASE EVENT
                        SkillBase deactiveSkill = Members[i - 1].DeactiveForce();
                        if (deactiveSkill.Data.HasEventHandler && deactiveSkill.Data.SkillType == Define.SkillType.Action)
                        {
                            if (deactiveSkill.Data.OriginalTemplate == Book.MasteryActionTemplate)
                            {
                                deactiveSkill.Owner.AnimCallback.OnActiveMasteryAction -= deactiveSkill.GetComponent<ActionSkill>().OnActiveMasteryActionHandler;
                                Utils.Log($"Release Event(OnActiveMasteryAction) : {deactiveSkill.Data.Name}");
                            }
                            else if (deactiveSkill.Data.OriginalTemplate == Book.EliteActionTemplate)
                            {
                                deactiveSkill.Owner.AnimCallback.OnActiveEliteAction -= deactiveSkill.GetComponent<ActionSkill>().OnActiveEliteActionHandler;
                                Utils.Log($"Release Event(OnActiveEliteAction) : {deactiveSkill.Data.Name}");
                            }
                        }

                        // ADD EVENT
                        SkillBase unlockSkill = Members[i].Unlock();
                        if (unlockSkill.Data.HasEventHandler && unlockSkill.Data.SkillType == Define.SkillType.Action)
                        {
                            if (unlockSkill.Data.OriginalTemplate == Book.MasteryActionTemplate)
                            {
                                unlockSkill.Owner.AnimCallback.OnActiveMasteryAction += unlockSkill.GetComponent<ActionSkill>().OnActiveMasteryActionHandler;
                                Utils.Log($"Add Event(OnActiveMasteryAction) : {unlockSkill.Data.Name}");
                            }
                            else if (unlockSkill.Data.OriginalTemplate == Book.EliteActionTemplate)
                            {
                                unlockSkill.Owner.AnimCallback.OnActiveEliteAction += unlockSkill.GetComponent<ActionSkill>().OnActiveEliteActionHandler;
                                Utils.Log($"Add Event(OnActiveEliteAction) : {unlockSkill.Data.Name}");
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

        public SkillBase GetCanActiveSkillMember()
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

        public SkillTemplate MasteryActionTemplate { get; private set; } = SkillTemplate.None;
        public SkillTemplate EliteActionTemplate { get; private set; } = SkillTemplate.None;
        public SkillTemplate UltimateActionTemplate { get; private set; } = SkillTemplate.None;

        // +++ Key(int) : Skill Origin Template, Value : SkillGroup +++
        [System.Serializable] public class SkillGroupDictionary : SerializableGroupDictionary<int, SkillGroup> { }
        [field: SerializeField] public SkillGroupDictionary SkillGroupsDict { get; private set; } = new SkillGroupDictionary();

        public int SequenceIdx { get; set; } = 0;
        [field: SerializeField] public List<ActionSkill> ActionSkills { get; private set; } = new List<ActionSkill>();

        public Shield CachedShield { get; private set; } = null;
        public SecondWind CachedSecondWind { get; private set; } = null;

        public void LateInit()
        {
            //this.SetFirstSkill();
            foreach (var group in SkillGroupsDict)
            {
                group.Value.Book = this;
                for (int i = 0; i < group.Value.MemberCount; ++i)
                {
                    SkillBase skill = group.Value.Members[i].SkillOrigin;
                    if (skill.Data.SkillType == Define.SkillType.Action)
                    {
                        switch (skill.Data.OriginalTemplate)
                        {
                            case SkillTemplate.PaladinMastery:
                            case SkillTemplate.KnightMastery:
                            case SkillTemplate.PhantomKnightMastery:
                                this.MasteryActionTemplate = skill.Data.OriginalTemplate;
                                break;

                            case SkillTemplate.Shield_Elite_Solo:
                                {
                                    if (this.CachedShield == null)
                                        this.CachedShield = skill.GetComponent<Shield>();

                                    this.EliteActionTemplate = skill.Data.OriginalTemplate;
                                }
                                break;

                            case SkillTemplate.SecondWind_Elite_Solo:
                                {
                                    if (this.CachedSecondWind == null)
                                        this.CachedSecondWind = skill.GetComponent<SecondWind>();

                                    this.EliteActionTemplate = skill.Data.OriginalTemplate;
                                }
                                break;
                            case SkillTemplate.VoidSoul_Elite_Solo:
                                this.EliteActionTemplate = skill.Data.OriginalTemplate;
                                break;

                            case SkillTemplate.JudgementOfHeaven_Ultimate_Solo:
                            case SkillTemplate.DanceOfSwords_Ultimate_Solo:
                            case SkillTemplate.RiseOfDarkness_Ultimate_Solo:
                                this.UltimateActionTemplate = skill.Data.OriginalTemplate;
                                break;
                        }

                        ActionSkills.Add(skill as ActionSkill);
                    }
                }
            }
        }

        public void ReserveNextSequence(SkillTemplate currentEnd)
        {
        }

        public Define.InGameGrade GetCurrentSkillGrade(SkillTemplate skillTemplate)
        {
            if (SkillGroupsDict.TryGetValue((int)skillTemplate, out SkillGroup group) == false)
                Utils.LogCritical(nameof(SkillBook), nameof(GetCurrentSkillGrade), "Failed to load Skill Group of Firs tSkill Template.");

            return group.GetCanActiveSkillMember().Data.Grade;
        }

        public void LevelUp(SkillTemplate templateOrigin)
        {
            SkillBase newSkill = Unlock(templateOrigin);
            /*
                DO SOMETHING BELOW
            */
        }

        private SkillBase Unlock(SkillTemplate templateOrigin)
        {
            if (SkillGroupsDict.TryGetValue((int)templateOrigin, out SkillGroup group) == false)
                Utils.LogCritical(nameof(SkillBook), nameof(Unlock), $"Check TemplateID : {templateOrigin}");

            return group.Unlock();
        }

        public void Activate(SkillTemplate templateOrigin)
        {
            if (SkillGroupsDict.TryGetValue((int)templateOrigin, out SkillGroup group) == false)
                Utils.LogCritical(nameof(SkillBook), nameof(Activate), $"Check TemplateID : {templateOrigin}");

            group.Activate();
        }

        public void ActivateAll()
        {
            foreach (var group in SkillGroupsDict)
                group.Value.Activate();
        }

        public void Deactivate(SkillTemplate templateOrigin)
        {
            if (SkillGroupsDict.TryGetValue((int)templateOrigin, out SkillGroup group) == false)
                Utils.LogCritical(nameof(SkillBook), nameof(Deactivate), $"Check TemplateID : {templateOrigin}");

            group.Deactivate();
        }

        public void DeactivateAll()
        {
            foreach (var group in SkillGroupsDict)
                group.Value.Deactivate();
        }

        public SkillBase GetCanActiveSkillMember(SkillTemplate templateOrigin)
        {
            if (SkillGroupsDict.TryGetValue((int)templateOrigin, out SkillGroup group) == false)
                Utils.LogCritical(nameof(SkillBook), nameof(GetCanActiveSkillMember), $"Check TemplateID : {templateOrigin}");

            SkillBase skill = group.GetCanActiveSkillMember();
            if (skill == null)
                Utils.LogStrong(nameof(SkillGroup), nameof(GetCanActiveSkillMember), "You need to unlock first skill.");

            return skill;
        }

        public SkillBase ForceGetSkillMember(SkillTemplate templateOrigin, int idxOfGroupMember)
        {
            if (SkillGroupsDict.TryGetValue((int)templateOrigin, out SkillGroup group) == false)
                Utils.LogCritical(nameof(SkillBook), nameof(ForceGetSkillMember), $"Check TemplateID : {templateOrigin}");

            for (int i = 0; i < group.MemberCount; ++i)
            {
                if (idxOfGroupMember == i)
                    return group.Members[i].SkillOrigin;
            }

            return null;
        }

        // callback 전용 메서드 정의
        public void RandomizeSequenceGroup(SkillTemplate prevTemplateOrigin)
        {
            this.Deactivate(prevTemplateOrigin); // End Prev Sequence Skill Template
            //Utils.Log("Randomize Sequence Something if you want to.");
        }

        public bool IsOnShield => (this.CachedShield != null) ? this.CachedShield.IsOnShield : false;
        public void HitShield() => this.CachedShield.Hit();
        //public void OffSheild() => this.CachedShield.OffShield();
        public void OffSheild() => this.CachedShield.IsOnShield = false;

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
                        if (skillOrigin.Data.SkillType == Define.SkillType.Action)
                        {
                            if (skillOrigin.Owner.AnimCallback.OnActiveMasteryAction != null)
                            {
                                skillOrigin.Owner.AnimCallback.OnActiveMasteryAction -= skillOrigin.GetComponent<ActionSkill>().OnActiveMasteryActionHandler;
                                Utils.Log($"Release Event(OnActiveMasteryAction) : {skillOrigin.Data.Name}");
                            }
                            else if (skillOrigin.Owner.AnimCallback.OnActiveEliteAction != null)
                            {
                                skillOrigin.Owner.AnimCallback.OnActiveEliteAction -= skillOrigin.GetComponent<ActionSkill>().OnActiveEliteActionHandler;
                                Utils.Log($"Release Event(OnActiveEliteAction) : {skillOrigin.Data.Name}");
                            }
                        }
                    }
                }
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
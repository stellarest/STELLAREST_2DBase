using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using STELLAREST_2D.Data;
using UnityEngine;
using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

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
                    if (i == 0) // 있어야하는게 맞음
                    {
                        SkillBase unlockSkill = Members[0].Unlock();
                        if (unlockSkill.Data.HasEventHandler) // HasEventHandler : Exclusive Skill
                        {
                            if (unlockSkill.Data.SkillType == Define.SkillType.Repeat)
                            {
                                unlockSkill.Owner.AnimCallback.OnActiveRepeatSkill += unlockSkill.GetComponent<RepeatSkill>().OnActiveRepeatSkillHandler;
                                Utils.Log($"Add Event(OnActiveRepeatSkill) : {unlockSkill.Data.Name}");
                            }
                            else if (unlockSkill.Data.SkillType == Define.SkillType.Sequence)
                            {
                                unlockSkill.Owner.AnimCallback.OnActiveSequenceSkill += unlockSkill.GetComponent<SequenceSkill>().OnActiveSequenceSkillHandler;
                                Utils.Log($"Release Event(OnActiveSequenceSkill) : {unlockSkill.Data.Name}");
                            }
                        }

                        return unlockSkill;
                    }
                    else // REST OF SKILLS
                    {
                        // RELEASE EVENT
                        SkillBase deactiveSkill = Members[i - 1].DeactiveForce();
                        if (deactiveSkill.Data.HasEventHandler)
                        {
                            if (deactiveSkill.Data.SkillType == Define.SkillType.Repeat)
                            {
                                deactiveSkill.Owner.AnimCallback.OnActiveRepeatSkill -= deactiveSkill.GetComponent<RepeatSkill>().OnActiveRepeatSkillHandler;
                                Utils.Log($"Release Event(OnActiveRepeatSkill) : {deactiveSkill.Data.Name}");
                            }
                            else if (deactiveSkill.Data.SkillType == Define.SkillType.Sequence)
                            {
                                deactiveSkill.Owner.AnimCallback.OnActiveSequenceSkill -= deactiveSkill.GetComponent<SequenceSkill>().OnActiveSequenceSkillHandler;
                                Utils.Log($"Release Event(OnActiveSequenceSkill) : {deactiveSkill.Data.Name}");
                            }
                        }

                        SkillBase unlockSkill = Members[i].Unlock();
                        if (unlockSkill.Data.HasEventHandler)
                        {
                            if (unlockSkill.Data.SkillType == Define.SkillType.Repeat)
                            {
                                unlockSkill.Owner.AnimCallback.OnActiveRepeatSkill += unlockSkill.GetComponent<RepeatSkill>().OnActiveRepeatSkillHandler;
                                Utils.Log($"Add Event(OnActiveRepeatSkill) : {unlockSkill.Data.Name}");
                            }
                            else if (unlockSkill.Data.SkillType == Define.SkillType.Sequence)
                            {
                                unlockSkill.Owner.AnimCallback.OnActiveSequenceSkill += unlockSkill.GetComponent<SequenceSkill>().OnActiveSequenceSkillHandler;
                                Utils.Log($"Add Event(OnActiveSequenceSkill) : {unlockSkill.Data.Name}");
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
    }

    [System.Serializable]
    public class SkillBook : MonoBehaviour
    {
        public CreatureController Owner { get; set; }
        public SkillTemplate FirstSkill { get; private set; } = SkillTemplate.None; // REPEAT OR SEQUENCE
        public SkillTemplate SecondSequenceSkill { get; private set; } = SkillTemplate.None; // MUST BE SEQUENCE
        public SkillTemplate LastSequenceSkill { get; private set; } = SkillTemplate.None; // MUST BE SEQUENCE

        // +++ Key(int) : Skill Origin Template, Value : SkillGroup +++
        [System.Serializable] public class SkillGroupDictionary : SerializableGroupDictionary<int, SkillGroup> { }
        [field: SerializeField] public SkillGroupDictionary SkillGroupsDict { get; private set; } = new SkillGroupDictionary();

        public int SequenceIdx { get; set; } = 0;
        [field: SerializeField] public List<SequenceSkill> SequenceSkills { get; private set; } = new List<SequenceSkill>();

        public Shield CachedShield { get; private set; } = null;
        public SecondWind CachedSecondWind { get; private set; } = null;

        public void LateInit()
        {
            this.SetFirstSkill();
            foreach (var group in SkillGroupsDict)
            {
                for (int i = 0; i < group.Value.MemberCount; ++i)
                {
                    if (group.Value.Members[i].SkillOrigin.Data.SkillType == Define.SkillType.Sequence)
                        SequenceSkills.Add(group.Value.Members[i].SkillOrigin as SequenceSkill);
                }
            }

            if (this.Owner?.IsPlayer() == true)
                SetSequenceSkills();
        }

        private void SetSequenceSkills()
        {
            for (int i = 0; i < SequenceSkills.Count; ++i)
            {
                if (SequenceSkills[i].Data.OriginalTemplate == SkillTemplate.Shield)
                {
                    SecondSequenceSkill = SequenceSkills[i].Data.OriginalTemplate;
                    if (this.CachedShield == null)
                    {
                        this.CachedShield = SequenceSkills[i].GetComponent<Shield>();
                        Utils.Log("Success Init Cache : Shield");
                    }
                }

                if (SequenceSkills[i].Data.OriginalTemplate == SkillTemplate.SecondWind)
                {
                    SecondSequenceSkill = SequenceSkills[i].Data.OriginalTemplate;
                    if (this.CachedSecondWind == null)
                    {
                        this.CachedSecondWind = SequenceSkills[i].GetComponent<SecondWind>();
                        Utils.Log("Success Init Cache : Second Wind");
                    }
                }

                // if (i == 0)
                //     SecondSequenceSkill = SequenceSkills[i].Data.OriginalTemplate;
                // else
                //     LastSequenceSkill = SequenceSkills[i].Data.OriginalTemplate;
            }
        }

        public void ReserveNextSequence(SkillTemplate currentEnd)
        {
        }

        private void SetFirstSkill()
            => FirstSkill = (SkillGroupsDict.Count > 0) ? (SkillTemplate)SkillGroupsDict.First().Key : SkillTemplate.None;

        public Define.InGameGrade GetFirstSkillGrade()
        {
            if (SkillGroupsDict.TryGetValue((int)this.FirstSkill, out SkillGroup group) == false)
                Utils.LogCritical(nameof(SkillBook), nameof(GetFirstSkillGrade), "Failed to load Skill Group of Firs tSkill Template.");

            return group.GetCanActiveSkillMember().Data.Grade;
        }

        public void LevelUp(SkillTemplate templateOrigin)
        {
            SkillBase newSkill = Unlock(templateOrigin);
            // DO SOMETHING // 
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
            // if (this.CachedShield == null && templateOrigin == SkillTemplate.Shield)
            //     this.CachedShield = this.GetCanActiveSkillMember(SkillTemplate.Shield).GetComponent<Shield>();
            // if (this.CachedSecondWind == null && templateOrigin == SkillTemplate.SecondWind)
            //     this.CachedSecondWind = this.GetCanActiveSkillMember(SkillTemplate.SecondWind).GetComponent<SecondWind>();
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

        // callback 전용 메서드 정의
        public void RandomizeSequenceGroup(SkillTemplate prevTemplateOrigin)
        {
            this.Deactivate(prevTemplateOrigin); // End Prev Sequence Skill Template
            //Utils.Log("Randomize Sequence Something if you want to.");
        }

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
                        if (skillOrigin.Data.SkillType == Define.SkillType.Repeat)
                        {
                            if (skillOrigin.Owner.AnimCallback.OnActiveRepeatSkill != null)
                            {
                                skillOrigin.Owner.AnimCallback.OnActiveRepeatSkill -= skillOrigin.GetComponent<RepeatSkill>().OnActiveRepeatSkillHandler;
                                Utils.Log($"{skillOrigin.Data.Name}, Release Events(OnActiveRepeatSkill)");
                            }
                        }
                        else if (skillOrigin.Data.SkillType == Define.SkillType.Sequence)
                        {
                            if (skillOrigin.Owner.AnimCallback.OnActiveSequenceSkill != null)
                            {
                                skillOrigin.Owner.AnimCallback.OnActiveSequenceSkill -= skillOrigin.GetComponent<SequenceSkill>().OnActiveSequenceSkillHandler;
                                Utils.Log($"{skillOrigin.Data.Name}, Release Events(OnActiveSequenceSkill)");
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

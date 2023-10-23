using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.HeroEditor.Common.Scripts.ExampleScripts;
using STELLAREST_2D.Data;
using UnityEngine;
using UnityEngine.UIElements;
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
                    // FIRST SKILL
                    if (i == 0)
                    {
                        SkillBase unlockSkill = Members[0].Unlock();
                        if (unlockSkill.Data.HasEventHandler)
                        {
                            if (unlockSkill.Data.SkillType == Define.SkillType.Repeat)
                            {
                                unlockSkill.Owner.AnimCallback.OnCloneRepeatSkill += unlockSkill.GetComponent<RepeatSkill>().OnCloneRepeatSkillHandler;
                                Utils.Log($"Add Event : {unlockSkill.Data.Name}");
                            }
                        }

                        return unlockSkill;
                    }
                    else // REST OF SKILLS
                    {
                        SkillBase deactiveSkill = Members[i - 1].DeactiveForce();
                        if (deactiveSkill.Data.HasEventHandler)
                        {
                            if (deactiveSkill.Data.SkillType == Define.SkillType.Repeat)
                            {
                                deactiveSkill.Owner.AnimCallback.OnCloneRepeatSkill -= deactiveSkill.GetComponent<RepeatSkill>().OnCloneRepeatSkillHandler;
                                Utils.Log($"Release Event : {deactiveSkill.Data.Name}");
                            }
                        }

                        SkillBase unlockSkill = Members[i].Unlock();
                        if (unlockSkill.Data.HasEventHandler)
                        {
                            if (unlockSkill.Data.SkillType == Define.SkillType.Repeat)
                            {
                                unlockSkill.Owner.AnimCallback.OnCloneRepeatSkill += unlockSkill.GetComponent<RepeatSkill>().OnCloneRepeatSkillHandler;
                                Utils.Log($"Add Event : {unlockSkill.Data.Name}");
                            }
                        }

                        return unlockSkill;
                    }
                }
                else if (i == MemberCount - 1)
                {
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
        public SkillTemplate FirstSkill { get; private set; } = SkillTemplate.None;
        // +++ Key(int) : Skill Origin Template, Value : SkillGroup +++
        [System.Serializable] public class SkillGroupDictionary : SerializableGroupDictionary<int, SkillGroup> { }
        [field: SerializeField] public SkillGroupDictionary SkillGroupsDict { get; private set; } = new SkillGroupDictionary();

        [field: SerializeField] public List<SequenceSkill> SequenceSkills { get; private set; } = new List<SequenceSkill>();
        public int SequenceIdx { get; set; } = 0;

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
        }

        private void SetFirstSkill()
            => FirstSkill = (SkillGroupsDict.Count > 0) ? (SkillTemplate)SkillGroupsDict.First().Key : SkillTemplate.None;

        public void LevelUp(SkillTemplate templateOrigin)
        {
            SkillBase newSkill = Acquire(templateOrigin);
            // DO SOMETHING // 
        }

        private SkillBase Acquire(SkillTemplate templateOrigin)
        {
            if (SkillGroupsDict.TryGetValue((int)templateOrigin, out SkillGroup group) == false)
                Utils.LogCritical(nameof(SkillBook), nameof(Acquire), $"Check TemplateID : {templateOrigin}");

            return group.Unlock();
        }

        public void Activate(SkillTemplate templateOrigin)
        {
            if (SkillGroupsDict.TryGetValue((int)templateOrigin, out SkillGroup group) == false)
                Utils.LogCritical(nameof(SkillBook), nameof(Acquire), $"Check TemplateID : {templateOrigin}");

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
                Utils.LogCritical(nameof(SkillBook), nameof(Acquire), $"Check TemplateID : {templateOrigin}");

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
                Utils.LogCritical(nameof(SkillBook), nameof(Acquire), $"Check TemplateID : {templateOrigin}");

            SkillBase skill = group.GetCanActiveSkillMember();
            if (skill == null)
                Utils.LogStrong(nameof(SkillGroup), nameof(GetCanActiveSkillMember), "You need to unlock first skill.");

            return skill;
        }

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
                            if (skillOrigin.Owner.AnimCallback.OnCloneRepeatSkill != null)
                            {
                                skillOrigin.Owner.AnimCallback.OnCloneRepeatSkill -= skillOrigin.GetComponent<RepeatSkill>().OnCloneRepeatSkillHandler;
                                Utils.Log($"{skillOrigin.Data.Name}, Release Events");
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
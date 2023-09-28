using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.HeroEditor.Common.Scripts.ExampleScripts;
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
            skillOrigin.Deactivate();
            this.SkillOrigin = skillOrigin;
            this.Name = skillOrigin.Data.Name;
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
                        if (unlockSkill.Data.IsExclusive)
                        {
                            if (unlockSkill.Data.SkillType == Define.SkillType.Repeat)
                            {
                                unlockSkill.Owner.AnimCallback.OnCloneExclusiveRepeatSkill += unlockSkill.GetComponent<RepeatSkill>().OnCloneExclusiveRepeatSkillHandler;
                                Utils.Log($"Add Event : {unlockSkill.Data.Name}");
                            }
                        }

                        return unlockSkill;
                    }
                    else // REST OF SKILLS
                    {
                        SkillBase deactiveSkill = Members[i - 1].DeactiveForce();
                        if (deactiveSkill.Data.IsExclusive)
                        {
                            if (deactiveSkill.Data.SkillType == Define.SkillType.Repeat)
                            {
                                deactiveSkill.Owner.AnimCallback.OnCloneExclusiveRepeatSkill -= deactiveSkill.GetComponent<RepeatSkill>().OnCloneExclusiveRepeatSkillHandler;
                                Utils.Log($"Release Event : {deactiveSkill.Data.Name}");
                            }
                        }

                        SkillBase unlockSkill = Members[i].Unlock();
                        if (unlockSkill.Data.IsExclusive)
                        {
                            if (unlockSkill.Data.SkillType == Define.SkillType.Repeat)
                            {
                                unlockSkill.Owner.AnimCallback.OnCloneExclusiveRepeatSkill += unlockSkill.GetComponent<RepeatSkill>().OnCloneExclusiveRepeatSkillHandler;
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
                    continue;

                if (Members[i].IsLearned == false)
                    continue;

                if (Members[i].IsLast == false)
                    continue;

                if (Members[i].IsLearned && Members[i].IsLearned)
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

                if (Members[i].IsLearned && Members[i].IsLearned)
                {
                    Members[i].SetActiveLog(false);
                    Members[i].SkillOrigin.Deactivate();
                }
            }
        }

        public SkillBase GetSkillMember()
        {
            for (int i = 0; i < MemberCount; ++i)
            {
                if (Members[i].IsLearned == false)
                    continue;

                if (Members[i].IsLast == false)
                    continue;

                if (Members[i].IsLearned && Members[i].IsLearned)
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

    // 일종의 소형 스킬 매니저. QuestBook도 이런식으로 파주면 관리하기 편해짐
    // 이런거 만드는건 "진짜 자유롭게" 만들라고함
    [System.Serializable]
    public class SkillBook : MonoBehaviour
    {
        public CreatureController Owner { get; set; }
        public SkillTemplate FirstExclusiveSkill { get; private set; } = SkillTemplate.None;

        [System.Serializable] public class SkillGroupDictionary : SerializableGroupDictionary<int, SkillGroup> { }
        [field: SerializeField] public SkillGroupDictionary SkillGroupsDict { get; private set; } = new SkillGroupDictionary();

        //=> FirstExclusiveSkill = (SkillTemplate)SkillGroupsDict.First().Key;
        public void SetFirstExclusiveSkill()
            => FirstExclusiveSkill = (SkillGroupsDict.Count > 0) ? (SkillTemplate)SkillGroupsDict.First().Key : SkillTemplate.None;

        public void LevelUp(SkillTemplate templateOrigin)
        {
            SkillBase newSkill = Acquire(templateOrigin);
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

        public void Deactivate(SkillTemplate templateOrigin)
        {
            if (SkillGroupsDict.TryGetValue((int)templateOrigin, out SkillGroup group) == false)
                Utils.LogCritical(nameof(SkillBook), nameof(Acquire), $"Check TemplateID : {templateOrigin}");

            group.Deactivate();
        }

        public SkillBase GetSkillMember(SkillTemplate templateOrigin)
        {
            if (SkillGroupsDict.TryGetValue((int)templateOrigin, out SkillGroup group) == false)
                Utils.LogCritical(nameof(SkillBook), nameof(Acquire), $"Check TemplateID : {templateOrigin}");

            SkillBase skill = group.GetSkillMember();
            if (skill == null)
                Utils.LogStrong(nameof(SkillGroup), nameof(GetSkillMember), "You need to unlock first skill.");

            return skill;
        }

        public void ActivateAll()
        {
            foreach (KeyValuePair<int, SkillGroup> pair in SkillGroupsDict)
            {
                for (int i = 0; i < pair.Value.MemberCount; ++i)
                {
                    SkillBase skillOrigin = pair.Value.Members[i].SkillOrigin;
                    if (skillOrigin.IsLearned && skillOrigin.IsLast)
                        skillOrigin.Activate();
                }
            }
        }

        public void DeactivateAll()
        {
            foreach (KeyValuePair<int, SkillGroup> pair in SkillGroupsDict)
            {
                for (int i = 0; i < pair.Value.MemberCount; ++i)
                {
                    SkillBase skillOrigin = pair.Value.Members[i].SkillOrigin;
                    if (skillOrigin.IsLearned && skillOrigin.IsLast)
                        skillOrigin.Deactivate();
                }
            }
        }

        private void OnDestroy()
        {
            foreach (KeyValuePair<int, SkillGroup> pair in SkillGroupsDict)
            {
                for (int i = 0; i < pair.Value.MemberCount; ++i)
                {
                    SkillBase skillOrigin = pair.Value.Members[i].SkillOrigin;
                    if (skillOrigin.Data.IsExclusive)
                    {
                        if (skillOrigin.Data.SkillType == Define.SkillType.Repeat)
                        {
                            if (skillOrigin.Owner.AnimCallback.OnCloneExclusiveRepeatSkill != null)
                            {
                                skillOrigin.Owner.AnimCallback.OnCloneExclusiveRepeatSkill -= skillOrigin.GetComponent<RepeatSkill>().OnCloneExclusiveRepeatSkillHandler;
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
// public SkillBase Activate(SkillTemplate templateOrigin)
// {
//     if (SkillGroupsDict.TryGetValue((int)templateOrigin, out SkillGroup group) == false)
//         Utils.LogCritical(nameof(SkillBook), nameof(Acquire), $"Check TemplateID : {templateOrigin}");

//     SkillBase skill = group.GetSkillMember();
//     if (skill == null)
//     {
//         Utils.LogStrong(nameof(SkillBook), nameof(Activate), "Failed to activate skill.");
//         return null;
//     }
//     else
//         skill.Activate();

//     return skill;
// }

// public SkillBase Deactivate(SkillTemplate templateOrigin)
// {
//     if (SkillGroupsDict.TryGetValue((int)templateOrigin, out SkillGroup group) == false)
//         Utils.LogCritical(nameof(SkillBook), nameof(Acquire), $"Check TemplateID : {templateOrigin}");

//     SkillBase skill = group.GetSkillMember();
//     if (skill == null)
//     {
//         Utils.LogStrong(nameof(SkillBook), nameof(Deactivate), "Failed to activate skill.");
//         return null;
//     }
//     else
//         skill.Deactivate();

//     return skill;
// }

// public void LaunchSkill(SkillTemplate templateOrigin)
        // {
        //     if (SkillGroupsDict.TryGetValue((int)templateOrigin, out SkillGroup group) == false)
        //         Utils.LogCritical(nameof(SkillBook), nameof(Acquire), $"Check TemplateID : {templateOrigin}");

        //     SkillBase skillOrigin = group.GetSkillMember();
        //     //Activate를 하면, CoStartSkill() -> DoSkillJob()

        //     SkillBase clone = Managers.Object.Spawn<SkillBase>(spawnPos: transform.position,  templateID: skillOrigin.Data.TemplateID, 
        //                 spawnObjectType: Define.ObjectType.Skill, isPooling: true);

        //     clone.Init(skillOrigin.Owner, skillOrigin.Data);
        // }

        // UpdateCurrentOwnerState (Ready to shot projectile)

        // Melee Swing, Ranged Shot Instead
        // private IEnumerator CoLaunchSkill(SkillBase skillOrigin)
        // {
        //     // Vector3 shootDir = Owner.ShootDir;
        //     // Define.LookAtDirection lootAtDir = Owner.LookAtDir;
        //     // Vector3 indicatorAngle = Owner.Indicator.eulerAngles;
        //     // Vector3 spawnPos = Vector3.zero;

        //     // ProjectileController pc = Managers.Object.Spawn<ProjectileController>(spawnPos, skillOrigin.Data.TemplateID, 
        //     //     Define.ObjectType.Projectile, true);

        //     yield return null;

        //     // -------------------------------------------------------------------------------------
        //     // -------------------------------------------------------------------------------------
        //     // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //     // TODO : CreatureController에 ShootDir, LootAtDir, Indicator를 모두 넣는다.
        //     // Owner를 통해서 받아온다 (이러면 몬스터인지 플레이어인지 구분하지 않아도 된다.)
        //     // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //     // if (Owner?.IsPlayer() == true)
        //     // {
        //     //     // Managers.Game.Player.AttackEndPoint = transform.position; // ---> CHECK,,,
        //     //     Vector3 shootDir = Managers.Game.Player.ShootDir;
        //     //     Define.LookAtDirection lootAtDir = Managers.Game.Player.LookAtDir;
        //     //     Vector3 indicatorAngle = Managers.Game.Player.Indicator.eulerAngles;

        //     //     Vector3 spawnPos = Vector3.zero;
        //     //     SkillData data = skillOrigin.Data;

        //     //     if (data.IsOnFireSocket)
        //     //         spawnPos = Managers.Game.Player.FireSocketPosition;
        //     //     else
        //     //         spawnPos = Managers.Game.Player.transform.position;

        //     //     // +++ Local Position Add : TEST DO SOMETHING
        //     //     Vector3 localScale = Managers.Game.Player.LocalScale;
        //     //     localScale *= 0.8f;

        //     //     // +++ FIRST SET +++
        //     //     int skillCount = data.ContinuousCount;
        //     //     float[] continuousAngles = new float[skillCount];
        //     //     float[] continuousSpeedRatios = new float[skillCount];
        //     //     float[] continuousFlipXs = new float[skillCount];
        //     //     float[] continuousFlipYs = new float[skillCount];
        //     //     float[] interPolateTargetScaleXs = new float[skillCount];
        //     //     float[] interPolateTargetScaleYs = new float[skillCount];
        //     //     bool[] isOnlyVisibles = new bool[skillCount];
        //     //     for (int i = 0; i < skillCount; ++i)
        //     //     {
        //     //         continuousSpeedRatios[i] = data.ContinuousSpeedRatios[i];
        //     //         continuousFlipXs[i] = data.ContinuousFlipXs[i];
        //     //         continuousFlipYs[i] = data.ContinuousFlipYs[i];
        //     //         isOnlyVisibles[i] = data.IsOnlyVisibles[i];
        //     //         if (Managers.Game.Player.IsFacingRight == false)
        //     //         {
        //     //             continuousAngles[i] = data.ContinuousAngles[i] * -1;
        //     //             interPolateTargetScaleXs[i] = data.ScaleInterpolations[i].x * -1;
        //     //             interPolateTargetScaleYs[i] = data.ScaleInterpolations[i].y;
        //     //         }
        //     //         else
        //     //         {
        //     //             continuousAngles[i] = data.ContinuousAngles[i];
        //     //             interPolateTargetScaleXs[i] = data.ScaleInterpolations[i].x;
        //     //             interPolateTargetScaleYs[i] = data.ScaleInterpolations[i].y;
        //     //         }
        //     //     }

        //     //     //this.Owner.SetAttackStartPoint();                    
        //     //     // +++ SECOND SHOOT +++
        //     //     for (int i = 0; i < skillCount; ++i)
        //     //     {
        //     //         ProjectileController pc = Managers.Object.Spawn<ProjectileController>(spawnPos, data.TemplateID, Define.ObjectType.Projectile, true);
        //     //         if (pc.IsFirstPooling)
        //     //             pc.SetInitialCloneInfo(skillOrigin);

        //     //         pc.SetProjectileInfo(shootDir: shootDir, lootAtDir: lootAtDir, localScale: localScale, indicatorAngle: indicatorAngle,
        //     //                              continuousAngle: continuousAngles[i], continuousSpeedRatio: continuousSpeedRatios[i], continuousFlipX: continuousFlipXs[i], continuousFlipY: continuousFlipYs[i],
        //     //                              interPolateTargetScaleX: interPolateTargetScaleXs[i], interpolateTargetScaleY: interPolateTargetScaleYs[i], isOnlyVisible: isOnlyVisibles[i]);

        //     //         yield return new WaitForSeconds(data.ContinuousSpacing);
        //     //     }
        //     // }
        // }
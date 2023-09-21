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
            this.SkillOrigin = skillOrigin;
            this.Name = skillOrigin.Data.Name;
            this.IsLearned = skillOrigin.IsLearned;
            this.IsLast = skillOrigin.IsLast;
        }

        public SkillBase Unlock()
        {
            this.SkillOrigin.IsLearned = true;
            this.IsLearned = true;

            this.SkillOrigin.IsLast = true;
            this.IsLast = true;

            return SkillOrigin;
        }

        public void Deactivate()
        {
            this.SkillOrigin.IsLast = false;
            this.IsLast = false;
        }

        [field: SerializeField] public SkillBase SkillOrigin { get; private set; } = null;
        [field: SerializeField, ShowOnly] public string Name { get; private set; } = string.Empty;
        [field: SerializeField, ShowOnly] public bool IsLearned { get; private set; } = false;
        [field: SerializeField, ShowOnly] public bool IsLast { get; private set; } = false;
    }

    [System.Serializable]
    public class SkillGroup : IGroupDictionary
    {
        public SkillGroup(SkillBase skillOrigin)
        {
            this.SkillOrigin = skillOrigin;
        }

        public void InitLeader(object groupLeader)
        {
            SkillGroup leader = groupLeader as SkillGroup;
            if (leader != null)
            {
                Tag = $"Group : {leader.SkillOrigin.Data.Name}";
                SkillType = leader.SkillOrigin.Data.SkillType;
                SkillMembers = new List<SkillMember>();
                SkillMembers.Add(new SkillMember(leader.SkillOrigin));
                ++SkillCount;
            }
        }

        public void AddMember(object groupLeader, object groupMember)
        {
            SkillGroup leader = groupLeader as SkillGroup;
            SkillGroup member = groupMember as SkillGroup;
            if (leader != null && member != null)
            {
                leader.SkillMembers.Add(new SkillMember(member.SkillOrigin));
                ++SkillCount;
            }
        }

        public SkillBase Unlock()
        {
            // SkillMembers의 개수가 1개일수도 있고, 2개일수도 있고, 3개일수도 있다.
            for (int i = 0; i < SkillCount; ++i)
            {
                if (SkillMembers[i].IsLearned == false)
                {
                    // FIRST SKILL
                    if (i == 0)
                    {
                        return SkillMembers[0].Unlock();
                    }
                    else // REST OF SKILLS
                    {
                        SkillMembers[i - 1].Deactivate();
                        return SkillMembers[i].Unlock();
                    }
                }
                else if (i == SkillCount - 1)
                {
                    Utils.Log($"{SkillMembers[i].Name} is <color=cyan>MAX LEVEL</color> in this group.");
                    return SkillMembers[i].SkillOrigin;
                }
            }

            return null;
        }

        public SkillBase SkillOrigin { get; private set; } = null;
        [field: SerializeField, ShowOnly] public string Tag { get; private set; } = string.Empty;
        [field: SerializeField] public Define.SkillType SkillType { get; private set; } = Define.SkillType.None;
        [field: SerializeField, ShowOnly] public int SkillCount { get; private set; } = 0;
        [field: SerializeField] public List<SkillMember> SkillMembers { get; private set; } = null;
    }

    // 일종의 소형 스킬 매니저. QuestBook도 이런식으로 파주면 관리하기 편해짐
    // 이런거 만드는건 "진짜 자유롭게" 만들라고함
    [System.Serializable]
    public class SkillBook : MonoBehaviour
    {
        public CreatureController Owner { get; set; }

        [System.Serializable] public class SkillGroupDictionary : SerializableGroupDictionary<int, SkillGroup> { }
        [field: SerializeField] public SkillGroupDictionary SkillGroupsDict { get; private set; } = new SkillGroupDictionary();

        public void LevelUp(SkillTemplate templateOrigin, bool isActiveImmediately = false)
        {
            SkillBase newSkill = Acquire(templateOrigin);
            if (isActiveImmediately)
                newSkill.Activate();
            else
                newSkill.Deactivate();
        }

        private SkillBase Acquire(SkillTemplate templateOrigin)
        {
            if (SkillGroupsDict.TryGetValue((int)templateOrigin, out SkillGroup group) == false)
                Utils.LogCritical(nameof(SkillBook), nameof(Acquire), $"Check TemplateID : {templateOrigin}");
           
            return group.Unlock();
        }

        public void StopAll()
        {
        }

        public void ReStartAll()
        {
            
        }

        // public void ActivateAll(Define.SkillType type, bool isOnDeactiveAll = false)
        // {
        //     if (type == Define.SkillType.Repeat)
        //     {
        //         if (isOnDeactiveAll == false)
        //         {
        //             for (int i = 0; i < LastLearnedAllRepeatSkills.Count; ++i)
        //             {
        //                 Utils.Log($"Activate one of the members : {LastLearnedAllRepeatSkills[i].Data.Name}");
        //                 LastLearnedAllRepeatSkills[i].Activate();
        //             }
        //         }
        //         else
        //         {
        //             for (int i = 0; i < LastLearnedAllRepeatSkills.Count; ++i)
        //             {
        //                 Utils.Log($"Deactivate one of the members : {LastLearnedAllRepeatSkills[i].Data.Name}");
        //                 LastLearnedAllRepeatSkills[i].Deactivate();
        //             }
        //         }
        //     }
        // }

        public void OnSkillEnabledHandler(SkillTemplate templateOrigin)
        {
        }

        private IEnumerator CoGenerateExclusiveSkill(SkillBase exclusiveSkill)
        {
            // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // TODO : CreatureController에 ShootDir, LootAtDir, Indicator를 모두 넣는다.
            // Owner를 통해서 받아온다 (이러면 몬스터인지 플레이어인지 구분하지 않아도 된다.)
            // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            if (Owner?.IsPlayer() == true)
            {
                // Managers.Game.Player.AttackEndPoint = transform.position; // ---> CHECK,,,
                Vector3 shootDir = Managers.Game.Player.ShootDir;
                Define.LookAtDirection lootAtDir = Managers.Game.Player.LookAtDir;
                Vector3 indicatorAngle = Managers.Game.Player.Indicator.eulerAngles;

                Vector3 spawnPos = Vector3.zero;
                SkillData data = exclusiveSkill.Data;

                if (data.IsOnFireSocket)
                    spawnPos = Managers.Game.Player.FireSocketPosition;
                else
                    spawnPos = Managers.Game.Player.transform.position;

                // +++ Local Position Add : TEST DO SOMETHING
                Vector3 localScale = Managers.Game.Player.LocalScale;
                localScale *= 0.8f;

                // +++ FIRST SET +++
                int skillCount = data.ContinuousCount;
                float[] continuousAngles = new float[skillCount];
                float[] continuousSpeedRatios = new float[skillCount];
                float[] continuousFlipXs = new float[skillCount];
                float[] continuousFlipYs = new float[skillCount];
                float[] interPolateTargetScaleXs = new float[skillCount];
                float[] interPolateTargetScaleYs = new float[skillCount];
                bool[] isOnlyVisibles = new bool[skillCount];
                for (int i = 0; i < skillCount; ++i)
                {
                    continuousSpeedRatios[i] = data.ContinuousSpeedRatios[i];
                    continuousFlipXs[i] = data.ContinuousFlipXs[i];
                    continuousFlipYs[i] = data.ContinuousFlipYs[i];
                    isOnlyVisibles[i] = data.IsOnlyVisibles[i];
                    if (Managers.Game.Player.IsFacingRight == false)
                    {
                        continuousAngles[i] = data.ContinuousAngles[i] * -1;
                        interPolateTargetScaleXs[i] = data.ScaleInterpolations[i].x * -1;
                        interPolateTargetScaleYs[i] = data.ScaleInterpolations[i].y;
                    }
                    else
                    {
                        continuousAngles[i] = data.ContinuousAngles[i];
                        interPolateTargetScaleXs[i] = data.ScaleInterpolations[i].x;
                        interPolateTargetScaleYs[i] = data.ScaleInterpolations[i].y;
                    }
                }

                //this.Owner.SetAttackStartPoint();                    
                // +++ SECOND SHOOT +++
                for (int i = 0; i < skillCount; ++i)
                {
                    ProjectileController pc = Managers.Object.Spawn<ProjectileController>(spawnPos, data.TemplateID, Define.ObjectType.Projectile, true);
                    if (pc.IsFirstPooling)
                        pc.SetInitialCloneInfo(exclusiveSkill);

                    pc.SetProjectileInfo(shootDir: shootDir, lootAtDir: lootAtDir, localScale: localScale, indicatorAngle: indicatorAngle,
                                         continuousAngle: continuousAngles[i], continuousSpeedRatio: continuousSpeedRatios[i], continuousFlipX: continuousFlipXs[i], continuousFlipY: continuousFlipYs[i],
                                         interPolateTargetScaleX: interPolateTargetScaleXs[i], interpolateTargetScaleY: interPolateTargetScaleYs[i], isOnlyVisible: isOnlyVisibles[i]);

                    yield return new WaitForSeconds(data.ContinuousSpacing);
                }

                // +++++ TEMP +++++
                // for (int i = 0; i < skillCount; ++i)
                // {
                //     Managers.Game.Player.AttackEndPoint = transform.position;
                //     ProjectileController pc = Managers.Object.Spawn<ProjectileController>(spawnPos, exclusiveSkill.Data.TemplateID, Define.ObjectType.RepeatProjectile, true);
                //     if (pc.IsFirstPooling)
                //         pc.SetInitialCloneInfo(exclusiveSkill);

                //     pc.SetProjectileInfo(shootDir: shootDir, lootAtDir: lootAtDir, localScale: localScale, indicatorAngle: indicatorAngle,
                //                          continuousAngle: continuousAngles[i], continuousSpeedRatio: continuousSpeedRatios[i],
                //                          continuousFlipX: continuousFlipXs[i], continuousFlipY: continuousFlipYs[i], continuousPower: continuousPowers[i],
                //                          interPolateTargetScaleX: interPolateTargetScaleXs[i], interpolateTargetScaleY: interPolateTargetScaleYs[i], isOnlyVisible: isOnlyVisibles[i]);

                //     yield return new WaitForSeconds(_defaultRepeatSkill.Data.ContinuousSpacing);
                // }
            }
        }
    }
}
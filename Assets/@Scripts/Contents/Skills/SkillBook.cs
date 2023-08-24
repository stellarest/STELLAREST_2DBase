using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace STELLAREST_2D
{
    // 일종의 소형 스킬 매니저. QuestBook도 이런식으로 파주면 관리하기 편해짐
    // 이런거 만드는건 "진짜 자유롭게" 만들라고함
    public class SkillBook : MonoBehaviour
    {
        public CreatureController Owner { get; set; }
        public Define.TemplateIDs.SkillType PlayerDefaultSkill { get; set; }


        public List<RepeatSkill> RepeatSkills { get; } = new List<RepeatSkill>();
        public List<SequenceSkill> SequenceSkills { get; } = new List<SequenceSkill>();


        public List<RepeatSkill> LearnedRepeatSkills { get; } = new List<RepeatSkill>();
        public List<SequenceSkill> LearnedSequenceSkills { get; } = new List<SequenceSkill>();


        // +++ 최초 한 번 실행 +++
        public void AddRepeatSkill(RepeatSkill repeatSkill) => RepeatSkills.Add(repeatSkill);
        public void AddSequenceSkill(SequenceSkill sequenceSkill) => SequenceSkills.Add(sequenceSkill);

        public SkillBase GetPlayerDefaultSkill(Define.InGameGrade grade)
                => (RepeatSkills[(int)grade - 1].SkillData.IsPlayerDefaultAttack == false) ? null : RepeatSkills[(int)grade - 1];

        public SkillBase GetCurrentPlayerDefaultSkill
                => LearnedRepeatSkills.FirstOrDefault(s => s.SkillData.IsPlayerDefaultAttack == true);

        // +++++ OnRepeatAttackHandler에서 호출하게 됨 +++++
        public void GeneratePlayerAttack(Define.TemplateIDs.SkillType skillType)
        {
            RepeatSkill skill = RepeatSkills.FirstOrDefault(s => s.SkillData.TemplateID == (int)skillType);
            StartCoroutine(CoGeneratePlayerAttack(skill));
        }

        private IEnumerator CoGeneratePlayerAttack(SkillBase skill)
        {
            Data.SkillData skillData = skill.SkillData;

            Vector3 originShootDir = Managers.Game.Player.ShootDir;
            float turningSide = Managers.Game.Player.TurningAngle;
            Vector3 indicatorAngle = Managers.Game.Player.Indicator.eulerAngles;

            Vector3 spawnPos = Vector3.zero;
            if (skillData.IsOnFireSocket)
                spawnPos = Managers.Game.Player.FireSocket;
            else
                spawnPos = Managers.Game.Player.transform.position;

            // 1.25, 1.25, 1.25 to 1, 1, 1
            Vector3 localScale = Managers.Game.Player.AnimationLocalScale;
            localScale *= 0.8f;

            // +++++ 이건 FacingRight떄문에 이대로 해야할것임 +++++
            float[] continuousAngles = new float[skillData.ContinuousAngles.Length];
            if (skillData.ContinuousAngles.Length > 0)
            {
                continuousAngles = new float[skillData.ContinuousAngles.Length];
                if (Managers.Game.Player.IsFacingRight == false)
                {
                    for (int i = 0; i < continuousAngles.Length; ++i)
                    {
                        continuousAngles[i] = skillData.ContinuousAngles[i] * -1;
                    }
                }
                else
                {
                    for (int i = 0; i < continuousAngles.Length; ++i)
                    {
                        continuousAngles[i] = skillData.ContinuousAngles[i];
                    }
                }
            }

            // +++++ Interpolate Scales +++++
            float?[] interPolTargetXs = null;
            float?[] interPolTargetYs = null;
            if (skillData.InterpolateTargetScales.Length > 0)
            {
                interPolTargetXs = new float?[skillData.InterpolateTargetScales.Length];
                interPolTargetYs = new float?[skillData.InterpolateTargetScales.Length];
                for (int i = 0; i < skillData.InterpolateTargetScales.Length; ++i)
                {
                    if (Managers.Game.Player.IsFacingRight == false)
                    {
                        interPolTargetXs[i] = skillData.InterpolateTargetScales[i].x * -1;
                        interPolTargetYs[i] = skillData.InterpolateTargetScales[i].y;
                    }
                    else
                    {
                        interPolTargetXs[i] = skillData.InterpolateTargetScales[i].x;
                        interPolTargetYs[i] = skillData.InterpolateTargetScales[i].y;
                    }
                }
            }
            else
            {
                interPolTargetXs = new float?[skillData.ContinuousCount];
                interPolTargetYs = new float?[skillData.ContinuousCount];
                for (int i = 0; i < skillData.ContinuousCount; ++i)
                {
                    interPolTargetXs[i] = null;
                    interPolTargetYs[i] = null;
                }
            }

            bool?[] isOnHits = null;
            if (skillData.IsOnHits.Length > 0)
            {
                isOnHits = new bool?[skillData.IsOnHits.Length];
                for (int i = 0; i < skillData.IsOnHits.Length; ++i)
                    isOnHits[i] = skillData.IsOnHits[i];
            }
            else
            {
                isOnHits = new bool?[skillData.ContinuousCount];
                for (int i = 0; i < skillData.ContinuousCount; ++i)
                    isOnHits[i] = null;
            }

            for (int i = 0; i < skillData.ContinuousCount; ++i)
            {
                Managers.Game.Player.EndAttackPos = transform.position;
                ProjectileController pc = Managers.Object.Spawn<ProjectileController>(transform.position, skillData.TemplateID);

                Quaternion rot = Quaternion.identity;
                Vector3 shootDir = Vector3.zero;
                if (skillData.IsOnlyFixedRotation == false)
                {
                    rot = Quaternion.Euler(0, 0, continuousAngles[i]);
                    shootDir = rot * originShootDir;
                }
                else
                {
                    // +++++ TEMP +++++
                    //shootDir = Quaternion.Euler(0, 0, 20) * originShootDir;
                    shootDir = originShootDir;
                    float fixAngle = Managers.Game.Player.Indicator.rotation.eulerAngles.z;
                    //pc.transform.rotation = Quaternion.Euler(0, 0, fixAngle + (continuousAngles[i] + 20));
                    pc.transform.rotation = Quaternion.Euler(0, 0, fixAngle + continuousAngles[i]);
                }

                // +++++ TEMP +++++
                pc.SetProjectileInfo(Owner, skill, shootDir, spawnPos, localScale, indicatorAngle, turningSide, 
                    skillData.ContinuousSpeedRatios[i], continuousAngles[i], skillData.ContinuousFlipXs[i], skillData.ContinuousFlipYs[i],
                    skillData.ShootDirectionIntensities[i], isOnHits[i], interPolTargetXs[i], interPolTargetYs[i]);

                yield return new WaitForSeconds(skillData.ContinuousSpacing);
            }
        }

        public void UpgradeRepeatSkill(int originTemplateID)
        {
            if (Managers.Effect.IsPlayingGlitch)
                return;

            if (IsLeanredSkill(originTemplateID) == false)
            {
                RepeatSkill skill = RepeatSkills.FirstOrDefault(s => s.SkillData.OriginTemplateID == originTemplateID);
                LearnedRepeatSkills.Add(skill);
                skill.ActivateSkill();
            }
            else
            {
                // Origin Template ID중에서 배웠던 마지막 스킬을 가져온다. 어차피 하나씩 지울거라.
                RepeatSkill latestSkill = LearnedRepeatSkills.FirstOrDefault(s => s.SkillData.OriginTemplateID == originTemplateID);
                if (latestSkill.SkillData.InGameGrade == Define.InGameGrade.Legendary)
                {
                    Utils.Log(latestSkill.gameObject.name + " is already max skill level !!");
                    return;
                }

                RepeatSkill newSkill = RepeatSkills.FirstOrDefault(s => s.SkillData.TemplateID == originTemplateID + (int)latestSkill.SkillData.InGameGrade);
                LearnedRepeatSkills.Remove(latestSkill);
                latestSkill.DeactivateSkill();
                LearnedRepeatSkills.Add(newSkill);

                if (newSkill.SkillData.IsPlayerDefaultAttack)
                {
                    Managers.Game.Player.AnimEvents.PlayerDefaultSkill++;
                    // Managers.Sprite.UpgradePlayerSprite(newSkill.Owner.GetComponent<PlayerController>(),
                    //     newSkill.SkillData.InGameGrade);


                    // 여기서 실행을 하니까,,,
                    // Managers.Sprite.UpgradePlayerAppearance(newSkill.Owner.GetComponent<PlayerController>(), newSkill.SkillData.InGameGrade);

                    Owner.GetComponent<PlayerController>().UpgradePlayerAppearance(newSkill);
                    return;
                }

                newSkill.ActivateSkill();
            }
        }

        private bool IsLeanredSkill(int originTemplateID)
                => LearnedRepeatSkills.FirstOrDefault(s => s.SkillData.OriginTemplateID == originTemplateID) != null ? true : false;

        // SequenceSkill(하나의 스킬을 끝내야지만 다른 스킬을 사용할 수 있는 스킬) List에 등록된 녀석들을 인공지능에서 따로 판단을 하던
        // 아니면 여기서 순차적으로 등록된 애들을 사용을 하던 여기다가 관리해주면 된다고 함
        private int _sequenceIndex = 0;
        public void StartNextSequenceSkill() // 지금까지 등록된 스킬들을 쭉 실행하세요
        {
            if (Stopped)
                return;
            if (SequenceSkills.Count == 0)
                return;

            SequenceSkills[_sequenceIndex].DoSkill(OnFinishedSequenceSkill);
        }

        private void OnFinishedSequenceSkill()
        {
            // 순차적으로 실행하기 싫다면, 랜덤하게 하건 어떻게 하건 수정하면 됨
            _sequenceIndex = (_sequenceIndex + 1) % SequenceSkills.Count;
            StartNextSequenceSkill();
        }

        public bool Stopped { get; set; } = false;
        public void StopSkills() // 몬스터가 죽으면 중단
        {
            Stopped = true;
            foreach (var skill in RepeatSkills)
            {
                skill.StopAllCoroutines();
            }

            foreach (var skill in SequenceSkills)
            {
                skill.StopAllCoroutines();
            }
        }
    }
}

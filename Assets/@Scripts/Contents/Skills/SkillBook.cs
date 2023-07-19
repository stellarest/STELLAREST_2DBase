using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace STELLAREST_2D
{
    // 일종의 소형 스킬 매니저. QuestBook도 이런식으로 파주면 관리하기 편해짐
    // 이런거 만드는건 "진짜 자유롭게" 만들라고함
    public class SkillBook : MonoBehaviour
    {
        public CreatureController Owner { get; set; }
        public Define.TemplateIDs.SkillType DefaultPlayerSkill { get; set; }


        public List<RepeatSkill> RepeatSkills { get; } = new List<RepeatSkill>();
        public List<SequenceSkill> SequenceSkills { get; } = new List<SequenceSkill>();


        public List<RepeatSkill> LearnedRepeatSkills { get; } = new List<RepeatSkill>();
        public List<SequenceSkill> LearnedSequenceSkills { get; } = new List<SequenceSkill>();


        // +++ 최초 한 번 실행 +++
        public void AddRepeatSkill(RepeatSkill repeatSkill) => RepeatSkills.Add(repeatSkill);
        public void AddSequenceSkill(SequenceSkill sequenceSkill) => SequenceSkills.Add(sequenceSkill);
        // +++

        public Define.InGameGrade RepeatCurrentGrade(Define.TemplateIDs.SkillType skillType)
            => LearnedRepeatSkills.FirstOrDefault(s => s.SkillData.OriginTemplateID == (int)skillType).SkillData.InGameGrade;

        public void PlayerDefaultAttack(Define.TemplateIDs.SkillType skillType)
        {
            RepeatSkill skill = RepeatSkills.FirstOrDefault(s => s.SkillData.TemplateID == (int)skillType);
            StartCoroutine(GeneratePlayerAttack(skill.SkillData));
        }

        private IEnumerator GeneratePlayerAttack(Data.SkillData skillData)
        {
            Vector3 originShootDir = Managers.Game.Player.ShootDir;
            float turningSide = Managers.Game.Player.TurningAngle;
            Vector3 indicatorAngle = Managers.Game.Player.Indicator.eulerAngles;

            Vector3 pos = Managers.Game.Player.transform.position;
            // GameObject go = LearnedRepeatSkills.FirstOrDefault(s => s.SkillData.TemplateID == skillData.TemplateID).gameObject;
            // pos += go.transform.localPosition;

            Vector3 localScale = Managers.Game.Player.AnimationLocalScale;

            if (skillData.ContinuousCount != skillData.ContinuousSpeedRatios.Length)
            {
                for (int i = 0; i < skillData.ContinuousSpeedRatios.Length; ++i)
                    skillData.ContinuousSpeedRatios[i] = 1f;
            }

            if (skillData.ContinuousCount != skillData.ContinuousAngles.Length)
            {
                for (int i = 0; i < skillData.ContinuousAngles.Length; ++i)
                    skillData.ContinuousAngles[i] = 0f;
            }

            float[] angles = new float[skillData.ContinuousAngles.Length];
            if (Managers.Game.Player.IsFacingRight == false)
            {
                for (int i = 0; i < angles.Length; ++i)
                    angles[i] = skillData.ContinuousAngles[i] * -1;
            }
            else
            {
                for (int i = 0; i < angles.Length; ++i)
                    angles[i] = skillData.ContinuousAngles[i];
            }

            for (int i = 0; i < skillData.ContinuousCount; ++i)
            {
                Managers.Game.Player.EndAttackPos = transform.position;
                ProjectileController pc = Managers.Object.Spawn<ProjectileController>(transform.position, skillData.TemplateID);

                Quaternion rot = Quaternion.Euler(0, 0, angles[i]);
                Vector3 shootDir = rot * originShootDir;

                pc.SetSwingInfo(Owner, skillData, shootDir, turningSide, indicatorAngle, pos, localScale,
                                    skillData.ContinuousSpeedRatios[i], angles[i], skillData.ContinuousFlipXs[i]);
                yield return new WaitForSeconds(skillData.ContinuousSpacing);
            }
        }


        public void UpgradeRepeatSkill(int originTemplateID)
        {
            if (IsLeanredSkill(originTemplateID) == false)
            {
                RepeatSkill skill = RepeatSkills.FirstOrDefault(s => s.SkillData.OriginTemplateID == originTemplateID);
                LearnedRepeatSkills.Add(skill);

                skill.ActivateSkill();
            }
            else
            {
                RepeatSkill latestSkill = LearnedRepeatSkills[LearnedRepeatSkills.Count - 1];
                if (latestSkill.SkillData.InGameGrade == Define.InGameGrade.Legendary)
                {
                    Utils.Log(latestSkill.gameObject.name + " is already max skill level !!");
                    return;
                }

                RepeatSkill newSkill = RepeatSkills.FirstOrDefault(s => s.SkillData.TemplateID
                                     == originTemplateID + (int)latestSkill.SkillData.InGameGrade);

                LearnedRepeatSkills.Remove(latestSkill);
                latestSkill.DeactivateSkill();

                LearnedRepeatSkills.Add(newSkill);

                if (newSkill.SkillData.IsPlayerDefaultAttack)
                {
                    Managers.Game.Player.AnimEvents.PlayerDefaultAttack++;
                    Managers.Sprite.UpgradePlayerSprite(newSkill.Owner.GetComponent<PlayerController>(), 
                        newSkill.SkillData.InGameGrade);
                }

                newSkill.ActivateSkill();
            }
        }

        private bool IsLeanredSkill(int originTemplateID)
        {
            if (LearnedRepeatSkills.FirstOrDefault(s => s.SkillData.OriginTemplateID == originTemplateID) != null)
                return true;

            if (LearnedSequenceSkills.FirstOrDefault(s => s.SkillData.OriginTemplateID == originTemplateID) != null)
                return true;

            return false;
        }

        // SequenceSkill(하나의 스킬을 끝내야지만 다른 스킬을 사용할 수 있는 스킬) List에 등록된 녀석들을 인공지능에서 따로 판단을 하던
        // 아니면 여기서 순차적으로 등록된 애들을 사용을 하던 여기다가 관리해주면 된다고 함
        private int _sequenceIndex = 0;
        public void StartNextSequenceSkill() // 지금까지 등록된 스킬들을 쭉 실행하세요
        {
            if (_stopped)
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

        private bool _stopped = false;
        public void StopSkills() // 몬스터가 죽으면 중단
        {
            _stopped = true;
            foreach (var skill in RepeatSkills)
            {
                skill.StopAllCoroutines();
            }
        }
    }
}

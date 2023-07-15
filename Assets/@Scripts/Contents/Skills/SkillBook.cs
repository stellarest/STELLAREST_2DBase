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

        public void AddRepeatSkill(RepeatSkill repeatSkill) => RepeatSkills.Add(repeatSkill);
        public void AddSequenceSkill(SequenceSkill sequenceSkill) => SequenceSkills.Add(sequenceSkill);

        public void PlayerDefaultAttack(Define.TemplateIDs.SkillType skillType)
        {
            RepeatSkill skill = RepeatSkills.FirstOrDefault(s => s.SkillData.TemplateID == (int)skillType);
            Owner.IsAttackStart = false;
            StartCoroutine(GeneratePlayerAttack(skill.SkillData));
            // Managers.Game.Player.EndAttackPos = transform.position;
            // ProjectileController pc = Managers.Object.Spawn<ProjectileController>(transform.position, skill.SkillData.TemplateID);
            // pc.SetInfo(Owner, skill.SkillData, Managers.Game.Player.ShootDir);
        }

        // TODO
        // Skill Upgrade는 이전 스킬 발사가 모두 완료가 된 이후에 적용. (O)
        // 연속 2번 이상 때리는 것은 같은 방향만 적용.
        // 플레이어가 프로젝타일을 쏘고 나서 방향을 틀면 그 방향으로 나가는게 아니라 똑같은 방향으로
         private IEnumerator GeneratePlayerAttack(Data.SkillData skillData)
        {
            // TODO
            float angle = 0f;
            for (int i = 0; i < skillData.ContinuousCount; ++i)
            {
                Managers.Game.Player.EndAttackPos = transform.position;
                ProjectileController pc = Managers.Object.Spawn<ProjectileController>(transform.position, skillData.TemplateID);
                Vector3 shootDir = Quaternion.Euler(0, 0, angle) * Managers.Game.Player.ShootDir;

                //pc.SetInfo(Owner, skillData, Managers.Game.Player.ShootDir);
                pc.SetInfo(Owner, skillData, shootDir);
                pc.SetAngle(angle);

                yield return new WaitForSeconds(skillData.ContinuousSpacing);
            }
        }

        // private IEnumerator GeneratePlayerAttack_Temp(Data.SkillData skillData)
        // {
        //     // TODO
        //     float angle = 0f;
        //     for (int i = 0; i < 3; ++i)
        //     {
        //         Managers.Game.Player.EndAttackPos = transform.position;
        //         ProjectileController pc = Managers.Object.Spawn<ProjectileController>(transform.position, skillData.TemplateID);

        //         Vector3 shootDir = Quaternion.Euler(0, 0, angle) * Managers.Game.Player.ShootDir;

        //         //pc.SetInfo(Owner, skillData, Managers.Game.Player.ShootDir);
        //         pc.SetInfo(Owner, skillData, shootDir);
        //         pc.SetAngle(angle);

        //         if (Managers.Game.Player.IsFacingRight)
        //         {
        //             if (i == 0)
        //                 angle = 45f;
        //             else if (i == 1)
        //                 angle = -45f;
        //         }
        //         else
        //         {
        //             if (i == 0)
        //                 angle = -45f;
        //             else if (i == 1)
        //                 angle = 45f;
        //         }
                
        //         yield return new WaitForSeconds(skillData.ContinuousSpacing);
        //     }
        // }

        public void ActivateRepeatSkill(int templateID)
        {
            RepeatSkill skill = RepeatSkills.FirstOrDefault(s => s.SkillData.TemplateID == templateID);
            if (skill != null)
                skill.ActivateSkill();
            else
                Debug.LogError("### Failed to activate skill !! ###");
        }

        public void UpgradeRepeatSkill(int templateID)
        {
            RepeatSkill skill = RepeatSkills.FirstOrDefault(s => s.SkillData.OriginTemplateID == templateID);
            if (skill != null)
                skill.UpgradeSkill();
            else
                Utils.LogError("Failed UpgradeRepeatSkill");
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

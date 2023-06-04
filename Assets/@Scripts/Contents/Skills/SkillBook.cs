using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    // 일종의 소형 스킬 매니저
    // QuestBook도 이런식으로 파주면 관리하기 편해짐
    // 이런거 만드는건 진짜 자유롭게 만들라고함
    public class SkillBook : MonoBehaviour
    {
        public List<SkillBase> Skills { get; } = new List<SkillBase>();
        public List<SkillBase> RepeatSkills { get; } = new List<SkillBase>();
        // 프리팹 만들까?
        public List<SequenceSkill> SequenceSkills { get; } = new List<SequenceSkill>();

        public T AddSkill<T>(Vector3 position, Transform parent = null) where T : SkillBase
        {
            // 이 부분 나중에 Type이 아닌 TemplateID로 바꾸는게 더 좋다고 함.
            // 데이터 시트에 따라서 프리팹을 한번에 로드하는 방식으로 가능
            // 여튼 이 부분도 깔끔하게 바꿔주는게 좋다고 함. 스폰하는 부분만 if문 넣어주고 알아서 깔끔하게
            // 반복적으로 공통적으로 사용되고 있는 코드가 많아지므로

            // 나중에 스킬을 배워야지만 발동되는 다른 스킬이라면 이부분도 바꿔줘야함. 따로 함수로 빼주던지 식으로
            System.Type type = typeof(T);
            if (type == typeof(EgoSword)) 
            {
                var egoSword = Managers.Object.Spawn<EgoSword>(position, (int)Define.PlayerData.SkillTemplateIDs.EgoSword);
                egoSword.transform.SetParent(parent);
                egoSword.ActivateSkill();

                Skills.Add(egoSword);
                RepeatSkills.Add(egoSword);

                return egoSword as T;
            }
            else if (type == typeof(FireballSkill))
            {
                var fireball = Managers.Object.Spawn<FireballSkill>(position, (int)Define.PlayerData.SkillTemplateIDs.FireBall);
                fireball.transform.SetParent(parent);
                fireball.ActivateSkill();

                Skills.Add(fireball);
                RepeatSkills.Add(fireball);

                return fireball as T;
            }
            else if (type.IsSubclassOf(typeof(SequenceSkill)))
            {
                var skill = gameObject.GetOrAddComponent<T>();
                Skills.Add(skill);
                SequenceSkills.Add(skill as SequenceSkill);

                return skill as T;
            }

            return null;
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

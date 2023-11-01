using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public abstract class SequenceSkill : SkillBase
    {
        public override void Activate()
        {
            base.Activate();

            if (_coSkillActivate != null)
                StopCoroutine(_coSkillActivate);

            IsStopped = false;
            Utils.Log("ACTIVATE SEQUENCE.");
            _coSkillActivate = StartCoroutine((CoStartSkill()));
        }

        protected virtual IEnumerator CoStartSkill() // VIRTUAL
        {
            DoSkillJob(); // ABSTRACT
            yield return null;
            // WaitForSeconds wait = new WaitForSeconds(Data.CoolTime);
            // while (true)
            // {
            //     DoSkillJob();
            //     yield return wait;
            // }
        }

        public abstract void DoSkillJob(System.Action callback = null); // 어차피 무조건 재정의해야되서 재정의하는 곳에서 callback 정의하면 될 듯

        public virtual void OnActiveSequenceSkillHandler() { }
        

        public override void Deactivate()
        {
            if (_coSkillActivate != null)
            {
                Utils.Log("DEACTIVATE SEQUENCE SKILL !!");
                StopCoroutine(_coSkillActivate);
                _coSkillActivate = null;
            }

            base.Deactivate();
        }
    }
}


// private List<SequenceSkill> SequenceSkills = new List<SequenceSkill>();

// private int _sequenceIdx = 0;
// public void StartNextSequenceSkill()
// {
//     if (this.IsStopped)
//         return;

//     if (this.SequenceSkills.Count == 0)
//         return;

//     //SequenceSkills[_sequenceIdx].DoSkillJob(OnFinishedSequenceSkill);
//     SequenceSkills[_sequenceIdx].DoSkillJob(delegate()
//     {
//         _sequenceIdx = (_sequenceIdx + 1) % SequenceSkills.Count;
//         StartNextSequenceSkill();
//     });
// }

// 하나의 스킬을 쓸때, 다른걸 못쓰니까 Action으로 콜백을 받는 방식으로하면 굉장히 편해질수 있음
// private void OnFinishedSequenceSkill()
// {
//     _sequenceIdx = (_sequenceIdx + 1) % SequenceSkills.Count;
//     StartNextSequenceSkill();
// }
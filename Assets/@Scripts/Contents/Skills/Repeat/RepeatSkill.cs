using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    // RepeatSkill 자체로는 스킬 제작 불가능
    // 추상 클래스라 RepeatSkill 상속받아서 제작해야함.
    public abstract class RepeatSkill : SkillBase
    {
        public float CoolTime { get; set; } = 1.0f; // 데이터로 빼야함. 무적권
        public RepeatSkill() : base(Define.GameData.SkillType.Repeat)
        {
        }

        private Coroutine _coSkill;
        public override void ActivateSkill()
        {
            //base.ActivateSkill();
            if (_coSkill != null)
                StopCoroutine(_coSkill);
            _coSkill = StartCoroutine(CoStartSkill());
        }

        protected abstract void DoSkillJob();

        protected virtual IEnumerator CoStartSkill()
        {
            WaitForSeconds wait = new WaitForSeconds(CoolTime);
            while (true)
            {
                // TODO
                DoSkillJob();
                yield return wait;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    // RepeatSkill 자체로는 스킬 제작 불가능
    // 추상 클래스라 RepeatSkill 상속받아서 제작해야함.
    // RepeatSkill은 동시에 2개 이상의 스킬이 발동된다는 특징이 있음
    // Fireball, EgoSword가 동시에 발동됨
    public abstract class RepeatSkill : SkillBase
    {
        private Coroutine _coSkill;
        public override void ActivateSkill()
        {
            base.ActivateSkill();
            if (_coSkill != null)
                StopCoroutine(_coSkill);
            _coSkill = StartCoroutine(CoStartSkill());
        }

        public abstract void OnPreSpawned();
        protected abstract void DoSkillJob();
        protected abstract IEnumerator CoStartSkill();
    }
}

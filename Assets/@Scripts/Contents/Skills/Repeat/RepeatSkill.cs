using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    // RepeatSkill 자체로는 스킬 제작 불가능
    // 추상 클래스라 RepeatSkill 상속받아서 제작해야함.
    // RepeatSkill은 동시에 2개 이상의 스킬이 발동된다는 특징이 있음
    // Fireball, EgoSword가 동시에 발동됨
    public abstract class RepeatSkill : SkillBase
    {
        private Coroutine _coSkillActivate = null;

        // private bool _isLearned = false;
        // public bool IsLearned
        // {
        //     get => _isLearned; 
        //     set
        //     {
        //         _isLearned = value;
        //     }
        // }

        // private bool _isLast = false;
        // public bool IsLast
        // {
        //     get => _isLast;
        //     set
        //     {
        //         _isLast = value;
        //         if (_isLast == false)
        //         {
        //             if (IsStopped)
        //             {
        //                 Utils.Log($"{Data.Name} is already stopped.");
        //                 return;
        //             }

        //             this.Deactivate();
        //             Utils.Log(this.Data.Name + " deactivated !!");
        //         }
        //     }
        // }

        //public Data.SkillData Data => this.SkillData;
        public abstract void InitRepeatSkill(RepeatSkill originRepeatSkill);
        
        // SkillBook에서 Activate 시켜보자.
        public override void Activate()
        {
            base.Activate();
            if (_coSkillActivate != null)
                StopCoroutine(_coSkillActivate);

            IsStopped = false;
            _coSkillActivate = StartCoroutine(CoStartSkill());
        }

        public override void Deactivate()
        {
            if (IsStopped)
            {
                Utils.Log(nameof(RepeatSkill), nameof(Deactivate), Data.Name, "is already stopped.");
                return;
            }

            base.Deactivate();
            if (_coSkillActivate != null)
            {
                StopCoroutine(_coSkillActivate);
                _coSkillActivate = null;
            }

            IsStopped = true;
        }

        protected virtual IEnumerator CoStartSkill()
        {
            WaitForSeconds wait = new WaitForSeconds(Data.CoolTime);
            while (true)
            {
                DoSkillJob();
                yield return wait;
            }
        }

        protected abstract void DoSkillJob();
    }
}

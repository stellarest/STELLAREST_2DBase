using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class HeavensJudgement : UniqueSkill
    {
        protected override IEnumerator CoStartSkill()
        {
            DoSkillJob();
            yield return null;
        }

        protected override void DoSkillJob(Action callback = null)
        {
        }
    }
}

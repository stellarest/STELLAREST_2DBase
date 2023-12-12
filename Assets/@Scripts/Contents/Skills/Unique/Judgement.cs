using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class Judgement : UniqueSkill
    {
        protected override IEnumerator CoStartSkill()
        {
            yield return null;
        }

        protected override void DoSkillJob(Action callback = null)
        {
        }
    }
}

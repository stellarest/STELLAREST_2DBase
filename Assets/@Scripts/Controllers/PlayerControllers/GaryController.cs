using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class GaryController : PlayerController
    {
        public override void Init(int templateID)
        {
            base.Init(templateID);
        }

        protected override void RunSkill() => PlayerAnimController.RunSkill();
    }
}


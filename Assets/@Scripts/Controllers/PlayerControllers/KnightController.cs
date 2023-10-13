using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class KnightController : PlayerController
    {
        public override void Init(int templateID)
        {
            base.Init(templateID);
        }

        protected override void RunSkill()
        {
            //base.RunSkill();
            PlayerAnimController.Attack();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using STELLAREST_2D;
using UnityEngine;

namespace STELLAREST_2D
{
    public class ArrowMasterController : PlayerController
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

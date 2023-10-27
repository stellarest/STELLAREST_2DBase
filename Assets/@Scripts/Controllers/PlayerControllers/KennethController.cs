using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class KennethController : PlayerController
    {
        public override void Init(int templateID)
        {
            base.Init(templateID);
        }

        protected override void RunSkill()
        {
            PlayerAnimController.Attack();
        }
    }
}


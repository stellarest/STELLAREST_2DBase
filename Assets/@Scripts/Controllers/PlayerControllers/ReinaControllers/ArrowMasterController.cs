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

        private void LateUpdate()
        {
            float modifiedAngle = (Indicator.eulerAngles.z + _armBowFixedAngle);
            if (LocalScale.x < 0)
                modifiedAngle = 360f - modifiedAngle;

            BodyParts.ArmLeft.localRotation = Quaternion.Euler(0, 0, modifiedAngle);
        }
        
        protected override void RunSkill()
        {
            PlayerAnimController.Attack();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VFXMuzzle = STELLAREST_2D.Define.TemplateIDs.VFX.Muzzle;

namespace STELLAREST_2D
{
    public class ReinaController : PlayerController
    {
        public override void Init(int templateID) => base.Init(templateID);
        public override void ShowMuzzle() => Managers.VFX.Muzzle(VFXMuzzle.Bow, this);

        [field: SerializeField] public bool LockHandle { get; set; } = false;
        private void LateUpdate()
        {
            if (LockHandle == false)
            {
                float modifiedAngle = (Indicator.eulerAngles.z + _armBowFixedAngle);
                if (LocalScale.x < 0)
                    modifiedAngle = 360f - modifiedAngle;

                BodyParts.ArmLeft.localRotation = Quaternion.Euler(0, 0, modifiedAngle);
            }
        }
        
        protected override void UpdateSkill() => PlayerAnimController.RunSkill();
    }
}

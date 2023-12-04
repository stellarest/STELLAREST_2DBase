using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_2D.Define;

namespace STELLAREST_2D
{
    public class KennethController : PlayerController
    {
        public override void Init(int templateID)
        {
            base.Init(templateID);
        }

        public override void ShowMuzzle()
        {
            if (Utils.IsNinja(this))
                Managers.VFX.Muzzle(VFXMuzzleType.White, this);
        }

        protected override void UpdateSkill() => PlayerAnimController.RunSkill();
    }
}


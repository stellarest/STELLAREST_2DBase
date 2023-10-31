using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VFXMuzzle = STELLAREST_2D.Define.TemplateIDs.VFX.Muzzle;

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
                Managers.VFX.Muzzle(VFXMuzzle.Bow, this);
        }

        protected override void RunSkill() => PlayerAnimController.RunSkill();
    }
}


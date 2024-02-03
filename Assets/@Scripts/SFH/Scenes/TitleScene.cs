using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class TitleScene : BaseScene
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            this.SceneType = EScene.TitleScene;
            return true;
        }

        public override void Clear()
        {
        }
    }

}

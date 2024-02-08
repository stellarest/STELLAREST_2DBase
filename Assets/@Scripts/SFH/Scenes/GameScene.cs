using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class GameScene : BaseScene
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            this.SceneType = EScene.GameScene;
            Util.Log($"ENTERED : {SceneType}");

            // DO SOMETHING,,,
            Managers.UI.ShowBaseUI<UI_Joystick>();
            //Managers.UI.ShowBaseUI<UI_Joystick>(FixedValue.String.UI_JOYSTICK);

            // Util.Log($"Data Total Count : {Managers.Data.TestData_Temp.Count}");
            // foreach (KeyValuePair<int, Data.TestData> pair in Managers.Data.TestData_Temp)
            // {
            //     Util.Log($"LEVEL : {pair.Key}");
            //     Util.Log($"EXP : {pair.Value.Exp}");
            // }

            return true;
        }

        public override void Clear()
        {
        }
    }
}

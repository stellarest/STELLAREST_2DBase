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
            
            // Util.Log("----------------------------------------------------------------------");
            // Util.Log($"{nameof(GameScene)}, {nameof(Init)}, {nameof(Clear)}, hello world");
            // Util.Log($"{nameof(GameScene)}, {nameof(Init)}, {nameof(Clear)}, hello world", true);
            // Util.LogCritical($"{nameof(GameScene)}, {nameof(Init)}, {nameof(Clear)}, Log Critical Test,,,");
            // Util.Log("----------------------------------------------------------------------");

            // DO SOMETHING,,,
            // Managers.UI.ShowBaseUI<UI_Joystick>();
            //Hero hero = Managers.Object.Spawn<Hero>(Vector3.zero, -1);

            return true;
        }

        public override void Clear()
        {
        }
    }
}

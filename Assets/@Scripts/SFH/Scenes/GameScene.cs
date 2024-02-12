using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
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

            HeroCamp camp = Managers.Resource.Instantiate(FixedValue.String.HERO_CAMP).GetOrAddComponent<HeroCamp>();

            CameraController cam = Camera.main.GetOrAddComponent<CameraController>();
            cam.Target = camp;

            UI_Joystick ui = Managers.UI.ShowBaseUI<UI_Joystick>();
            ui.CampDestinationSPR = camp.Destination.GetComponent<SpriteRenderer>();

            {
                Hero hero = Managers.Object.Spawn<Hero>(Vector3.zero, -1);
                hero.transform.SetParent(camp.transform);
            }

            return true;
        }

        public override void Clear()
        {
        }
    }
}

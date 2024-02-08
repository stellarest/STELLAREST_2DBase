using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class UI_TitleScene : UI_Scene
    {
        private enum GameObjects
        {
            StartImage
        }

        private enum Texts
        {
            DisplayText
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            BindObjects(typeof(GameObjects));
            BindTexts(typeof(Texts));
            GetObject((int)GameObjects.StartImage).BindEvent(evt => 
            {
                Util.Log("Change Scene.");
                Managers.Scene.LoadScene(EScene.GameScene);
            }, EUIEvent.Click);

            // GetObject((int)GameObjects.StartImage).BindEvent(delegate(PointerEventData evtData)
            // {
            //     Util.Log("Change Scene.");
            //     Managers.Scene.LoadScene(EScene.GameScene);
            // }, EUIEvent.Click);

            StartLoadAsset();
            GetObject((int)GameObjects.StartImage).SetActive(false);
            GetText((int)Texts.DisplayText).text = $"...";

            return true;
        }

        // Init DataManager
        private void StartLoadAsset()
        {
            Managers.Resource.LoadAllAsync<UnityEngine.Object>(FixedValue.String.PRE_LOAD,
                    delegate (string primaryKey, int loadCount, int totalCount)
            {
                Util.Log($"Loaded Key : {primaryKey}, Loaded Count : {loadCount} / {totalCount}");
                if (loadCount == totalCount)
                {
                    Util.Log("Success load complete.");
                    Managers.Data.Init();

                    GetObject((int)GameObjects.StartImage).SetActive(true);
                    GetText((int)Texts.DisplayText).text = $"SFH_TOUCH TO START";
                }
            });
        }
    }
}

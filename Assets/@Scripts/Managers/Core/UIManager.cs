using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STELLAREST_2D.UI; // UI 스크립트 불러오는중

namespace STELLAREST_2D
{
    public class UIManager
    {
        private UI_Base _fixedSceneUI; // 고정된 UI
        public T GetFixedSceneUI<T>() where T : UI_Base => _fixedSceneUI as T;

        private Stack<UI_Base> _uiPopupStack = new Stack<UI_Base>(); // 가장 마지막으로 켜진 팝업이 끌 때는 가장 먼저 꺼져야함

        public T ShowFixedSceneUI<T>() where T : UI_Base
        {
            if (_fixedSceneUI != null)
                return GetFixedSceneUI<T>();
            
            string key = typeof(T).Name + ".prefab";
            T ui = Managers.Resource.Instantiate(key, pooling: true).GetComponent<T>();
            _fixedSceneUI = ui;

            return ui;
        }


        public T ShowPopup<T>() where T : UI_Base
        {
            string key = typeof(T).Name + ".prefab";
            // 껏키를 하지 않더라도, 코드로 pooling을 사용하면 껏키와 똑같은 효과를 볼 수 있음.
            T ui = Managers.Resource.Instantiate(key, pooling: true).GetOrAddComponent<T>();
            _uiPopupStack.Push(ui);
            RefreshTimeScale();

            return ui;
        }
        public void ClosePopup()
        {
            if (_uiPopupStack.Count == 0)
            {
                Utils.LogCritical(nameof(UIManager), nameof(ClosePopup), "UI stack count is zero.");
                return;
            }

            UI_Base ui = _uiPopupStack.Pop();
            Managers.Resource.Destroy(ui.gameObject); // 어차피 풀링으로 설정되서 알아서 껐다 켰다로 작동함
            RefreshTimeScale();
        }

        public void RefreshTimeScale()
        {
            if (_uiPopupStack.Count > 0)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
    }
}

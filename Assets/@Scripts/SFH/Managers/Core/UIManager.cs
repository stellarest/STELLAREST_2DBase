using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class UIManager
    {
        private int _order = 10;
        private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
        private UI_Scene _sceneUI = null;
        public UI_Scene SceneUI { get => this._sceneUI; set => _sceneUI = value; }
        public T GetSceneUI<T>() where T : UI_Base => _sceneUI as T;

        public GameObject Root
        {
            get
            {
                GameObject root = GameObject.Find(FixedValue.String.UI_ROOT);
                return root != null ? root : new GameObject { name = FixedValue.String.UI_ROOT };
            }
        }

        public void SetCanvas(GameObject go, bool sort = true, int sortOrder = 0)
        {
            Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
            if (canvas != null)
            {
                canvas.renderMode = UnityEngine.RenderMode.ScreenSpaceOverlay;
                canvas.overrideSorting = true; // true로 켜야 캔버스에 배치된 UI의 Sorting Order를 개별적으로 제어할 수 있게됨.
            }

            CanvasScaler scaler = go.GetOrAddComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080, 1920);
            }
            go.GetOrAddComponent<GraphicRaycaster>();
            if (sort)
                canvas.sortingOrder = _order++;
            else
                canvas.sortingOrder = sortOrder;
        }

        public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
        {
            name = (string.IsNullOrEmpty(name)) ? typeof(T).Name : name;
            GameObject go = Managers.Resource.Instantiate(name);
            if (parent != null)
                go.transform.SetParent(parent);

            Canvas canvas = go.GetOrAddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;

            return Util.GetOrAddComponent<T>(go);
        }

        public T ShowBaseUI<T>(string name = null) where T : UI_Base
        {
            name = string.IsNullOrEmpty(name) ? typeof(T).Name : name;
            GameObject go = Managers.Resource.Instantiate(name);

            T baseUI = Util.GetOrAddComponent<T>(go);
            go.transform.SetParent(Root.transform);
            return baseUI;
        }

        public T ShowSceneUI<T>(string name = null) where T : UI_Scene
        {
            name = string.IsNullOrEmpty(name) ? typeof(T).Name : name;
            GameObject go = Managers.Resource.Instantiate(name);

            T sceneUI = Util.GetOrAddComponent<T>(go);
            this._sceneUI = sceneUI;
            go.transform.SetParent(Root.transform);

            return sceneUI;
        }

        public T ShowPopupUI<T>(string name = null) where T : UI_Popup
        {
            name = string.IsNullOrEmpty(name) ? typeof(T).Name : name;
            GameObject go = Managers.Resource.Instantiate(name);

            T popupUI = Util.GetOrAddComponent<T>(go);
            this._popupStack.Push(popupUI);
            go.transform.SetParent(Root.transform);

            return popupUI;
        }

        public void ClosePopupUI()
        {
            if (_popupStack.Count == 0)
                return;

            UI_Popup popup = _popupStack.Pop();
            Managers.Resource.Destroy(popup.gameObject);
            this._order--;
        }

        public void ClosePopupUI(UI_Popup popupUI)
        {
            if (_popupStack.Count == 0)
                return;

            if (_popupStack.Peek() != popupUI)
            {
                Util.Log($"{nameof(ClosePopupUI)}, Failed to close Popup UI", true);
                return;
            }

            ClosePopupUI();
        }

        public void CloseAllPopup()
        {
            while (_popupStack.Count > 0)
                ClosePopupUI();
        }

        public int GetPopupCount => _popupStack.Count;

        public void Clear()
        {
            CloseAllPopup();
            _sceneUI = null;
        }
    }

}

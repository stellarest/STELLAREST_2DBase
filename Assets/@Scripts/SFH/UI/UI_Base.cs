using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class UI_Base : InitBase
    {
        protected Dictionary<System.Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

        protected void Bind<T>(System.Type enumType) where T : UnityEngine.Object
        {
            // ex) StartImage
            string[] names = System.Enum.GetNames(enumType);
            UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
            _objects.Add(typeof(T), objects);

            for (int i = 0; i < names.Length; ++i)
            {
                if (typeof(T) == typeof(UnityEngine.GameObject))
                    objects[i] = Util.FindChild(gameObject, names[i], true);
                else
                    objects[i] = Util.FindChild<T>(gameObject, names[i], true);
                    
#if UNITY_EDITOR
                if (objects[i] == null)
                    Util.LogCritical(nameof(Bind), $"Failed to bind : {names[i]}");
#endif
            }
        }

        protected void BindObjects(System.Type enumType) => this.Bind<UnityEngine.GameObject>(enumType);
        protected void BindImages(System.Type enumType) => this.Bind<UnityEngine.UI.Image>(enumType);
        protected void BindTexts(System.Type enumType) => this.Bind<TMPro.TMP_Text>(enumType);
        protected void BindButtons(System.Type enumType) => this.Bind<UnityEngine.UI.Button>(enumType);
        protected void BindToggles(System.Type enumType) => this.Bind<UnityEngine.UI.Toggle>(enumType);

        private T Get<T>(int idx) where T : UnityEngine.Object
        {
            if (_objects.TryGetValue(typeof(T), out UnityEngine.Object[] objects) == false)
                return null;

            return objects[idx] as T;
        }

        protected GameObject GetObject(int idx) => this.Get<GameObject>(idx);
        protected TMPro.TMP_Text GetText(int idx) => this.Get<TMPro.TMP_Text>(idx);
        protected UnityEngine.UI.Button GetButton(int idx) => this.Get<UnityEngine.UI.Button>(idx);
        protected UnityEngine.UI.Image GetImage(int idx) => this.Get<UnityEngine.UI.Image>(idx);

        // EXTENSION
        public static void BindEvent(GameObject go, Action<PointerEventData> action = null, EUIEvent evtType = EUIEvent.Click)
        {
            UI_EventHandler evtHandler = Util.GetOrAddComponent<UI_EventHandler>(go);
            switch (evtType)
            {
                case EUIEvent.Click:
                    evtHandler.OnClickHandler -= action;
                    evtHandler.OnClickHandler += action;
                    break;

                case EUIEvent.PointerDown:
                    evtHandler.OnPointerDownHandler -= action;
                    evtHandler.OnPointerDownHandler += action;
                    break;

                case EUIEvent.PointerUp:
                    evtHandler.OnPointerUpHandler -= action;
                    evtHandler.OnPointerUpHandler += action;
                    break;

                case EUIEvent.Drag:
                    evtHandler.OnDragHandler -= action;
                    evtHandler.OnDragHandler += action;
                    break;
            }
        }
    }
}


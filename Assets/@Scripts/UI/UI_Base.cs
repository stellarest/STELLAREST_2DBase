using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace STELLAREST_2D.UI
{
    public class UI_Base : MonoBehaviour
    {
        protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();
        protected bool _init = false;

        public virtual bool Init()
        {
            if (_init)
                return false;

            _init = true;
            return true;
        }

        private void Start()
        {
            Init();
        }

        protected void Bind<T>(Type type) where T : UnityEngine.Object
        {
            string[] names = Enum.GetNames(type);
            if (names.Length == 0)
            {
                Debug.LogError("@@@ Something is wrong !! names length is zero. @@@");
                return;
            }

            UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
            _objects.Add(typeof(T), objects);

            for (int i = 0; i < names.Length; ++i)
            {
                if (typeof(T) == typeof(UnityEngine.GameObject))
                    objects[i] = Utils.FindChild(gameObject, names[i], true);
                else
                    objects[i] = Utils.FindChild<T>(gameObject, names[i], true);

                if (objects[i] == null)
                    Debug.LogWarning($"@@@ Failed to ui bind : {names[i]}");
            }
        }

        protected void BindGameObject(Type type) => this.Bind<GameObject>(type);
        protected void BindImage(Type type) => this.Bind<Image>(type);
        protected void BindText(Type type) => this.Bind<TMP_Text>(type); // TMP_Text : TextMeshPro, TextMeshProUGUI의 Base Class
        protected void BindButton(Type type) => this.Bind<Button>(type);
        protected void BindToggle(Type type) => this.Bind<Toggle>(type);

        protected T Get<T>(int idx) where T : UnityEngine.Object
        {
            UnityEngine.Object[] objects = null;
            if (_objects.TryGetValue(typeof(T), out objects) == false)
                return null;

            return objects[idx] as T;
        }

        protected GameObject GetGameObject(int idx) => Get<GameObject>(idx);
        protected Image GetImage(int idx) => Get<Image>(idx);
        protected TMP_Text GetText(int idx) => Get<TMP_Text>(idx);
        protected Button GetButton(int idx) => Get<Button>(idx);
        protected Toggle GetToggle(int idx) => Get<Toggle>(idx);

        public static void BindEvent(GameObject go, Action action = null, Action<UnityEngine.EventSystems.BaseEventData> dragAction = null, Define.UIEvent evtType = Define.UIEvent.Click)
        {
            UI_EventHandler evt = go.GetOrAddComponent<UI_EventHandler>();
            switch (evtType)
            {
                case Define.UIEvent.Click:
                    {
                        evt.OnClickHandler -= action;
                        evt.OnClickHandler += action;
                    }
                    break;

                case Define.UIEvent.Pressed:
                    {
                        evt.OnPressedHandler -= action;
                        evt.OnPressedHandler += action;
                    }
                    break;

                case Define.UIEvent.PointerDown:
                    {
                        evt.OnPointerDownHandler -= action;
                        evt.OnPointerDownHandler += action;
                    }
                    break;

                case Define.UIEvent.PointerUp:
                    {
                        evt.OnPointerUpHandler -= action;
                        evt.OnPointerUpHandler += action;
                    }
                    break;

                case Define.UIEvent.BeginDrag:
                    {
                        evt.OnBeginDragHandler -= dragAction;
                        evt.OnBeginDragHandler += dragAction;
                    }
                    break;

                case Define.UIEvent.Drag:
                    {
                        evt.OnDragHandler -= dragAction;
                        evt.OnDragHandler += dragAction;
                    }
                    break;

                case Define.UIEvent.EndDrag:
                    {
                        evt.OnEndDragHandler -= dragAction;
                        evt.OnEndDragHandler += dragAction;
                    }
                    break;
            }
        }
    }
}

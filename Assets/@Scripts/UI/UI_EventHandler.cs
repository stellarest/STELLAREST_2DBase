using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace STELLAREST_2D.UI
{
    public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Action OnClickHandler;
        public Action OnPressedHandler;
        public Action OnPointerDownHandler;
        public Action OnPointerUpHandler;

        public Action<BaseEventData> OnBeginDragHandler;
        public Action<BaseEventData> OnDragHandler;
        public Action<BaseEventData> OnEndDragHandler;

        private bool _pressed = false;
        private void Update()
        {
            if (_pressed)
                OnPressedHandler?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (OnClickHandler != null)
                OnClickHandler?.Invoke();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _pressed = true;
            OnPointerDownHandler?.Invoke();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            _pressed = false;
            OnPointerUpHandler?.Invoke();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnBeginDragHandler?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _pressed = true;
            OnDragHandler?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnEndDragHandler?.Invoke(eventData);
        }
    }
}

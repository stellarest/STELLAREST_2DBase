using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class UI_Joystick : UI_Base
    {
        private enum GameObjects
        {
            JoystickBG,
            BG_FocusLT,
            BG_FocusRT,
            BG_FocusLB,
            BG_FocusRB,
            JoystickCursor
        }

        private GameObject _background = null;
        private GameObject _backgroundLT = null;
        private GameObject _backgroundRT = null;
        private GameObject _backgroundLB = null;
        private GameObject _backgroundRB = null;
        private GameObject _cursor = null;

        private float _radius = 0f;
        private Vector2 _touchPos = Vector2.zero;

        public SpriteRenderer CampDestinationSPR { get; set; } = null;

        [SerializeField] private Vector2 _moveDir = Vector2.zero;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            BindObjects(typeof(GameObjects));
            _background = GetObject((int)GameObjects.JoystickBG);
            _backgroundLT = GetObject((int)GameObjects.BG_FocusLT);
            _backgroundRT = GetObject((int)GameObjects.BG_FocusRT);
            _backgroundLB = GetObject((int)GameObjects.BG_FocusLB);
            _backgroundRB = GetObject((int)GameObjects.BG_FocusRB);
            _cursor = GetObject((int)GameObjects.JoystickCursor);
            _radius = _background.GetComponent<RectTransform>().sizeDelta.x / 2;

            gameObject.BindEvent(OnPointerDown, evtType: EUIEvent.PointerDown);
            gameObject.BindEvent(OnPointerUp, evtType: EUIEvent.PointerUp);
            gameObject.BindEvent(OnDrag, evtType: EUIEvent.Drag);
            return true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ShowJoystick(true);
            GlowFocus(Vector2.zero);

            _background.transform.position = eventData.position;
            _cursor.transform.position = eventData.position;
            _touchPos = eventData.position;

            _moveDir = Vector2.zero;
            Managers.Game.MoveDir = Vector2.zero;
            Managers.Game.JoystickState = EJoystickState.PointerDown;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _cursor.transform.position = _touchPos;
            _moveDir = Vector2.zero;

            GlowFocus(_moveDir);
            ShowJoystick(false);

            Managers.Game.MoveDir = Vector2.zero;
            Managers.Game.JoystickState = EJoystickState.PointerUp;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 touchDir = (eventData.position - _touchPos);
            _moveDir = touchDir.normalized;

            float maxDist = _radius * GetViewportScale();
            float moveDist = Mathf.Min(touchDir.magnitude, maxDist);
            //GlowFocus(_moveDir, moveDist, maxDist);
            GlowFocus(_moveDir);

            Vector2 newPos = _touchPos + (_moveDir * moveDist);
            _cursor.transform.position = newPos;

            Managers.Game.MoveDir = _moveDir;
            Managers.Game.JoystickState = EJoystickState.Drag;
        }

        private float GetViewportScale() => Screen.height / 1080;

        private void ShowJoystick(bool isOn)
        {
            _background.SetActive(isOn);
            _cursor.SetActive(isOn);
        }

        private void GlowFocus(Vector2 moveDir)
        {
            if (moveDir == Vector2.zero)
            {
                _backgroundLT.SetActive(false);
                _backgroundRT.SetActive(false);
                _backgroundLB.SetActive(false);
                _backgroundRB.SetActive(false);
                ShowDestination(false);
                return;
            }

            ShowDestination(true);
            float x = UnityEngine.Mathf.Sign(moveDir.x);
            float y = UnityEngine.Mathf.Sign(moveDir.y);
            if (x == -1f)
            {
                if (y == 1f)
                {
                    _backgroundLT.SetActive(true);
                    _backgroundRT.SetActive(false);
                    _backgroundLB.SetActive(false);
                    _backgroundRB.SetActive(false);
                }
                else if (moveDir.y >= FixedValue.Numeric.GLOW_BG_FOCUS_MIN &&
                        moveDir.y <= FixedValue.Numeric.GLOW_BG_FOCUS_MAX)
                {
                    _backgroundLT.SetActive(true);
                    _backgroundRT.SetActive(false);
                    _backgroundLB.SetActive(true);
                    _backgroundRB.SetActive(false);
                }
                else if (y == -1f)
                {
                    _backgroundLT.SetActive(false);
                    _backgroundRT.SetActive(false);
                    _backgroundLB.SetActive(true);
                    _backgroundRB.SetActive(false);
                }

            }
            else if (moveDir.x >= FixedValue.Numeric.GLOW_BG_FOCUS_MIN &&
                    moveDir.x <= FixedValue.Numeric.GLOW_BG_FOCUS_MAX)
            {
                if (y == 1f)
                {
                    _backgroundLT.SetActive(true);
                    _backgroundRT.SetActive(true);
                    _backgroundLB.SetActive(false);
                    _backgroundRB.SetActive(false);
                }
                else if (y == -1f)
                {
                    _backgroundLT.SetActive(false);
                    _backgroundRT.SetActive(false);
                    _backgroundLB.SetActive(true);
                    _backgroundRB.SetActive(true);
                }
            }
            else if (x == 1f)
            {
                if (y == 1f)
                {
                    _backgroundLT.SetActive(false);
                    _backgroundRT.SetActive(true);
                    _backgroundLB.SetActive(false);
                    _backgroundRB.SetActive(false);
                }
                else if (moveDir.y >= FixedValue.Numeric.GLOW_BG_FOCUS_MIN &&
                        moveDir.y <= FixedValue.Numeric.GLOW_BG_FOCUS_MAX)
                {
                    _backgroundLT.SetActive(false);
                    _backgroundRT.SetActive(true);
                    _backgroundLB.SetActive(false);
                    _backgroundRB.SetActive(true);
                }
                else if (y == -1f)
                {
                    _backgroundLT.SetActive(false);
                    _backgroundRT.SetActive(false);
                    _backgroundLB.SetActive(false);
                    _backgroundRB.SetActive(true);
                }
            }
        }

        private void ShowDestination(bool show)
            => CampDestinationSPR.enabled = show;

        // PREV VER
        // private void GlowFocus(Vector2 moveDir, float? currentDist = null, float? maxDist = null)
        // {
        //     if (moveDir == Vector2.zero)
        //     {
        //         _backgroundLT.SetActive(false);
        //         _backgroundRT.SetActive(false);
        //         _backgroundLB.SetActive(false);
        //         _backgroundRB.SetActive(false);
        //         return; 
        //     }

        //     if (currentDist.HasValue && maxDist.HasValue)
        //     {
        //         if (currentDist.Value < (maxDist.Value - Mathf.Epsilon))
        //         {
        //             _backgroundLT.SetActive(false);
        //             _backgroundRT.SetActive(false);
        //             _backgroundLB.SetActive(false);
        //             _backgroundRB.SetActive(false);
        //             return;
        //         }
        //     }

        //     float x = UnityEngine.Mathf.Sign(moveDir.x);
        //     float y = UnityEngine.Mathf.Sign(moveDir.y);
        //     if (x == -1f)
        //     {
        //         if (y == 1f)
        //         {
        //             _backgroundLT.SetActive(true);
        //             _backgroundRT.SetActive(false);
        //             _backgroundLB.SetActive(false);
        //             _backgroundRB.SetActive(false);
        //         }
        //         else if (moveDir.y >= FixedValue.Numeric.GLOW_BG_FOCUS_MIN &&
        //                 moveDir.y <= FixedValue.Numeric.GLOW_BG_FOCUS_MAX)
        //         {
        //             _backgroundLT.SetActive(true);
        //             _backgroundRT.SetActive(false);
        //             _backgroundLB.SetActive(true);
        //             _backgroundRB.SetActive(false);
        //         }
        //         else if (y == -1f)
        //         {
        //             _backgroundLT.SetActive(false);
        //             _backgroundRT.SetActive(false);
        //             _backgroundLB.SetActive(true);
        //             _backgroundRB.SetActive(false);
        //         }

        //     }
        //     else if (moveDir.x >= FixedValue.Numeric.GLOW_BG_FOCUS_MIN &&
        //             moveDir.x <= FixedValue.Numeric.GLOW_BG_FOCUS_MAX)
        //     {
        //         if (y == 1f)
        //         {
        //             _backgroundLT.SetActive(true);
        //             _backgroundRT.SetActive(true);
        //             _backgroundLB.SetActive(false);
        //             _backgroundRB.SetActive(false);
        //         }
        //         else if (y == -1f)
        //         {
        //             _backgroundLT.SetActive(false);
        //             _backgroundRT.SetActive(false);
        //             _backgroundLB.SetActive(true);
        //             _backgroundRB.SetActive(true);
        //         }
        //     }
        //     else if (x == 1f)
        //     {
        //         if (y == 1f)
        //         {
        //             _backgroundLT.SetActive(false);
        //             _backgroundRT.SetActive(true);
        //             _backgroundLB.SetActive(false);
        //             _backgroundRB.SetActive(false);
        //         }
        //         else if (moveDir.y >= FixedValue.Numeric.GLOW_BG_FOCUS_MIN &&
        //                 moveDir.y <= FixedValue.Numeric.GLOW_BG_FOCUS_MAX)
        //         {
        //             _backgroundLT.SetActive(false);
        //             _backgroundRT.SetActive(true);
        //             _backgroundLB.SetActive(false);
        //             _backgroundRB.SetActive(true);
        //         }
        //         else if (y == -1f)
        //         {
        //             _backgroundLT.SetActive(false);
        //             _backgroundRT.SetActive(false);
        //             _backgroundLB.SetActive(false);
        //             _backgroundRB.SetActive(true);
        //         }
        //     }
        // }
    }
}

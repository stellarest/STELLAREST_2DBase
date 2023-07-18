using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace STELLAREST_2D // UI가 맞긴한데 조금 애매하니까 일단 .UI를 빼자
{
    public class UI_Joystick : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private Image _background;
        [SerializeField] private Image _handler;

        public enum FocusLine { LT, RT, LB, RB, Max }
        [SerializeField] private GameObject[] _focusLine;

        private Vector2 _touchPosition;
        private Vector2 _moveDir;
        private float _joystickRadius;

        private const float FOCUS_MIN = -0.15f;
        private const float FOCUS_MAX = 0.15f;

        private void Start()
        {
            //_joystickRadius = (_background.GetComponent<RectTransform>().sizeDelta / 2).x;
            _joystickRadius = 155f;

            ActiveJoystick(false);
        }

        public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
        }

        public void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            ActiveJoystick(true);

            // ActiveJoystick(true);
            _background.transform.position = eventData.position;
            _handler.transform.position = eventData.position;
            _touchPosition = eventData.position;
        }

        public void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
        {
            _handler.transform.position = _touchPosition;
            _moveDir = Vector2.zero;
            ActiveFocusLine(_moveDir);

            Managers.Game.MoveDir = _moveDir;
            ActiveJoystick(false);
        }

        public void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Vector2 touchDir = eventData.position - _touchPosition;
            float magnitude = Mathf.Sqrt((touchDir.x * touchDir.x) + (touchDir.y * touchDir.y));
            float moveDist = Mathf.Min(magnitude, _joystickRadius);
            _moveDir = touchDir.normalized;
            ActiveFocusLine(_moveDir);

            // 1.피타고라스로 AB 벡터의 크기를 구함
            // 2.AB벡터를 AB벡터의 크기로 나눈 벡터가 normalized 벡터임.
            // 3. normalized 벡터를 피타고라스로 검증해보면 실제로 크기가 1인 벡터임.
            // _moveDir = (touchDir / magnitude); // normalized vector 구하기
            Vector2 newHandlerPosition = _touchPosition + (_moveDir * moveDist);
            _handler.transform.position = newHandlerPosition;

            Managers.Game.MoveDir = _moveDir;
        }

        private void ActiveJoystick(bool isActive)
        {
            _background.gameObject.SetActive(isActive);
            _handler.gameObject.SetActive(isActive);
        }

        private void ActiveFocusLine(Vector3 dir)
        {
            if (dir == Vector3.zero)
            {
                ActiveFocusLine(false, FocusLine.LT, FocusLine.RT, FocusLine.LB, FocusLine.RB);
                return;
            }

            float x = Mathf.Sign(dir.x);
            float y = Mathf.Sign(dir.y);
            if (x == -1)
            {
                // y위 -> 중간 -> 아래
                if (y == 1)
                {
                    ActiveFocusLine(true, FocusLine.LT);
                    ActiveFocusLine(false, FocusLine.RT, FocusLine.LB, FocusLine.RB);
                }
                else if (dir.y >= FOCUS_MIN && dir.y <= FOCUS_MAX)
                {
                    ActiveFocusLine(true, FocusLine.LT, FocusLine.LB);
                    ActiveFocusLine(false, FocusLine.RT, FocusLine.RB);
                }
                else if (y == -1)
                {
                    ActiveFocusLine(true, FocusLine.LB);
                    ActiveFocusLine(false, FocusLine.LT, FocusLine.RT, FocusLine.RB);
                }
            }
            else if (dir.x >= FOCUS_MIN && dir.x <= FOCUS_MAX)
            {
                if (y == 1)
                {
                    ActiveFocusLine(true, FocusLine.LT, FocusLine.RT);
                    ActiveFocusLine(false, FocusLine.LB, FocusLine.RB);
                }
                else if (y == -1)
                {
                    ActiveFocusLine(true, FocusLine.LB, FocusLine.RB);
                    ActiveFocusLine(false, FocusLine.LT, FocusLine.RT);
                }
            }
            else if (x == 1)
            {
                if (y == 1)
                {
                    ActiveFocusLine(true, FocusLine.RT);
                    ActiveFocusLine(false, FocusLine.LT, FocusLine.LB, FocusLine.RB);
                }
                else if (dir.y >= FOCUS_MIN && dir.y <= FOCUS_MAX)
                {
                    ActiveFocusLine(true, FocusLine.RT, FocusLine.RB);
                    ActiveFocusLine(false, FocusLine.LT, FocusLine.LB);
                }
                else if(y == -1)
                {
                    ActiveFocusLine(true, FocusLine.RB);
                    ActiveFocusLine(false, FocusLine.LT, FocusLine.RT, FocusLine.LB);
                }
            }
        }

        private void ActiveFocusLine(bool active, params FocusLine[] focusLine)
        {
            for (int i = 0; i < focusLine.Length; ++i)
            {
                int index = (int)focusLine[i];
                _focusLine[index].SetActive(active);
            }
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace STELLAREST_2D
{
    public class UI_Joystick : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private Image _background;
        [SerializeField] private Image _handler;
        private Vector2 _touchPosition;
        private Vector2 _moveDir;
        private float _joystickRadius;

        public PlayerController PlayerController { get; set; }

        private void Start()
        {
            _joystickRadius = (_background.GetComponent<RectTransform>().sizeDelta / 2).x;
            ActiveJoystick(false);
        }

        public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
        }

        public void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            ActiveJoystick(true);
            _background.transform.position = eventData.position;
            _handler.transform.position = eventData.position;
            _touchPosition = eventData.position;
        }

        public void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
        {
            _handler.transform.position = _touchPosition;
            _moveDir = Vector2.zero;

            PlayerController.MoveDir = _moveDir;
            ActiveJoystick(false);
        }

        public void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Vector2 touchDir = eventData.position - _touchPosition;
            float magnitude = Mathf.Sqrt((touchDir.x * touchDir.x) + (touchDir.y * touchDir.y));
            float moveDist = Mathf.Min(magnitude, _joystickRadius);
            
            _moveDir = touchDir.normalized;
            // _moveDir = (touchDir / magnitude); // normalized vector 구하기

            Vector2 newHandlerPosition = _touchPosition + (_moveDir * moveDist);
            _handler.transform.position = newHandlerPosition;

            PlayerController.MoveDir = touchDir;
        }

        private void ActiveJoystick(bool isActive)
        {
            _background.gameObject.SetActive(isActive);
            _handler.gameObject.SetActive(isActive);
        }
    }
}
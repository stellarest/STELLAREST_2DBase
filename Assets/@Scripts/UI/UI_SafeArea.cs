using UnityEngine;

namespace STELLAREST_2D.UI
{
    public class UI_SafeArea : MonoBehaviour
    {
        private RectTransform _rectTr;
        private UnityEngine.Rect _safeArea;
        private Vector2 _minAnchor;
        private Vector2 _maxAnchor;

        private void Awake()
        {
            _rectTr = GetComponent<RectTransform>();
            _safeArea = UnityEngine.Screen.safeArea;
            _minAnchor = _safeArea.position;
            _maxAnchor = _safeArea.position + _safeArea.size;

            _minAnchor.x = _minAnchor.x / Screen.width;
            _minAnchor.y = _maxAnchor.y / Screen.height;
            _maxAnchor.x = _maxAnchor.x / Screen.width;
            _maxAnchor.y = _maxAnchor.y / Screen.height;

            _rectTr.anchorMin = _minAnchor;
            _rectTr.anchorMax = _maxAnchor;
        }
    }
}

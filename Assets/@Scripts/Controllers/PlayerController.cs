using UnityEngine;

namespace STELLAREST_2D
{
    public class PlayerController : MonoBehaviour
    {
        private Vector2 _moveDir = Vector2.zero;
        public Vector2 MoveDir
        {
            get => _moveDir;
            set { _moveDir = value.normalized; }
        }

        private float _speed = 5f;

        private void Start()
        {
            Managers.GameManager.OnMoveChanged += OnMoveChangedHandler;
        }

        private void Update()
        {
            MovePlayerByController();
        }

        private void OnDestroy()
        {
            if (Managers.GameManager != null)
                Managers.GameManager.OnMoveChanged -= OnMoveChangedHandler;
        }

        public void MovePlayerByController()
        {
            Vector3 dir = _moveDir.normalized * _speed * Time.deltaTime;
            transform.position += dir;
        }

        private void OnMoveChangedHandler(Vector2 moveDir)
        {
            this._moveDir = moveDir;
        }
    }
}

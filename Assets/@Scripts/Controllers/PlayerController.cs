using UnityEngine;

namespace STELLAREST_2D
{
    public class PlayerController : MonoBehaviour
    {
        private Vector2 _moveDir = Vector2.zero;
        public Vector2 MoveDir
        {
            get => _moveDir;
            set { _moveDir = value; }
        }

        private float _speed = 5f;

        private void Update()
        {
            //UpdateInput();
            //MovePlayerByKeyboard();
            
            MovePlayerByController();
        }

        // Device Simulator에서는 먹통일수도 있어서 나중에 UI로 교체해서 사용해야함.
        private void UpdateInput()
        {
            Vector2 moveDir = Vector2.zero;

            if (Input.GetKey(KeyCode.W))
                moveDir.y += 1f;
            if (Input.GetKey(KeyCode.S))
                moveDir.y -= 1f;
            if (Input.GetKey(KeyCode.A))
                moveDir.x -= 1f;
            if (Input.GetKey(KeyCode.D))
                moveDir.x += 1f;
            
            this._moveDir = moveDir.normalized;
            // Debug.Log("<color=yellow>" + this._moveDir.magnitude + "</color>"); // 대각선 이동까지 해결
        }

        private void MovePlayerByKeyboard()
        {
            Vector3 dir = _moveDir * _speed * Time.deltaTime;
            transform.position += dir;
        }

        public void MovePlayerByController()
        {
            Vector3 dir = _moveDir.normalized * _speed * Time.deltaTime;
            transform.position += dir;
        }
    }
}

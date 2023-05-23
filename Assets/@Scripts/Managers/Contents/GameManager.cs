using System;
using UnityEngine;

namespace STELLAREST_2D
{
    public class GameManager
    {
        private Vector2 _moveDir;
        public Action<Vector2> OnMoveChanged;

        public Vector2 MoveDir
        {
            get => _moveDir;
            set
            {
                _moveDir = value;
                OnMoveChanged?.Invoke(_moveDir);
            }
        }
    }
}

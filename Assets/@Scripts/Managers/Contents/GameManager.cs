using System;
using UnityEngine;

namespace STELLAREST_2D
{
    public class GameManager
    {
        public PlayerController Player { get => Managers.Object?.Player; }
        public int Gold { get; set; }
        public int Gem { get; set; }

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

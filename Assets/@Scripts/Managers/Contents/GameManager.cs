using System;
using UnityEngine;

namespace STELLAREST_2D
{
    public class GameManager
    {
        public PlayerController Player { get => Managers.Object?.Player; }
        public int Gold { get; set; }

        private int _gem = 0;
        public event Action<int> OnGemCountChanged;
        public int Gem 
        { 
            get => _gem; 
            set
            {
                _gem = value;
                OnGemCountChanged?.Invoke(_gem);
            }
        }

        private Vector2 _moveDir;
        public Vector2 MoveDir
        {
            get => _moveDir;
            set
            {
                _moveDir = value;
                OnMoveDirChanged?.Invoke(_moveDir);
            }
        }
        public event Action<Vector2> OnMoveDirChanged;

        private int _killCount;
        public event Action<int> OnKillCountChanged;
        public int KillCount
        {
            get => _killCount;
            set
            {
                _killCount = value;
                OnKillCountChanged?.Invoke(KillCount);
            }
        }
    }
}

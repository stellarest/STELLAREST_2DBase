using System;
using UnityEngine;

namespace STELLAREST_2D
{
    public class GameManager
    {
        public void GAME_START() => IsGameStart = true;
        private bool _isGameStart = false;
        public bool IsGameStart 
        { 
            get => _isGameStart;
            private set
            {
                _isGameStart = value;
                if (_isGameStart)
                    IsGameOver = false;
            }
        }

        public void GAME_OVER() => IsGameOver = true;
        private bool _isGameOver = false;
        public bool IsGameOver
        {
            get => _isGameOver;
            private set
            {
                _isGameOver = value;
                if (_isGameOver)
                    IsGameStart = false;
            }
        }

        public PlayerController Player { get => Managers.Object?.Player; }
        public int Gold { get; set; }
        
        public float TakeDamagee()
        {
            return 0f;
        }

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

        private Vector3 _moveDir = Vector3.zero;
        public event Action<Vector3> OnMoveDirChanged = null;
        public Vector3 MoveDir
        {
            get => _moveDir;
            set
            {   
                _moveDir = value;
                OnMoveDirChanged?.Invoke(_moveDir);
            }
        }

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

        public bool IsCreature(CreatureController creture, int templateID)
        {
            return false;
        }

        private bool IsAssassin(int templateID)
            => templateID == 200124;

        private bool IsThief(int templateID)
            => templateID == 200128;

        public bool IsReina(int templateID)
            => templateID == 100103 || templateID == 100104 || templateID == 100105 ? true : false;

        private bool IsChristian(int templateID)
            => templateID == 100112 || templateID == 100113 || templateID == 100114 ? true : false;
    }
}

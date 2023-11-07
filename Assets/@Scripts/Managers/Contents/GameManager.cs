using System;
using UnityEngine;

namespace STELLAREST_2D
{
    public class GameManager
    {
        public System.Action OnGameStart = null;
        private bool _isGameStart = false;
        public void GAME_START() => IsGameStart = true;
        public bool IsGameStart 
        { 
            get => _isGameStart;
            private set
            {
                _isGameStart = value;
                if (_isGameStart)
                {
                    Utils.Log("##### <color=cyan> GAME START </color> #####");
                    OnGameStart?.Invoke();
                    IsGameOver = false;
                }
            }
        }

        private bool _isGameOver = false;
        public void GAME_OVER() => IsGameOver = true;
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
        public System.Action OnPlayerIsDead = null;

        private const float MIN_CRITICAL_RATIO = 1.5f;
        private const float MAX_CRITICAL_RATIO = 2f;
        public (float dmgResult, bool isCritical) TakeDamage(CreatureController target, CreatureController attacker, SkillBase from)
        {
            // DODGE CHANCE OF TARGET
            if (UnityEngine.Random.Range(0f, 1f) <= target.Stat.Dodge)
                return (-1f, false);
            
            // DAMAGE FROM ATTACKER
            float armor = target.Stat.Armor;
            bool isCritical = false;
            float dmgSkill = UnityEngine.Random.Range(from.Data.MinDamage, from.Data.MaxDamage);
            float dmgResult = dmgSkill + (dmgSkill * attacker.Stat.Damage);
            if (UnityEngine.Random.Range(0f, 1f) <= attacker.Stat.Critical)
            {
                isCritical = true;
                float criticalRatio = UnityEngine.Random.Range(MIN_CRITICAL_RATIO, MAX_CRITICAL_RATIO);
                dmgResult = dmgResult + (dmgResult * criticalRatio);
            }

            dmgResult = dmgResult - (dmgResult * armor);
            return (dmgResult, isCritical);
        }

        public bool TryCrowdControl(SkillBase from) => UnityEngine.Random.Range(0f, 1f) <= from.Data.CrowdControlRatio;
        public bool TryCrowdControl(float ratio) => UnityEngine.Random.Range(0f, 1f) <= ratio;

        private int _gem = 0;
        public event Action<int> OnGemCountChanged = null;
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

using UnityEngine;

namespace STELLAREST_2D
{
    public class CreatureController : BaseController
    {
        [SerializeField] private int _hp;
        public int Hp 
        { 
            get => _hp; 
            protected set 
            { 
                _hp = value; 
            }
        }

        private int _maxHP;
        public int MaxHp 
        { 
            get => _maxHP; 
            protected set
            {
                _maxHP = value;
            } 
        }

        private float _moveSpeed;
        public float MoveSpeed 
        { 
            get => _moveSpeed; 
            protected set
            {
                _moveSpeed = value;
            }
        }

        private Vector2 _moveDir;
        public Vector2 MoveDir 
        { 
            get => _moveDir; 
            protected set
            {
                _moveDir = value.normalized;
            }
        }

        public virtual void OnDamaged(BaseController attacker, int damage)
        {
            Hp -= damage;
            if (Hp <= 0)
            {
                Hp = 0;
                OnDead();
            }
        }

        protected virtual void OnDead()
        {
        }
    }
}
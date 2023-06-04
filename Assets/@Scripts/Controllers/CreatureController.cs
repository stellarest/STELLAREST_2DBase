using UnityEngine;

namespace STELLAREST_2D
{
    public class CreatureController : BaseController
    {
        public SkillBook Skills { get; protected set; } // 몬스터도 공용으로 써야하므로.

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

        public override bool Init()
        {
            base.Init();

            Skills = gameObject.GetOrAddComponent<SkillBook>();

            return true;
        }

        public virtual void OnDamaged(BaseController attacker, int damage)
        {
            if (Hp <= 0)
                return;

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
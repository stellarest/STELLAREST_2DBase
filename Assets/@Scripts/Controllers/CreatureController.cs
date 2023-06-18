using UnityEngine;
using UnityEngine.Rendering;

namespace STELLAREST_2D
{
    public class CreatureController : BaseController
    {
        public PlayerAnimationController PAC { get; protected set; }
        public MonsterAnimationController MAC { get; protected set; }
        public Rigidbody2D RigidBody { get; protected set; }
        public CircleCollider2D CircleCol { get; protected set; }

        public Data.CreatureData CreatureData { get; protected set; }
        public Data.SkillData SkillData { get; protected set; }
        public SkillBook SkillBook { get; protected set; } // 플레이어, 몬스터도 공용

        public Define.WeaponType WeaponType { get; protected set; }
        protected WeaponController WeaponController { get; set; }

        public int TemplateID { get; protected set; }
        public string CreatureName { get; protected set; }
        private int _hp;
        public int Hp { get => _hp; set { _hp = value; } }

        private int _maxHp;
        public int MaxHp { get => _maxHp; set { _maxHp = value; } }

        private int _strength;
        public int Strength { get => _strength; set { _strength = value; } }

        private float _moveSpeed;
        public float MoveSpeed { get => _moveSpeed; set { _moveSpeed = value; } }

        private float _luck;
        public float Luck { get => _luck; set { _luck = value; } }

        private Vector2 _moveDir;
        public Vector2 MoveDir { get => _moveDir; set { _moveDir = value.normalized; } }

        protected Vector2 _initScale;
        protected float _attackRange = 1f;

        public virtual void SetInfo(int templateID)
        {
            if (Managers.Data.CreatureDict.TryGetValue(templateID, out Data.CreatureData creatureData) == false)
            {
                Debug.LogAssertion("!!!!! Failed to load creatureData !!!!!");
                Debug.Break();
            }

            _initScale = transform.localScale;
            this.CreatureData = creatureData;
            //gameObject.name = "@Player_" + creatureData.Name;
            SetInitialStat(creatureData);

            RigidBody = gameObject.GetOrAddComponent<Rigidbody2D>();
            CircleCol = gameObject.GetOrAddComponent<CircleCollider2D>();

            SkillBook = gameObject.GetOrAddComponent<SkillBook>();
            SetSortingGroup();
            SetWeapon();
        }

        protected virtual void SetSortingGroup() { }

        private void SetWeapon()
        {
            WeaponController = gameObject.GetOrAddComponent<WeaponController>();
            switch (WeaponType)
            {
                case Define.WeaponType.None:
                    break;

                case Define.WeaponType.Melee1H:
                    {

                    }
                    break;

                case Define.WeaponType.Firearm2H:
                    {
                        //Animator.SetBool("Ready", true); // 임시
                        WeaponController.GrabPoint = Utils.FindChild<Transform>(gameObject, Define.PlayerController.RIFLE_GRAB_POINT, true);
                        WeaponController.WeaponPoint = Utils.FindChild<Transform>(gameObject, Define.PlayerController.FIRE_TRANSFORM, true);
                    }
                    break;
            }
        }

        public virtual void SetInitialStat(Data.CreatureData creatureData)
        {
            TemplateID = creatureData.TemplateID;
            CreatureName = creatureData.Name;
            _maxHp = creatureData.MaxHp;
            _hp = MaxHp;
            _strength = creatureData.Strength;
            _moveSpeed = creatureData.MoveSpeed;
            _luck = creatureData.Luck;
            WeaponType = creatureData.WeaponType;
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

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

        protected virtual void OnDead() { }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace STELLAREST_2D
{
    public class CreatureController : BaseController
    {
        public PlayerAnimationController PAC { get; protected set; }
        public MonsterAnimationController MAC { get; protected set; }
        public Rigidbody2D RigidBody { get; protected set; }
        public Collider2D AttackCol { get; protected set; } // Creature에서 필요한 경우 사용

        // public Data.SkillData SkillData { get; protected set; } // 굳이 SkillBook이 있는데 왜??
        public SkillBook SkillBook { get; protected set; }

        protected WeaponController WeaponController { get; set; }
        public bool IsPlayingDamageEffect { get; set; } = false;

        public Define.InGameGrade CreatureGrade { get; set; } = Define.InGameGrade.Normal;

        public Data.CreatureData CreatureData { get; protected set; }
        public int TemplateID { get; protected set; }
        public string CreatureName { get; protected set; }
        public float MaxHp { get; protected set; }
        public float Hp { get; protected set; }
        public float Power { get; protected set; }
        public float Armor { get; protected set; }
        public float MoveSpeed { get; protected set; }
        public float Range { get; protected set; }
        public float Agility { get; protected set; }
        public float Critical { get; protected set; }
        public float RepeatAttackAnimSpeed { get; protected set; }
        public float RepeatAttackCoolTime { get; protected set; }
        public float Luck { get; protected set; }
        public float TotalExp { get; protected set; }
        // WeaponType, IconLabel은 일단 무시


        private Define.WeaponType _weaponType;
        public Define.WeaponType WeaponType { get => _weaponType; protected set { _weaponType = value; } }

        private Vector2 _moveDir;
        public Vector2 MoveDir { get => _moveDir; set { _moveDir = value.normalized; } }
        public bool IsMoving { get => _moveDir != Vector2.zero; }

        protected Vector2 _initScale;
        protected float _attackRange = 1f;

        public bool IsMonster()
        {
            switch (ObjectType)
            {
                case Define.ObjectType.Player:
                case Define.ObjectType.Projectile:
                case Define.ObjectType.Env:
                    return false;

                case Define.ObjectType.Monster:
                case Define.ObjectType.EliteMonster:
                case Define.ObjectType.MiddleBoss:
                case Define.ObjectType.Boss:
                    return true;

                default:
                    return false;
            }
        }

        protected void SetInitAttackCollider(Collider2D collider)
        {
            AttackCol = collider;
            AttackCol.enabled = false;
        }

        public virtual void SetInfo(int templateID)
        {
            RigidBody = gameObject.GetOrAddComponent<Rigidbody2D>();
            SkillBook = gameObject.GetOrAddComponent<SkillBook>();
            _initScale = transform.localScale;

            if (Managers.Data.CreatureDict.TryGetValue(templateID, out Data.CreatureData creatureData) == false)
            {
                Debug.LogAssertion("!!!!! Failed to load creature data !!!!!");
                Debug.Break();
            }
            this.CreatureData = creatureData;
            SetInitialStat(creatureData);
            SetInitialSkill(creatureData);

            SetSortingGroup();
            SetWeapon();
        }

        protected virtual void SetInitialStat(Data.CreatureData creatureData)
        {
            this.CreatureData = creatureData;
            TemplateID = creatureData.TemplateID;
            CreatureName = creatureData.Name;
            MaxHp = creatureData.MaxHp;
            Hp = MaxHp;
            Power = creatureData.Power;
            Armor = creatureData.Armor;
            MoveSpeed = creatureData.MoveSpeed;
            Range = creatureData.Range;
            Agility = creatureData.Agility;
            Critical = creatureData.Critical;
            RepeatAttackAnimSpeed = creatureData.RepeatAttackAnimSpeed;
            RepeatAttackCoolTime = creatureData.RepeatAttackCoolTime;
            Luck = creatureData.Luck;
            TotalExp = creatureData.TotalExp;
            // WeaponType, IconLabel은 일단 무시
        }

        protected virtual void SetInitialSkill(Data.CreatureData creatureData)
        {
            foreach (Define.TemplateIDs.SkillType skill in creatureData.InGameSkillList)
            {
                int templateID = (int)skill;
                string className = Define.NameSpaceLabels.STELLAREST_2D + "." + skill.ToString();

                if (typeof(RepeatSkill).IsAssignableFrom(System.Type.GetType(className)))
                {
                    string primaryKey = skill.ToString() + ".prefab";
                    GameObject go = Managers.Resource.Instantiate(primaryKey);
                    RepeatSkill repeatSkill = go.GetComponent<RepeatSkill>();
                    if (repeatSkill == null)
                    {
                        Debug.LogError("@@@ You have to add skill component in skill prefab in advance !!");
                        Debug.Break();
                    }
                    repeatSkill.SetInitialSkillInfo(this, templateID);
                    SkillBook.AddRepeatSkill(repeatSkill);
                }
                else if (typeof(SequenceSkill).IsAssignableFrom(System.Type.GetType(className)))
                {
                    Debug.Log(className + " is inheritted from SequenceSkill !!");
                }
                else
                {
                    Debug.LogError("@@@ Something is wrong !! @@@");
                    Debug.Break();
                }
            }
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



        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public virtual void OnDamaged(BaseController attacker, SkillBase skill, int damage)
        {
            // Hit Effect 처리
            if (gameObject.IsValid() || this.IsValid())
                StartCoroutine(HitEffect(attacker, damage));
        }

        private bool _isPlayHitEffect = false;
        public virtual IEnumerator HitEffect(BaseController attacker, int damage)
        {
            if (_isPlayHitEffect == false)
            {
                if (this?.IsMonster() == false)
                {
                    Debug.Log("Damage To Player !!");
                    attacker.GetComponent<CreatureController>().AttackCol.enabled = false;

                    _isPlayHitEffect = true;
                    // Managers.Effect.StartHitEffect(gameObject);
                    Managers.Effect.StartHitEffectToPlayer();

                    Managers.Effect.ShowDamageNumber(Define.PrefabLabels.DMG_NUMBER_TO_PLAYER,
                        transform.position + (Vector3.up * 2.5f), damage);

                    yield return new WaitForSeconds(0.1f); // 데미지는 0.1초에 한 번씩..

                    Managers.Effect.EndHitEffectToPlayer();

                    _isPlayHitEffect = false;
                }
                else
                {
                    Debug.Log("Damage To Monster !!");
                }

                Hp -= damage;
                if (Hp <= 0)
                {
                    Hp = 0;
                    OnDead();
                }
            }
            else
                yield break;
        }

        protected virtual void OnDead() { }
    }
}
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
        public Collider2D BodyCol { get; protected set; }
        public SkillBook SkillBook { get; protected set; }
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

        // TODO : WeaponType, IconLabel은 일단 무시
        private Vector2 _moveDir;
        public Vector2 MoveDir { get => _moveDir; set { _moveDir = value.normalized; } }
        public bool IsMoving { get => _moveDir != Vector2.zero; }
        protected Vector2 _initScale;

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

        public bool IsPlayer() => this.ObjectType == Define.ObjectType.Player;

        public virtual void SetInfo(int templateID)
        {
            RigidBody = gameObject.GetOrAddComponent<Rigidbody2D>();
            SkillBook = gameObject.GetOrAddComponent<SkillBook>();
            SkillBook.Owner = this;

            BodyCol = gameObject.GetComponent<Collider2D>();
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
        }

        protected virtual void SetInitialSkill(Data.CreatureData creatureData)
        {
            Define.InGameGrade skillGrade = Define.InGameGrade.Normal;

            GameObject goRepeatSkills = new GameObject() { name = "@RepeatSkills" };
            goRepeatSkills.transform.SetParent(this.transform);

            GameObject goSequenceSkills = new GameObject() { name = "@SequenceSkills" };
            goSequenceSkills.transform.SetParent(this.transform);

            foreach (Define.TemplateIDs.SkillType skill in creatureData.InGameSkillList)
            {
                int templateID = (int)skill;
                string className = Define.NameSpaceLabels.STELLAREST_2D + "." + skill.ToString();

                //200100, 200101, 200102, 200103 
                // PaladinSwing_Normal.prefab
                // PaladinSwing_Rare.prefab ...
                for (int i = templateID; i <= templateID + (int)Define.InGameGrade.Epic; ++i)
                {
                    string primaryKey = skill.ToString() + "_" + skillGrade.ToString() + ".prefab";
                    skillGrade++;
                    GameObject go = Managers.Resource.Instantiate(primaryKey);
                    if (go == null)
                        continue;

                    if (typeof(RepeatSkill).IsAssignableFrom(System.Type.GetType(className)))
                    {
                        RepeatSkill repeatSkill = go.GetOrAddComponent<RepeatSkill>();
                        repeatSkill.Owner = this;
                        repeatSkill.SkillData = Managers.Data.SkillDict[i]; // 이게 더 직관적

                        repeatSkill.OnPreSpawned();
                        SkillBook.AddRepeatSkill(repeatSkill);
                        go.transform.SetParent(goRepeatSkills.transform);
                    }
                    else if (typeof(SequenceSkill).IsAssignableFrom(System.Type.GetType(className)))
                    {
                    }
                    else
                        Utils.LogError("Something is wrong !!");

                    go.SetActive(false);
                }
            }
        }

        public void CoFadeEffect()
        {
            StartCoroutine(Managers.Effect.CoFadeEffect(this));
        }

        protected virtual void SetSortingGroup() { }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            return true;
        }

        public virtual void OnDamaged(BaseController attacker, SkillBase skill, float damage)
        {
            if (gameObject.IsValid() || this.IsValid())
                StartCoroutine(HitEffect(attacker, damage));
        }

        private bool _isPlayingPlayerHitEffect = false;
        public virtual IEnumerator HitEffect(BaseController attacker, float damage)
        {
            if (_isPlayingPlayerHitEffect == false)
            {
                if (this?.IsMonster() == false)
                {
                    Debug.Log("Damage To Player !!");
                    //attacker.GetComponent<CreatureController>().AttackCol.enabled = false;

                    _isPlayingPlayerHitEffect = true;
                    Managers.Effect.StartHitEffect(this);
                    Managers.Effect.ShowDamageFont(this, damage);

                    yield return new WaitForSeconds(0.1f);
                    Managers.Effect.EndHitEffect(this);
                    _isPlayingPlayerHitEffect = false;
                }
                else
                {
                    Debug.Log("Damage To Monster !!");
                    Managers.Effect.StartHitEffect(this);

                    // TEMP
                    int rand = Random.Range(0, 5);
                    if (rand <= 3)
                        Managers.Effect.ShowDamageFont(this, damage);
                    else
                        Managers.Effect.ShowDamageFont(this, damage, true);

                    yield return new WaitForSeconds(0.1f);
                    Managers.Effect.EndHitEffect(this);

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
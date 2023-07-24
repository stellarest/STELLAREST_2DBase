using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace STELLAREST_2D
{
    public class CreatureController : BaseController
    {
        protected Define.CreatureState _cretureState = Define.CreatureState.Idle;
        public Define.CreatureState CreatureState
        {
            get => _cretureState;
            set
            {
                _cretureState = value;
                UpdateAnimation();
            }
        }

        public virtual void UpdateAnimation() { }

        public Rigidbody2D RigidBody { get; protected set; }
        public Collider2D BodyCol { get; protected set; }
        public SkillBook SkillBook { get; protected set; }
        public CharacterData CharaData { get; protected set; }
        public bool IsPlayingDamageEffect { get; set; } = false;
        
        private Vector2 _moveDir;
        public Vector2 MoveDir { get => _moveDir; set { _moveDir = value.normalized; } }
        public bool IsMoving { get => _moveDir != Vector2.zero; }
        protected Vector2 _initScale;


        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            RigidBody = gameObject.GetOrAddComponent<Rigidbody2D>();
            SkillBook = gameObject.GetOrAddComponent<SkillBook>();
            SkillBook.Owner = this;

            BodyCol = gameObject.GetComponent<Collider2D>();
            _initScale = transform.localScale;

            return true;
        }

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
            if (Managers.Data.CreatureDict.TryGetValue(templateID, out Data.CreatureData creatureData) == false)
            {
                Debug.LogAssertion("!!!!! Failed to load creature data !!!!!");
                Debug.Break();
            }

            //this.CreatureData = creatureData;
            SetInitialStat(creatureData);
            SetInitialSkill(creatureData);

            SetSortingGroup();
        }

        protected virtual void SetInitialStat(Data.CreatureData creatureData)
                                    => CharaData = new CharacterData(creatureData);

        protected virtual void SetInitialSkill(Data.CreatureData creatureData)
        {
            Define.InGameGrade skillGrade = Define.InGameGrade.Normal;

            GameObject goRepeatSkills = new GameObject() { name = "@RepeatSkills" };
            goRepeatSkills.transform.SetParent(this.transform);
            goRepeatSkills.transform.localPosition = Vector3.zero;

            GameObject goSequenceSkills = new GameObject() { name = "@SequenceSkills" };
            goSequenceSkills.transform.SetParent(this.transform);
            goSequenceSkills.transform.localPosition = Vector3.zero;

            if (this?.IsMonster() == false)
            {
                GameObject goInventory = new GameObject() { name = "@Inventory" };
                goInventory.transform.SetParent(this.transform);

            }

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
                        repeatSkill.OnPreSpawned();

                        SkillBook.AddRepeatSkill(repeatSkill);
                        go.transform.SetParent(goRepeatSkills.transform);
                        repeatSkill.SetSkillInfo(this, i);
                    }
                    else if (typeof(SequenceSkill).IsAssignableFrom(System.Type.GetType(className)))
                    {
                        SequenceSkill sequenceSkill = go.GetOrAddComponent<SequenceSkill>();
                        sequenceSkill.OnPreSpawned();

                        SkillBook.AddSequenceSkill(sequenceSkill);
                        go.transform.SetParent(goSequenceSkills.transform);
                        sequenceSkill.SetSkillInfo(this, i);
                    }
                    else
                        Utils.LogError("Something is wrong !!");
                }
            }
        }

        protected virtual void SetSortingGroup() { }

        public virtual void OnDamaged(BaseController attacker, SkillBase skill)
        {
            if (gameObject.IsValid() || this.IsValid())
                StartCoroutine(HitEffect(attacker, skill));
        }

        private bool _isPlayingPlayerHitEffect = false;
        public virtual IEnumerator HitEffect(BaseController attacker, SkillBase skill)
        {
            if (_isPlayingPlayerHitEffect == false)
            {
                float damage = skill.GetDamage();
                float resultDamage = damage - (damage * CharaData.Armor);
                bool isCritical = skill.IsCritical;

                if (this?.IsMonster() == false)
                {
                    if (Managers.Effect.IsPlayingGlitch || Random.Range(0f, 1f) <= Mathf.Min(CharaData.DodgeChance, Define.MAX_DODGE_CHANCE))
                    {
                        Managers.Effect.ShowDodgeText(this);
                        yield break;
                    }

                    Debug.Log("Damage To Player !!");
                    //attacker.GetComponent<CreatureController>().AttackCol.enabled = false;

                    _isPlayingPlayerHitEffect = true;
                    Managers.Effect.StartHitEffect(this);
                    Managers.Effect.ShowDamageFont(this, resultDamage);

                    yield return new WaitForSeconds(0.1f);
                    Managers.Effect.EndHitEffect(this);
                    _isPlayingPlayerHitEffect = false;
                }
                else
                {
                    Debug.Log("Damage To Monster !!");
                    Managers.Effect.StartHitEffect(this);
                    Managers.Effect.ShowDamageFont(this, resultDamage, isCritical);

                    yield return new WaitForSeconds(0.1f);
                    Managers.Effect.EndHitEffect(this);
                }

                skill.IsCritical = isCritical ? !isCritical : isCritical;

                CharaData.Hp -= damage;
                if (CharaData.Hp <= 0)
                {
                    CharaData.Hp = 0;
                    CreatureState = Define.CreatureState.Death;
                    // OnDead();
                }
            }
            else
                yield break;
        }

        protected virtual void OnDead() { }

        // +++++ CREATURE EFFECT +++++
        public void CoEffectFadeOut(float startTime, float desiredTime, bool onDespawn = true) 
                => StartCoroutine(Managers.Effect.CoFadeOut(this, startTime, desiredTime, onDespawn));

        public void CoEffectFadeIn(float desiredTime)
             => StartCoroutine(Managers.Effect.CoFadeIn(this, desiredTime));

        public void CoEffectGlitch() => StartCoroutine(Managers.Effect.CoEffectGlitch(this));
    }
}
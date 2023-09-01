using System.Buffers;
using System.Collections;
using System.Linq.Expressions;
using System.Text;
using STELLAREST_2D.Data;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace STELLAREST_2D
{
    public class CreatureController : BaseController
    {
        public Define.CreatureType CreatureType { get; protected set; } = Define.CreatureType.Creture;
        
        public GameObject GoCCEffect { get; set; } = null;
        public BuffBase Buff { get; private set; } = null;

        public void UpgradeBonusBuff(SkillBase skill, int templateID)
        {
            Data.BonusBuffData buffData = Managers.Data.BuffDict[templateID];
            string label = buffData.PrimaryLabel;
            GameObject go = Managers.Resource.Instantiate(label, pooling: false);
            BuffBase buff = go.GetComponent<BuffBase>();
            buff.StartBuff(this, skill, buffData);
            this.Buff = buff;
        }

        //protected bool[] _ccStates = null;
        public bool[] _ccStates = null;

        public bool this[Define.CCType cc]
        {
            get => _ccStates[(int)cc];
            set
            {
                // 일단 몬스터만 적용한거라.
                _ccStates[(int)cc] = value;
                if (_ccStates[(int)Define.CCType.Stun])
                {
                    CreatureState = Define.CreatureState.Idle;
                    BodyCol.isTrigger = true;
                    SkillBook.StopSkills();
                }
                else if (_ccStates[(int)Define.CCType.Stun] == false)
                    SkillBook.Stopped = false;
                // 이후, 그밖에 ccState가 중복되었을 때 처리...

                
                if (_ccStates[(int)Define.CCType.KnockBack])
                {
                }
                else if (_ccStates[(int)Define.CCType.KnockBack] == false)
                {
                }
            }
        }

        public void ResetCCStates()
        {
            if (_ccStates != null)
            {
                for (int i = 0; i < _ccStates.Length; ++i)
                    _ccStates[i] = false;
            }
        }

        public Define.CreatureState _cretureState = Define.CreatureState.Idle;
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

        [field: SerializeField]
        public CharacterData CharaData { get; protected set; }

        public void UpgradeCharacterData(int templateID)
                => this.CharaData = CharaData.UpgradeCreatureStat(this, CharaData, templateID);

        /*
            public int TemplateID;
            public string Name;
            public string Description;
            Define.InGameGrade InGameGrade;
            public float MaxHpUp;
            public float DamageUp;
            public float CriticalUp;
            public float AttackSpeedUp;
            public float CoolDownUp;
            public float ArmorUp;
            public float DodgeUp;
            public float MoveSpeed;
            public float CollectRangeUp;
            public float LuckUp;
        */

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

            if (_ccStates == null)
            {
                _ccStates = new bool[(int)Define.CCType.Max];

                for (int i = 0; i < _ccStates.Length; ++i)
                    _ccStates[i] = false;
            }

            SetInitialStat(creatureData);
            SetInitialSkill(creatureData);

            SetSortingGroup();
        }

        protected virtual void SetInitialStat(Data.CreatureData creatureData)
                                    => CharaData = new CharacterData(this, creatureData);

        // TODO : 개선 필요
        protected virtual void SetInitialSkill(Data.CreatureData creatureData)
        {
            GameObject goRepeatSkills = new GameObject() { name = "@RepeatSkills" };
            goRepeatSkills.transform.SetParent(this.transform);
            goRepeatSkills.transform.localPosition = Vector3.zero;

            GameObject goSequenceSkills = new GameObject() { name = "@SequenceSkills" };
            goSequenceSkills.transform.SetParent(this.transform);
            goSequenceSkills.transform.localPosition = Vector3.zero;

            // StringBuilder stringBuilder = new StringBuilder();
            foreach (Define.TemplateIDs.SkillType skill in creatureData.InGameSkillList)
            {
                int templateID = (int)skill;
                string className = Define.NameSpace + "." + skill.ToString();
                Utils.LogStrong("CLASS NAME : " + className); // STELLAREST_2D.PaladinMeleeSwing

                Define.InGameGrade skillGrade = Define.InGameGrade.Normal;
                for (int i = templateID; i <= templateID + (int)Define.InGameGrade.Epic; ++i)
                {
                    // TODO : Special Skills
                    // STELLAREST_2D.QueenMeleeSwing_Special
                    string primaryKey = string.Empty;
                    primaryKey = skill.ToString() + "_" + skillGrade.ToString() + ".prefab";
                    skillGrade++;
                    // if (className.Contains("Special"))
                    // {
                    //     primaryKey = skill.ToString() + ".prefab";
                    // }
                    // else
                    // {
                    //     primaryKey = skill.ToString() + "_" + skillGrade.ToString() + ".prefab";
                    //     skillGrade++;
                    // }

                    Debug.Log("PRIMARY KEY : " + primaryKey);
                    GameObject go = Managers.Resource.Instantiate(primaryKey);
                    if (go == null)
                        continue;

                    // +++ TODO : USE STRING BUILDER? +++
                    if (primaryKey.Contains("MeleeSwing"))
                        className = Define.NameSpace + "." + "MeleeSwing";

                    if (primaryKey.Contains("RangedShot"))
                        className = Define.NameSpace + "." + "RangedShot";

                    if (primaryKey.Contains("RangedMagicShot"))
                        className = Define.NameSpace + "." + "RangedMagicShot";

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
                    if (Managers.Effect.IsPlayingGlitch || Random.Range(0f, 1f) <= Mathf.Min(CharaData.Dodge, Define.MAX_DODGE_CHANCE))
                    {
                        Managers.Effect.ShowDodgeText(this);
                        CoEffectHologram(); // --> 괜찮은듯.
                        yield break;
                    }

                    // +++++ DMG TO PLAYER +++++
                    _isPlayingPlayerHitEffect = true;

                    if (CharaData.ShieldHp > 0f)
                    {
                        Buff.GetComponent<GuardiansShield>().Hit();
                        Managers.Effect.ShowShieldDamageFont(this, resultDamage);
                        CharaData.ShieldHp -= damage;
                    }
                    else
                    {
                        Managers.Effect.StartHitEffect(this);
                        Managers.Effect.ShowDamageFont(this, resultDamage);
                        CharaData.Hp -= damage;
                    }

                    yield return new WaitForSeconds(0.1f);
                    Managers.Effect.EndHitEffect(this);
                    _isPlayingPlayerHitEffect = false;
                }
                else
                {
                    // +++++ DMG TO MONSTER +++++
                    Managers.Effect.StartHitEffect(this);
                    Managers.Effect.ShowDamageFont(this, resultDamage, isCritical);

                    CharaData.Hp -= damage;

                    yield return new WaitForSeconds(0.1f);
                    Managers.Effect.EndHitEffect(this);
                }

                skill.IsCritical = isCritical ? !isCritical : isCritical;

                if (CharaData.Hp <= 0)
                {
                    CharaData.Hp = 0;
                    CreatureController cc = attacker.GetComponent<CreatureController>();
                    if (cc != null && cc.IsValid())
                    {
                        GemController gem = null;
                        if (Random.Range(0f, 0.99f + Mathf.Epsilon) < cc.CharaData.Luck)
                        {
                            gem = Managers.Object.Spawn<GemController>(transform.position);
                            gem.GemSize = GemSize.Large;
                        }
                        else
                        {
                            gem = Managers.Object.Spawn<GemController>(transform.position);
                            gem.GemSize = GemSize.Normal;
                        }

                        gem.Init();
                    }

                    CreatureState = Define.CreatureState.Death;
                    // OnDead();
                }
            }
            else
                yield break;
        }

        protected virtual void OnDead() { }

        // +++++ CREATURE EFFECT +++++
        // +++++ Base Controller로 옮겨야할수도 있음 +++++
        // +++++ Co시리즈는 Base로 옮기자 !!
        public void CoEffectFadeOut(float startTime, float desiredTime, bool onDespawn = true) 
                => StartCoroutine(Managers.Effect.CoFadeOut(this, startTime, desiredTime, onDespawn));

        public void CoEffectFadeIn(float desiredTime)
             => StartCoroutine(Managers.Effect.CoFadeIn(this, desiredTime));

        public void CoEffectHologram()
            => StartCoroutine(Managers.Effect.CoHologram(this));

        public void CoEffectGlitch() => StartCoroutine(Managers.Effect.CoEffectGlitch(this));

        public void CoCCStun(CreatureController cc, GameObject goCCEffect, float duration) 
                => StartCoroutine(Managers.CC.CoStun(cc, goCCEffect, duration));

        public void CoCCKnockBack(CreatureController cc, Vector3 dir, float duration)
                => StartCoroutine(Managers.CC.CoKnockBack(cc, dir, duration));

        // public void CoStartKnockBack(CreatureController cc, Vector2 hitPoint, Vector2 knockBackDir, float duration, float intensity)
        //         => StartCoroutine(Managers.CC.CoStartKnockBack(cc, hitPoint, knockBackDir, duration, intensity));
    }
}
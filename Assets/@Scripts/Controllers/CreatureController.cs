using System.Collections;
using System.Linq.Expressions;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace STELLAREST_2D
{
    public class CreatureController : BaseController
    {
        public Define.CreatureType CreatureType { get; protected set; } = Define.CreatureType.Creture;
        
        public GameObject GoCCEffect { get; set; } = null;

        protected bool[] _ccStates = null;
        public bool this[Define.CCState ccState]
        {
            get => _ccStates[(int)ccState];
            set
            {
                _ccStates[(int)ccState] = value;

                if (_ccStates[(int)Define.CCState.Stun])
                {
                    CreatureState = Define.CreatureState.Idle;

                    // 넉백 상태인 경우에는 그대로 뒤로 쭈욱 밀려나간다
                    if (_ccStates[(int)Define.CCState.KnockBack] == false)
                        RigidBody.velocity = Vector2.zero;

                    BodyCol.isTrigger = true;
                    SkillBook.StopSkills();
                }
                else if (_ccStates[(int)Define.CCState.Stun] == false)
                    SkillBook.Stopped = false;


                if (_ccStates[(int)Define.CCState.KnockBack])
                {
                    // 점프모션이나 뒤로 걷는 모션도 있겠지만 일단 Idle
                    CreatureState = Define.CreatureState.Idle;
                    SkillBook.Stopped = false;
                }

                // 이후, 그밖에 ccState가 중복되었을 때 처리...
            }
        }

        public void ResetCCStates()
        {
            if (_ccStates != null)
            {
                for (int i = 0; i < _ccStates.Length; ++i)
                {
                    if (i == (int)Define.CCState.None)
                        this[Define.CCState.None + i] = true;
                    else
                        this[Define.CCState.None + i] = false;
                }
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
                _ccStates = new bool[(int)Define.CCState.Max];
                for (int i = 0; i < (int)Define.CCState.Max; ++i)
                {
                    if (i == (int)Define.CCState.None)
                    {
                        // 이렇게 사용하면 인덱서 프로퍼티에 적용이 안됨
                        //_ccStates[i] = true;
                        Debug.Log(gameObject.name + "@@@");
                        this[Define.CCState.None + i] = true;
                    }
                    else
                    {
                        //_ccStates[i] = false;
                        this[Define.CCState.None + i] = false;
                    }
                }
            }

            SetInitialStat(creatureData);
            SetInitialSkill(creatureData);

            SetSortingGroup();
        }

        protected virtual void SetInitialStat(Data.CreatureData creatureData)
                                    => CharaData = new CharacterData(creatureData);

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
                    if (Managers.Effect.IsPlayingGlitch || Random.Range(0f, 1f) <= Mathf.Min(CharaData.DodgeChance, Define.MAX_DODGE_CHANCE))
                    {
                        Managers.Effect.ShowDodgeText(this);
                        CoEffectHologram(); // --> 괜찮은듯. 홀로그램 발동할 때 표정 수정하고 한 번 테스트 해볼것 !!
                        // 그리고 공격중에는 눈이 이상하게 나오니까, 이거 예외 줄것
                        // Eyebrows까지 없애야할듯
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

        public void CoStartStun(CreatureController cc, GameObject goCCEffect, float duration) 
                => StartCoroutine(Managers.CC.CoStartStun(cc, goCCEffect, duration));

        public void CoStartKnockBack(CreatureController cc, Vector2 hitPoint, Vector2 knockBackDir, float duration, float intensity)
                => StartCoroutine(Managers.CC.CoStartKnockBack(cc, hitPoint, knockBackDir, duration, intensity));
    }
}
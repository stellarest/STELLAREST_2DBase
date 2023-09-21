using System.Collections;
using HeroEditor.Common;
using UnityEngine;

namespace STELLAREST_2D
{
    public class CreatureController : BaseController
    {
        // +++ CUSTOM TYPES +++
        public AnimationCallback AnimCallback { get; protected set; } = null;
        public RendererController RendererController { get; protected set; } = null;
        public Define.LookAtDirection LookAtDir { get; protected set; } = Define.LookAtDirection.Right;

        public SkillBook SkillBook { get; protected set; } = null;
        [field: SerializeField] public CreatureStat CreatureStat { get; protected set; } = null;
        public void UpdateCreatureStat(int templateID) => this.CreatureStat = CreatureStat.UpgradeStat(this, CreatureStat, templateID);
        [field: SerializeField] private Define.CreatureState _cretureState = Define.CreatureState.Idle;
        public Define.CreatureState CreatureState { get => _cretureState; set { _cretureState = value; UpdateAnimation(); } }
        public virtual void UpdateAnimation() { }

        // +++ PRIMITIVE TYPES +++
        public Transform Indicator { get; protected set; } = null;
        public Vector3 IndicatorPosition => Indicator.transform.position;
        public Transform FireSocket { get; protected set; } = null;
        public Vector3 FireSocketPosition => FireSocket.transform.position;
        public Vector3 ShootDir => (FireSocketPosition - IndicatorPosition).normalized;

        public Vector3 AttackStartPoint { get; protected set; } = Vector3.zero;
        public Vector3 AttackEndPoint { get; protected set; } = Vector3.zero;
        public float GetMovementPower => (AttackEndPoint - AttackStartPoint).magnitude;

        public Rigidbody2D RigidBody { get; protected set; } = null;
        public Collider2D BodyCollider { get; protected set; } = null;

        private Vector3 _moveDir = Vector3.zero;
        public Vector3 MoveDir { get => _moveDir; protected set { _moveDir = value.normalized; } }
        public bool IsMoving { get => _moveDir != Vector3.zero; }
        protected Vector3 _initialLocalScale = Vector3.zero;

        // +++ MAIN METHODS +++
        public virtual void Init(int templateID)
        {
            if (Managers.Data.CreaturesDict.TryGetValue(templateID, out Data.CreatureData creatureData) == false)
                Utils.LogCritical(nameof(CreatureController), nameof(Init), $"TemplateID : {templateID}");

            InitCreatureStat(creatureData);
            InitCreatureSkill(creatureData);
            InitCreatureRenderer(creatureData);

            if (RigidBody == null)
                RigidBody = GetComponent<Rigidbody2D>();

            if (BodyCollider == null)
                BodyCollider = GetComponent<Collider2D>();

            _initialLocalScale = transform.localScale;
            this.IsFirstPooling = false;
        }

        protected virtual void InitCreatureStat(Data.CreatureData creatureData)
        {
            // creatureData의 Pooling 정보도 CreatureStat여기에 넣어야할까?
            // 어쨋든 크리쳐는 creatureData가 아닌, CreatureStat을 통해서 통제하게 될듯.
            CreatureStat = new CreatureStat(this, creatureData);
        }

        protected virtual void InitCreatureSkill(Data.CreatureData creatureData)
        {
            if (SkillBook == null)
            {
                SkillBook = gameObject.GetComponentInChildren<SkillBook>();
                SkillBook.Owner = this;
            }

            LoadRepeatSkills(creatureData);
            // LoadSequenceSkills(creatureData);
        }

        private void LoadRepeatSkills(Data.CreatureData creatureData)
        {
            foreach (Define.TemplateIDs.Status.Skill templateOrigin in creatureData.RepeatSkillList)
            {
                int templateID = (int)templateOrigin; // 200200
                if (Managers.Data.SkillsDict.TryGetValue(templateID, out Data.SkillData value) == false)
                    Utils.LogCritical(nameof(CreatureController), nameof(LoadRepeatSkills), $"TemplateID : {templateID}");

                if (value.Grade == value.MaxGrade)
                {
                    GameObject go = Managers.Resource.Load<GameObject>(value.PrimaryLabel);
                    RepeatSkill repeatSkill = go.GetOrAddComponent<RepeatSkill>();
                    repeatSkill.Init(this, value, templateID);
                }
                else
                {
                    for (int i = templateID; i < templateID + (int)value.MaxGrade; ++i)
                    {
                        if (Managers.Data.SkillsDict.TryGetValue(i, out Data.SkillData data) == false)
                            Utils.LogCritical(nameof(CreatureController), nameof(LoadRepeatSkills), $"TemplateID : {templateID}");

                        GameObject go = Managers.Resource.Load<GameObject>(data.PrimaryLabel);
                        RepeatSkill repeatSkill = go.GetOrAddComponent<RepeatSkill>();
                        repeatSkill.Init(this, data, i);
                    }
                }
            }
        }

        private void LoadSequenceSkills(Data.CreatureData creatureData) 
        { 
            /* DO SOMETHING */ 
        }

        protected virtual void InitCreatureRenderer(Data.CreatureData creatureData)
        {
            if (RendererController == null)
            {
                RendererController = gameObject.GetOrAddComponent<RendererController>();
                RendererController.InitRendererController(this, creatureData);
                SetRenderSorting();
            }
        }


        // public void UpgradeBonusBuff(SkillBase skill, int templateID)
        // {
        //     Data.BuffSkillData buffSkillData = Managers.Data.BuffSkillsDict[templateID];
        //     string label = buffSkillData.PrimaryLabel;
        //     GameObject go = Managers.Resource.Instantiate(label, pooling: false);
        //     BuffBase buff = go.GetComponent<BuffBase>();
        //     buff.StartBuff(this, skill, buffSkillData);
        //     this.Buff = buff;
        // }

        // public bool this[Define.TemplateIDs.CCType cc]
        // {
        //     get => _ccStates[(int)cc];
        //     set
        //     {
        //         // 일단 몬스터만 적용한거라.
        //         _ccStates[(int)cc] = value;
        //         if (_ccStates[(int)Define.TemplateIDs.CCType.Stun])
        //         {
        //             CreatureState = Define.CreatureState.Idle;
        //             BodyCollider.isTrigger = true;
        //             SkillBook.StopSkills();
        //         }
        //         else if (_ccStates[(int)Define.TemplateIDs.CCType.Stun] == false)
        //             SkillBook.Stopped = false;
        //         // 이후, 그밖에 ccState가 중복되었을 때 처리...

        //         if (_ccStates[(int)Define.TemplateIDs.CCType.KnockBack])
        //         {
        //         }
        //         else if (_ccStates[(int)Define.TemplateIDs.CCType.KnockBack] == false)
        //         {
        //         }
        //     }ßß
        // }

        public virtual void OnDamaged(BaseController attacker, SkillBase skill)
        {
            // Debug.Log($"{gameObject.name} is damaged !! by {attacker.gameObject.name} / {skill.gameObject.name}");

            // if (gameObject.IsValid() || this.IsValid())
            //     StartCoroutine(HitEffect(attacker, skill));
            if (gameObject.IsValid() || this.IsValid())
            {
                //StartCoroutine(CoOnDamaged(attacker, skill));
                float damage = skill.GetDamage();
            }
        }

        private IEnumerator CoOnDamaged(BaseController attacker, SkillBase skill)
        {
            // 어쨋든 데미지를 받아오면
            float damage = skill.GetDamage();

            // 여기서 히트 이펙트를 생성함
            //Managers.Effect.Hit(skill, this);
            yield return null;
        }

        protected virtual void OnDead() { }

        // +++ SELF UTILS +++
        public bool IsPlayer() => this.ObjectType == Define.ObjectType.Player;
        public bool IsMonster()
        {
            switch (ObjectType)
            {
                case Define.ObjectType.Player:
                case Define.ObjectType.Projectile:
                    return false;

                case Define.ObjectType.Monster:
                case Define.ObjectType.EliteMonster:
                case Define.ObjectType.Boss:
                    return true;

                default:
                    return false;
            }
        }
    }
}

// ==============================================================================================================
// 모든 코루틴은 Base Controller에 옮기자.
// *** RendererControlelr에서 처리해도 될 것 같다 ***
// +++++ CREATURE EFFECT +++++
// +++++ Base Controller로 옮겨야할수도 있음 +++++
// +++++ Co시리즈는 Base로 옮기자 !!
// public void CoEffectFadeOut(float startTime, float desiredTime, bool onDespawn = true)
//         => StartCoroutine(Managers.Effect.CoFadeOut(this, startTime, desiredTime, onDespawn));

// public void CoEffectFadeIn(float desiredTime)
//      => StartCoroutine(Managers.Effect.CoFadeIn(this, desiredTime));

// public void CoEffectHologram()
//     => StartCoroutine(Managers.Effect.CoHologram(this));

// public void CoEffectGlitch() => StartCoroutine(Managers.Effect.CoEffectGlitch(this));

// public void CoCCStun(CreatureController cc, GameObject goCCEffect, float duration)
//         => StartCoroutine(Managers.CC.CoStun(cc, goCCEffect, duration));

// public void CoCCKnockBack(CreatureController cc, Vector3 dir, float duration)
//         => StartCoroutine(Managers.CC.CoKnockBack(cc, dir, duration));

// public void CoStartKnockBack(CreatureController cc, Vector2 hitPoint, Vector2 knockBackDir, float duration, float intensity)
//         => StartCoroutine(Managers.CC.CoStartKnockBack(cc, hitPoint, knockBackDir, duration, intensity));

// for (int i = templateID; i <= templateID + (int)Define.InGameGrade.Epic; ++i)
// {
//     if (Managers.Data.SkillsDict.TryGetValue(i, out Data.SkillData repeatSkillData) == false)
//     {
//         Debug.LogError($"Failed to load repeat skill data. TemplateID : {i}");
//         Debug.Break();
//     }

//     GameObject go = Managers.Resource.Instantiate(repeatSkillData.PrimaryLabel);
//     RepeatSkill skillOrigin = go.GetComponent<RepeatSkill>();
//     skillOrigin.Init(this, skillOrigin, repeatSkillData);
//     skillOrigin.OnPreSpawned();
//     skillOrigin.transform.SetParent(repeatSkills.transform);
//     SkillBook.RepeatSkills.Add(skillOrigin);

//     // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//     // 일단, 크리쳐가 Repeat 스킬을 가지고 있는 경우, 무조건 가장 첫번째 스킬이 디폴트 스킬이 된다.
//     // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//     // if (i == templateID && skillOrigin.Data.IsDefaultType)
//     // {
//     //     SkillBook.OwnerDefaultSkillType = creatureData.DefaultSkillType;
//     //     SkillBook.DefaultRepeatSkillType = skill;
//     // }
// }

// public virtual IEnumerator HitEffect(BaseController attacker, SkillBase skill)
// {
//     if (_isPlayingPlayerHitEffect == false)
//     {
//         float damage = skill.GetDamage();
//         float resultDamage = damage - (damage * CreatureData.Armor);
//         bool isCritical = skill.IsCritical;

//         if (this?.IsMonster() == false)
//         {
//             if (Managers.Effect.IsPlayingGlitch || Random.Range(0f, 1f) <= Mathf.Min(CreatureData.Dodge, Define.MAX_DODGE_CHANCE))
//             {
//                 Managers.Effect.ShowDodgeText(this);
//                 CoEffectHologram(); // --> 괜찮은듯.
//                 yield break;
//             }

//             // +++++ DMG TO PLAYER +++++
//             _isPlayingPlayerHitEffect = true;

//             if (CreatureData.ShieldHp > 0f)
//             {
//                 Buff.GetComponent<GuardiansShield>().Hit();
//                 Managers.Effect.ShowShieldDamageFont(this, resultDamage);
//                 CreatureData.ShieldHp -= damage;
//             }
//             else
//             {
//                 Managers.Effect.StartHitEffect(this);
//                 Managers.Effect.ShowDamageFont(this, resultDamage);
//                 CreatureData.Hp -= damage;
//             }

//             yield return new WaitForSeconds(0.1f);
//             Managers.Effect.EndHitEffect(this);
//             _isPlayingPlayerHitEffect = false;
//         }
//         else
//         {
//             // +++++ DMG TO MONSTER +++++
//             Managers.Effect.StartHitEffect(this);
//             Managers.Effect.ShowDamageFont(this, resultDamage, isCritical);

//             CreatureData.Hp -= damage;

//             yield return new WaitForSeconds(0.1f);
//             Managers.Effect.EndHitEffect(this);
//         }

//         skill.IsCritical = isCritical ? !isCritical : isCritical;

//         if (CreatureData.Hp <= 0)
//         {
//             CreatureData.Hp = 0;
//             CreatureController cc = attacker.GetComponent<CreatureController>();
//             if (cc != null && cc.IsValid())
//             {
//                 GemController gem = null;
//                 if (Random.Range(0f, 0.99f + Mathf.Epsilon) < cc.CreatureData.Luck)
//                 {
//                     gem = Managers.Object.Spawn<GemController>(transform.position);
//                     gem.GemSize = GemSize.Large;
//                 }
//                 else
//                 {
//                     gem = Managers.Object.Spawn<GemController>(transform.position);
//                     gem.GemSize = GemSize.Normal;
//                 }

//                 gem.Init();
//             }

//             CreatureState = Define.CreatureState.Death;
//         }
//     }
//     else
//         yield break;
// }
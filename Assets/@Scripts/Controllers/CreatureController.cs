using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public interface IHitFrom
    {
        public bool IsHitFrom_ThrowingStar { get; set; }
        public bool IsHitFrom_LazerBolt { get; set; }
    }

    public class CreatureController : BaseController, IHitFrom
    {
        // +++ BASE CHILD OBJECTS +++
        public Transform Indicator { get; protected set; } = null;
        public Vector3 IndicatorPosition => Indicator.transform.position;
        public Transform FireSocket { get; protected set; } = null;
        public Vector3 FireSocketPosition => FireSocket.transform.position;
        public Vector3 ShootDir => (FireSocketPosition - IndicatorPosition).normalized;
        public Transform AnimTransform { get; protected set; } = null;
        public SpriteRenderer[] SPRs { get; protected set; } = null;
        public Vector3 LocalScale
        {
            get => AnimTransform.transform.localScale;
            set => AnimTransform.transform.localScale = value;
        }
        public bool IsFacingRight => (LocalScale.x != LocalScale.x * -1f) ? true : false;

       // +++ BASE COMPONENTS +++
        public AnimationCallback AnimCallback { get; protected set; } = null;
        public BaseAnimationController AnimController { get; protected set; } = null;
        public Rigidbody2D RigidBody { get; protected set; } = null;
        public Collider2D HitCollider { get; protected set; } = null;

        // +++ STAT +++
        [field: SerializeField] public CreatureStat Stat { get; protected set; } = null;
        public void UpdateCreatureStat(int templateID) 
            => this.Stat = Stat.UpgradeStat(this, Stat, templateID);

        // +++ SKILLS +++
        public SkillBook SkillBook { get; protected set; } = null;
        
        // +++ RENDERERS +++
        public RendererController RendererController { get; protected set; } = null;
        
        // +++ FIELD, PROPERTY, METHODS +++
        [SerializeField] private Define.CreatureState _cretureState = Define.CreatureState.Idle;
        public Define.CreatureState CreatureState { get => _cretureState; set { _cretureState = value; UpdateAnimation(); } }
        public virtual void UpdateAnimation() { }

        private Vector3 _moveDir = Vector3.zero;
        public Vector3 MoveDir { get => _moveDir; protected set { _moveDir = value.normalized; } }
        public bool IsMoving => _moveDir != Vector3.zero;

        public Vector3 AttackStartPoint { get; set; } = Vector3.zero;
        public Vector3 AttackEndPoint { get; set; } = Vector3.zero;
        public float GetMovementPower => (AttackEndPoint - AttackStartPoint).magnitude;
        
        public Define.LookAtDirection LookAtDir { get; protected set; } = Define.LookAtDirection.Right;
        protected Vector3 _baseRootLocalScale = Vector3.zero;

        // +++ MAIN METHODS +++
        public override void Init(int templateID)
        {
            if (Managers.Data.CreaturesDict.TryGetValue(templateID, out Data.InitialCreatureData creatureData) == false)
                Utils.LogCritical(nameof(CreatureController), nameof(Init), $"TemplateID : {templateID}");

            InitChildObject();
            InitBaseComponents();

            InitCreatureStat(creatureData);
            InitCreatureSkill(creatureData);
            InitCreatureRenderer(creatureData);
            _baseRootLocalScale = transform.localScale;
        }

        protected virtual void InitChildObject()
        {
            Indicator = Utils.FindChild<Transform>(this.gameObject,
                Define.INDICATOR, true);

            FireSocket = Utils.FindChild<Transform>(this.gameObject,
                Define.FIRE_SOCKET, true);

            AnimTransform = Utils.FindChild<Transform>(this.gameObject, 
                Define.ANIMATION_BODY, true);

            SPRs = AnimTransform.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
        }

        protected virtual void InitBaseComponents()
        {
            if (AnimCallback == null)
            {
                AnimCallback = AnimTransform.GetComponent<AnimationCallback>();
                AnimCallback.Init(this);
            }

            if (AnimController == null)
            {
                AnimController = AnimTransform.GetComponent<BaseAnimationController>();
                AnimController.Init(this);
            }

            if (RigidBody == null)
            {
                RigidBody = GetComponent<Rigidbody2D>();
                RigidBody.simulated = true;
            }

            if (HitCollider == null)
            {
                HitCollider = GetComponent<Collider2D>();
                HitCollider.enabled = true;
            }
        }

        protected virtual void InitCreatureStat(Data.InitialCreatureData creatureData) 
            => Stat = new CreatureStat(this, creatureData);

        protected void InitCreatureStat(int templateID)
        {
            if (Managers.Data.CreaturesDict.TryGetValue(templateID, out Data.InitialCreatureData creatureData) == false)
                Utils.LogCritical(nameof(CreatureController), nameof(Init), $"TemplateID : {templateID}");

            this.Stat = new CreatureStat(this, creatureData);
        }

        protected virtual void InitCreatureSkill(Data.InitialCreatureData creatureData)
        {
            if (SkillBook == null)
            {
                SkillBook = gameObject.GetComponentInChildren<SkillBook>();
                SkillBook.Owner = this;
            }

            LoadRepeatSkills(creatureData);
            // LoadSequenceSkills(creatureData);
            this.SkillBook.SetFirstExclusiveSkill();
        }

        private void LoadRepeatSkills(Data.InitialCreatureData creatureData)
        {
            GameObject goRepeatSkills = new GameObject { name = "@RepeatSkills "};
            goRepeatSkills.transform.SetParent(SkillBook.transform);

            foreach (Define.TemplateIDs.Status.Skill templateOrigin in creatureData.RepeatSkillList)
            {
                int templateID = (int)templateOrigin; // 200200
                if (Managers.Data.SkillsDict.TryGetValue(templateID, out Data.SkillData value) == false)
                    Utils.LogCritical(nameof(CreatureController), nameof(LoadRepeatSkills), $"TemplateID : {templateID}");

                if (value.Grade == value.MaxGrade)
                {
                    GameObject go = Managers.Resource.Instantiate(value.PrimaryLabel);
                    //go.name = $"{go.name}_[{value.TemplateID}]";
                    go.transform.SetParent(goRepeatSkills.transform);

                    RepeatSkill repeatSkill = go.GetComponent<RepeatSkill>();
                    repeatSkill.InitOrigin(this, value);
                    SkillBook.SkillGroupsDict.AddGroup(templateID, new SkillGroup(repeatSkill));
                }
                else
                {
                    for (int i = templateID; i < templateID + (int)value.MaxGrade; ++i)
                    {
                        if (Managers.Data.SkillsDict.TryGetValue(i, out Data.SkillData data) == false)
                            Utils.LogCritical(nameof(CreatureController), nameof(LoadRepeatSkills), $"TemplateID : {templateID}");

                        GameObject go = Managers.Resource.Instantiate(data.PrimaryLabel);
                        //go.name = $"{go.name}_[{data.TemplateID}]";
                        go.transform.SetParent(goRepeatSkills.transform);

                        RepeatSkill repeatSkill = go.GetComponent<RepeatSkill>();
                        repeatSkill.InitOrigin(this, data);
                        SkillBook.SkillGroupsDict.AddGroup(i, new SkillGroup(repeatSkill));
                    }
                }
            }
        }

        private void LoadSequenceSkills(Data.InitialCreatureData creatureData) { /* DO SOMETHING */ }

        protected virtual void InitCreatureRenderer(Data.InitialCreatureData creatureData)
        {
            if (RendererController == null)
            {
                RendererController = gameObject.GetOrAddComponent<RendererController>();
                RendererController.InitRendererController(this, creatureData);
                SetSortingGroup();
            }
        }

        public virtual void OnDamaged(CreatureController attacker, SkillBase from)
        {
            if (gameObject.IsValid() || this.IsValid())
            {
                // TUPLE
                (float dmgResult, bool isCritical) = Managers.Game.TakeDamage(this, attacker, from);
                if (dmgResult == -1f && isCritical == false)
                {
                    // DODGE
                    return;
                }

                this.Stat.Hp -= dmgResult;
                Managers.VFX.Hit(this);
                Managers.VFX.DamageFont(this, dmgResult, isCritical);
                // Impact : 메모리 문제 발생할 것 같으면 크리티컬쪽에서 스폰
                //Managers.VFX.Impact(from.Data.VFX_Impact, from.transform.position);
                // 일단, ImpactPoint는 이와 같이 정정. 스킬 객체에서 터트릴지, target에서 터트릴지 옵션 줘야할듯.
                // 밀리 스윙은 타겟, 프로젝타일은 자기 자신의 객체
                Managers.VFX.Impact(from.Data.VFX_Impact, this.transform.position);

                if (this.Stat.Hp <= 0)
                {
                    this.Stat.Hp = 0f;
                    this.OnDead();
                }
            }
        }

        protected virtual void OnVFX_Hit() { }

        protected virtual void OnDead() { }

        // +++ UTILS +++
        public bool IsPlayer() => this.ObjectType == Define.ObjectType.Player;
        public bool IsMonster()
        {
            switch (this.ObjectType)
            {
                case Define.ObjectType.Player:
                case Define.ObjectType.Projectile:
                case Define.ObjectType.Skill:
                    return false;

                case Define.ObjectType.Monster:
                case Define.ObjectType.EliteMonster:
                case Define.ObjectType.Boss:
                    return true;

                default:
                    return false;
            }
        }

        public bool IsHitFrom_ThrowingStar { get; set; } = false;
        public bool IsHitFrom_LazerBolt { get; set; } = false;

        // FOR RESPAWN OR ANOTHER
        public void ResetAllHitFrom()
        {
            this.IsHitFrom_ThrowingStar = false;
            this.IsHitFrom_LazerBolt = false;
        }

        public void ResetHitFrom(Define.HitFromType hitFromtype, float delay)
        {
            if (this.IsValid())
                StartCoroutine(CoResetHitFrom(hitFromtype, delay));
        }

        private IEnumerator CoResetHitFrom(Define.HitFromType hitFromType, float delay)
        {
            yield return new WaitForSeconds(delay);
            switch (hitFromType)
            {
                case Define.HitFromType.ThrowingStar:
                    this.IsHitFrom_ThrowingStar = false;
                    Utils.Log($"{this.gameObject.name}, Reset ThrowingStar.");
                    break;

                case Define.HitFromType.LazerBolt:
                    this.IsHitFrom_LazerBolt = false;
                    break;
            }
        }

        public bool IsInLimitMaxPosX => Mathf.Abs(transform.position.x - Managers.Stage.RightTop.x) < Mathf.Epsilon ||
                        Mathf.Abs(transform.position.x - Managers.Stage.LeftBottom.x) < Mathf.Epsilon;

        public bool IsInLimitMaxPosY => Mathf.Abs(transform.position.y - Managers.Stage.RightTop.y) < Mathf.Epsilon ||
                                        Mathf.Abs(transform.position.y - Managers.Stage.LeftBottom.y) < Mathf.Epsilon;
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
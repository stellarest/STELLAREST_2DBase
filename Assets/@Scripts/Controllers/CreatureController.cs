using System.Collections;
using System.Collections.Generic;
using STELLAREST_2D.Data;
using UnityEngine;

using VFXEnv = STELLAREST_2D.Define.TemplateIDs.VFX.Environment;
using VFXImpact = STELLAREST_2D.Define.TemplateIDs.VFX.ImpactHit;
using CrowdControl = STELLAREST_2D.Define.TemplateIDs.CrowdControl;
using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;
using STELLAREST_2D.UI;

namespace STELLAREST_2D
{
    public interface IHitFrom // CC도 추가하면 괜찮을듯
    {
        public bool IsHitFrom_ThrowingStar { get; set; }
        public bool IsHitFrom_LazerBolt { get; set; }
    }

    [System.Serializable]
    public class CrowdControlState
    {
        [field: SerializeField] public string Tag { get; set; } = string.Empty;
        [field: SerializeField] public bool IsOn { get; set; } = false;
    }

    public class CreatureController : BaseController, IHitFrom
    {
        // +++ BASE CHILD OBJECTS +++
        public Transform Indicator { get; protected set; } = null;
        public Vector3 IndicatorPosition => Indicator.transform.position;
        public Transform FireSocket { get; protected set; } = null;
        public Vector3 FireSocketPosition => FireSocket.transform.position;
        public virtual Vector3 ShootDir => (FireSocketPosition - IndicatorPosition).normalized;

        public Transform AnimTransform { get; protected set; } = null;
        public Transform Center { get; protected set; } = null;
        public SpriteRenderer[] SPRs { get; protected set; } = null;
        public Vector3 LocalScale
        {
            get => AnimTransform.transform.localScale;
            set => AnimTransform.transform.localScale = value;
        }
        //public bool IsFacingRight => (LocalScale.x != LocalScale.x * -1f) ? true : false;
        public bool IsFacingRight => this.LookAtDir == Define.LookAtDirection.Right;
        public System.Action<Define.LookAtDirection> OnLookAtDirChanged = null;

        // +++ BASE COMPONENTS +++
        public AnimationCallback AnimCallback { get; protected set; } = null;
        public BaseAnimationController AnimController { get; protected set; } = null;
        public Rigidbody2D RigidBody { get; protected set; } = null;
        public Collider2D HitCollider { get; protected set; } = null;

        // +++ STAT +++
        [field: SerializeField] public CreatureStat Stat { get; protected set; } = null;
        public void UpdateCreatureStat(int templateID)
            => this.Stat = Stat.UpgradeStat(this, Stat, templateID);
        public bool IsInvincible { get; set; } = false;

        // +++ CROWD CONTROL +++
        public const float ORIGIN_SPEED_MODIFIER = 1f;
        public virtual float SpeedModifier { get; set; } = 1f;
        public virtual void ResetSpeedModifier() => SpeedModifier = ORIGIN_SPEED_MODIFIER;
        private const int FIRST_CROWD_CONTROL_ID = 300100;
        [SerializeField] private CrowdControlState[] _ccStates = null;
        public virtual bool this[CrowdControl crowdControlType]
        {
            get => _ccStates[(int)crowdControlType % FIRST_CROWD_CONTROL_ID].IsOn;
            set => _ccStates[(int)crowdControlType % FIRST_CROWD_CONTROL_ID].IsOn = value;
        }

        public bool IsCCStates(params CrowdControl[] ccTypes)
        {
            for (int i = 0; i < ccTypes.Length; ++i)
            {
                if (ccTypes[i] == CrowdControl.None || ccTypes[i] == CrowdControl.MaxCount)
                    return false;

                if (this[ccTypes[i]])
                    return true;
            }

            return false;
        }

        // +++ SKILLS +++
        public SkillBook SkillBook { get; protected set; } = null;
        public Define.SkillAnimationType SkillAnimationType { get; protected set; } = Define.SkillAnimationType.None;
        public void ReserveSkillAnimationType(Define.SkillAnimationType animType) => this.SkillAnimationType = animType;

        // +++ RENDERERS +++
        public RendererController RendererController { get; protected set; } = null;

        // +++ FIELD, PROPERTY, METHODS +++
        [SerializeField] private Define.CreatureState _cretureState = Define.CreatureState.Idle;
        public Define.CreatureState CreatureState { get => _cretureState; set { _cretureState = value; UpdateAnimation(); } }

        public virtual void UpdateAnimation() { }

        // FSM : IDLE
        protected virtual void UpdateIdle() { }
        protected Coroutine _coIdleTick = null;
        protected virtual IEnumerator CoIdleTick() { yield return null; }
        public void StopIdleTick()
        {
            if (_coIdleTick != null)
                StopCoroutine(_coIdleTick);

            _coIdleTick = null;
        }

        // FSM : RUN
        protected virtual void UpdateRun() { }

        // FSM : SKILL
        protected virtual void UpdateSkill() { }
        //protected virtual void RunSkill() { }

        // FSM : DEAD
        protected virtual void OnDead()
        {
            this.HitCollider.enabled = false;
            this.RigidBody.simulated = false;
            this.Stat.Hp = 0f;
            this.SkillBook.DeactivateAll();
        }

        private Vector3 _moveDir = Vector3.zero;
        public Vector3 MoveDir { get => _moveDir; protected set { _moveDir = value.normalized; } }
        public bool IsMoving => _moveDir != Vector3.zero;

        public Vector3 AttackStartPoint { get; set; } = Vector3.zero;
        public Vector3 AttackEndPoint { get; set; } = Vector3.zero;
        public float GetMovementPower => (AttackEndPoint - AttackStartPoint).magnitude;

        public Define.LookAtDirection LookAtDir { get; protected set; } = Define.LookAtDirection.Right;
        protected Vector3 _baseRootLocalScale = Vector3.zero;
        public PlayerController MainTarget { get; protected set; } = null;

        // +++ MAIN METHODS +++
        public override void Init(int templateID)
        {
            if (Managers.Data.CreaturesDict.TryGetValue(templateID, out Data.InitialCreatureData creatureData) == false)
                Utils.LogCritical(nameof(CreatureController), nameof(Init), $"TemplateID : {templateID}");

            InitChildObject();
            InitBaseComponents();
            InitCrowdControlStates();

            InitCreatureStat(creatureData);
            InitCreatureSkills(creatureData);
            InitCreatureRenderer(creatureData);
            _baseRootLocalScale = transform.localScale;

            if (Center == null)
                Utils.LogCritical(nameof(CreatureController), nameof(Init), "You have to set \"Center\" in child before starting game.");
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

        private void InitCrowdControlStates()
        {
            CrowdControl ccState = CrowdControl.Stun;
            _ccStates = new CrowdControlState[(int)CrowdControl.MaxCount];
            for (int i = 0; i < (int)CrowdControl.MaxCount; ++i, ++ccState)
            {
                _ccStates[i] = new CrowdControlState();
                _ccStates[i].Tag = ccState.ToString();
            }
        }

        protected void InitCreatureStat(int templateID)
        {
            if (Managers.Data.CreaturesDict.TryGetValue(templateID, out Data.InitialCreatureData creatureData) == false)
                Utils.LogCritical(nameof(CreatureController), nameof(Init), $"TemplateID : {templateID}");

            this.Stat = new CreatureStat(this, creatureData);
        }

        protected virtual void InitCreatureSkills(Data.InitialCreatureData creatureData)
        {
            if (SkillBook == null)
            {
                SkillBook = gameObject.GetComponentInChildren<SkillBook>();
                SkillBook.Owner = this;
            }

            LoadActionSkills(creatureData);
            LoadDefaultSkills(creatureData);
            this.SkillBook.LateInit();
        }

        protected virtual void LateInit() { }
        protected virtual void EnterInGame(int templateID) { }
        public bool IsCompleteReadyToAction { get; protected set; } = false;
        protected Coroutine _coReadyToAction = null;
        public virtual void ReadyToAction(bool onStartImmediately = false)
        {
            if (_coReadyToAction != null)
                StopCoroutine(_coReadyToAction);

            _coReadyToAction = StartCoroutine(CoReadyToAction(onStartImmediately));
        }
        protected virtual IEnumerator CoReadyToAction(bool onStartImmediately = false) { yield return null; }

        private void LoadActionSkills(Data.InitialCreatureData creatureData)
        {
            GameObject goActionSkills = new GameObject { name = "@ActionSkills" };
            goActionSkills.transform.SetParent(SkillBook.transform);
            goActionSkills.transform.localPosition = Vector3.zero;
            goActionSkills.transform.localScale = Vector3.one;
            foreach (Define.TemplateIDs.Status.Skill templateOrigin in creatureData.ActionSkillList)
            {
                int templateID = (int)templateOrigin;
                if (Managers.Data.SkillsDict.TryGetValue(templateID, out SkillData dataOrigin) == false)
                    Utils.LogCritical(nameof(CreatureController), nameof(LoadActionSkills), $"TemplateID : {templateID}");

                for (int i = templateID; i < templateID + (int)dataOrigin.MaxGrade; ++i)
                {
                    SkillData data = Managers.Data.SkillsDict[i];
                    GameObject go = Managers.Resource.Instantiate(data.PrimaryLabel);
                    go.transform.SetParent(goActionSkills.transform);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;

                    ActionSkill actionSkill = go.GetComponent<ActionSkill>();
                    actionSkill.InitOrigin(this, data);
                    SkillBook.SkillGroupsDict.AddGroup(templateID, new SkillGroup(actionSkill));
                }
            }
        }

        private void LoadDefaultSkills(Data.InitialCreatureData creatureData)
        {
            GameObject goDefaultSkills = new GameObject { name = "@DefaultSkills " };
            goDefaultSkills.transform.SetParent(SkillBook.transform);
            goDefaultSkills.transform.localPosition = Vector3.zero;
            goDefaultSkills.transform.localScale = Vector3.one;
            foreach (Define.TemplateIDs.Status.Skill templateOrigin in creatureData.DefaultSkillList)
            {
                int templateID = (int)templateOrigin;
                if (Managers.Data.SkillsDict.TryGetValue(templateID, out Data.SkillData dataOrigin) == false)
                    Utils.LogCritical(nameof(CreatureController), nameof(LoadDefaultSkills), $"TemplateID : {templateID}");

                for (int i = templateID; i < templateID + (int)dataOrigin.MaxGrade; ++i)
                {
                    SkillData data = Managers.Data.SkillsDict[i];
                    GameObject go = Managers.Resource.Instantiate(data.PrimaryLabel);
                    go.transform.SetParent(goDefaultSkills.transform);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;

                    DefaultSkill defaultSkill = go.GetComponent<DefaultSkill>();
                    defaultSkill.InitOrigin(this, data);
                    SkillBook.SkillGroupsDict.AddGroup(i, new SkillGroup(defaultSkill));
                }
            }
        }

        protected virtual void InitCreatureRenderer(Data.InitialCreatureData creatureData)
        {
            if (RendererController == null)
            {
                RendererController = gameObject.GetOrAddComponent<RendererController>();
                RendererController.InitRendererController(this, creatureData);
                SetSortingOrder();
            }
        }

        public virtual void ShowMuzzle() { }

        public virtual void OnDamaged(CreatureController attacker, SkillBase from)
        {
            if (this.IsValid() == false)
                return;

            if (this.IsInvincible)
            {
                Managers.VFX.ImpactHit(VFXImpact.Incinvible, this, from); // --> 메모리 문제 발생시, 크리티컬 쪽에서 스폰
                return;
            }

            // TUPLE
            (float dmgResult, bool isCritical) = Managers.Game.TakeDamage(this, attacker, from);
            if (dmgResult == -1f && isCritical == false)
            {
                // DODGE
                Managers.VFX.Material(Define.MaterialType.Hologram, this);
                Managers.VFX.Environment(VFXEnv.Dodge, this);
                return;
            }

            // CHECK SHIELD OR NOT
            if (this.SkillBook.IsOnShield)
            {
                this.Stat.ShieldHp -= dmgResult;
                if (this.Stat.ShieldHp < 0f)
                {
                    this.SkillBook.OffSheild();
                    //this.SkillBook.Deactivate(SkillTemplate.Shield);
                }

                this.SkillBook.HitShield();
            }
            else
                this.Stat.Hp -= dmgResult;
            Managers.VFX.Damage(this, dmgResult, isCritical);
            Managers.VFX.ImpactHit(from.Data.VFX_ImpactHit, this, from); // --> 메모리 문제 발생시, 크리티컬 쪽에서 스폰

            if (this.Stat.Hp <= 0 && this.IsDeadState == false)
            {
                if (this.SkillBook.IsReadySecondWind)
                {
                    this.IsInvincible = true;
                    this.SkillBook.OnSecondWind();
                }
                else
                    this.CreatureState = Define.CreatureState.Dead;
            }
            else
            {
                // Crowd Control (CC)
                if (Managers.Game.TryCrowdControl(from))
                    Managers.CrowdControl.Apply(this, from);

                if (this.SkillBook.IsOnShield == false)
                    Managers.VFX.Material(Define.MaterialType.Hit, this);
            }
        }

        // +++ UTILS +++
        public bool IsPlayer() => this.ObjectType == Define.ObjectType.Player;
        public bool IsMonster() => this.ObjectType == Define.ObjectType.Monster;

        public bool IsHitFrom_ThrowingStar { get; set; } = false;
        public bool IsHitFrom_LazerBolt { get; set; } = false;

        // FOR RESPAWN OR ANOTHER
        public void ClearHitFroms()
        {
            this.IsHitFrom_ThrowingStar = false;
            this.IsHitFrom_LazerBolt = false;
        }

        public void ResetHitFrom(Define.HitFromType hitFromType, float delay)
        {
            if (this.IsValid() == false)
                return;

            if (delay > 0f)
                StartCoroutine(CoResetHitFrom(hitFromType, delay));
            else
            {
                switch (hitFromType)
                {
                    case Define.HitFromType.ThrowingStar:
                        this.IsHitFrom_ThrowingStar = false;
                        Utils.Log($"{this.gameObject.name}, Reset HitFrom : ThrowingStar.");
                        break;

                    case Define.HitFromType.LazerBolt:
                        this.IsHitFrom_LazerBolt = false;
                        Utils.Log($"{this.gameObject.name}, Reset HitFrom : LazerBolt.");
                        break;
                }
            }
        }

        private IEnumerator CoResetHitFrom(Define.HitFromType hitFromType, float delay)
        {
            yield return new WaitForSeconds(delay);
            switch (hitFromType)
            {
                case Define.HitFromType.ThrowingStar:
                    this.IsHitFrom_ThrowingStar = false;
                    Utils.Log($"{this.gameObject.name}, Reset HitFrom : ThrowingStar.");
                    break;

                case Define.HitFromType.LazerBolt:
                    this.IsHitFrom_LazerBolt = false;
                    Utils.Log($"{this.gameObject.name}, Reset HitFrom : LazerBolt.");
                    break;
            }
        }

        // UTILITIES
        public bool IsInLimitMaxPosX => Mathf.Abs(transform.position.x - Managers.Stage.RightTop.x) < Mathf.Epsilon ||
                        Mathf.Abs(transform.position.x - Managers.Stage.LeftBottom.x) < Mathf.Epsilon;

        public bool IsInLimitMaxPosY => Mathf.Abs(transform.position.y - Managers.Stage.RightTop.y) < Mathf.Epsilon ||
                                        Mathf.Abs(transform.position.y - Managers.Stage.LeftBottom.y) < Mathf.Epsilon;

        public virtual float ADDITIONAL_SPAWN_WIDTH { get; protected set; } = 0f;
        public virtual float ADDITIONAL_SPAWN_HEIGHT { get; protected set; } = 0f;

        protected virtual float ReadyToActionCompleteTime() => 1f;
        public virtual Vector3 LoadVFXEnvSpawnScale(VFXEnv templateOrigin) => Vector3.one;
        public virtual Vector3 LoadVFXEnvSpawnPos(VFXEnv templateOrigin) => this.Center.transform.position;

        public bool IsIdleState => this.CreatureState == Define.CreatureState.Idle && (this.Stat.Hp > 0);
        public bool IsRunState => this.CreatureState == Define.CreatureState.Run && (this.Stat.Hp > 0);
        public bool IsSkillState => this.CreatureState == Define.CreatureState.Skill && (this.Stat.Hp > 0);
        public bool IsDeadState => this.CreatureState == Define.CreatureState.Dead && (this.Stat.Hp <= 0);

        public void SetDefaultHead() => this.RendererController.OnFaceDefaultHandler();
        public void SetBattleHead() => this.RendererController.OnFaceBattleHandler();
        public void SetDeadHead() => this.RendererController.OnFaceDeadHandler();

        public void RequestCrowdControl(SkillBase from)
        {
            CrowdControl ccType = from.Data.CrowdControlType;
            switch (ccType)
            {
                case CrowdControl.None:
                    return;

                case CrowdControl.Stun:
                    {
                        if (this[ccType] == false)
                        {
                            StartCoroutine(Managers.CrowdControl.CoStun(this, from));
                            TryContinuousCrowControl(this, from);
                        }
                        else
                            Utils.Log("Already Stun,,,");
                    }
                    break;

                case CrowdControl.Slow:
                    {
                        if (this[ccType] == false)
                        {
                            StartCoroutine(Managers.CrowdControl.CoSlow(this, from));
                            TryContinuousCrowControl(this, from);
                        }
                        else
                            Utils.Log("Already Slow,,,");
                    }
                    break;

                case CrowdControl.KnockBack:
                    {
                        if (this[ccType] == false)
                            StartCoroutine(Managers.CrowdControl.CoKnockBack(this, from));
                        else
                            Utils.Log("Already KnockBack,,,");
                    }
                    break;

                case CrowdControl.Slience:
                    {
                        if (this[ccType] == false)
                        {
                            StartCoroutine(Managers.CrowdControl.CoSilence(this, from));
                        }
                        else
                            Utils.Log("Already Silence,,,");
                    }
                    break;
            }
        }

        private void TryContinuousCrowControl(CreatureController target, SkillBase from)
        {
            if (Managers.Game.TryCrowdControl(from.Data.ContinuousCrowdControlRatio) == false)
                return;

            CrowdControl continuousCCType = from.Data.ContinuousCrowdControlType;
            switch (continuousCCType)
            {
                case CrowdControl.None:
                    return;

                case CrowdControl.Slience:
                    {
                        if (this[continuousCCType] == false)
                        {
                            Managers.VFX.ImpactHit(from.Data.VFX_ImpactHit_ForContinuousCrowdControl, target, from);
                            StartCoroutine(Managers.CrowdControl.CoSilence(this, from, true));
                        }
                        else
                            Utils.Log("Already Silence,,,");
                    }
                    break;

                case CrowdControl.KnockBack:
                    {
                        if (this[continuousCCType] == false)
                        {
                            StartCoroutine(Managers.CrowdControl.CoKnockBack(this, from, true));
                        }
                        else
                            Utils.Log("Already KnockBack,,,");
                    }
                    break;
            }

            // CHECK SUB CROWD CONTROL
            // if (from.Data.ContinuousCrowdControlType != CrowdControl.None && 
            //     Managers.Game.TryCrowdControl(from.Data.ContinuousCrowdControlRatio))
            // {
            //     if (this[from.Data.ContinuousCrowdControlType] == false)
            //         StartCoroutine(Managers.CrowdControl.)
            //     else
            //         Utils.Log($"Already {from.Data.ContinuousCrowdControlType},,,");
            // }
        }

        public void ClearCrowdControlStates()
        {
            for (int i = 0; i < _ccStates.Length; ++i)
                _ccStates[i].IsOn = false;
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

// public void UpgradeBonusBuff(SkillBase skill, int templateID)
// {
//     Data.BuffSkillData buffSkillData = Managers.Data.BuffSkillsDict[templateID];
//     string label = buffSkillData.PrimaryLabel;
//     GameObject go = Managers.Resource.Instantiate(label, pooling: false);
//     BuffBase buff = go.GetComponent<BuffBase>();
//     buff.StartBuff(this, skill, buffSkillData);
//     this.Buff = buff;
// }

// Indexer
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

// ------------------------------------------------------------------------------------------
// ------------------------------------------------------------------------------------------
// int templateID = (int)templateOrigin; // 200200
// if (Managers.Data.SkillsDict.TryGetValue(templateID, out Data.SkillData value) == false)
//     Utils.LogCritical(nameof(CreatureController), nameof(LoadRepeatSkills), $"TemplateID : {templateID}");

// if (value.Grade == value.MaxGrade)
// {
//     GameObject go = Managers.Resource.Instantiate(value.PrimaryLabel);
//     //go.name = $"{go.name}_[{value.TemplateID}]";
//     go.transform.SetParent(goRepeatSkills.transform);

//     RepeatSkill repeatSkill = go.GetComponent<RepeatSkill>();
//     repeatSkill.InitOrigin(this, value);
//     SkillBook.SkillGroupsDict.AddGroup(templateID, new SkillGroup(repeatSkill));
// }
// else
// {
//     for (int i = templateID; i < templateID + (int)value.MaxGrade; ++i)
//     {
//         if (Managers.Data.SkillsDict.TryGetValue(i, out Data.SkillData data) == false)
//             Utils.LogCritical(nameof(CreatureController), nameof(LoadRepeatSkills), $"TemplateID : {templateID}");

//         GameObject go = Managers.Resource.Instantiate(data.PrimaryLabel);
//         //go.name = $"{go.name}_[{data.TemplateID}]";
//         go.transform.SetParent(goRepeatSkills.transform);

//         RepeatSkill repeatSkill = go.GetComponent<RepeatSkill>();
//         repeatSkill.InitOrigin(this, data);
//         SkillBook.SkillGroupsDict.AddGroup(i, new SkillGroup(repeatSkill));
//     }
// }

// if (value.Grade == value.MaxGrade)
// {
//     GameObject go = Managers.Resource.Instantiate(value.PrimaryLabel);
//     go.transform.SetParent(goSequenceSkills.transform);
//     go.transform.localPosition = Vector3.zero;
//     go.transform.localScale = Vector3.one;

//     SequenceSkill sequenceSkill = go.GetComponent<SequenceSkill>();
//     sequenceSkill.InitOrigin(this, value);
//     SkillBook.SkillGroupsDict.AddGroup(templateID, new SkillGroup(sequenceSkill));
// }
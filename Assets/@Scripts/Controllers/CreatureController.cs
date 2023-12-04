using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_2D.Define;
using CrowdControlType = STELLAREST_2D.Define.FixedValue.TemplateID.CrowdControl;
using STELLAREST_2D.Data;

namespace STELLAREST_2D
{
    public interface IHitFrom
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
        public Transform Indicator { get; protected set; } = null;
        public Vector3 IndicatorPosition => Indicator.transform.position;
        public Transform FireSocket { get; protected set; } = null;
        public Vector3 FireSocketPosition => FireSocket.transform.position;
        public virtual Vector3 ShootDir => (FireSocketPosition - IndicatorPosition).normalized;
        public Transform AnimTransform { get; protected set; } = null;
        public Transform Center { get; protected set; } = null;
        public Vector3 LocalScale
        {
            get => AnimTransform.transform.localScale;
            set => AnimTransform.transform.localScale = value;
        }
        public bool IsFacingRight => this.LookAtDir == Define.LookAtDirection.Right;
        public System.Action<Define.LookAtDirection> OnLookAtDirChanged = null;
        public AnimationCallback AnimCallback { get; protected set; } = null;
        public CreatureAnimationController CreatureAnimController { get; protected set; } = null;
        public Rigidbody2D RigidBody { get; protected set; } = null;
        public Collider2D HitCollider { get; protected set; } = null;
        [field: SerializeField] public CreatureStat Stat { get; protected set; } = null;

        public bool IsInvincible { get; set; } = false;

        // ************** TODO *************** 고쳐야함 개판임
        public const float ORIGIN_SPEED_MODIFIER = 1F;
        public virtual float SpeedModifier { get; set; } = 1f;
        public virtual void ResetSpeedModifier() => SpeedModifier = ORIGIN_SPEED_MODIFIER;
        
        private const int FIRST_CROWD_CONTROL_ID = 300100;
        [SerializeField] private CrowdControlState[] _ccStates = null;
        public virtual bool this[CrowdControlType crowdControlType]
        {
            get => _ccStates[(int)crowdControlType % FIRST_CROWD_CONTROL_ID].IsOn;
            set => _ccStates[(int)crowdControlType % FIRST_CROWD_CONTROL_ID].IsOn = value;
        }

        public SkillBook SkillBook { get; protected set; } = null;

        public FixedValue.TemplateID.SkillAnimation CreatureSkillAnimType { get; set; } =  FixedValue.TemplateID.SkillAnimation.None;
        [SerializeField] private CreatureState _cretureState = CreatureState.Idle;
        public Define.CreatureState CreatureState { get => _cretureState; set { _cretureState = value; UpdateAnimation(); } }
        protected virtual void UpdateAnimation() { }
        protected virtual void UpdateIdle() { }
        protected virtual void UpdateRun() { }
        protected virtual void UpdateSkill() { }
        protected virtual void OnDead()
        {
            this.HitCollider.enabled = false;
            this.RigidBody.simulated = false;
            this.Stat.HP = 0f;
            this.SkillBook.DeactivateAll();
        }

        private Vector3 _moveDir = Vector3.zero;
        public Vector3 MoveDir { get => _moveDir; protected set { _moveDir = value.normalized; } }

        private Vector3 _lastMovementDir = Vector3.right;
        public Vector3 LastMovementDir { get => _lastMovementDir; protected set{ _lastMovementDir = value.normalized; } }

        public bool IsMoving => _moveDir != Vector3.zero;

        public Vector3 AttackStartPoint { get; set; } = Vector3.zero;
        public Vector3 AttackEndPoint { get; set; } = Vector3.zero;
        public float GetMovementPower => (AttackEndPoint - AttackStartPoint).magnitude;

        public Define.LookAtDirection LookAtDir { get; protected set; } = Define.LookAtDirection.Right;
        protected Vector3 _baseRootLocalScale = Vector3.zero;
        public PlayerController MainTarget { get; protected set; } = null;

        public override void Init(int templateID)
        {
            if (Managers.Data.CreaturesDict.TryGetValue(templateID, out CreatureData creatureData) == false)
                Utils.LogCritical(nameof(CreatureController), nameof(Init), $"Input : {templateID}");

            InitChild(templateID);
            InitBaseComponents();
            InitCrowdControlStates();

            InitCreatureStat(creatureData);
            InitCreatureSkills(creatureData);
            InitRendererController(creatureData);

            _baseRootLocalScale = transform.localScale;

            if (Center == null)
                Utils.LogCritical(nameof(CreatureController), nameof(Init), "You have to set \"Center\" in hierarchy before starting game.");
        }

        protected virtual void InitChild(int templateID)
        {
            Indicator = Utils.FindChild<Transform>(this.gameObject, FixedValue.Find.INDICATOR, true);
            FireSocket = Utils.FindChild<Transform>(this.gameObject, FixedValue.Find.FIRE_SOCKET, true);
            AnimTransform = Utils.FindChild<Transform>(this.gameObject, FixedValue.Find.ANIMATION_BODY, true);
        }

        protected virtual void InitBaseComponents()
        {
            if (AnimCallback == null)
            {
                AnimCallback = AnimTransform.GetComponent<AnimationCallback>();
                AnimCallback.Init(this);
            }

            if (CreatureAnimController == null)
            {
                CreatureAnimController = AnimTransform.GetComponent<CreatureAnimationController>();
                CreatureAnimController.Init(this);
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

        protected virtual void InitCreatureStat(CreatureData creatureData)
            => Stat = new CreatureStat(this, creatureData);

        private void InitCrowdControlStates()
        {
            CrowdControlType ccState = CrowdControlType.Stun;
            _ccStates = new CrowdControlState[(int)CrowdControlType.MaxCount];
            for (int i = 0; i < (int)CrowdControlType.MaxCount; ++i, ++ccState)
            {
                _ccStates[i] = new CrowdControlState();
                _ccStates[i].Tag = ccState.ToString();
            }
        }

        protected void InitCreatureStat(int templateID)
        {
            if (Managers.Data.CreaturesDict.TryGetValue(templateID, out CreatureData creatureData) == false)
                Utils.LogCritical(nameof(CreatureController), nameof(Init), $"Input : {templateID}");

            this.Stat = new CreatureStat(this, creatureData);
        }

        protected virtual void InitCreatureSkills(CreatureData creatureData)
        {
            if (SkillBook == null)
            {
                SkillBook = gameObject.GetComponentInChildren<SkillBook>();
                SkillBook.Owner = this;
            }

            LoadUniqueSkills(creatureData);
            LoadPublicSkills(creatureData);
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

        private void LoadUniqueSkills(CreatureData creatureData)
        {
            GameObject goUniqueSkills = new GameObject { name = "@UniqueSkills" };
            goUniqueSkills.transform.SetParent(SkillBook.transform);
            goUniqueSkills.transform.localPosition = Vector3.zero;
            goUniqueSkills.transform.localScale = Vector3.one;
            foreach (FixedValue.TemplateID.Skill uniqueSkillTemplateID in creatureData.UniqueSkills)
            {
                int templateID = (int)uniqueSkillTemplateID;
                if (Managers.Data.SkillsDict.TryGetValue(templateID, out SkillData dataOrigin) == false)
                    Utils.LogCritical(nameof(CreatureController), nameof(LoadUniqueSkills), $"Input : {templateID}");

                for (int i = templateID; i < templateID + (int)dataOrigin.MaxGrade; ++i)
                {
                    SkillData data = Managers.Data.SkillsDict[i];
                    GameObject go = Managers.Resource.Instantiate(data.PrimaryLabel);
                    go.transform.SetParent(goUniqueSkills.transform);
                    go.transform.localPosition = Vector3.zero;
                    if (data.UsePresetLocalScale == false)
                        go.transform.localScale = Vector3.one;

                    UniqueSkill actionSkill = go.GetComponent<UniqueSkill>();
                    actionSkill.InitOrigin(this, data);
                    SkillBook.SkillGroupsDict.AddGroup(templateID, new SkillGroup(actionSkill));
                }
            }
        }

        private void LoadPublicSkills(CreatureData creatureData)
        {
            GameObject goPublicSkills = new GameObject { name = "@PublicSkills " };
            goPublicSkills.transform.SetParent(SkillBook.transform);
            goPublicSkills.transform.localPosition = Vector3.zero;
            goPublicSkills.transform.localScale = Vector3.one;
            foreach (FixedValue.TemplateID.Skill publicSkillTemplateID in creatureData.PublicSkills)
            {
                int templateID = (int)publicSkillTemplateID;
                if (Managers.Data.SkillsDict.TryGetValue(templateID, out Data.SkillData dataOrigin) == false)
                    Utils.LogCritical(nameof(CreatureController), nameof(LoadPublicSkills), $"Input : {templateID}");

                for (int i = templateID; i < templateID + (int)dataOrigin.MaxGrade; ++i)
                {
                    SkillData data = Managers.Data.SkillsDict[i];
                    GameObject go = Managers.Resource.Instantiate(data.PrimaryLabel);
                    go.transform.SetParent(goPublicSkills.transform);
                    go.transform.localPosition = Vector3.zero;
                    if (data.UsePresetLocalScale == false)
                        go.transform.localScale = Vector3.one;

                    PublicSkill defaultSkill = go.GetComponent<PublicSkill>();
                    defaultSkill.InitOrigin(this, data);
                    SkillBook.SkillGroupsDict.AddGroup(i, new SkillGroup(defaultSkill));
                }
            }
        }

        public CreatureRendererController CreatureRendererController { get; private set; } = null;
        public void InitRendererController(CreatureData creatureData)
        {
            if (RendererController == null)
            {
                RendererController = gameObject.GetOrAddComponent<RendererController>();
                RendererController.InitRendererController(this, creatureData);
                SetSortingOrder();
                CreatureRendererController = RendererController.GetComponent<CreatureRendererController>();
            }
        }

        public virtual void ShowMuzzle() { }
        public virtual void OnDamaged(CreatureController attacker, SkillBase from)
        {
            if (this.IsValid() == false)
                return;

            if (this.IsInvincible || this.SkillBook.IsOnBarrier)
            {
                Managers.VFX.ImpactHit(FixedValue.TemplateID.VFX.ImpactHit.Incinvible, this, from);
                if (this.SkillBook.IsOnBarrier)
                    this.SkillBook.HitBarrier();
                
                return;
            }

            // TUPLE
            (float dmgResult, bool isCritical) = Managers.Game.TakeDamage(this, attacker, from);
            if (dmgResult == -1f && isCritical == false)
            {
                // DODGE
                //Managers.VFX.Material(Define.MaterialType.Hologram, this);
                StartCoroutine(Managers.VFX.CoMatHologram(this, 
                                    startCallback: () => this.CreatureRendererController.HideFace(true), 
                                    endCallback: () => this.CreatureRendererController.HideFace(false)));

                Managers.VFX.Environment(VFXEnvType.Dodge, this);
                return;
            }

            // CHECK SHIELD OR NOT
            if (this.SkillBook.IsOnShield)
            {
                this.Stat.ShieldHP -= dmgResult;
                if (this.Stat.ShieldHP < 0f)
                {
                    this.SkillBook.OffSheild();
                }

                this.SkillBook.HitShield();
            }
            else
                this.Stat.HP -= dmgResult;

            Managers.VFX.Damage(this, dmgResult, isCritical);
            Managers.VFX.ImpactHit(from.Data.VFX_ImpactHit_TemplateID, this, from); // --> 메모리 문제 발생시, 크리티컬 쪽에서 스폰 체크

            if (this.Stat.HP <= 0 && this.IsDeadState == false)
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
                {
                    //Managers.VFX.Material(Define.MaterialType.Hit, this);
                    StartCoroutine(Managers.VFX.CoMatHit(this, 
                                    startCallback: () => this.CreatureRendererController.HideFace(true), 
                                    endCallback: () => this.CreatureRendererController.HideFace(false)));
                }
            }
        }

        public void OnFixedDamaged(float fixedDamage, CrowdControlType vfxForDamage = CrowdControlType.None)
        {
            if (this.IsValid() == false || this.IsDeadState)
                return;

            this.Stat.HP -= fixedDamage;
            if (this.Stat.HP <= 0)
                this.CreatureState = Define.CreatureState.Dead;
            
            switch (vfxForDamage)
            {
                case CrowdControlType.None:
                    return;

                case CrowdControlType.Poison:
                {
                    Managers.VFX.PoisonDamage(this, fixedDamage);
                    break;
                }
            }
        }

        // +++ UTILS +++
        public bool IsPlayer => this.ObjectType == Define.ObjectType.Player;
        public bool IsMonster => this.ObjectType == Define.ObjectType.Monster;
        public bool IsHitFrom_ThrowingStar { get; set; } = false;
        public bool IsHitFrom_LazerBolt { get; set; } = false;

        public void ClearHitFrom()
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
        
        public virtual Vector3 LoadVFXEnvSpawnScale(VFXEnvType vfxEnvType) => Vector3.one;
        public virtual Vector3 LoadVFXEnvSpawnPos(VFXEnvType vfxEnvType) => this.Center.transform.position;

        public bool IsIdleState => this.CreatureState == Define.CreatureState.Idle && (this.Stat.HP > 0);
        public bool IsRunState => this.CreatureState == Define.CreatureState.Run && (this.Stat.HP > 0);
        public bool IsSkillState => this.CreatureState == Define.CreatureState.Skill && (this.Stat.HP > 0);
        public bool IsDeadState => this.CreatureState == Define.CreatureState.Dead && (this.Stat.HP <= 0);

        public void SetDefaultHead() => this.RendererController.OnFaceDefaultHandler();
        public void SetBattleHead() => this.RendererController.OnFaceCombatHandler();
        public void SetDeadHead() => this.RendererController.OnFaceDeadHandler();

        public void RequestApplyingCrowdControl(SkillBase from)
        {
            // TryContinuousCrowControl
            // 연속된 CC기 이므로, 이전 CC기가 걸려야 다음 CC기가 걸릴지 여부를 판단
            CrowdControlType ccType = from.Data.CrowdControlTemplateID;
            switch (ccType)
            {
                case CrowdControlType.None:
                    return;

                case CrowdControlType.Stun:
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

                case CrowdControlType.Slow:
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

                case CrowdControlType.KnockBack:
                    {
                        if (this[ccType] == false)
                        {
                            StartCoroutine(Managers.CrowdControl.CoKnockBack(this, from));
                            TryContinuousCrowControl(this, from);
                        }
                        else
                            Utils.Log("Already KnockBack,,,");
                    }
                    break;

                case CrowdControlType.Slience:
                    {
                        if (this[ccType] == false)
                        {
                            StartCoroutine(Managers.CrowdControl.CoSilence(this, from));
                            TryContinuousCrowControl(this, from);
                        }
                        else
                            Utils.Log("Already Silence,,,");
                    }
                    break;

                case CrowdControlType.Targeted:
                    {
                        if (this[ccType] == false)
                        {
                            StartCoroutine(Managers.CrowdControl.CoTargeted(this, from));
                            TryContinuousCrowControl(this, from);
                        }
                        else
                            Utils.Log("Already Targeted,,,");
                    }
                    break;

                case CrowdControlType.Poison:
                    {
                        if (this[ccType] == false)
                        {
                            Utils.Log("Apply Poison.");
                            StartCoroutine(Managers.CrowdControl.CoPoisoning(this, from));
                            TryContinuousCrowControl(this, from);
                        }
                        else
                            Utils.Log("Already Poison,,,");
                    }
                    break;
            }
        }

        private void TryContinuousCrowControl(CreatureController target, SkillBase from)
        {
            if (Managers.Game.TryCrowdControl(from.Data.ContinuousCrowdControlChance) == false)
                return;

            CrowdControlType continuousCCType = from.Data.ContinuousCrowdControlTemplateID;
            switch (continuousCCType)
            {
                case CrowdControlType.None:
                    return;

                case CrowdControlType.Stun:
                    {
                        if (this[continuousCCType] == false)
                        {
                            Managers.VFX.ImpactHit(from.Data.ContinuousCrowdControl_VFX_ImpactHit_TemplateID, target, from);
                            StartCoroutine(Managers.CrowdControl.CoStun(this, from, true));
                        }
                        else
                            Utils.Log("Already Stun,,,");
                    }
                    break;

                case CrowdControlType.Slow:
                    {
                        if (this[continuousCCType] == false)
                        {
                            Managers.VFX.ImpactHit(from.Data.ContinuousCrowdControl_VFX_ImpactHit_TemplateID, target, from);
                            StartCoroutine(Managers.CrowdControl.CoSlow(this, from, true));
                        }
                        else
                            Utils.Log("Already Slow,,,");
                    }
                    break;

                case CrowdControlType.KnockBack:
                    {
                        if (this[continuousCCType] == false)
                        {
                            Managers.VFX.ImpactHit(from.Data.ContinuousCrowdControl_VFX_ImpactHit_TemplateID, target, from);
                            StartCoroutine(Managers.CrowdControl.CoKnockBack(this, from, true));
                        }
                        else
                            Utils.Log("Already KnockBack,,,");
                    }
                    break;

                case CrowdControlType.Slience:
                    {
                        if (this[continuousCCType] == false)
                        {
                            Managers.VFX.ImpactHit(from.Data.ContinuousCrowdControl_VFX_ImpactHit_TemplateID, target, from);
                            StartCoroutine(Managers.CrowdControl.CoSilence(this, from, true));
                        }
                        else
                            Utils.Log("Already Silence,,,");
                    }
                    break;

                case CrowdControlType.Targeted:
                    {
                        if (this[continuousCCType] == false)
                        {
                            Managers.VFX.ImpactHit(from.Data.ContinuousCrowdControl_VFX_ImpactHit_TemplateID, target, from);
                            StartCoroutine(Managers.CrowdControl.CoTargeted(this, from, true));
                        }
                        else
                            Utils.Log("Already Targeted,,,");
                    }
                    break;

                case CrowdControlType.Poison:
                    {
                        if (this[continuousCCType] == false)
                        {
                            Managers.VFX.ImpactHit(from.Data.ContinuousCrowdControl_VFX_ImpactHit_TemplateID, target, from);
                            StartCoroutine(Managers.CrowdControl.CoPoisoning(this, from, true));
                        }
                        else
                            Utils.Log("Already Poison,,,");
                    }
                    break;

            }
        }

        public void ClearCrowdControlStates()
        {
            for (int i = 0; i < _ccStates.Length; ++i)
                _ccStates[i].IsOn = false;
        }
    }
}

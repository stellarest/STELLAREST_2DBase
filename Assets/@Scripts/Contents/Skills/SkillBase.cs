using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_2D.Define;
using STELLAREST_2D.Data;

namespace STELLAREST_2D
{
    [System.Serializable]
    public class AdditionalCustomValue
    {
        public bool Boolean;
#if UNITY_EDITOR
        public string Comment_Boolean;
#endif

        public List<int> Integers;
#if UNITY_EDITOR
        public List<string> Comment_Integers;
#endif

        public List<float> Floatings;
#if UNITY_EDITOR
        public List<string> Comment_Floatings;
#endif

        public Vector3 Point3D;
#if UNITY_EDITOR
        public string Comment_Point3D;
#endif
    }

    [System.Serializable]
    public class SkillBase : BaseController
    {
        public System.Action<Vector3, LookAtDirection, float, float, float> OnSetParticleInfo = null;
        public event System.EventHandler<ProjectileLaunchInfoEventArgs> OnProjectileLaunchInfo = null;
        public CreatureController Owner { get; protected set; } = null;

        [field: SerializeField] public SkillData Data { get; protected set; } = null;
        public float InitialCooldown { get; protected set; } = 0f;

        public ProjectileController PC { get; protected set; } = null;
        public SpriteRenderer SR { get; protected set; } = null;
        public Rigidbody2D RigidBody { get; protected set; } = null;
        public Collider2D HitCollider { get; protected set; } = null;
        public Vector3 HitPoint { get; protected set; } = Vector3.zero;

        public virtual void InitOrigin(CreatureController owner, SkillData data)
        {
            this.Owner = owner;
            this.Data = data;
            this.InitialCooldown = data.Cooldown;

            this.IsLearned = false;
            this.IsLast = false;
            _isCompleteInitOrigin = true;
        }

        public virtual void InitClone(CreatureController ownerFromOrigin, SkillData dataFromOrigin)
        {
            this.Owner = ownerFromOrigin;
            this.Data = dataFromOrigin;
            if (this.Data.IsProjectile)
            {
                this.PC = GetComponent<ProjectileController>();
                if (this.SR != null)
                {
                    this.PC.SR = this.SR;
                    SetSortingOrder();
                }

                if (this.RigidBody != null)
                    this.PC.RigidBody = this.RigidBody;

                if (this.HitCollider != null)
                    this.PC.HitCollider = this.HitCollider;

                this.PC.Owner = ownerFromOrigin;
                this.PC.Data = dataFromOrigin;
                this.OnProjectileLaunchInfo += this.PC.OnProjectileLaunchInfoHandler;
            }

            SetClonedRootTargetOnParticleStopped();
            if (ownerFromOrigin?.IsPlayer == true)
                Managers.Collision.InitCollisionLayer(gameObject, CollisionLayers.PlayerAttack);
            else
                Managers.Collision.InitCollisionLayer(gameObject, CollisionLayers.MonsterAttack);
        }

        public void InitCloneManually(CreatureController ownerFromOrigin, SkillData dataFromOrigin)
        {
            if (this.IsFirstPooling)
            {
                this.Owner = ownerFromOrigin;
                this.Data = dataFromOrigin;

                SR = GetComponentInChildren<SpriteRenderer>();
                RigidBody = GetComponent<Rigidbody2D>();
                HitCollider = GetComponent<Collider2D>();
                if (this.Data.IsProjectile)
                {
                    this.PC = GetComponent<ProjectileController>();
                    this.PC.SR = this.SR;
                    SetSortingOrder();

                    this.PC.RigidBody = this.RigidBody;
                    this.PC.HitCollider = this.HitCollider;

                    this.PC.Owner = ownerFromOrigin;
                    this.PC.Data = dataFromOrigin;
                    this.OnProjectileLaunchInfo += this.PC.OnProjectileLaunchInfoHandler;
                }

                SetClonedRootTargetOnParticleStopped();
                if (ownerFromOrigin?.IsPlayer == true)
                    Managers.Collision.InitCollisionLayer(gameObject, CollisionLayers.PlayerAttack);
                else
                    Managers.Collision.InitCollisionLayer(gameObject, CollisionLayers.MonsterAttack);

                this.IsFirstPooling = false;
            }
        }

        protected virtual IEnumerator CoGenerateProjectile()
        {
            if (this.Data.IsProjectile == false)
            {
                Utils.LogCritical(nameof(SkillBase), nameof(CoGenerateProjectile), $"Input(Data.IsProjectile) : {this.Data.IsProjectile}");
                yield break;
            }

            Define.LookAtDirection lootAtDir = Owner.LookAtDir;
            Vector3 shootDir = Owner.ShootDir;
            Vector3 localScale = transform.localScale;
            if (this.Data.UsePresetLocalScale == false)
            {
                localScale = Owner.LocalScale;
                localScale *= 0.8f;
            }
            Vector3 indicatorAngle = Owner.Indicator.eulerAngles;

            int count = this.Data.Count;
            if (count <= 0)
            {
                Utils.LogCritical(nameof(SkillBase), nameof(CoGenerateProjectile), $"Input(Data.Count) : {this.Data.Count}");
                yield break;
            }
            float spacing = this.Data.Spacing;
            float duration = this.Data.Duration;
            float movementSpeed = this.Data.MovementSpeed;
            float rotationSpeed = this.Data.RotationSpeed;
            int maxBounceCount = this.Data.MaxBounceCount;
            int maxPenetrationCount = this.Data.MaxPenetrationCount;

            float[] continuousAngles = new float[count];
            float[] continuousFlipXs = new float[count];
            float[] continuousFlipYs = new float[count];
            float[] addContinuousMovementSpeedRatios = new float[count];
            float[] addContinuousRotationSpeedRatios = new float[count];
            float[] interpolateTargetScaleXs = new float[count];
            float[] interpolateTargetScaleYs = new float[count];

            Vector3 additionalSpawnPoint = Vector3.zero;
            if ((this.Data.CustomValue != null) && (this.Data.CustomValue.Point3D != Vector3.zero))
                additionalSpawnPoint = this.Data.CustomValue.Point3D;

            if (count > 1)
            {
                for (int i = 0; i < count; ++i)
                {
                    continuousAngles[i] = this.Data.ContinuousAngles.Length > 1
                    ? (this.Owner.IsFacingRight ? this.Data.ContinuousAngles[i] : -this.Data.ContinuousAngles[i])
                    : 0f;

                    continuousFlipXs[i] = this.Data.ContinuousFlipXs.Length > 1
                    ? this.Data.ContinuousFlipXs[i]
                    : 0f;

                    continuousFlipYs[i] = this.Data.ContinuousFlipYs.Length > 1
                    ? this.Data.ContinuousFlipYs[i]
                    : 0f;

                    addContinuousMovementSpeedRatios[i] = this.Data.AddContinuousMovementSpeedRatios.Length > 1
                    ? this.Data.AddContinuousMovementSpeedRatios[i]
                    : 1f;

                    addContinuousRotationSpeedRatios[i] = this.Data.AddContinuousRotationSpeedRatios.Length > 1
                    ? this.Data.AddContinuousMovementSpeedRatios[i]
                    : 1f;

                    interpolateTargetScaleXs[i] = this.Data.TargetScaleInterpolations.Length > 1
                    ? (this.Owner.IsFacingRight ? this.Data.TargetScaleInterpolations[i].x : -this.Data.TargetScaleInterpolations[i].x)
                    : (this.Owner.IsFacingRight ? localScale.x : -localScale.x);

                    interpolateTargetScaleYs[i] = this.Data.TargetScaleInterpolations.Length > 1
                    ? this.Data.TargetScaleInterpolations[i].y
                    : localScale.y;
                }
                for (int i = 0; i < count; ++i)
                {
                    Vector3 spawnPos = (this.Data.IsOnFromFireSocket) ? this.Owner.FireSocketPosition : this.Owner.transform.position + additionalSpawnPoint;
                    SkillBase clone = Managers.Object.Spawn<SkillBase>(spawnPos: spawnPos, templateID: this.Data.TemplateID,
                            spawnObjectType: Define.ObjectType.Skill, isPooling: true);
                    clone.InitClone(this.Owner, this.Data);
                    if (clone.PC != null)
                    {
                        clone.OnProjectileLaunchInfo?.Invoke(this, new ProjectileLaunchInfoEventArgs(
                        lootAtDir: lootAtDir,
                        shootDir: shootDir,
                        indicatorAngle: indicatorAngle,
                        localScale: localScale,
                        angle: continuousAngles[i],
                        flipX: continuousFlipXs[i],
                        flipY: continuousFlipYs[i],
                        addMovementSpeedRatio: addContinuousMovementSpeedRatios[i],
                        addRotationSpeedRatio: addContinuousRotationSpeedRatios[i],
                        targetScaleX: interpolateTargetScaleXs[i],
                        targetScaleY: interpolateTargetScaleYs[i],
                        duration: duration,
                        movementSpeed: movementSpeed,
                        rotationSpeed: rotationSpeed,
                        maxBounceCount: maxBounceCount,
                        maxPenetrationCount: maxPenetrationCount
                        ));

                        clone.PC.Launch(this.Data.TemplateOrigin);
                    }

                    yield return new WaitForSeconds(spacing);
                }

                Owner.AttackEndPoint = transform.position;
            }
            else if (count == 1)
            {
                continuousAngles[0] = this.Data.ContinuousAngles.Length == 1
                    ? (this.Owner.IsFacingRight ? this.Data.ContinuousAngles[0] : -this.Data.ContinuousAngles[0])
                    : 0f;

                continuousFlipXs[0] = this.Data.ContinuousFlipXs.Length == 1
                    ? this.Data.ContinuousFlipXs[0]
                    : 0f;

                continuousFlipYs[0] = this.Data.ContinuousFlipYs.Length == 1
                    ? this.Data.ContinuousFlipYs[0]
                    : 0f;

                addContinuousMovementSpeedRatios[0] = this.Data.AddContinuousMovementSpeedRatios.Length == 1
                    ? this.Data.AddContinuousMovementSpeedRatios[0]
                    : 1f;

                addContinuousRotationSpeedRatios[0] = this.Data.AddContinuousRotationSpeedRatios.Length == 1
                    ? this.Data.AddContinuousRotationSpeedRatios[0]
                    : 1f;

                interpolateTargetScaleXs[0] = this.Data.TargetScaleInterpolations.Length == 1
                    ? (this.Owner.IsFacingRight ? this.Data.TargetScaleInterpolations[0].x : -this.Data.TargetScaleInterpolations[0].x)
                    : (this.Owner.IsFacingRight ? localScale.x : -localScale.x);

                interpolateTargetScaleYs[0] = this.Data.TargetScaleInterpolations.Length == 1
                    ? this.Data.TargetScaleInterpolations[0].y
                    : localScale.y;

                Vector3 spawnPos = (this.Data.IsOnFromFireSocket) ? this.Owner.FireSocketPosition : this.Owner.transform.position + additionalSpawnPoint;
                SkillBase clone = Managers.Object.Spawn<SkillBase>(spawnPos: spawnPos, templateID: this.Data.TemplateID,
                        spawnObjectType: Define.ObjectType.Skill, isPooling: true);
                clone.InitClone(this.Owner, this.Data);
                if (clone.PC != null)
                {
                    clone.OnProjectileLaunchInfo?.Invoke(this, new ProjectileLaunchInfoEventArgs(
                    lootAtDir: lootAtDir,
                    shootDir: shootDir,
                    indicatorAngle: indicatorAngle,
                    localScale: localScale,
                    angle: continuousAngles[0],
                    flipX: continuousFlipXs[0],
                    flipY: continuousFlipYs[0],
                    addMovementSpeedRatio: addContinuousMovementSpeedRatios[0],
                    addRotationSpeedRatio: addContinuousRotationSpeedRatios[0],
                    targetScaleX: interpolateTargetScaleXs[0],
                    targetScaleY: interpolateTargetScaleYs[0],
                    duration: duration,
                    movementSpeed: movementSpeed,
                    rotationSpeed: rotationSpeed,
                    maxBounceCount: maxBounceCount,
                    maxPenetrationCount: maxPenetrationCount
                    ));

                    clone.PC.Launch(this.Data.TemplateOrigin);
                    Owner.AttackEndPoint = transform.position;
                }
            }
        }

        public virtual void SetClonedRootTargetOnParticleStopped()
        {
            Transform root = transform.root;
            foreach (var particleStopped in root.GetComponentsInChildren<OnCustomParticleStopped>(includeInactive: true))
            {
                ParticleSystem particle = particleStopped.gameObject.GetComponent<ParticleSystem>();
                if (particle == null)
                    Utils.LogCritical(nameof(SkillBase), nameof(SetClonedRootTargetOnParticleStopped), "Failed to set root target.");
                else
                {
                    if (particleStopped.SkillParticleRootTarget != null)
                        continue; // FIND NEXT OBJECT

                    var main = particle.main;
                    main.stopAction = ParticleSystemStopAction.Callback;

                    SkillBase rootTarget = particleStopped.transform.parent.GetComponent<SkillBase>();
                    if (rootTarget == null)
                        Utils.LogCritical(nameof(SkillBase), nameof(SetClonedRootTargetOnParticleStopped), "Failed to set root target.");
                    else
                    {
                        particleStopped.SkillParticleRootTarget = rootTarget;
                        return;
                    }
                }
            }
        }

        protected bool _isCompleteInitOrigin = false;

        private bool _isLearned = false;
        public bool IsLearned
        {
            get => _isLearned;
            set => _isLearned = value;
        }

        private bool _isLast = false;
        public virtual bool IsLast
        {
            get => _isLast;
            set
            {
                _isLast = value;
                if (_isLast == false && _isCompleteInitOrigin)
                    this.RequestDestroy();
            }
        }

        [field: SerializeField] public bool IsStopped { get; protected set; } = false;
        protected Coroutine _coSkillActivate = null;

        public virtual void Activate()
        {
            if (gameObject.activeSelf)
            {
                Utils.Log($"{this.Data.Name} is already activated.");
                return;
            }

            IsStopped = false;
            gameObject.SetActive(true);

            Transform root = Managers.Pool.GetRoot(this.gameObject.name);
            if (root != null)
            {
                for (int i = 0; i < root.childCount; ++i)
                    root.GetChild(i).GetComponent<SkillBase>().IsStopped = this.IsStopped;
            }
        }

        public virtual void Deactivate()
        {
            IsStopped = true;
            gameObject.SetActive(false);

            Transform root = Managers.Pool.GetRoot(this.gameObject.name);
            if (root != null)
            {
                for (int i = 0; i < root.childCount; ++i)
                    root.GetChild(i).GetComponent<SkillBase>().IsStopped = this.IsStopped;
            }
        }

        public void RequestDestroy()
        {
            this.Deactivate();
            Managers.Object.DestroySpawnedObject<SkillBase>(this);
        }

        public void OnClonedDeactivateHandler(bool isStopped)
        {
            this.IsStopped = isStopped;
        }

        protected Coroutine _coDestroy = null;
        public void StartDestroy(float delaySeconds)
        {
            StopDestroy();
            _coDestroy = StartCoroutine(CoDestroy(delaySeconds));
        }

        private IEnumerator CoDestroy(float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            if (this.IsValid())
                Managers.Object.Despawn(this);
        }

        public void StopDestroy()
        {
            if (_coDestroy != null)
            {
                StopCoroutine(_coDestroy);
                _coDestroy = null;
            }
        }
    }
}

// =========================================================================================================
// public virtual void SetClonedRootTargetOnParticleStopped()
// {
//     Transform root = transform.root;
//     foreach (var particle in root.GetComponentsInChildren<ParticleSystem>(includeInactive: true))
//     {
//         OnCustomParticleStopped particleStopped = particle.GetComponent<OnCustomParticleStopped>();
//         if (particleStopped != null && particleStopped.SkillParticleRootTarget == null)
//         {
//             var main = particle.main;
//             if (main.stopAction != ParticleSystemStopAction.Callback)
//                 main.stopAction = ParticleSystemStopAction.Callback;

//             particleStopped.SkillParticleRootTarget = particle.transform.parent.GetComponent<SkillBase>();
//             Utils.Log($"RootTarget : {particleStopped.SkillParticleRootTarget}");
//             return;
//         }
//         else if (particleStopped != null && particleStopped.SkillParticleRootTarget != null)
//         {
//             Utils.Log("?");
//             return;
//         }
//     }
// }

// int maxBounceCount = loader.MaxBounceCount;
// int maxPenetrationCount = loader.MaxPenetrationCount;
// Vector3 additionalSpawnPos = Vector3.zero;
// if (this.Data.AdditionalCustomValues.Length != 0)
//     additionalSpawnPos = this.Data.AdditionalCustomValues[0].Point3D;

// Vector3 spawnPosOnFirstPoint = (this.Data.IsLaunchedFromFireSocket) ? this.Owner.FireSocketPosition : this.Owner.transform.position;
// for (int i = 0; i < count; ++i)
// {
//     Vector3 spawnPos = (this.Data.IsLaunchedFromFireSocket) ? this.Owner.FireSocketPosition : this.Owner.transform.position + additionalSpawnPos;
//     if (Utils.IsThief(this.Owner))
//         spawnPos = spawnPosOnFirstPoint;

//     SkillBase clone = Managers.Object.Spawn<SkillBase>(spawnPos: spawnPos, templateID: this.Data.TemplateID,
//            spawnObjectType: Define.ObjectType.Skill, isPooling: true);
//     clone.InitClone(this.Owner, this.Data);
//     if (clone.PC != null)
//     {
//         clone.OnProjectileLaunchInfo?.Invoke(this, new ProjectileLaunchInfoEventArgs(
//             lootAtDir: lootAtDir,
//             shootDir: shootDir,
//             localScale: localScale,
//             indicatorAngle: indicatorAngle,
//             movementSpeed: movementSpeed,
//             rotationSpeed: rotationSpeed,
//             lifeTime: duration,
//             continuousAngle: continuousAngles[i],
//             continuousSpeedRatio: continuousSpeedRates[i],
//             continuousFlipX: continuousFlipXs[i],
//             continuousFlipY: continuousFlipYs[i],
//             interpolateTargetScaleX: interpolateTargetScaleXs[i],
//             interpolateTargetScaleY: interpolateTargetScaleYs[i],
//             isColliderHalfRatio: this.Data.SetColliderHalfLifeTime,
//             maxBounceCount: maxBounceCount,
//             maxPenetrationCount: maxPenetrationCount
//         ));

//         clone.PC.Launch();
//     }

//     yield return new WaitForSeconds(loader.ContinuousSpacing);
// }

// Owner.AttackEndPoint = transform.position;
// yield return null;
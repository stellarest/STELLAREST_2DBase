using System.Collections;
using UnityEngine;

using STELLAREST_2D.Data;

namespace STELLAREST_2D
{
    /*
        * SkillBase 
            - Unique Skill : "캐릭터 고유의 전용 스킬". 전용 애니메이션 있을수도 있고 없을 수도 있음.
            - Public Skill : "모든 캐릭터가 획득할 수 있는 스킬". 전용 애니메이션 있을수도 있고 없을수도 있지만 거의 없을듯.
                             때문에, 이들의 클래스는 단순 구분용으로 작성하게 된 것.

        * Player Default Skill
        - Mastery Skill : 모든 캐릭터가 기본적으로 가지고 있는 스킬. Default -> Elite -> Ultimate 등급으로 구성
            > Mastery Attack : 캐릭터의 기본 공격 스킬. 애니메이션 발동. (Default -> Elite -> Ultimate)
            > Mastery Elite Plus
            > Mastery Ultimate Plus 
    */


    [System.Serializable]
    public class SkillBase : BaseController
    {
        public System.Action<Vector3, Define.LookAtDirection, float, float, float> OnSetParticleInfo = null;
        public System.EventHandler<ProjectileLaunchInfoEventArgs> OnProjectileLaunchInfo = null;
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
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
            else
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.MonsterAttack);
        }

        public void InitCloneManually(CreatureController ownerFromOrigin, Data.SkillData dataFromOrigin)
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
                    Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
                else
                    Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.MonsterAttack);

                this.IsFirstPooling = false;
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
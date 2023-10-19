using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

namespace STELLAREST_2D
{
    [System.Serializable]
    public class SkillBase : BaseController
    {
        public System.Action<Vector3, Define.LookAtDirection, float, float, float> OnSetParticleInfo = null;
        public System.EventHandler<ProjectileLaunchInfoEventArgs> OnProjectileLaunchInfo = null;
        public CreatureController Owner { get; protected set; } = null;
        public Data.SkillData Data { get; protected set; } = null;
        public ProjectileController PC { get; protected set; } = null;
        
        public SpriteRenderer SR { get; protected set; } = null;
        public Rigidbody2D RigidBody { get; protected set; } = null;
        public Collider2D HitCollider { get; protected set; } = null;

        public virtual void InitOrigin(CreatureController owner, Data.SkillData data)
        {
            this.Owner = owner;
            this.Data = data;

            this.IsLearned = false;
            this.IsLast = false;
            _isCompleteInitOrigin = true;
        }

        public virtual void InitClone(CreatureController ownerFromOrigin, Data.SkillData dataFromOrigin)
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
            if (ownerFromOrigin?.IsPlayer() == true)
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
            else
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.MonsterAttack);
        }

        public virtual void SetClonedRootTargetOnParticleStopped() 
        {
            Transform root = transform.root;
            foreach (var particle in root.GetComponentsInChildren<ParticleSystem>(includeInactive: true))
            {
                OnCustomParticleStopped particleStopped = particle.GetComponent<OnCustomParticleStopped>();
                if (particleStopped != null && particleStopped.SkillParticleRootTarget == null)
                {
                    var main = particle.main;
                    if (main.stopAction != ParticleSystemStopAction.Callback)
                        main.stopAction = ParticleSystemStopAction.Callback;

                    //particleStopped.RootTarget = particle.transform.parent.gameObject;
                    // Utils.LogBreak($"Particle Name : {particle.gameObject.name}");
                    // Utils.LogBreak($"Parent Name : {particle.transform.parent.name}");

                    particleStopped.SkillParticleRootTarget = particle.transform.parent.GetComponent<SkillBase>();
                    return;
                }
                else if (particleStopped != null && particleStopped.SkillParticleRootTarget != null)
                    return;
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

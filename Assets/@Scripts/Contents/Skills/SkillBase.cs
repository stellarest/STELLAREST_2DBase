using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.HeroEditor.Common.Scripts.ExampleScripts;
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
        public Collider2D HitCollider { get; protected set; } = null;
        public Rigidbody2D RigidBody { get; protected set; } = null;

        public virtual void InitOrigin(CreatureController owner, Data.SkillData data)
        {
            this.Owner = owner;
            this.Data = data;
            
            this.IsLearned = false;
            this.IsLast = false;
        }

        public virtual void InitClone(CreatureController ownerFromOrigin, Data.SkillData dataFromOrigin)
        {
            this.Owner = ownerFromOrigin;
            this.Data = dataFromOrigin;
            SetSortingGroup();

            if (this.Data.IsProjectile)
            {
                this.PC = GetComponent<ProjectileController>();
                this.PC.HitCollider = GetComponent<Collider2D>();
                this.PC.RigidBody = GetComponent<Rigidbody2D>();
                this.PC.Owner = ownerFromOrigin;
                this.PC.Data = dataFromOrigin;
                this.OnProjectileLaunchInfo += this.PC.OnProjectileLaunchInfoHandler;
            }

            if (ownerFromOrigin?.IsPlayer() == true)
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
            else
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.MonsterAttack);
        }

        // --------------------------------------------------------------------------------------------------
        // ***** Projectile is must be skill. But, skill is not sometimes projectil. It's just a skill. *****
        // --------------------------------------------------------------------------------------------------
        private bool _isLearned = false;
        public bool IsLearned
        {
            get => _isLearned;
            set => _isLearned = value;
        }

        private bool _isLast = false;
        public bool IsLast
        {
            get => _isLast;
            set
            {
                _isLast = value;
                if (_isLast == false) // When Upgrade Skill
                {
                    if (this.IsStopped)
                    {
                        Utils.Log(nameof(SkillBase), nameof(IsLast), Data.Name, "is already stopped.");
                        return;
                    }

                    this.Deactivate(true);
                }
            }
        }

        public bool IsStopped { get; protected set; } = false;

        private float? _damageBuffRatio = null;
        public float? DamageBuffRatio
        {
            get => _damageBuffRatio;
            set => _damageBuffRatio = value;
        }

        public virtual void Activate()
        {
            if (gameObject.activeSelf)
            {
                Utils.Log($"{this.Data.Name} is already activated.");
                return;
            }

            IsStopped = false;
            gameObject.SetActive(true);
        }
        
        public virtual void Deactivate(bool isPoolingClear = false)
        {
            IsStopped = true;
            gameObject.SetActive(false);
            if(isPoolingClear)
                Managers.Pool.ClearPool<SkillBase>(this.gameObject);
        } 

        public bool IsCritical { get; set; } = false;

        public float GetDamage()
        {
            return 0f;
        }

        protected Coroutine _coDestroy;
        public void StartDestroy(float delaySeconds)
        {
            StopDestroy();
            _coDestroy = StartCoroutine(CoDestroy(delaySeconds));
        }

        private IEnumerator CoDestroy(float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            if (this.IsValid())
            {
                //Managers.Object.Despawn(this.GetComponent<ProjectileController>());
                Managers.Object.Despawn(this.GetComponent<SkillBase>());
            }
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

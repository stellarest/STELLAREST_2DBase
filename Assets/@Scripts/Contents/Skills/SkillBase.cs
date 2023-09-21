using System;
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
        public CreatureController Owner { get; protected set; } = null;
        public Data.SkillData Data { get; protected set; }  = null;

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
                        //Utils.Log($"{this.Data.Name} is <color=cyan>already</color> stopped");
                        Utils.Log(nameof(SkillBase), nameof(IsLast), Data.Name, "is already stopped.");
                        return;
                    }

                    this.Deactivate();
                    Utils.Log(nameof(SkillBase), nameof(IsLast), Data.Name, "is deactivated.");
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

        public virtual void Init(CreatureController owner, Data.SkillData data, int templateID) 
        {
            _isLearned = false;
            _isLast = false;

            // 당연히 스킬마다 Owner와 Data를 설정해야함. !!
            this.Owner = owner;
            this.Data = data;
            owner.SkillBook.SkillGroupsDict.AddGroup(templateID, new SkillGroup(this));
        }

        public virtual void SetParticleInfo(Vector3 startAngle, Define.LookAtDirection lookAtDir, float continuousAngle, float continuousFlipX, float continuousFlipY) { }
        public virtual void OnPreSpawned() => gameObject.SetActive(false);
        public virtual void Activate()
        {
            if (gameObject.activeSelf)
            {
                Utils.Log($"{this.Data.Name} is already activated.");
                return;
            }

            gameObject.SetActive(true);
        }
        public virtual void Deactivate() => gameObject.SetActive(false);

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
                Managers.Object.Despawn(this.GetComponent<ProjectileController>());
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

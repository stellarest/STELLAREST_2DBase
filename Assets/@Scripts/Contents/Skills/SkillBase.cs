using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace STELLAREST_2D
{
    public class SkillBase : BaseController
    {
        public Define.TemplateIDs.SkillType SkillType { get; protected set; } = Define.TemplateIDs.SkillType.None;
        public CreatureController Owner { get; protected set; }
        public Data.SkillData SkillData { get; protected set; }

        private float? _damageBuffRatio = null;
        public float? DamageBuffRatio
        {
            get => _damageBuffRatio;
            set => _damageBuffRatio = value;
        }

        public virtual void OnPreSpawned() => gameObject.SetActive(false);
        public virtual void ActivateSkill() => gameObject.SetActive(true);
        public virtual void DeactivateSkill() => gameObject.SetActive(false);

        public bool IsCritical { get; set; } = false;
        public float GetDamage()
        {
            float damage = Random.Range(SkillData.MinDamage, SkillData.MaxDamage);

            if (_damageBuffRatio.HasValue)
                damage = damage + (damage * _damageBuffRatio.Value);

            if (Random.Range(0f, 0.99f + Mathf.Epsilon) < Owner.CharaData.Critical)
            {
                IsCritical = true;
                float criticalRatio = Random.Range(1.5f, 2f);
                damage = damage * criticalRatio;
            }

            float damageResult = damage + (damage * Owner.CharaData.Damage);

            return damageResult;
        }

        public virtual void SetSkillInfo(CreatureController owner, int templateID)
        {
            if (Managers.Data.SkillDict.TryGetValue(templateID, out Data.SkillData skillData) == false)
            {
                Debug.LogError("Failed to load SkillDict, template ID : " + templateID);
                Debug.Break();
            }

            this.Owner = owner;
            this.SkillData = skillData;
        }

        protected Coroutine _coDestroy;
        public void StartDestroy(float delaySeconds)
        {
            StopDestroy();
            _coDestroy = StartCoroutine(CoDestroy(delaySeconds));
        }

        public void StopDestroy()
        {
            if (_coDestroy != null)
            {
                StopCoroutine(_coDestroy);
                _coDestroy = null;
            }
        }

        private IEnumerator CoDestroy(float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            if (this.IsValid())
            {
                Managers.Object.Despawn(this.GetComponent<ProjectileController>());
            }
        }
    }
}

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
        public CreatureController Owner { get; set; }
        public Data.SkillData SkillData { get; set; }

        public virtual void OnPreSpawned() => gameObject.SetActive(false);
        public virtual void ActivateSkill() => gameObject.SetActive(true);
        public virtual void DeactivateSkill() => gameObject.SetActive(false);

        public bool IsCritical { get; set; } = false;
        public float GetDamage()
        {
            float damage = Random.Range(SkillData.MinDamage, SkillData.MaxDamage);
            if (Random.Range(0f, 0.99f + Mathf.Epsilon) < Owner.CharaData.CriticalChance)
            {
                IsCritical = true;
                float criticalRatio = Random.Range(1.5f, 2f);
                damage = damage * criticalRatio;
            }

            return damage + (damage * Owner.CharaData.DamageUp);
        }

        // protected virtual void GenerateProjectile(int templateID, CreatureController owner, Vector3 startPos, Vector3 dir, Vector3 targetPos)
        // {
        //     ProjectileController pc = Managers.Object.Spawn<ProjectileController>(startPos, templateID);
        //     //pc.SetInfo(owner, dir);
        // }

        // protected virtual void GenerateProjectile(Vector3 startPos, SkillBase skill)
        // {
        //     ProjectileController pc = Managers.Object.Spawn<ProjectileController>(startPos, skill.SkillData.TemplateID);
        //     //pc.SetInfo(owner, dir);
        // }

        // public void UpgradeSkill()
        // {
        //     int currentID = SkillData.TemplateID;
        //     if (Managers.Data.SkillDict.TryGetValue(currentID + 1, out Data.SkillData newSkillData))
        //     {
        //         StartCoroutine(CoWaitForUpgrade(SkillData, newSkillData));
        //     }
        //     else
        //         Utils.LogStrong("Failed to upgrade skill !!");
        // }

        // public bool UpgradeSkill(SkillBase currentSKill, SkillBase nextSkill)
        // {
        //     DeactivateSkill(currentSKill.gameObject);
        //     // currentSKill : 이전에 했던 스킬을 제어하려면 
        //     if (nextSkill.SkillData.IsPlayerDefaultAttack)
        //         Managers.Game.Player.AnimEvents.PlayerDefaultAttack++;

        //     return currentSKill.gameObject.activeInHierarchy == false;
        // }


        // private IEnumerator CoWaitForUpgradeSkill(Data.SkillData currentSkillData, Data.SkillData newSkillData)
        // {
        //     GameObject currentSkillObj = Owner.SkillBook.SpawnedSkillList.FirstOrDefault(s => s.SkillData.TemplateID == currentSkillData.TemplateID).gameObject;
        //     DeactivateSkill(currentSkillObj);
        //     yield return new WaitUntil(() => Owner.IsAttackStart);
        //     //SkillData = newSkillData;
        //     if (newSkillData.IsPlayerDefaultAttack)
        //         Managers.Game.Player.AnimEvents.PlayerDefaultAttack++;
            
        //     // new object

        //     Utils.Log("Success to upgrade skill !!");
        // }

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

        // TEMP
        public virtual void SetAngle(float angle)
        {
            // 일단 회전만 설정
            Vector3 tempAngle = Managers.Game.Player.Indicator.eulerAngles;
            tempAngle.z += angle;
            transform.rotation = Quaternion.Euler(tempAngle);

            var main = GetComponent<ParticleSystem>().main;
            main.startRotation = Mathf.Deg2Rad * tempAngle.z * -1f;
            main.flipRotation = Managers.Game.Player.TurningAngle;
            transform.position = Managers.Game.Player.transform.position;
            transform.localScale = Managers.Game.Player.AnimationLocalScale;
            GetComponent<Collider2D>().enabled = true;
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

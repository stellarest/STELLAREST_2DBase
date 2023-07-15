using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class SkillBase : BaseController
    {
        public CreatureController Owner { get; set; }
        public Data.SkillData SkillData { get; set; }

        public virtual void ActivateSkill() => gameObject.SetActive(true);
        public virtual void DeactivateSkill() { }

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

        public void UpgradeSkill()
        {
            int currentID = SkillData.TemplateID;
            if (Managers.Data.SkillDict.TryGetValue(currentID + 1, out Data.SkillData newSkillData))
            {
                StartCoroutine(CoWaitForUpgrade(newSkillData));
            }
            else
                Utils.LogStrong("Failed to upgrade skill !!");
        }

        private IEnumerator CoWaitForUpgrade(Data.SkillData newSkillData)
        {
            yield return new WaitUntil(() => Owner.IsAttackStart);
            SkillData = newSkillData;
            if (newSkillData.IsPlayerDefaultAttack)
                Managers.Game.Player.AnimEvents.PlayerDefaultAttack++;

            Utils.Log("Success to upgrade skill !!");
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

        private Coroutine _coDestroy;
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

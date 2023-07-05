using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class SkillBase : BaseController
    {
        public CreatureController Owner { get; set; }
        public Data.SkillData SkillData { get; protected set ; }

        public virtual void ActivateSkill() { }
        protected virtual void GenerateProjectile(int templateID, CreatureController owner, Vector3 startPos, Vector3 dir, Vector3 targetPos)
        {
            ProjectileController pc = Managers.Object.Spawn<ProjectileController>(startPos, templateID);
            pc.SetInfo(owner, dir);
        }

        public bool IsLearnedSkill => SkillGrade != Define.InGameGrade.Normal;
        public Define.InGameGrade SkillGrade { get; protected set; }

        public void UpgradeSkill()
        {
            int currentID = SkillData.TemplateID;
            if (Managers.Data.SkillDict.TryGetValue(currentID + 1, out Data.SkillData newSkillData))
            {
                SkillData = this.SkillGrade < newSkillData.InGameGrade ?
                                newSkillData : this.SkillData;

                this.SkillGrade = newSkillData.InGameGrade;

                Utils.LogWhite("SkillData Upgrade !!");
            }
            else
                Utils.LogWhite("Already Max Skill Grade");
        }

        public virtual void SetInitialSkillInfo(CreatureController owner, int templateID)
        {
            if (Managers.Data.SkillDict.TryGetValue(templateID, out Data.SkillData skillData) == false)
            {
                Debug.LogError("Failed to load SkillDict, template ID : " + templateID);
                Debug.Break();
            }

            this.Owner = owner;
            this.SkillData = skillData;
            this.SkillGrade = skillData.InGameGrade;
            Debug.Log("SkillGrade : " + this.SkillGrade);
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
                Managers.Object.Despawn(this);
            }
        }
    }
}

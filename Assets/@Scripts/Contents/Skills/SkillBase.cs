using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class SkillBase : BaseController
    {
        /*
          "TemplateID" : "10001",
          "Name" : "Gary_EgoSword",
          "PrefabLabel" : "EgoSword.prefab",
          "Damage" : "1000",
          "ProjectileSpeed" : "0f",
          "CoolTime" : "2f"
        */

        private Data.SkillData _skillData;
        public Data.SkillData SkillData 
        { 
            get => _skillData; 
            set
            {
                _skillData = value;
                TemplateID = value.TemplateID;
                SkillName = value.Name;
                _damage = value.Damage;
                _projectileSpeed = value.ProjectileSpeed;
                _coolTime = value.CoolTime;
            }
        }

        public virtual void ActivateSkill() { }

        protected virtual void GenerateProjectile(int templateID, CreatureController owner, Vector3 startPos, Vector3 dir, Vector3 targetPos)
        {
            ProjectileController pc = Managers.Object.Spawn<ProjectileController>(startPos, templateID);
            pc.SetInfo(owner, dir);
        }

        public CreatureController Owner { get; set; }
        private Define.SkillType _skillType = Define.SkillType.None;
        public Define.SkillType SkillType { get => _skillType; protected set { _skillType = value; } }

        public int SkillLevel { get; set; } = 0;
        public bool IsLearnedSkill => SkillLevel > 0;

        public int TemplateID { get; protected set; }
        public string SkillName { get; protected set; }
        private int _damage;
        public int Damage { get => _damage; protected set { _damage = value; } }

        private float _projectileSpeed;
        public float ProjectileSpeed { get => _projectileSpeed; protected set { _projectileSpeed = value; } }

        private float _coolTime;
        public float CoolTime { get => _coolTime; protected set { _coolTime = value; } }

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

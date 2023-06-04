using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class SkillBase : BaseController
    {
        /*
            "templateID" : "1",
            "name" : "FireBall",
            "type" : "2",
            "prefab" : "FireProjectile.prefab",
            "damage" : "1000",
            "speed" : "3f"
        */

        public SkillBase(Define.GameData.SkillType skillType)
        {
            SkillType = skillType; // 이미 받는중...
            // 하지만 이렇게하면 SkillBase를 상속받은 모든 애들은 기본 생성자가 막히기 때문에 여기에 모두 필수적으로 전달해야함
        }

        public virtual void ActivateSkill(){ }
        protected virtual void GenerateProjectile(int templateID, CreatureController owner, Vector3 startPos, Vector3 dir, Vector3 targetPos)
        {
            ProjectileController pc = Managers.Object.Spawn<ProjectileController>(startPos, templateID);
            pc.SetInfo(owner, dir);
        }

        public CreatureController Owner { get; set; }
        private Define.GameData.SkillType _skillType = Define.GameData.SkillType.None;
        public Define.GameData.SkillType SkillType
        {
            get => _skillType;
            protected set
            {
                _skillType = value;
            }
        }

        public int SkillLevel { get; set; } = 0;
        public bool IsLearnedSkill => SkillLevel > 0;

        private int _damage;
        public int Damage
        {
            get => _damage;
            protected set
            {
                _damage = value;
            }
        }

        private float _speed;
        public float Speed
        {
            get => _speed;
            protected set
            {
                _speed = value;
            }
        }

        private Data.SkillData _skillData;
        public Data.SkillData SkillData 
        { 
            get => _skillData; 
            set
            {
                _skillData = value;
                _skillType = _skillData.type;
                _damage = _skillData.damage;
                _speed = _skillData.speed;
            }
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

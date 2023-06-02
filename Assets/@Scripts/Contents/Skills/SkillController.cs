using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class SkillController : BaseController
    {
        /*
            "templateID" : "1",
            "name" : "FireBall",
            "type" : "2",
            "prefab" : "FireProjectile.prefab",
            "damage" : "1000",
            "speed" : "3f"
        */

        private Define.GameData.SkillType _skillType = Define.GameData.SkillType.None;
        public Define.GameData.SkillType SkillType
        {
            get => _skillType;
            protected set
            {
                _skillType = value;
            }
        }

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
                if (_skillType == Define.GameData.SkillType.Melee)
                    _speed = 0f;
                else if (_skillType == Define.GameData.SkillType.Projectile)
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

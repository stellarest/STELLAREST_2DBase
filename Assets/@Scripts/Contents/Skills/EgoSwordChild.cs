using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class EgoSwordChild : MonoBehaviour
    {
        private BaseController _owner;
        private int _damage;

        public void SetOwner(BaseController owner)
        {
            this._owner = owner;
        }

        public void SetInfo(Data.SkillData skillData)
        {
            _damage = skillData.damage;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
             // 당연히 몬스터한테도 이 스킬이 붙을수도 있음. 그땐 여기를 세분화하면 됨
            MonsterController mc = other.gameObject.GetComponent<MonsterController>();
            if (mc.IsValid() == false)
                return;
            
            mc.OnDamaged(_owner, _damage);
        }
    }
}

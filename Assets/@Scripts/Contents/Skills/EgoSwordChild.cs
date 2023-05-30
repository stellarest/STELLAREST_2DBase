using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class EgoSwordChild : MonoBehaviour
    {
        private BaseController _owner;
        private int _damage;

        public void SetInfo(BaseController owner, int damage)
        {
            this._owner = owner;
            this._damage = damage;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            MonsterController mc = other.gameObject.GetComponent<MonsterController>();
            if (mc.IsValid() == false)
                return;
            
            mc.OnDamaged(_owner, _damage);
        }
    }
}

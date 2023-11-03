using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace STELLAREST_2D
{
    public class SecondWindChild : SecondWind
    {
        public void Init(CreatureController owner, Data.SkillData data)
        {
            this.Owner = owner;
            this.Data = data;

            RigidBody = GetComponent<Rigidbody2D>();
            HitCollider = GetComponent<Collider2D>();

            Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            HitPoint = other.ClosestPoint(this.transform.position);
            cc.OnDamaged(this.Owner, this);
            RigidBody.simulated = false;
        }
    }
}

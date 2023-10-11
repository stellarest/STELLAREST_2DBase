// using System.Collections;
// using System.Collections.Generic;
// using STELLAREST_2D.Data;
// using UnityEditor.Animations;
// using UnityEngine;

using Unity.Collections;
using UnityEngine;

namespace STELLAREST_2D
{
    public class BombTrapChild : BombTrap
    {
        public void Init(CreatureController owner, Data.SkillData data)
        {
            this.Owner = owner;
            this.Data = data;

            RigidBody = GetComponent<Rigidbody2D>();
            RigidBody.simulated = false;

            HitCollider = GetComponent<Collider2D>();
            HitCollider.isTrigger = true;
            HitCollider.enabled = false;

            base.SetClonedRootTargetOnParticleStopped();
            GetComponent<ParticleSystemRenderer>().sortingOrder = (int)Define.SortingOrder.EnvEffect;
            if (this.Owner?.IsPlayer() == true)
                Managers.Collision.InitCollisionLayer(this.gameObject, Define.CollisionLayers.PlayerAttack);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            cc?.OnDamaged(this.Owner, this);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace STELLAREST_2D
{
    public class RangedShotChild : RangedShot
    {
        private RangedShot _parent = null;
        public Collider2D ChildHitCollider { get; private set; } = null;

        public void Init(CreatureController owner, Data.SkillData data, RangedShot parent)
        {
            this.Owner = owner;
            this.Data = data;
            _parent = parent;
            ChildHitCollider = GetComponent<Collider2D>();

            if (owner?.IsPlayer == true)
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
            else
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.MonsterAttack);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            HitPoint = other.ClosestPoint(this.transform.position);
            // Debug.DrawRay(cc.Center.position, (cc.Center.position - HitPoint).normalized * 999f, Color.magenta, -1f);
            // Utils.LogBreak("BREAK.");
            cc.OnDamaged(this.Owner, this);
            ChildHitCollider.enabled = false;
        }
    }
}

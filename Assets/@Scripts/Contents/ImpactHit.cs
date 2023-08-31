using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace STELLAREST_2D
{
    public class ImpactHit : MonoBehaviour
    {
        private Define.ImpactHits _impactHit;
        private SkillBase _skillOwner;
        private ProjectileController _projectile;
        private CreatureController _owner;
        private Collider2D _col;
        private bool _setCollision;

        public void SetInfo(Define.ImpactHits impact, CreatureController owner, SkillBase skill, ProjectileController projectile = null)
        {
            _impactHit = impact;
            _owner = owner;
            _skillOwner = skill;
            _projectile = projectile;

            if (_col == null)
                _col = GetComponent<Collider2D>();

            if (_setCollision == false)
            {
                if (_owner?.IsPlayer() == true)
                    Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
                _setCollision = true;
            }
            
            _col.enabled = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            MonsterController mc = other.GetComponent<MonsterController>();
            if (mc.IsValid() == false)
                return;

            switch (_impactHit)
            {
                case Define.ImpactHits.Leaves:
                    {
                        _skillOwner.DamageBuffRatio = 0.5f;
                        mc.OnDamaged(_owner, _skillOwner);
                        Managers.CC.ApplyCC<MonsterController>(mc, _skillOwner.SkillData, _projectile);
                        _col.enabled = false;
                    }
                    break;
            }
        }
    }
}

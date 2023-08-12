using System.Collections;
using System.Collections.Generic;
using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    public class BombTrapChild : MonoBehaviour
    {
        private BombTrap _parent = null;
        private Collider2D _explosionCol = null;

        private void OnEnable()
        {
            _explosionCol = GetComponent<Collider2D>();
            _explosionCol.enabled = true;

            _parent = transform.parent.GetComponent<BombTrap>();
            if (_parent.Owner?.IsPlayer() == true)
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);

            StartCoroutine(CoDisableCollider());
        }

        private IEnumerator CoDisableCollider()
        {
            float t = 0f;
            while (true)
            {
                t += Time.deltaTime;
                if (t > 0.1f)
                    _explosionCol.enabled = false;

                yield return null;
            }
        }

        public void OnParticleSystemStopped()
        {
            _parent.Generator.Count--;
            _parent.IsOnStepped = false;
            Managers.Resource.Destroy(_parent.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            MonsterController mc = other.GetComponent<MonsterController>();
            if (mc.IsValid() == false)
                return;

            if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.MonsterBody, other.gameObject.layer))
            {
                mc.OnDamaged(_parent.Owner, _parent);
                // +++ CC 적용시 주의 사항은, 데미지를 먼저 전달해야 한다 +++
                if (_parent.SkillData.InGameGrade == Define.InGameGrade.Legendary)
                {
                    if (mc.CharaData.Hp > 0)
                        Managers.CC.ApplyCC(mc, Define.CCStatus.Stun, 3f);
                }
            }
        }
    }
}

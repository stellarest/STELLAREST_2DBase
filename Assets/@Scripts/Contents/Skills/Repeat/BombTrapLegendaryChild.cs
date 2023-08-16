using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class BombTrapLegendaryChild : MonoBehaviour
    {
        private BombTrap _parent = null;
        private Collider2D _explosionCol = null;

        private void OnEnable()
        {
            if (_parent == null)
            {
                _parent = transform.parent.GetComponent<BombTrap>();
                if (_parent.Owner?.IsPlayer() == true)
                    Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
            }

            if (_explosionCol == null)
                _explosionCol = GetComponent<Collider2D>();
            _explosionCol.enabled = true;

            StartCoroutine(CoDisableCollider());
        }

        private IEnumerator CoDisableCollider()
        {
            float t = 0f;
            while (true)
            {
                t += Time.deltaTime;
                if (t > 1f)
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

            if (mc.CCStatus == Define.CCStatus.Stun && mc.GoCCEffect != null)
                return;

            if (mc.CharaData.Hp > 0)
                Managers.CC.ApplyCC<MonsterController>(mc, Define.CCStatus.Stun, 3f);
        }
    }
}

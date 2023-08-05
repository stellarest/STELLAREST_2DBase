using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace STELLAREST_2D
{
    public class BoomerangChild : MonoBehaviour
    {
        private Boomerang parent;
        public SpriteTrail.SpriteTrail Trail { get; private set; }


        [SerializeField]
        private AnimationCurve _curve;
        public AnimationCurve Curve => _curve;

        public float RotStartTime { get; set; } = 0f;
        public float ToOwnerStartTime { get; set; } = 0f;
        public readonly float DesiredSelfRotTime = 3f;

        private float _minDistnace = 1f;
        private float _maxDistance = 6f;

        private void Start()
        {
            parent = transform.parent.GetComponent<Boomerang>();
            Trail = GetComponent<SpriteTrail.SpriteTrail>();
            Trail.enabled = false;

            transform.localPosition = Vector2.right;
        }

        private float _dist = 0f;
        public bool IsReadyToOwner { get; private set; } = false;
        public void RotateAround(bool isToOwner = false)
        {
            if (isToOwner == false)
                _dist = Mathf.Lerp(_minDistnace, _maxDistance, _curve.Evaluate(Mathf.Clamp01((Time.time - RotStartTime) / 2f)));
            else
            {
                _dist = Mathf.Lerp(_maxDistance, _minDistnace, _curve.Evaluate(Mathf.Clamp01((Time.time - ToOwnerStartTime))));
                if (_dist <= _minDistnace)
                    IsReadyToOwner = true;
            }

            transform.localPosition = Vector2.right * _dist;
        }

        private void OnDisable()
        {
            _dist = 0f;
            IsReadyToOwner = false;
            RotStartTime = 0f;
            ToOwnerStartTime = 0f;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            MonsterController mc = other.GetComponent<MonsterController>();
            if (mc.IsValid() == false)
                return;

            if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.MonsterBody, other.gameObject.layer))
            {
                mc.OnDamaged(parent.Owner, parent);
            }
        }
    }
}


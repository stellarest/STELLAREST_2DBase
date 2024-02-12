using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class HeroCamp : BaseObject
    {
        private Vector2 _moveDir = Vector2.zero;
        public float Speed { get; private set; } = 5f;

        public Transform Pivot { get; private set; } = null;
        public Transform Destination { get; private set; } = null;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Managers.Game.OnMoveDirChanged -= OnMoveDirChangedHandler;
            Managers.Game.OnMoveDirChanged += OnMoveDirChangedHandler;

            Collider.includeLayers = (1 << (int)Define.ELayer.Obstacle);
            Collider.excludeLayers = (1 << (int)Define.ELayer.Hero) | (1 << (int)Define.ELayer.Monster);

            ObjectType = Define.EObjectType.HeroCamp;

            Pivot = Util.FindChild<Transform>(gameObject, FixedValue.String.PIVOT);
            Destination = Util.FindChild<Transform>(gameObject, FixedValue.String.DESTINATION, true);

            GetComponent<SpriteRenderer>().enabled = false;
            Destination.GetComponent<SpriteRenderer>().enabled = false;

            return true;
        }

        private void Update()
        {
            transform.Translate(_moveDir * Speed * Time.deltaTime);
        }

        private void OnMoveDirChangedHandler(Vector2 dir)
        {
            _moveDir = dir;
            if (_moveDir != Vector2.zero)
            {
                float angle = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;
                Pivot.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
    }
}

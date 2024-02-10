using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class BaseObject : InitBase
    {
        public EObjectType ObjectType { get; protected set; } = EObjectType.None;
        public CircleCollider2D Collider { get; private set; } = null;
        public BaseAnimController BaseAnim { get; private set; } = null;
        public Rigidbody2D RigidBody { get; private set; } = null;

        public float ColliderRadius => (this.Collider != null) ? this.Collider.radius : 0f;
        public Vector3 CenterPosition => transform.position + (Vector3.up * this.ColliderRadius); // 디폴트 피벗이 일단 발이 중심이 되어야할듯,,
        public ELookAtDirection LookAtDirection { get; protected set; } = ELookAtDirection.Right;
        public Vector3 LocalScale { get; protected set; } = Vector3.zero; // 이것도 TEMP

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Collider = gameObject.GetOrAddComponent<CircleCollider2D>();
            RigidBody = gameObject.GetOrAddComponent<Rigidbody2D>();
            LocalScale = Util.FindChild<Transform>(gameObject, FixedValue.String.ANIMATION_BODY, true).localScale;

            return true;
        }

        public void TranslateEx(Vector3 dir)
        {
            transform.Translate(dir);
        }

        private void Flip(float angle)
        {
            if (BaseAnim == null)
                return;

            if (Mathf.Sign(angle) < 0)
            {
                LookAtDirection = ELookAtDirection.Right;
            }
            else
            {
                LookAtDirection = ELookAtDirection.Left;
            }
           
            //LocalScale = new Vector3((int))
        }

        protected virtual void UpdateAnimation() { }
    }
}


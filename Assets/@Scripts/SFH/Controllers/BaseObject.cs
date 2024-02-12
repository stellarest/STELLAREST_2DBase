using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.iOS;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class BaseObject : InitBase
    {
        public EObjectType ObjectType { get; protected set; } = EObjectType.None;
        public CircleCollider2D Collider { get; private set; } = null;
        public Rigidbody2D RigidBody { get; private set; } = null;

        // ColliderRadius : 0.8F (CURRENT)
        public float ColliderRadius => (this.Collider != null) ? this.Collider.radius : 0f;
        public Vector3 CenterPosition => transform.position + (Vector3.up * this.ColliderRadius); // 디폴트 피벗이 일단 발이 중심이 되어야할듯,,
        
        public BaseAnimation BaseAnim { get; private set; } = null;
        public ELookAtDirection LookAtDirection { get; protected set; } = ELookAtDirection.Right;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Collider = gameObject.GetOrAddComponent<CircleCollider2D>();
            RigidBody = gameObject.GetOrAddComponent<Rigidbody2D>();
            RigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            RigidBody.gravityScale = 0f;

            GameObject animBody = Util.FindChild(gameObject, FixedValue.String.ANIMATION_BODY);
            if (animBody != null)
                BaseAnim = animBody.GetOrAddComponent<BaseAnimation>();

            return true;
        }

        private void Flip(float angle)
        {
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


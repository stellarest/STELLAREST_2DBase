using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


namespace STELLAREST_2D
{
    public class BaseController : MonoBehaviour
    {
        public bool IsFirstPooling { get; protected set; } = true;
        public Define.ObjectType ObjectType { get; set; } = Define.ObjectType.None;
        public Transform TrailSocket { get; protected set; } = null;
        public RendererController RendererController { get; protected set; } = null;

        public virtual void Init(int templateID) { }
        public virtual void LateInit() { }
        protected virtual void SetSortingOrder() { }
    }
}

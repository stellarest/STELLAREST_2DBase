using UnityEditor;
using UnityEngine;

namespace STELLAREST_2D
{
    public class BaseController : MonoBehaviour
    {
        public bool IsFirstPooling { get; protected set; } = true;
        public Define.ObjectType ObjectType { get; set; } = Define.ObjectType.None;

        protected virtual void SetRenderSorting() { }
        public void CoTrailEffect(BaseController followTarget) 
                    => StartCoroutine(Managers.Effect.CoTrailEffect(gameObject, followTarget));
        public void EffectHit()
        {
        }
    }
}

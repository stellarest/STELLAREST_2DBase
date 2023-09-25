using UnityEditor;
using UnityEngine;

namespace STELLAREST_2D
{
    public interface IHitStatus
    {
        public bool IsThrowingStarHit { get; set; }
        public bool IsLazerBoltHit { get; set; }
    }

    public class BaseController : MonoBehaviour
    {
        public bool IsFirstPooling { get; protected set; } = true;
        public Define.ObjectType ObjectType { get; set; } = Define.ObjectType.None;

        public virtual void Init(int templateID) { }
        protected virtual void SetSortingGroup() { }
        public void CoTrailEffect(BaseController followTarget) 
                    => StartCoroutine(Managers.Effect.CoTrailEffect(gameObject, followTarget));
        public void EffectHit()
        {
        }
    }
}

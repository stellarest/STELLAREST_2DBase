using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

using CrowdControlType = STELLAREST_2D.Define.TemplateIDs.CrowdControl;

namespace STELLAREST_2D
{

    public class BaseController : MonoBehaviour
    {
        public bool IsFirstPooling { get; protected set; } = true;
        public Define.ObjectType ObjectType { get; set; } = Define.ObjectType.None;
        public Transform TrailSocket { get; protected set; } = null;

        public virtual void Init(int templateID) { }
        protected virtual void SetSortingOrder() { }

        private Coroutine _coCrowdControl = null;
        public void RequestCrowdControl(CrowdControlType ccType)
        {
            switch (ccType)
            {
                case CrowdControlType.None:
                    {
                        if (_coCrowdControl != null)
                            StopCoroutine(_coCrowdControl);
                        
                        _coCrowdControl = null;
                    }
                    return;

                case CrowdControlType.Stun:
                    {
                        if (_coCrowdControl != null)
                            StopCoroutine(_coCrowdControl);

                    }
                    break;
            }
        }
    }
}

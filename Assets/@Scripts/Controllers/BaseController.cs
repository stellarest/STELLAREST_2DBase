using UnityEngine;

namespace STELLAREST_2D
{
    public class BaseController : MonoBehaviour
    {
        public Define.ObjectType ObjectType { get; protected set; }

        private void Awake()
        {
            Init();
        }

        private bool _init = false;
        public virtual bool Init()
        {
            if (_init)
                return false;

            _init = true;
            return true;
        }

        // private void Update()
        // {
        //     UpdateController();
        // }

        // public virtual void UpdateController()
        // {
        // }
    }
}

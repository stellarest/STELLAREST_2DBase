using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace STELLAREST_SFH
{
    public class InitBase : MonoBehaviour
    {
        protected bool _init = false;

        public virtual bool Init()
        {
            if (_init)
                return false;

            _init = true;
            return true;
        }

        private void Awake() => Init();

#if UNITY_EDITOR
        [Conditional("UNITY_EDITOR")]
        public void InitLog(object type)
            => Util.Log($"{nameof(type)}::{nameof(Init)}");
#endif        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class CameraController : InitBase
    {
        private BaseObject _target = null;
        public BaseObject Target
        {
            get => _target;
            set => _target = value;
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Camera.main.orthographicSize = FixedValue.Numeric.CAM_DEFAULT_ORTHO_SIZE;
            
            return true;
        }

        private void LateUpdate()
        {
            if (_target == null)
                return;

            // TEMP
            Vector3 targetPosition = new Vector3(_target.transform.position.x, 
                _target.transform.position.y + 1.3f, -10f);
            transform.position = targetPosition;
        }
    }
}


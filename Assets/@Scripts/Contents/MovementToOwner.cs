using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace STELLAREST_2D
{
    public class MovementToOwner : MonoBehaviour
    {
        private Transform Owner = null;
        public void SetOwner(Transform owner) => Owner = owner;
        private void LateUpdate()
        {
            if (Owner != null)
                transform.position = Owner.transform.position;
        }
    }
}


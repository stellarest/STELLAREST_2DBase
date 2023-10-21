using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace STELLAREST_2D
{
    public class MovementToOwner : MonoBehaviour
    {
        public CreatureController Owner { get; set; } = null;
        public bool IsOnFireSocket { get; set; } = false;
        private void LateUpdate()
        {
            if (Owner != null && IsOnFireSocket)
                transform.position = Owner.FireSocketPosition;
            else if (Owner != null && IsOnFireSocket == false)
                transform.position = Owner.Center.position;
        }
    }
}


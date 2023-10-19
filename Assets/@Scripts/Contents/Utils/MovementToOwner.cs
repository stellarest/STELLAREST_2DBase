using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace STELLAREST_2D
{
    public class MovementToOwner : MonoBehaviour
    {
        public CreatureController Owner { get; set; } = null;
        private void LateUpdate()
        {
            if (Owner != null)
                transform.position = Owner.FireSocketPosition;
        }
    }
}


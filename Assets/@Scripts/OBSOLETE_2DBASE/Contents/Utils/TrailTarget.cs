using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class TrailTarget : MonoBehaviour
    {
        public CreatureController Owner { get; set; } = null;
        public BaseController Target { get; set; } = null;

        private void LateUpdate()
        {
            if (Target.IsValid() == false)
            {
                this.transform.position = Owner.FireSocketPosition;
                Managers.Resource.Destroy(gameObject);
                Owner = null;
                Target = null;
                return;
            }

            this.transform.position = Target.TrailSocket.position;
        }
    }
}


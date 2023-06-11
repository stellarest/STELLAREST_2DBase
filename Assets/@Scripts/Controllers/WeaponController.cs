using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class WeaponController : MonoBehaviour
    {
        public Transform GrabPoint { get; set; }
        public Transform WeaponPoint { get; set; }
        public void UpdateWeapon(Define.WeaponType weaponType, float angle)
        {
            switch (weaponType)
            {
                case Define.WeaponType.Firearm2H:
                    RotateFirearm2H(angle);
                    break;

                default:
                    break;
            }
        }

        // 나중에 수정. 일단은 Fixed
        private void RotateFirearm2H(float angle)
        {
        }
    }
}

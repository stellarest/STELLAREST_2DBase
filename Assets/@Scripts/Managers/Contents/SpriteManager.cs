using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace STELLAREST_2D
{
    public class SpriteManager
    {
        private const string SPRITE_NINJA_SWORD = "NinjaSword.sprite";

        public enum WeaponType
        {
            NinjaSword
        }

        public Sprite LoadWeapon(WeaponType type) => GetWeapon(type);
        private Sprite GetWeapon(WeaponType type)
        {
            switch (type)
            {
                case WeaponType.NinjaSword:
                    return Managers.Resource.Load<Sprite>(SPRITE_NINJA_SWORD);

                default:
                    return null;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
#if UNITY_EDITOR
    public class DevUtility : MonoBehaviour
    {
        [field: SerializeField] public bool SetPlayerLowerHP { get; set; }

        private IEnumerator Start()
        {
            if (this.SetPlayerLowerHP == false)
                yield break;

            yield return new WaitForSeconds(2f);
            if (Managers.Game.Player != null)
            {
                if (this.SetPlayerLowerHP)
                {
                    Managers.Game.Player.Stat.MaxHP = 100f;
                    Managers.Game.Player.Stat.Armor = 0f;
                }
            }

            Utils.LogEndMethod(nameof(DevUtility), "SET LOWER HP");
        }
    }
#endif
}
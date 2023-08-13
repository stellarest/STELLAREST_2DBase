using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class TempSortingParticle : MonoBehaviour
    {
        [ContextMenu("Temp_SetParticleOrder")]
        private void Temp_SetParticleOrder()
        {
            foreach (var pr in GetComponentsInChildren<ParticleSystemRenderer>())
                pr.sortingOrder = (int)Define.SortingOrder.ParticleEffect;
        }
    }
}

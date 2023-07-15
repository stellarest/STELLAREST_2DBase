using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class PaladinSwingChild_Temp : MonoBehaviour
    {
        private void OnParticleSystemStopped()
        {
            Debug.Log("Paladin Swing Stopped !!");
            gameObject.SetActive(false);
            Managers.Resource.Destroy(this.gameObject);
        }
    }
}

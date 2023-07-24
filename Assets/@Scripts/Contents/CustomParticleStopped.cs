using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class CustomParticleStopped : MonoBehaviour
    {
        public void OnParticleSystemStopped() => 
                Managers.Resource.Destroy(gameObject);
    }
}


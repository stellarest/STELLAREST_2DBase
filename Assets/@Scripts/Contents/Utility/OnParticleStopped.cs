using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    public class OnParticleStopped : MonoBehaviour
    {
        // Init Clone으로부터 SetClonedRootTargetOnParticleStopped, Set RootTarget
        public GameObject RootTarget { get; set; } = null;

        public void OnParticleSystemStopped()
        {
            if (RootTarget != null)
                Managers.Resource.Destroy(RootTarget);
            else
                Managers.Resource.Destroy(gameObject);
        }
    }
}


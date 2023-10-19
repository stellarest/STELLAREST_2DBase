using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    public class OnCustomParticleStopped : MonoBehaviour
    {
        // Init Clone으로부터 SetClonedRootTargetOnParticleStopped, Set RootTarget
        public SkillBase SkillParticleRootTarget { get; set; } = null;

        public void OnParticleSystemStopped()
        {
            if (SkillParticleRootTarget != null)
                Managers.Object.Despawn(SkillParticleRootTarget);
            else
                Managers.Resource.Destroy(gameObject);
        }
    }
}


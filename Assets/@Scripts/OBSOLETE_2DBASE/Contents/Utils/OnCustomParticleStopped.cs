using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    public class OnCustomParticleStopped : MonoBehaviour
    {
        public SkillBase SkillParticleRootTarget = null;

        public void OnParticleSystemStopped()
        {
            if (SkillParticleRootTarget != null)
                Managers.Object.Despawn(SkillParticleRootTarget);
            else
                Managers.Resource.Destroy(gameObject);
        }
    }
}


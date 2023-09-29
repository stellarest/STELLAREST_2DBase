using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    public class OnParticleStopped : MonoBehaviour
    {
        // Child Object의 ParticleSystem에 Callback을 하고 싶은 경우
        // 반드시 스킬 클래스로부터 InitOnParticleStopped()를 호출해야함
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


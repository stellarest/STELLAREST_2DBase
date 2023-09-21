using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    // FOR CHLOE
    public class RangedMagicShot : RepeatSkill
    {
        protected override void DoSkillJob()
        {
        }

        public override void OnPreSpawned()
        {
            base.OnPreSpawned();

            foreach (var particle in GetComponentsInChildren<ParticleSystem>())
            {
                var emission = particle.emission;
                emission.enabled = false;
            }

            GetComponent<Rigidbody2D>().simulated = false;
            GetComponent<Collider2D>().enabled = false;
        }

        public override void SetParticleInfo(Vector3 startAngle, Define.LookAtDirection lookAtDir, 
                                            float continuousAngle, float continuousFlipX, float continuousFlipY)
        {
        }

        public override void InitRepeatSkill(RepeatSkill originRepeatSkill)
        {
            throw new System.NotImplementedException();
        }
    }
}

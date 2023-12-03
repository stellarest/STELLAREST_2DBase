using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    // FOR CHLOE
    public class RangedMagicShot : PublicSkill
    {
        protected override void DoSkillJob()
        {
        }

        // public override void OnPreSpawned()
        // {
        //     base.OnPreSpawned();

        //     foreach (var particle in GetComponentsInChildren<ParticleSystem>())
        //     {
        //         var emission = particle.emission;
        //         emission.enabled = false;
        //     }

        //     GetComponent<Rigidbody2D>().simulated = false;
        //     GetComponent<Collider2D>().enabled = false;
        // }
    }
}

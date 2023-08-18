using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class MeleeSwing : RepeatSkill // Projectile
    {
        public float TestAngle = 0f;

        public void SetSwingInfo(CreatureController owner, int templateID, Vector3 indicatorAngle, 
        float turningSide, float continuousAngle, float continuousFlipX, float continuousFlipY)
        {
            base.SetSkillInfo(owner, templateID);

            //SkillType = Define.TemplateIDs.SkillType.PaladinMeleeSwing;
            SkillType = owner.SkillBook.PlayerDefaultSkill;
            Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
            
            foreach (var particleRenderer in GetComponentsInChildren<ParticleSystemRenderer>())
            {
                particleRenderer.sortingOrder = (int)Define.SortingOrder.ParticleEffect;

                Vector3 tempAngle = indicatorAngle;
                // transform.localEulerAngles = tempAngle;
                tempAngle.z += continuousAngle;
                tempAngle.z += TestAngle;
                transform.rotation = Quaternion.Euler(tempAngle);

                var main = GetComponent<ParticleSystem>().main;
                main.startRotation = Mathf.Deg2Rad * tempAngle.z * -1f;
                main.flipRotation = turningSide;
                // transform.position = pos;
                // transform.localScale = localScale;
                particleRenderer.flip = new Vector3(continuousFlipX, continuousFlipY, 0);
            }

            // +++ PARTICLE RENDERER TEMP +++
            // var particleRenderer = GetComponent<ParticleSystemRenderer>();
            // particleRenderer.sortingOrder = (int)Define.SortingOrder.ParticleEffect;

            // Vector3 tempAngle = indicatorAngle;
            // // transform.localEulerAngles = tempAngle;
            // tempAngle.z += continuousAngle;
            // tempAngle.z += TestAngle;
            // transform.rotation = Quaternion.Euler(tempAngle);

            // var main = GetComponent<ParticleSystem>().main;
            // main.startRotation = Mathf.Deg2Rad * tempAngle.z * -1f;
            // main.flipRotation = turningSide;
            // // transform.position = pos;
            // // transform.localScale = localScale;
            // particleRenderer.flip = new Vector3(continuousFlipX, continuousFlipY, 0);
            // +++ PARTICLE RENDERER TEMP END +++

            GetComponent<Collider2D>().enabled = true;
        }

        protected override void DoSkillJob()
        {
            //Managers.Game.Player.Attack();
            Managers.Game.Player.CreatureState = Define.CreatureState.Attack;
        }

        public override void OnPreSpawned()
        {
            base.OnPreSpawned();

            // if (GetComponent<ParticleSystem>() != null)
            // {
            //     var emission = GetComponent<ParticleSystem>().emission;
            //     emission.enabled = false;
            // }
            // else
            // {
            //     foreach (var particle in GetComponentsInChildren<ParticleSystem>())
            //     {
            //         var emission = particle.emission;
            //         emission.enabled = false;
            //     }
            // }

            foreach (var particle in GetComponentsInChildren<ParticleSystem>())
            {
                var emission = particle.emission;
                emission.enabled = false;
            }

            foreach (var col in GetComponents<Collider2D>())
                col.enabled = false;
        }
    }
}


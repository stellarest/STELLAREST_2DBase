using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace STELLAREST_2D
{
    public class MeleeSwing : RepeatSkill // Projectile
    {
        public float TestParticleAngle = 0f;
        private ParticleSystem[] _particles = null;
        private ParticleSystemRenderer[] _particleRenderers = null;

        public override void InitRepeatSkill(RepeatSkill originRepeatSkill)
        {
            // 단순히 프리팹에 미리 AddComponent만 했기 때문에 당연히 RepeatSkill과 Owner가 누구인지 알 수 없다. Set해야함.
            // this.RepeatSkill = originRepeatSkill;
            // Owner = originRepeatSkill.Owner;
            // Type = Define.SkillType.Repeat;

            // _particles = GetComponents<ParticleSystem>();
            // _particleRenderers = GetComponents<ParticleSystemRenderer>();
            SetRenderSorting();

            //Utils.LogStrong("Success::InitRepeatSkill in MeleeSwing.");
        }

        protected override void SetRenderSorting() 
                => GetComponent<SortingGroup>().sortingOrder = (int)Define.SortingOrder.ParticleEffect;

        public override void SetParticleInfo(Vector3 startAngle, Define.LookAtDirection lookAtDir, 
                                            float continuousAngle, float continuousFlipX, float continuousFlipY)
        {
            // foreach (var particleRenderer in GetComponentsInChildren<ParticleSystemRenderer>())
            // {
            //     particleRenderer.sortingOrder = (int)Define.SortingOrder.ParticleEffect;

            //     Vector3 tempAngle = startAngle;
            //     tempAngle.z += continuousAngle;
            //     tempAngle.z += TestParticleAngle;
            //     transform.rotation = Quaternion.Euler(tempAngle);

            //     var main = GetComponent<ParticleSystem>().main;
            //     main.startRotation = Mathf.Deg2Rad * tempAngle.z * -1f;
            //     main.flipRotation = (int)lookAtDir;
            //     particleRenderer.flip = new Vector3(continuousFlipX, continuousFlipY, 0);
            // }

            for (int i = 0; i < _particles.Length; ++i)
            {
                Vector3 tempAngle = startAngle;
                tempAngle.z += continuousAngle;
                //tempAngle.z += TestParticleAngle;
                transform.rotation = Quaternion.Euler(tempAngle);

                var main = _particles[i].main;
                main.startRotation = Mathf.Deg2Rad * tempAngle.z * -1f;
                main.flipRotation = (int)lookAtDir;
                _particleRenderers[i].flip = new Vector3(continuousFlipX, continuousFlipY, 0);
            }
        }

        // public void SetSwingInfo(CreatureController owner, int templateID, Vector3 indicatorAngle, 
        // Define.LookAtDirection lookAtDir, float continuousAngle, float continuousFlipX, float continuousFlipY)
        // {
        //     //base.SetSkill(owner, templateID);
        //     //RepeatSkillType = owner.SkillBook.MasterySkillType;

        //     Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
            
        //     foreach (var particleRenderer in GetComponentsInChildren<ParticleSystemRenderer>())
        //     {
        //         particleRenderer.sortingOrder = (int)Define.SortingOrder.ParticleEffect;

        //         Vector3 tempAngle = indicatorAngle;
        //         tempAngle.z += continuousAngle;
        //         tempAngle.z += TestParticleAngle;
        //         transform.rotation = Quaternion.Euler(tempAngle);

        //         var main = GetComponent<ParticleSystem>().main;
        //         main.startRotation = Mathf.Deg2Rad * tempAngle.z * -1f;
        //         main.flipRotation = (int)lookAtDir;
        //         particleRenderer.flip = new Vector3(continuousFlipX, continuousFlipY, 0);
        //     }
        // }

        protected override void DoSkillJob()
        {
            //Managers.Game.Player.Attack();
            //Managers.Game.Player.CreatureState = Define.CreatureState.Attack;
            //Debug.Log("Player Melee Attack.");
            if (Owner?.IsPlayer() == true)
            {
                Managers.Game.Player.CreatureState = Define.CreatureState.Attack;
            }
        }

        public override void OnPreSpawned()
        {
            base.OnPreSpawned();
            
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


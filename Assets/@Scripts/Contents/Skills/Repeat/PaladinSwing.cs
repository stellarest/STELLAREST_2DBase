using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class PaladinSwing : RepeatSkill // Projectile
    {
        public void SetSwingInfo(CreatureController owner, int templateID,
            Vector3 indicatorAngle, float turningSide, Vector3 pos, Vector3 localScale, float continuousAngle, float continuousFlipX)
        {
            base.SetSkillInfo(owner, templateID);

            var particleRenderer = GetComponent<ParticleSystemRenderer>();
            particleRenderer.sortingOrder = (int)Define.SortingOrder.ParticleEffect;
            //GetComponent<ParticleSystemRenderer>().sortingOrder = (int)Define.SortingOrder.ParticleEffect;

            Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);

            // 일단 회전만 설정
            Vector3 tempAngle = indicatorAngle;
            // transform.localEulerAngles = tempAngle;
            tempAngle.z += continuousAngle;
            transform.rotation = Quaternion.Euler(tempAngle);

            var main = GetComponent<ParticleSystem>().main;
            main.startRotation = Mathf.Deg2Rad * tempAngle.z * -1f;
            main.flipRotation = turningSide;
            transform.position = pos;
            transform.localScale = localScale;

            particleRenderer.flip = new Vector3(continuousFlipX, 0, 0);
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
            var emission = GetComponent<ParticleSystem>().emission;
            emission.enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}


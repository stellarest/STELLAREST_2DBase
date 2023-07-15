using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class PaladinSwing : RepeatSkill // Projectile
    {
        public override void SetSkillInfo(CreatureController owner, int templateID)
        {
            base.SetSkillInfo(owner, templateID);
            GetComponent<ParticleSystemRenderer>().sortingOrder = (int)Define.SortingOrder.ParticleEffect;
            Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);

            // 일단 회전만 설정
            Vector3 tempAngle = Managers.Game.Player.Indicator.eulerAngles;
            transform.localEulerAngles = tempAngle;

            var main = GetComponent<ParticleSystem>().main;
            main.startRotation = Mathf.Deg2Rad * tempAngle.z * -1f;
            main.flipRotation = Managers.Game.Player.TurningAngle;
            transform.position = Managers.Game.Player.transform.position;
            transform.localScale = Managers.Game.Player.AnimationLocalScale;
            GetComponent<Collider2D>().enabled = true;
        }

        protected override IEnumerator CoStartSkill()
        {
            WaitForSeconds wait = new WaitForSeconds(SkillData.CoolTime);
            while (true)
            {
                DoSkillJob();
                yield return wait;
            }
        }

        protected override void DoSkillJob()
        {
            Managers.Game.Player.Attack();
        }

        public override void OnPreSpawned()
        {
            var emission = GetComponent<ParticleSystem>().emission;
            emission.enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }

        // if (Managers.Game.Player.IsMoving && _offParticle == false) // Moving Melee Attack
        //     _speed = Owner.CreatureData.MoveSpeed + SkillData.Speed;
        // else // Static Melee Attack
        //     _speed = SkillData.Speed;
        // SetOffParticle();
        //transform.position += _shootDir * SkillData.Speed * Time.deltaTime;
        //transform.position += _shootDir * _speed * Time.deltaTime;
    }
}


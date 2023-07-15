using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class PaladinSwing_Temp : RepeatSkill
    {
        private ParticleSystem[] _particles;
        private WaitForSeconds _coolTime;
        private int _attackCount;

        // public override void SetSkillInfo(CreatureController owner, int templateID)
        // {
        //     base.SetSkillInfo(owner, templateID);

        //     _particles = new ParticleSystem[transform.childCount];
        //     for (int i = 0; i < _particles.Length; ++i)
        //     {
        //         _particles[i] = transform.GetChild(i).GetComponent<ParticleSystem>();
        //         _particles[i] = transform.GetChild(i).GetComponent<ParticleSystem>();
        //         _particles[i].GetComponent<ParticleSystemRenderer>().sortingOrder
        //                                          = (int)Define.SortingOrder.ParticleEffect;

        //         // Managers.Collision.InitCollisionLayer(_particles[i].gameObject, Define.CollisionLayers.PlayerAttack);
        //         if (_particles[i].transform.childCount > 0)
        //         {
        //             for (int j = 0; j < _particles[i].transform.childCount; ++j)
        //             {
        //                 _particles[i].transform.GetChild(j).GetComponent<ParticleSystemRenderer>().sortingOrder
        //                                          = (int)Define.SortingOrder.ParticleEffect;
        //                 // Managers.Collision.InitCollisionLayer(_particles[i].transform.GetChild(j).gameObject, Define.CollisionLayers.PlayerAttack);
        //             }
        //         }
        //     }

        //     _coolTime = new WaitForSeconds(SkillData.CoolTime);
        //     _attackCount = SkillData.ContinuousCount;
        //     // Managers.Game.Player.AnimEvents.OnRepeatAttack += Activate;
        // }

        private Vector3 _shootDir = Vector3.zero;
        private float _speed = 0f;
        private float _initialTurningDir = 0f; // turn -> turn 하면 다시 파티클이 플레이어를 쫓아가는 것을 막아줌
        private bool _offParticle = false;

        private void Activate()
        {
            int currentSkillGrade = (int)SkillData.InGameGrade - 1;
            _attackCount = SkillData.ContinuousCount;

            Vector3 tempAngle = Managers.Game.Player.Indicator.eulerAngles;
            transform.localEulerAngles = tempAngle;

            var main = _particles[currentSkillGrade].main;
            main.startRotation = Mathf.Deg2Rad * tempAngle.z * -1f;
            main.flipRotation = Managers.Game.Player.TurningAngle;
            transform.position = Managers.Game.Player.transform.position;
            transform.localScale = Managers.Game.Player.AnimationLocalScale;

            _shootDir = Managers.Game.Player.ShootDir;
            _speed = Owner.CreatureData.MoveSpeed;
            _initialTurningDir = Managers.Game.Player.TurningAngle;
            _offParticle = false;

            if (_attackCount == 1)
            {
                _particles[currentSkillGrade].gameObject.SetActive(true);
            }
            else
                StartCoroutine(CoContinuousAttack(_particles[currentSkillGrade]));
        }

        private IEnumerator CoContinuousAttack(ParticleSystem particle)
        {
            // 미리 위치 회전부터 세팅
            for (int i = 0; i < particle.transform.childCount; ++i)
            {
                Vector3 tempAngle = Managers.Game.Player.Indicator.eulerAngles;
                var main = particle.transform.GetChild(i).GetComponent<ParticleSystem>().main;
                main.startRotation = Mathf.Deg2Rad * tempAngle.z * -1f;
                main.flipRotation = Managers.Game.Player.TurningAngle;
                // 부모부터 on하고 싶은데 자식을 미리 false 안해놓으면 부모까지 한번에 true가 되버림
                particle.transform.GetChild(i).gameObject.SetActive(false);
            }

            particle.gameObject.SetActive(true);
            
            for (int i = 0; i < particle.transform.childCount; ++i)
            {
                yield return new WaitForSeconds(SkillData.CoolTime / (particle.transform.childCount + 1) - 0.1f);
                particle.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if (Managers.Game.Player.IsMoving && _offParticle == false)
                MovingSwingAttack();
            else
                StaticSwingAttack();
        }

        private void MovingSwingAttack()
        {
            _speed = Owner.CreatureData.MoveSpeed + SkillData.Speed;
            transform.position += _shootDir * _speed * Time.deltaTime;
        }

        private void StaticSwingAttack()
        {
            _speed = SkillData.Speed;
            transform.position += _shootDir * _speed * Time.deltaTime;
        }

        private void LateUpdate()
        {
            if (_initialTurningDir != Managers.Game.Player.TurningAngle)
            {
                //_speed = SkillData.ProjectileSpeed;
                _offParticle = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            MonsterController mc = other.GetComponent<MonsterController>();
            if (mc.IsValid() == false)
                return;

            if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.MonsterBody, other.gameObject.layer))
            {
                Debug.Log("DMG : " + Owner.CreatureData.Power * SkillData.DamageUpMultiplier);
                mc.OnDamaged(Owner, this, Owner.CreatureData.Power * SkillData.DamageUpMultiplier);
            }
        }

        protected override IEnumerator CoStartSkill()
        {
            //WaitForSeconds wait = new WaitForSeconds(SkillData.CoolTime);
            while (true)
            {
                DoSkillJob();
                yield return _coolTime;
                // yield return null;
            }
        }

        protected override void DoSkillJob()
        {
            _coolTime = new WaitForSeconds(SkillData.CoolTime);
            Managers.Game.Player.Attack();
        }

        public override void OnPreSpawned()
        {
            throw new System.NotImplementedException();
        }
    }
}

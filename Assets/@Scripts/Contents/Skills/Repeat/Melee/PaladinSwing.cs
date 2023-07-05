using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class PaladinSwing : RepeatSkill
    {
        private ParticleSystem[] _particles;
        private WaitForSeconds _coolTime;
        private int _attackCount;

        public override void SetInitialSkillInfo(CreatureController owner, int templateID)
        {
            base.SetInitialSkillInfo(owner, templateID);

            _particles = new ParticleSystem[transform.childCount];
            for (int i = 0; i < _particles.Length; ++i)
            {
                _particles[i] = transform.GetChild(i).GetComponent<ParticleSystem>();
                _particles[i].GetComponent<ParticleSystemRenderer>().sortingOrder = (int)Define.SortingOrder.RepeatParticleEffect;
            }

            Managers.Game.Player.AnimEvents.OnRepeatAttack += Activate;
            _coolTime = new WaitForSeconds(SkillData.CoolTime);
            _attackCount = SkillData.ProjectileCount;
        }

        private Vector3 _shootDir = Vector3.zero;
        private float _speed = 0f;
        //private bool _switchDir = false;
        private float _initialTurningDir = 0f;
        // turn -> turn 하면 다시 파티클이 플레이어를 쫓아가는 것을 막아줌
        private bool _offParticle = false;
        private void Activate()
        {
            int currentSkillGrade = (int)this.SkillGrade - 1;
            _attackCount = SkillData.ProjectileCount;

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
                _particles[currentSkillGrade].gameObject.SetActive(true);
            else
            {
                //Debug.Log("ACTIVE !!! !!! !!!");
                StartCoroutine(CoContinuousAttack(_particles[currentSkillGrade]));
            }
        }

        private IEnumerator CoContinuousAttack(ParticleSystem particle)
        {
            for (int i = 0; i < particle.transform.childCount; ++i)
            {
                //yield return new WaitForSeconds(0.2f);
                particle.transform.GetChild(i).GetComponent<ParticleSystemRenderer>().sortingOrder =
                                                                (int)Define.SortingOrder.RepeatParticleEffect;

                Vector3 tempAngle = Managers.Game.Player.Indicator.eulerAngles;
                var main = particle.transform.GetChild(i).GetComponent<ParticleSystem>().main;
                main.startRotation = Mathf.Deg2Rad * tempAngle.z * -1f;
                main.flipRotation = Managers.Game.Player.TurningAngle;
                //particle.transform.GetChild(i).gameObject.SetActive(true);
            }

            particle.gameObject.SetActive(true);
            particle.transform.GetChild(0).gameObject.SetActive(false);
            particle.transform.GetChild(1).gameObject.SetActive(false);
            for (int i = 0; i < particle.transform.childCount; ++i)
            {
                yield return new WaitForSeconds(0.2f);
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
            _speed = Owner.CreatureData.MoveSpeed + SkillData.ProjectileSpeed;
            transform.position += _shootDir * _speed * Time.deltaTime;
        }

        private void StaticSwingAttack()
        {
            _speed = SkillData.ProjectileSpeed;
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

        protected override IEnumerator CoStartSkill()
        {
            //WaitForSeconds wait = new WaitForSeconds(SkillData.CoolTime);
            while (true)
            {
                DoSkillJob();
                yield return _coolTime;
            }
        }

        protected override void DoSkillJob()
        {
            _coolTime = new WaitForSeconds(SkillData.CoolTime);
            Managers.Game.Player.Attack();
        }
    }
}

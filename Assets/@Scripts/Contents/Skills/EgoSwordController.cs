using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class EgoSwordController : SkillController
    {
        [SerializeField] private ParticleSystem[] _swingParticles;

        protected enum SwingType
        {
            First,
            Second,
            Third,
            Fourth
        }

        public override bool Init()
        {
            base.Init();

            // Active 될 때 까지 콜라이더 물리적용 X
            for (int i = 0; i < _swingParticles.Length; ++i)
                _swingParticles[i].GetComponent<Rigidbody2D>().simulated = false;

            for (int i = 0; i < _swingParticles.Length; ++i)
                _swingParticles[i].gameObject.GetOrAddComponent<EgoSwordChild>().SetInfo(Managers.Object.Player, 100);

            return true;
        }

        public void ActivateSkill()
        {
            StartCoroutine(CoSwingSword());
        }

        private float CoolTime = 2.0f;
        private IEnumerator CoSwingSword()
        {
            while (true)
            {
                yield return new WaitForSeconds(CoolTime);

                SetParticles(SwingType.First);
                _swingParticles[(int)SwingType.First].Play();
                TurnOnPhysics(SwingType.First, true);
                yield return new WaitForSeconds(_swingParticles[(int)SwingType.First].main.duration);
                TurnOnPhysics(SwingType.First, false);


                SetParticles(SwingType.Second);
                _swingParticles[(int)SwingType.Second].Play();
                TurnOnPhysics(SwingType.Second, true);
                yield return new WaitForSeconds(_swingParticles[(int)SwingType.Second].main.duration);
                TurnOnPhysics(SwingType.Second, false);


                SetParticles(SwingType.Third);
                _swingParticles[(int)SwingType.Third].Play();
                TurnOnPhysics(SwingType.Third, true);
                yield return new WaitForSeconds(_swingParticles[(int)SwingType.Third].main.duration);
                TurnOnPhysics(SwingType.Third, false);


                SetParticles(SwingType.Fourth);
                _swingParticles[(int)SwingType.Fourth].Play();
                TurnOnPhysics(SwingType.Fourth, true);
                yield return new WaitForSeconds(_swingParticles[(int)SwingType.Fourth].main.duration);
                TurnOnPhysics(SwingType.Fourth, false);
            }
        }

        private void SetParticles(SwingType swingType)
        {
            float z = transform.parent.transform.eulerAngles.z;
            float radian = (Mathf.PI / 180f) * z * -1;

            var main = _swingParticles[(int)swingType].main;
            main.startRotation = radian;
        }

        private void TurnOnPhysics(SwingType swingType, bool simulated) // simulated On : 충돌 체크, 피격 판정
        {
            for (int i = 0; i < _swingParticles.Length; ++i)
                _swingParticles[i].GetComponent<Rigidbody2D>().simulated = false;

            _swingParticles[(int)swingType].GetComponent<Rigidbody2D>().simulated = simulated;
        }
    }
}


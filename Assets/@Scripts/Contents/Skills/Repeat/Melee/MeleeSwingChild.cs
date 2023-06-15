using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class MeleeSwingChild : MonoBehaviour
    {
        //private float _speed = 30f;
        private float _speed = 10f;

        private Vector3 _shootDir = Vector3.zero;
        public ParticleSystem Particle { get; set; }
        public Transform MyParent { get; set; }

        private Vector3 _initLocalRot;
        private float _initRadian;
        private float _initFlip;

        private void LateUpdate()
        {
            transform.position += _shootDir * _speed * Time.deltaTime;

            // transform.localEulerAngles = _initLocalRot;
            // var main = Particle.main;
            // main.startRotation = _initRadian;

            // 반전에서 막힘. 스프라이트 랜더러를 전부 플립하면 되긴할텐데, 지금 스프라이트 랜더러가 너무 많아서
        }

        private void OnEnable()
        {
            if (Managers.Game.Player == null)
                return;

            // transform.eulerAngles : 0 ~ 360f
            _shootDir = Managers.Game.Player.ShootDir;

            Vector3 tempAngle = Managers.Game.Player.Indicator.eulerAngles;
            transform.localEulerAngles = tempAngle;
            _initLocalRot = tempAngle;

            float radian = Mathf.Deg2Rad * tempAngle.z * -1f;
            var main = Particle.main;
            main.startRotation = radian;
            _initRadian = radian;

            main.flipRotation = Managers.Game.Player.TurningAngle;
            // main.stopAction = ParticleSystemStopAction.Callback;
            _initFlip = main.flipRotation;

            //transform.position = Managers.Game.Player.FireSocket;
            transform.position = Managers.Game.Player.transform.position;
            _shootDir = Managers.Game.Player.ShootDir;
            // transform.SetParent(null);

            transform.localScale = Managers.Game.Player.AnimationLocalScale;

            // if (Managers.Game.Player.TurningAngle == 1f)
            //     transform.localScale *= -1f;
            // else
            //     transform.localScale *= 1f;

            // 기본 우측 -1
            // 좌측 터닝 1
        }

        // void OnParticleSystemStopped()
        // {
        //     transform.SetParent(MyParent);
        //     gameObject.SetActive(false);
        // }
    }
}


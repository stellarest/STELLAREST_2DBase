using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class InfernoSwingChild : MonoBehaviour
    {
        //private float _speed = 30f;
        [SerializeField]  float _speed = 10f; // TEST

        private Vector3 _shootDir = Vector3.zero;
        public ParticleSystem Particle { get; set; }
        // public Transform MyParent { get; set; }

        private Vector3 _initLocalRot;
        private float _initRadian;
        private float _initFlip;
        private BoxCollider2D _boxCollider;

        public void Init()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
        }

        private void LateUpdate()
        {
            transform.position += _shootDir * _speed * Time.deltaTime;
        }

        Coroutine _coOffCollider;
        private IEnumerator OffCollider()
        {
            _boxCollider.enabled = true;
            // yield return new WaitForSeconds(0.15f);
            // yield return new WaitForSeconds(0.05f); <<-- Particle Duration이 제일 자연스러운듯
            yield return new WaitForSeconds(0.1f);
            _boxCollider.enabled = false;
            //GetComponent<BoxCollider2D>().enabled = false;
        }

        private void OnEnable()
        {
            if (Managers.Game.Player == null)
                return;

            ControlBoxCollider();

            // transform.eulerAngles : 0 ~ 360f
            // _shootDir = Managers.Game.Player.ShootDir * 99f;

            Vector3 tempAngle = Managers.Game.Player.Indicator.eulerAngles;
            transform.localEulerAngles = tempAngle;
            _initLocalRot = tempAngle;

            float radian = Mathf.Deg2Rad * tempAngle.z * -1f;
            var main = Particle.main;
            main.startRotation = radian;
            _initRadian = radian;

            main.flipRotation = Managers.Game.Player.TurningAngle;
            //main.stopAction = ParticleSystemStopAction.Callback;
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

        private void ControlBoxCollider()
        {
            if (_coOffCollider != null)
                StopCoroutine(_coOffCollider);
            _coOffCollider = StartCoroutine(OffCollider());
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc?.IsMonster() == true)
            {
                Debug.Log("MONSTER");
                cc.OnDamaged(Managers.Game.Player, null, 0);
            }
        }

        private void OnParticleSystemStopped()
        {
            Debug.Log("Particle Stopped !!");
            //transform.SetParent(MyParent);
            gameObject.SetActive(false);
        }
    }
}


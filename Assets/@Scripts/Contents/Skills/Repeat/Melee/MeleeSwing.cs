using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class MeleeSwing : RepeatSkill
    {
        private ParticleSystem[] _swingParticles;
        private Vector3 _initLocalScale;
        private Vector3 _endLocalScale;
        private bool _coScaleLerp = false;

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Debug.Log("### Melee Swing Init ###");
            _swingParticles = new ParticleSystem[(int)SwingType.Max];
            _initLocalScale = new Vector3(1f, 1f, 1f);
            _endLocalScale = new Vector3(3f, 3f, 1f);

            for (int i = 0; i < _swingParticles.Length; ++i)
            {
                string childName = SkillData.Name;
                childName += (i + 1).ToString("D2");
                _swingParticles[i] = Utils.FindChild(gameObject, childName).GetComponent<ParticleSystem>();

                var meleeSwingChild = _swingParticles[i].gameObject.GetOrAddComponent<MeleeSwingChild>();
                meleeSwingChild.Particle = _swingParticles[i];
                meleeSwingChild.MyParent = transform;
            }

            // Managers.Game.Player.AnimEvents.OnCustomEvent += Swing;

            // Check Debug
            foreach(Transform tr in _swingParticles[(int)SwingType.Fifth].gameObject.transform)
                Debug.Log(tr.gameObject.name);

            return true;
        }

        protected override IEnumerator CoStartSkill()
        {
            //Managers.Game.Player.AnimEvents.OnCustomEvent += (s => Debug.Log(s));
            Managers.Game.Player.AnimEvents.OnCustomEvent += Swing;

            //WaitForSeconds wait = new WaitForSeconds(CoolTime);
            Debug.Log("Melee1H Swing Start !!");
            Debug.Log(CoolTime);

            while (true)
            {
                // SetParticle(SwingType.First);
                //_swingParticles[(int)SwingType.First].gameObject.SetActive(true);
                if (_coScaleLerp)
                    StartCoroutine(CoLerp());

                Managers.Game.Player.Attack(2f);
                yield return new WaitForSeconds(1f);
                //yield return new WaitForSeconds(_swingParticles[(int)SwingType.First].main.duration);

                // SetParticle(SwingType.Second);
                // _swingParticles[(int)SwingType.Second].gameObject.SetActive(true);
                // yield return new WaitForSeconds(0.25f);
                // // //yield return new WaitForSeconds(_swingParticles[(int)SwingType.Second].main.duration);

                // // SetParticle(SwingType.Third);
                // _swingParticles[(int)SwingType.Third].gameObject.SetActive(true);
                // yield return new WaitForSeconds(0.25f);
                // // //yield return new WaitForSeconds(_swingParticles[(int)SwingType.Third].main.duration);

                // // SetParticle(SwingType.Fourth);
                // _swingParticles[(int)SwingType.Fourth].gameObject.SetActive(true);
                // yield return new WaitForSeconds(0.25f);
                // // //yield return new WaitForSeconds(_swingParticles[(int)SwingType.Fourth].main.duration);

                // // SetParticle(SwingType.Fifth);
                // _swingParticles[(int)SwingType.Fifth].gameObject.SetActive(true);
                // yield return new WaitForSeconds(0.25f);
                //yield return new WaitForSeconds(_swingParticles[(int)SwingType.Fifth].main.duration);

                // yield return wait;
                // yield return null;
            }
        }

        private float elapsedTime = 0f;
        private float desiredTime = 1f;
        private IEnumerator CoLerp()
        {
            float percent = 0f;
            while (percent < 1f)
            {
                elapsedTime += Time.deltaTime * 2f;
                percent = elapsedTime / desiredTime;
                transform.localScale = Vector3.Lerp(_initLocalScale, _endLocalScale, percent);
                yield return null;
            }
            elapsedTime = 0f;
        }

        private void Swing(string s)
        {
            _swingParticles[(int)SwingType.First].gameObject.SetActive(true);
        }

        // private void SetParticle(SwingType swingType)
        // {
        //     if (Managers.Game.Player == null)
        //         return;

        //     // transform.eulerAngles : 0 ~ 360f
        //     Vector3 tempAngle = Managers.Game.Player.Indicator.eulerAngles;
        //     transform.localEulerAngles = tempAngle;
        //     transform.position = Managers.Game.Player.transform.position;

        //     float radian = Mathf.Deg2Rad * tempAngle.z * -1f;
        //     var main = _swingParticles[(int)swingType].main;
        //     main.startRotation = radian;
        //     main.flipRotation = Managers.Game.Player.TurningAngle;
        // }

        protected override void DoSkillJob() 
        {
        }

        private void OnDestroy()
        {
            Managers.Game.Player.AnimEvents.OnCustomEvent -= Swing;       
        }
    }
}


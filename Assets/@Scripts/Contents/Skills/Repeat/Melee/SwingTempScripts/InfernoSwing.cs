using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class InfernoSwing : RepeatSkill
    {
        private ParticleSystem[] _swingParticles;
        private Vector3 _initLocalScale;
        private Vector3 _endLocalScale;

        // 나중에 테스트용
        private bool _coScaleLerp = false;

        public override void SetInitialSkillInfo(CreatureController owner, int templateID)
        {
            base.SetInitialSkillInfo(owner, templateID);
            Debug.Log("### PowerSwing::SetInitialSkillInfo ###");

            _swingParticles = new ParticleSystem[(int)SwingType.Max];
            _initLocalScale = new Vector3(1f, 1f, 1f);
            _endLocalScale = new Vector3(3f, 3f, 1f);

            for (int i = 0; i < _swingParticles.Length; ++i)
            {
                string childName = SkillData.Name;
                childName += "_Lv";
                childName += (i + 1).ToString("D2");
                Debug.Log("CHILD NAME : " + childName);
                _swingParticles[i] = Utils.FindChild(gameObject, childName).GetComponent<ParticleSystem>();
                ParticleSystemRenderer particleRenderer = _swingParticles[i].GetComponent<ParticleSystemRenderer>();
                particleRenderer.sortingOrder = (int)Define.SortingOrder.RepeatParticleEffect;

                var infernoSwingChild = _swingParticles[i].gameObject.GetOrAddComponent<InfernoSwingChild>();
                infernoSwingChild.Particle = _swingParticles[i];
                infernoSwingChild.Init();
                // meleeSwingChild.MyParent = transform;
            }
        }

        public float TestAttackSpeed = 1f;
        public float TestCoolTime = 1f;

        // Activate Skill
        // 이녀석을 어디서 호출할까? 오버라이드 된거니까
        protected override IEnumerator CoStartSkill()
        {
            //Managers.Game.Player.AnimEvents.OnCustomEvent += (s => Debug.Log(s));
            Managers.Game.Player.AnimEvents.OnCustomEvent += Swing;
            //WaitForSeconds wait = new WaitForSeconds(CoolTime);
            // Debug.Log(CoolTime);

            while (true)
            {
                // SetParticle(SwingType.First);
                //_swingParticles[(int)SwingType.First].gameObject.SetActive(true);

                // TEMP
                if (_coScaleLerp)
                    StartCoroutine(CoLerp());

                //Debug.Log(Owner.CreatureData.RepeatAttackSpeed);
                //Managers.Game.Player.Attack(Owner.CreatureData.RepeatAttackSpeed);
                yield return new WaitForSeconds(Owner.CreatureData.RepeatAttackCoolTime);
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
            Debug.Log("CoLerp !!");
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

        protected override void DoSkillJob() // CoStartSkill -> DoSkillJob
        {
        }

        private void OnDestroy()
        {
            Managers.Game.Player.AnimEvents.OnCustomEvent -= Swing;
        }
    }
}

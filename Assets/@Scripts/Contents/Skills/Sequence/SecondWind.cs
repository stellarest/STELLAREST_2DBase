using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    public class SecondWind : SequenceSkill
    {
        private ParticleSystem _waitLoop = null;
        private ParticleSystem[] _onGroup = null;
        private ParticleSystem[] _burstGroup = null;

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);

            _waitLoop = transform.GetChild(0).GetComponent<ParticleSystem>();
            _waitLoop.transform.localPosition = Vector3.up;
            _waitLoop.gameObject.SetActive(false);

            _onGroup = transform.GetChild(1).GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            for (int i = 0; i < _onGroup.Length; ++i)
                _onGroup[i].gameObject.SetActive(false);

            _burstGroup = transform.GetChild(2).GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            transform.GetChild(2).GetComponent<SecondWindChild>().Init(owner, data, null);
            for (int i = 0; i < _burstGroup.Length; ++i)
                _burstGroup[i].gameObject.SetActive(false);
        }

        public override void DoSkillJob(Action callback = null)
        {
            Utils.Log("!!! ACTIVATE SECOND WIND !!!");
        }

        public void Wait()
        {
            _waitLoop.gameObject.SetActive(true);
            _waitLoop.Play();
        }

        public void On()
        {
            this.Owner.IsInvincible = true;

            for (int i = 0; i < _onGroup.Length; ++i)
            {
                _onGroup[i].gameObject.SetActive(true);
                _onGroup[i].Play();
            }

            StartCoroutine(CoOnSecondWind());
        }

        //private const float DESIRED_RECOVERY_TIME = 2.5f;
        private const float DESIRED_RECOVERY_TIME = 5f;

        private IEnumerator CoOnSecondWind()
        {
            float delta = 0f;
            float percentage = 0f;
            int percent = 0;

            while (percentage < 1f)
            {
                delta += Time.deltaTime;
                percentage = delta / DESIRED_RECOVERY_TIME;
                percent = Mathf.FloorToInt(percentage * 100);
                if (percent < percent + 1)
                {
                    Managers.VFX.Percentage(this.Owner, percent);
                }

                yield return null;
            }

            Burst();            
        }

        public void Burst()
        {
            _waitLoop.gameObject.SetActive(false);
            for (int i = 0; i < _onGroup.Length; ++i)
                _onGroup[i].gameObject.SetActive(false);

            for (int i = 0; i < _burstGroup.Length; ++i)
            {
                _burstGroup[i].gameObject.SetActive(true);
                _burstGroup[i].Play();
            }

            this.Owner.IsInvincible = false;
        }
    }
}

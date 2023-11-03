using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using STELLAREST_2D.Data;
using UnityEngine;

using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

namespace STELLAREST_2D
{
    public class SecondWind : SequenceSkill
    {
        private ParticleSystem _readyLoop = null;
        private ParticleSystem[] _onGroup = null;
        private ParticleSystem[] _burstGroup = null;
        [field: SerializeField] public bool IsReady { get; private set; } = false;

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);

            _readyLoop = transform.GetChild(0).GetComponent<ParticleSystem>();
            _readyLoop.transform.localPosition = Vector3.up;
            _readyLoop.gameObject.SetActive(false);

            _onGroup = transform.GetChild(1).GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            for (int i = 0; i < _onGroup.Length; ++i)
                _onGroup[i].gameObject.SetActive(false);

            _burstGroup = transform.GetChild(2).GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            transform.GetChild(2).GetComponent<SecondWindChild>().Init(owner, data);
            
            for (int i = 0; i < _burstGroup.Length; ++i)
                _burstGroup[i].gameObject.SetActive(false);
        }

        public override void DoSkillJob(Action callback = null)
        {
            this.Ready();
        }

        public void Ready()
        {
            IsReady = true;
            _readyLoop.gameObject.SetActive(true);
            _readyLoop.Play();
        }

        public void On()
        {
            this.Owner.SkillBook.Deactivate(SkillTemplate.KnightMastery);
            this.Owner.ReserveSkillAnimationType(this.Data.AnimationType);
            Owner.CreatureState = Define.CreatureState.Skill;

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
                this.Owner.Stat.Hp = Recovery(percent);
                Managers.VFX.Percentage(this.Owner, percent);

                yield return null;
            }

            this.Owner.Stat.Hp = this.Owner.Stat.MaxHp;
            Burst();            
        }

        private float Recovery(int percent)
        {
            float maxHp = this.Owner.Stat.MaxHp;
            float recoveryPercentage = percent / 100f;
            float recoveryAmount = maxHp * recoveryPercentage;
            return recoveryAmount;
        }

        public void Burst()
        {
            _readyLoop.gameObject.SetActive(false);
            for (int i = 0; i < _onGroup.Length; ++i)
                _onGroup[i].gameObject.SetActive(false);

            for (int i = 0; i < _burstGroup.Length; ++i)
            {
                _burstGroup[i].gameObject.SetActive(true);
                _burstGroup[i].Play();
            }

            this.Owner.IsInvincible = false;
            IsReady = false;

            this.Owner.SkillBook.Activate(SkillTemplate.KnightMastery);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_2D.Data;
using UnityEngine;

using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

namespace STELLAREST_2D
{
    public class Shield : SequenceSkill
    {
        private ParticleSystem[] _onShields = null;
        private ParticleSystem[] _onShieldsEnter = null;

        private ParticleSystem[] _offShields = null;
        private ParticleSystem _hitBurst = null;
        private const string INIT_HIT_BURST = "Hit_Burst";
        private const int ON_SHIELDS_ENTER_COUNT = 5;

        private bool _isOnShield = false;
        public bool IsOnShield 
        {
            get => _isOnShield;
            private set
            {
                this._isOnShield = value;
                if (_isOnShield)
                    this.Owner.Stat.ShieldHp = (this.Owner.Stat.MaxHp * 0.8f);
                else
                    this.Owner.Stat.ShieldHp = 0f;
            }
        }

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            this.transform.localPosition = new Vector3(0, 0.75f, 0);
            _onShields = transform.GetChild(0).gameObject.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            _onShieldsEnter = new ParticleSystem[ON_SHIELDS_ENTER_COUNT];

            for (int i = 0; i < _onShields.Length; ++i)
            {
                _onShields[i].gameObject.SetActive(false);
                if (_onShields[i].gameObject.name.Contains(INIT_HIT_BURST))
                    _hitBurst = _onShields[i].gameObject.GetComponent<ParticleSystem>();

                if (i >= 1 && i <= 5)
                    _onShieldsEnter[i - 1] = _onShields[i].gameObject.GetComponent<ParticleSystem>();
            }

            _offShields = transform.GetChild(1).gameObject.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            for (int i = 0; i < _offShields.Length; ++i)
                _offShields[i].gameObject.SetActive(false);

            IsOnShield = false;
        }

        public override void DoSkillJob(Action callback = null)
        {
            this.Owner.SkillBook.Deactivate(SkillTemplate.PaladinMastery);
            this.Owner.ReserveSkillAnimationType(this.Data.AnimationType);
            Owner.CreatureState = Define.CreatureState.Skill;
        }

        public override void OnActiveSequenceSkillHandler() => OnShield();
        public void Hit() => _hitBurst.Play();

        public void OnShield()
        {
            this.IsOnShield = true;

            for (int i = 0; i < _offShields.Length; ++i)
                _offShields[i].gameObject.SetActive(false);

            for (int i = 0; i < _onShields.Length; ++i)
            {
                _onShields[i].gameObject.SetActive(true);
                _onShields[i].Play();
            }

            StartCoroutine(CoIsPlayingOnShield());
        }

        private IEnumerator CoIsPlayingOnShield()
        {
            bool isPlaying = true;
            while (isPlaying)
            {
                bool isAnyPlaying = false;
                for (int i = 0; i < _onShieldsEnter.Length; ++i)
                {
                    if (_onShieldsEnter[i].isPlaying)
                    {
                        isAnyPlaying = true;
                        break;
                    }
                }

                if (isAnyPlaying == false)
                    isPlaying = false;

                yield return null;
            }

            this.Owner.SkillBook.Activate(SkillTemplate.PaladinMastery);
        }

        public void OffShield()
        {
            this.IsOnShield = false;

            for (int i = 0; i < _onShields.Length; ++i)
                _onShields[i].gameObject.SetActive(false);

            for (int i = 0; i < _offShields.Length; ++i)
            {
                _offShields[i].gameObject.SetActive(true);
                _offShields[i].Play();
            }
        }

        private IEnumerator CoIsPlayingOffShield()
        {
            OffShield();
            bool isPlaying = true;
            while (isPlaying)
            {
                bool isAnyPlaying = false;
                for (int i = 0; i < _offShields.Length; ++i)
                {
                    if (_offShields[i].isPlaying)
                    {
                        isAnyPlaying = true;
                        break;
                    }
                }

                if (isAnyPlaying == false)
                    isPlaying = false;

                yield return null;
            }

            base.Deactivate();
        }

        public override void Deactivate()
        {
            if (this.IsOnShield == false)
            {
                base.Deactivate();
                return;
            }

            StartCoroutine(CoIsPlayingOffShield());
        }
    }
}

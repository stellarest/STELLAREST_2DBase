using System;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_2D.Data;
using UnityEngine;

using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

namespace STELLAREST_2D
{
    /*
        > Ability Info <
        lv.1 : Shield Hp = Max Hp 50%
        lv.2 : Shield Hp = Max Hp 60%
        lv.3 : Shield Hp = Max Hp 90%
    */

    public class Shield : ActionSkill
    {
        private ParticleSystem[] _onShields = null;
        private ParticleSystem[] _onShieldsEnter = null;

        private ParticleSystem[] _offShields = null;
        private ParticleSystem _hitBurst = null;
        private const string INIT_HIT_BURST = "Hit_Burst";
        private const int ON_SHIELDS_ENTER_COUNT = 5;
        private float _shieldMaxHp = 0f;

        private bool _isOnShield = false;
        public bool IsOnShield 
        {
            get => _isOnShield;
            set
            {
                this._isOnShield = value;
                if (_isOnShield)
                    OnShield();
                else
                {
                    OffShield();
                }
            }
        }

        private Coroutine _coRecoveryShield = null;
        private const float RECOVERY_INTERVAL = 1.25f;
        private IEnumerator CoRecoveryShield()
        {
            float delta = 0f;
            while (this.IsOnShield)
            {
                delta += Time.deltaTime;
                if (delta >= RECOVERY_INTERVAL)
                {
                    this.Owner.Stat.ShieldHp++;
                    if (this.Owner.Stat.ShieldHp >= _shieldMaxHp)
                        this.Owner.Stat.ShieldHp = _shieldMaxHp;

                    delta = 0f;
                }

                yield return null;
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

            //IsOnShield = false;
            _isOnShield = false;
        }

        protected override IEnumerator CoStartSkill()
        {
            DoSkillJob();
            yield return null;
        }

        protected override void DoSkillJob(Action callback = null)
        {
            this.Owner.SkillBook.Deactivate(SkillTemplate.PaladinMastery);
            this.Owner.ReserveSkillAnimationType(this.Data.AnimationType);
            Owner.CreatureState = Define.CreatureState.Skill;
        }

        public override void OnActiveEliteActionHandler() => this.IsOnShield = true;
        public void Hit() => _hitBurst.Play();
        private const float SHIELD_MAX_HP_RATIO = 0.5f;
        private void OnShield()
        {
            //this.IsOnShield = true;
            for (int i = 0; i < _offShields.Length; ++i)
                _offShields[i].gameObject.SetActive(false);

            for (int i = 0; i < _onShields.Length; ++i)
            {
                _onShields[i].gameObject.SetActive(true);
                _onShields[i].Play();
            }

            _shieldMaxHp = (this.Owner.Stat.MaxHp * SHIELD_MAX_HP_RATIO);
            this.Owner.Stat.ShieldHp = _shieldMaxHp;

            if (_coRecoveryShield != null)
                StopCoroutine(_coRecoveryShield);
            _coRecoveryShield = StartCoroutine(CoRecoveryShield());
            this.Owner.SkillBook.Activate(SkillTemplate.PaladinMastery);
        }

        private void OffShield()
        {
            if (_coRecoveryShield != null)
            {
                StopCoroutine(_coRecoveryShield);
                _coRecoveryShield = null;
            }
            this.Owner.Stat.ShieldHp = 0f;

            for (int i = 0; i < _onShields.Length; ++i)
                _onShields[i].gameObject.SetActive(false);

            for (int i = 0; i < _offShields.Length; ++i)
            {
                _offShields[i].gameObject.SetActive(true);
                _offShields[i].Play();
            }

            StartCoroutine(CoIsPlayingOffShield());
        }

        private IEnumerator CoIsPlayingOffShield()
        {
            //OffShield();
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

            for (int i = 0; i < _offShields.Length; ++i)
                _offShields[i].gameObject.SetActive(false);

            this.Owner.SkillBook.Deactivate(SkillTemplate.Shield_Elite_Solo);
            //base.Deactivate();
        }

        public override void Deactivate() => base.Deactivate();
    }
}

// ==================================================
/*
// private IEnumerator CoIsPlayingOnShield()
        // {
        //     bool isPlaying = true;
        //     while (isPlaying)
        //     {
        //         bool isAnyPlaying = false;
        //         for (int i = 0; i < _onShieldsEnter.Length; ++i)
        //         {
        //             if (_onShieldsEnter[i].isPlaying)
        //             {
        //                 isAnyPlaying = true;
        //                 break;
        //             }
        //         }

        //         if (isAnyPlaying == false)
        //             isPlaying = false;

        //         yield return null;
        //     }

        //     this.Owner.SkillBook.Activate(SkillTemplate.PaladinMastery);
        // }
*/
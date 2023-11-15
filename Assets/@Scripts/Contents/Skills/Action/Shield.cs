using System;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_2D.Data;
using UnityEngine;

using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

namespace STELLAREST_2D
{
    /*
        [ Ability Info : Shield (Elite Action, Paladin) ]
        (Currently Temped Set : lv.3)

        lv.1 : 매 웨이브마다 최대 체력의 30%에 해당하는 쉴드 획득. 
            (쉴드가 활성화 되어 있을 때, 1.5초 마다 1%의 쉴드 회복)
        lv.2 : 매 웨이브마다 최대 체력의 40%에 해당하는 쉴드 획득. 
            (쉴드가 활성화 되어 있을 때, 1.25초 마다 1%의 쉴드 회복)
        lv.3 : 매 웨이브마다 최대 체력의 60%에 해당하는 쉴드 획득.
            (쉴드가 활성화 되어 있을 때, 0.75초 마다 1%의 쉴드 회복)
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
        private const float FIXED_RECOVERY_INTERVAL = 0.75f;
        private const float FIXED_RECOVERY_RATIO = 0.01f;

        private IEnumerator CoRecoveryShield()
        {
            float delta = 0f;
            while (this.IsOnShield)
            {
                delta += Time.deltaTime;
                if (delta >= FIXED_RECOVERY_INTERVAL)
                {
                    // Mathf.CeilToInt(3.5f) : 올림, 4
                    // Mathf.RoundToInt(3.5f) : 반올림, 4
                    // Mathf.FloorToInt(3.5f) : 내림, 3
                    int recoveryCount = Mathf.FloorToInt(delta / FIXED_RECOVERY_INTERVAL);
                    for (int i = 0; i < recoveryCount; ++i)
                    {
                        this.Owner.Stat.ShieldHp += (_shieldMaxHp * FIXED_RECOVERY_RATIO);
                        if (this.Owner.Stat.ShieldHp >= _shieldMaxHp)
                            this.Owner.Stat.ShieldHp = _shieldMaxHp;
                    }

                    delta %= FIXED_RECOVERY_INTERVAL;
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
            EnableParticles(_offShields, false);

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
        private const float SHIELD_MAX_HP_RATIO = 0.6f;
        private void OnShield()
        {
            EnableParticles(_offShields, false);
            EnableParticles(_onShields, true);

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

            EnableParticles(_onShields, false);
            EnableParticles(_offShields, true);

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

            EnableParticles(_offShields, false);
            this.Owner.SkillBook.Deactivate(SkillTemplate.Shield_Elite_Solo);
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

       // private IEnumerator CoRecoveryShield_PREV()
        // {
        //     float delta = 0f;
        //     while (this.IsOnShield)
        //     {
        //         delta += Time.deltaTime;
        //         if (delta >= FIXED_RECOVERY_INTERVAL)
        //         {
        //             this.Owner.Stat.ShieldHp += (_shieldMaxHp * FIXED_RECOVERY_RATIO);
        //             if (this.Owner.Stat.ShieldHp >= _shieldMaxHp)
        //                 this.Owner.Stat.ShieldHp = _shieldMaxHp;

        //             delta = 0f;
        //         }

        //         yield return null;
        //     }
        // }
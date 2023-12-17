using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_2D.Define;
using STELLAREST_2D.Data;

namespace STELLAREST_2D
{
    public class Shield : UniqueSkill
    {
        private enum ShieldValue
        {
            MAX_HP_RATIO,
            RECOVERY_RATIO_PER_SECOND
        }

        private ParticleSystem[] _onShields = null;
        private ParticleSystem[] _onShieldsEnter = null;
        private ParticleSystem[] _offShields = null;
        private ParticleSystem _hitBurst = null;
        private const string FIXED_INIT_HIT_BURST = "Hit_Burst";
        private const int FIXED_ON_SHIELDS_ENTER_COUNT = 5;
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
                    OffShield();
            }
        }

        private Coroutine _coRecoveryShield = null;
        //private const float FIXED_RECOVERY_INTERVAL = 0.75f;
        private const float FIXED_RECOVERY_INTERVAL = 1F; // ---> 없애도 됨

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
                        //this.Owner.Stat.ShieldHP += (_shieldMaxHp * FIXED_RECOVERY_RATIO);
                        this.Owner.Stat.ShieldHP += (_shieldMaxHp * _recoveryRate);

                        if (this.Owner.Stat.ShieldHP >= _shieldMaxHp)
                            this.Owner.Stat.ShieldHP = _shieldMaxHp;
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
            _onShieldsEnter = new ParticleSystem[FIXED_ON_SHIELDS_ENTER_COUNT];

            for (int i = 0; i < _onShields.Length; ++i)
            {
                _onShields[i].gameObject.SetActive(false);
                if (_onShields[i].gameObject.name.Contains(FIXED_INIT_HIT_BURST))
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
            this.Owner.SkillBook.Deactivate(FixedValue.TemplateID.Skill.Paladin_Unique_Mastery);
            this.Owner.CreatureSkillAnimType = this.Data.OwnerAnimationType;
            this.Owner.CreatureState = CreatureState.Skill;
            callback?.Invoke();
        }

        public override void OnActiveEliteActionHandler() => this.IsOnShield = true;
        public void Hit() => _hitBurst.Play();

        [SerializeField] private float _maxShiledHPRatio = 0f;
        [SerializeField] private float _recoveryRate = 0f;
        private void OnShield()
        {
            for (int i = 0; i < this.Data.CustomValue.Floatings.Count; ++i)
            {
                if (i == 0)
                    _maxShiledHPRatio = this.Data.CustomValue.Floatings[(int)ShieldValue.MAX_HP_RATIO];
                if (i == 1)
                    _recoveryRate = this.Data.CustomValue.Floatings[(int)ShieldValue.RECOVERY_RATIO_PER_SECOND];
            }

            EnableParticles(_offShields, false);
            EnableParticles(_onShields, true);

            _shieldMaxHp = (this.Owner.Stat.MaxHP * _maxShiledHPRatio);
            this.Owner.Stat.ShieldHP = _shieldMaxHp;

            if (_coRecoveryShield != null)
                StopCoroutine(_coRecoveryShield);
            _coRecoveryShield = StartCoroutine(CoRecoveryShield());
            this.Owner.SkillBook.Activate(FixedValue.TemplateID.Skill.Paladin_Unique_Mastery);
        }

        private void OffShield()
        {
            if (_coRecoveryShield != null)
            {
                StopCoroutine(_coRecoveryShield);
                _coRecoveryShield = null;
            }
            this.Owner.Stat.ShieldHP = 0f;

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
            this.Owner.SkillBook.Deactivate(FixedValue.TemplateID.Skill.Paladin_Unique_Elite);
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
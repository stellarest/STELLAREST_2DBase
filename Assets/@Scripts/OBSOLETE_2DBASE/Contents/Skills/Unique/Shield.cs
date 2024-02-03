using System;
using System.Collections;
using UnityEngine;

using static STELLAREST_2D.Define;
using STELLAREST_2D.Data;

namespace STELLAREST_2D
{
    public class Shield : UniqueSkill
    {
        private enum CustomFloatingsIdx
        {
            MAX_SHIELD_HP_RATIO,
            SHIELD_HP_RECOVERY_RATE_PER_SECONDS
        }

        private float _shieldMaxHp = 0f;
        private ParticleSystem[] _onGroup = null;
        private ParticleSystem[] _offGroup = null;
        private ParticleSystem _hitBurst = null;
        private Vector3 _localShieldPos = new Vector3(0, 0.75f, 0f);
        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            this.transform.localPosition = _localShieldPos;
            _onGroup = transform.GetChild(0).gameObject.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            for (int i = 0; i < _onGroup.Length; ++i)
            {
                _onGroup[i].gameObject.SetActive(false);
                if (_onGroup[i].gameObject.name.Contains(FixedValue.Find.INIT_SHIELD_HIT_BURST))
                    _hitBurst = _onGroup[i].gameObject.GetComponent<ParticleSystem>();
            }

            _offGroup = transform.GetChild(1).gameObject.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            EnableParticles(_offGroup, false);
            _isOnShield = false;
        }

        protected override IEnumerator CoStartSkill()
        {
            DoSkillJob();
            yield return null;
        }

        protected override void DoSkillJob(Action callback = null)
        {
            this.Owner.SkillBook.Deactivate(FixedValue.TemplateID.Skill.US_Paladin_Default);
            this.Owner.CreatureSkillAnimType = this.Data.OwnerAnimationType;
            this.Owner.CreatureState = CreatureState.Skill;
            callback?.Invoke();
        }

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

        public override void OnActiveEliteActionHandler() => this.IsOnShield = true;
        public void Hit() => _hitBurst.Play();

        [SerializeField] private float _maxShiledHPRatio = 0f;
        [SerializeField] private float _recoveryRate = 0f;
        private void OnShield()
        {
            for (int i = 0; i < this.Data.CustomValue.Floatings.Count; ++i)
            {
                if (i == 0)
                    _maxShiledHPRatio = this.Data.CustomValue.Floatings[(int)CustomFloatingsIdx.MAX_SHIELD_HP_RATIO];
                if (i == 1)
                    _recoveryRate = this.Data.CustomValue.Floatings[(int)CustomFloatingsIdx.SHIELD_HP_RECOVERY_RATE_PER_SECONDS];
            }

            EnableParticles(_offGroup, false);
            EnableParticles(_onGroup, true);

            _shieldMaxHp = (this.Owner.Stat.MaxHP * _maxShiledHPRatio);
            this.Owner.Stat.ShieldHP = _shieldMaxHp;

            if (_coRecoveryShield != null)
                StopCoroutine(_coRecoveryShield);
            _coRecoveryShield = StartCoroutine(CoRecoveryShield());
            this.Owner.SkillBook.Activate(FixedValue.TemplateID.Skill.US_Paladin_Default);
        }

        private void OffShield()
        {
            if (_coRecoveryShield != null)
            {
                StopCoroutine(_coRecoveryShield);
                _coRecoveryShield = null;
            }
            this.Owner.Stat.ShieldHP = 0f;

            EnableParticles(_onGroup, false);
            EnableParticles(_offGroup, true);

            StartCoroutine(CoIsPlayingOffShield());
        }

        private IEnumerator CoIsPlayingOffShield()
        {
            //OffShield();
            bool isPlaying = true;
            while (isPlaying)
            {
                bool isAnyPlaying = false;
                for (int i = 0; i < _offGroup.Length; ++i)
                {
                    if (_offGroup[i].isPlaying)
                    {
                        isAnyPlaying = true;
                        break;
                    }
                }

                if (isAnyPlaying == false)
                    isPlaying = false;

                yield return null;
            }

            EnableParticles(_offGroup, false);
            this.Owner.SkillBook.Deactivate(FixedValue.TemplateID.Skill.US_Paladin_Elite);
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
using System;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    public class Shield : SequenceSkill
    {
        private ParticleSystem[] _onShields = null;
        private ParticleSystem[] _offShields = null;
        private ParticleSystem _hitBurst = null;
        private const string HIT_BURST = "Hit_Burst";
        private float _shieldMaxHp = 0f;

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            this.transform.localPosition = new Vector3(0, 0.75f, 0);

            _onShields = transform.GetChild(0).gameObject.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            for (int i = 0; i < _onShields.Length; ++i)
            {
                if (_onShields[i].gameObject.name.Contains(HIT_BURST))
                    _hitBurst = _onShields[i].gameObject.GetComponent<ParticleSystem>();
            }

            _offShields = transform.GetChild(1).gameObject.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
        }

        public override void OnActiveSequenceSkillHandler()
        {
            for (int i = 0; i < _onShields.Length; ++i)
            {
                _onShields[i].gameObject.SetActive(true);
                _onShields[i].Play();
            }
        }

        public override void DoSkillJob(Action callback = null)
        {
            this.Owner.ReserveSkillAnimationType(Define.SkillAnimationType.EliteSequence);
            Owner.CreatureState = Define.CreatureState.Skill;
        }

        private void OnShield()
        {
            for (int i = 0; i < _onShields.Length; ++i)
            {
                _onShields[i].gameObject.SetActive(true);
                _onShields[i].Play();
            }
        }

        private void OffShield()
        {
            for (int i = 0; i < _offShields.Length; ++i)
            {
                _offShields[i].gameObject.SetActive(true);
                _offShields[i].Play();
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }
    }
}

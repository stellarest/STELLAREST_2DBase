using System;
using System.Collections;
using UnityEngine;

using static STELLAREST_2D.Define;
using STELLAREST_2D.Data;

namespace STELLAREST_2D
{
    public class ForestBarrier : UniqueSkill
    {
        private ParticleSystem[] _particles = null;
        private ReinaController _ownerController = null;

        private bool _isOnBarrier = false;
        public bool IsOnBarrier 
        {
            get => _isOnBarrier;
            private set
            {
                _isOnBarrier = value;
                if (_isOnBarrier)
                {
                    _barrierCount = BARRIER_MAX_COUNT;
                    EnableParticles(_particles, true);
                    this.Owner.Stat.AddMovementSpeedRatio(0.2f);
                }
                else
                {
                    EnableParticles(_particles, false);
                    //Managers.VFX.ImpactHit(FixedValue.TemplateID.VFX.ImpactHit.Leaves, this.Owner, this);
                    Managers.VFX.ImpactHit(this.Data.VFX_ImpactHit_TemplateID, this.Owner, this);

                    this.Owner.SkillBook.Deactivate(FixedValue.TemplateID.Skill.Unlock_ForestGuardian_Elite);
                    this.Owner.Stat.ResetMovementSpeed();
                }
            }
        }

        private const int BARRIER_MAX_COUNT = 5;
        [SerializeField] private int _barrierCount = 0;
        public int BarrierCount
        {
            get => _barrierCount;
            set
            {
                _barrierCount = value;
                if (_barrierCount <= 0)
                {
                    _barrierCount = 0;
                    IsOnBarrier = false;
                }
            }
        }

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            this.transform.localPosition = new Vector3(1.85f, -1.5f, 0f);
            this.transform.localScale = Vector3.one * 3f;
            _particles = GetComponentsInChildren<ParticleSystem>();
            _ownerController = owner.GetComponent<ReinaController>();
            EnableParticles(_particles, false);
            _barrierCount = BARRIER_MAX_COUNT;
        }

        protected override IEnumerator CoStartSkill()
        {
            DoSkillJob();
            yield return null;
        }

        protected override void DoSkillJob(Action callback = null) => StartCoroutine(CoOnForestBarrier(callback));

        private const float FIXED_WAIT_TIME_AFTER_SKILL = 1.25f;
        private IEnumerator CoOnForestBarrier(Action callback = null)
        {
            this.Owner.SkillBook.Deactivate(FixedValue.TemplateID.Skill.ForestGuardianMastery);
            _ownerController.PlayerAnimController.EnterNextState(false);
            _ownerController.LockHandle = true;

            this.Owner.CreatureSkillAnimType = this.Data.SkillAnimationTemplateID;
            this.Owner.CreatureState = CreatureState.Skill;
            callback?.Invoke();
            this.IsOnBarrier = true;

            yield return new WaitForSeconds(FIXED_WAIT_TIME_AFTER_SKILL);
            _ownerController.LockHandle = false;
            _ownerController.PlayerAnimController.EnterNextState(true);
            this.Owner.SkillBook.Activate(FixedValue.TemplateID.Skill.ForestGuardianMastery);
        }
    }
}

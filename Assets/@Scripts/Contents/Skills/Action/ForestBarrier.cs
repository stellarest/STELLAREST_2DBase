using System;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_2D.Data;
using UnityEngine;

using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;
using VFXImpact = STELLAREST_2D.Define.TemplateIDs.VFX.ImpactHit;

namespace STELLAREST_2D
{
    /*
        [ Ability Info : Forest Barrier (Elite Action, Forest Guardian) ]
        (Currently Temped Set : lv.3)

        lv.1 : 매 웨이브마다 숲의 보호막을 1회 활성화한다. 숲의 보호막은 2회의 공격을 보호해준다.
               숲의 보호막이 활성화되어 있는 동안 이동 속도가 5% 증가한다.
               (+ 숲의 보호막이 비활성화 될 때, 맵 전체에 있는 모든 적에게 2초간 침묵시킨다.)
        lv.2 : 매 웨이브마다 숲의 보호막을 1회 활성화한다. 숲의 보호막은 3회의 공격을 보호해준다.
               숲의 보호막이 활성화되어 있는 동안 이동 속도가 10% 증가한다.
               (+ 숲의 보호막이 비활성화 될 때, 맵 전체에 있는 모든 적에게 3초간 침묵시킨다.)
        lv.3 : 매 웨이브마다 숲의 보호막을 1회 활성화한다. 숲의 보호막은 5회의 공격을 보호해준다.
               숲의 보호막이 활성화되어 있는 동안 이동 속도가 20% 증가한다.
               (+ 숲의 보호막이 비활성화 될 때, 맵 전체에 있는 모든 적에게 5초간 침묵시킨다.)
    */

    public class ForestBarrier : ActionSkill
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
                    this.Owner.Stat.AddMovementRatio(0.2f);
                }
                else
                {
                    EnableParticles(_particles, false);
                    Managers.VFX.ImpactHit(VFXImpact.Leaves, this.Owner, this);
                    this.Owner.SkillBook.Deactivate(SkillTemplate.ForestBarrier_Elite_Solo);
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
            this.Owner.SkillBook.Deactivate(SkillTemplate.ForestGuardianMastery);
            _ownerController.PlayerAnimController.SetCanEnterNextState(false);
            _ownerController.LockHandle = true;

            this.Owner.ReserveSkillAnimationType(this.Data.AnimationType);
            Owner.CreatureState = Define.CreatureState.Skill;
            this.IsOnBarrier = true;

            yield return new WaitForSeconds(FIXED_WAIT_TIME_AFTER_SKILL);
            _ownerController.LockHandle = false;
            _ownerController.PlayerAnimController.SetCanEnterNextState(true);
            this.Owner.SkillBook.Activate(SkillTemplate.ForestGuardianMastery);
        }
    }
}

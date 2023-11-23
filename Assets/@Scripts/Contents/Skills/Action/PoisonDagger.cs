using System;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_2D.Data;
using UnityEngine;

using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

namespace STELLAREST_2D
{
    // Take off Burst from Parent
    public class PoisonDagger : ActionSkill
    {
        private ParticleSystem[] _chargeGroup = null;
        private ParticleSystem[] _burstGroup = null;
        private KennethController _ownerController = null;

        private Vector3 _startChargeScale = Vector3.zero;
        private Vector3 _targetChargeScale = Vector3.zero;
        //private const float FIXED_TARGET_CHARGE_SCALE_RATIO = 5f;
        private const float FIXED_TARGET_CHARGE_SCALE_RATIO = 3f;

        //private const float FIXED_DESIRED_COMPLETE_CHARGE_TIME = 2f;
        private const float FIXED_DESIRED_COMPLETE_CHARGE_TIME = 1.25f;

        //private const float FIXED_AFTER_COMPLETED_CHARGE_WAIT_TIME = 0.5f;
        private const float FIXED_AFTER_COMPLETED_CHARGE_WAIT_TIME = 0.3f;

        [SerializeField] private AnimationCurve _chargeCurve = null;
        private bool _isEndOfSkill = false;

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            this.transform.localPosition = owner.Center.position + (Vector3.up * 0.75f);

            _startChargeScale = this.transform.localScale;
            _targetChargeScale = this.transform.localScale * FIXED_TARGET_CHARGE_SCALE_RATIO;

            _chargeGroup = transform.GetChild(0).GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            EnableParticles(_chargeGroup, false);
            _burstGroup = transform.GetChild(1).GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            EnableParticles(_burstGroup, false);

            _ownerController = owner.GetComponent<KennethController>();
        }
        
        protected override IEnumerator CoStartSkill()
        {
            DoSkillJob(null);
            yield return null;
        }

        protected override void DoSkillJob(Action callback = null)
        {
            _isEndOfSkill = false;
            this.Owner.SkillBook.Deactivate(SkillTemplate.AssassinMastery);
            _ownerController.PlayerAnimController.SetCanEnterNextState(false);

            this.Owner.ReserveSkillAnimationType(this.Data.AnimationType);
            Owner.CreatureState = Define.CreatureState.Skill;
        }

        public override void OnActiveEliteActionHandler()
        {
            if (_isEndOfSkill)
                return;

            StartCoroutine(CoOnPoisonDagger());
        }

        private IEnumerator CoOnPoisonDagger()
        {
            float delta = 0f;
            float percent = 0f;
            EnableParticles(_chargeGroup, true);
            while (true)
            {
                delta += Time.deltaTime;
                percent = delta / FIXED_DESIRED_COMPLETE_CHARGE_TIME;
                this.transform.localScale = Vector3.Lerp(_startChargeScale, _targetChargeScale, _chargeCurve.Evaluate(percent));
                if (percent >= 1f)
                {
                    this.transform.localScale = _targetChargeScale; 
                    break;
                }

                yield return null;
            }
            EnableParticles(_chargeGroup, false);
            
            TakeOffFromParent(_burstGroup[0]);
            EnableParticles(_burstGroup, true);

            yield return new WaitUntil(() => this.WaitUntilEndOfPlayingParticles(_burstGroup));
            TakeOnToParent(_burstGroup);

            // 1 (Origin)
            StartCoroutine(Managers.VFX.CoMatInnerOutline(_ownerController.BodyParts.HandLeft_MeleeWeapon.GetComponent<SpriteRenderer>(),
                this.Data.Duration, delegate{
                    // DO SOMETHING AFTER COMPLETING SKILL..
                }));

            StartCoroutine(Managers.VFX.CoMatInnerOutline(_ownerController.BodyParts.HandRight_MeleeWeapon.GetComponent<SpriteRenderer>(),
                this.Data.Duration, delegate{
                    StartCoroutine(CoOffPoisonDagger());
                }));

            // 2 (Todo)
            // SpriteRenderer weaponSPR = _ownerController.BodyParts.HandLeft_MeleeWeapon.GetComponent<SpriteRenderer>();
            // Managers.VFX.Material(weaponSPR, Define.MaterialType.FadeOut, Define.MaterialColor.UsePreset, this.Data.Duration, 0.5f, false,
            // delegate{
            //     StartCoroutine(CoOffPoisonDagger());
            // });
            
            _ownerController.PlayerAnimController.SetCanEnterNextState(true);
            yield return new WaitForSeconds(FIXED_AFTER_COMPLETED_CHARGE_WAIT_TIME);

            if (this.Owner.SkillBook.ForceGetSkillMember(SkillTemplate.StabPoisonDagger_Elite_Solo, 0).IsLearned == false)
            {
                this.Owner.SkillBook.LevelUp(SkillTemplate.StabPoisonDagger_Elite_Solo);
                this.Owner.SkillBook.Activate(SkillTemplate.StabPoisonDagger_Elite_Solo);
            }
            else
                this.Owner.SkillBook.Activate(SkillTemplate.StabPoisonDagger_Elite_Solo);
        }

        private IEnumerator CoOffPoisonDagger()
        {
            _ownerController.PlayerAnimController.SetCanEnterNextState(false);
            _isEndOfSkill = true;

            this.Owner.ReserveSkillAnimationType(this.Data.AnimationType);
            Owner.CreatureState = Define.CreatureState.Skill;

            _burstGroup[0].transform.SetParent(null);

            TakeOffFromParent(_burstGroup[0]);
            EnableParticles(_burstGroup, true);

            this.Owner.SkillBook.Deactivate(SkillTemplate.StabPoisonDagger_Elite_Solo);
            yield return new WaitUntil(() => this.WaitUntilEndOfPlayingParticles(_burstGroup));
            yield return new WaitForSeconds(FIXED_AFTER_COMPLETED_CHARGE_WAIT_TIME);
            _ownerController.PlayerAnimController.SetCanEnterNextState(true);

            TakeOnToParent(_burstGroup);
            EnableParticles(_burstGroup, false);

            yield return new WaitForSeconds(FIXED_AFTER_COMPLETED_CHARGE_WAIT_TIME);
            this.Owner.SkillBook.Deactivate(SkillTemplate.PoisonDagger_Elite_Solo);
            this.Owner.SkillBook.Activate(SkillTemplate.AssassinMastery);
            
            yield break; // END OF SKILL
        }

        private void TakeOffFromParent(ParticleSystem particle) => particle.transform.SetParent(null);
        private void TakeOnToParent(ParticleSystem[] particles)
        {
            particles[0].transform.SetParent(this.transform);
            for (int i = 0; i < particles.Length; ++i)
                particles[i].transform.localPosition = Vector3.zero;
        }
    }
}

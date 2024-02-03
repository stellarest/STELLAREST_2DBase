using System;
using System.Collections;
using UnityEngine;

using static STELLAREST_2D.Define;
using STELLAREST_2D.Data;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

namespace STELLAREST_2D
{
    public class SecondWind : UniqueSkill
    {
        /*
            Elite
                - Recovery Heal HP up to 65% of Max HP
                - +100% Around Damage and Stunned for 3.5s

            Ultimate
                - Recovery Heal HP up to 100% of Max HP
                - +100% Around Damage and Stunned for 5s
                - Max Armor Up(85%) Buff for 12s
        */

        private enum CustomValueInfo
        {
            MAX_RECOVERY_PERCENT,
            STAT_BUFF_DURATION
        }

        private ParticleSystem _readyLoop = null;
        private ParticleSystem[] _onGroup = null;
        private ParticleSystem[] _burstGroup = null;
        [field: SerializeField] public bool IsReady { get; private set; } = false;
        private ParticleSystem[] _lastBuffs = null;

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);

            _readyLoop = transform.GetChild(0).GetComponent<ParticleSystem>();
            _readyLoop.transform.localPosition = Vector3.up;
            _readyLoop.gameObject.SetActive(false);

            _onGroup = transform.GetChild(1).GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            EnableParticles(_onGroup, false);

            _burstGroup = transform.GetChild(2).GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            transform.GetChild(2).GetComponent<SecondWindChild>().Init(owner, data);
            EnableParticles(_burstGroup, false);

            _lastBuffs = transform.GetChild(3).GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            _lastBuffs[0].transform.localScale = Vector3.one * 1.5f;
            _lastBuffs[0].transform.localPosition = Vector3.up * 0.7f;
            EnableParticles(_lastBuffs, false);
        }

        protected override IEnumerator CoStartSkill()
        {
            DoSkillJob();
            yield return null;
        }

        protected override void DoSkillJob(Action callback = null) => this.Ready();

        private void Ready()
        {
            IsReady = true;
            _readyLoop.gameObject.SetActive(true);
            _readyLoop.Play();
        }

        public void On()
        {
            this.Owner.RendererController.OnFaceCombatHandler();
            this.Owner.SkillBook.Deactivate(FixedValue.TemplateID.Skill.Knight_Unique_Mastery);

            this.Owner.CreatureSkillAnimType = this.Data.OwnerAnimationType;
            this.Owner.CreatureState = CreatureState.Skill;
            
            EnableParticles(_onGroup, true);
            StartCoroutine(CoOnSecondWind());
        }

        private IEnumerator CoOnSecondWind()
        {
            float delta = 0f;
            float percent = 0f;
            int healUpPercent = 0;

            float maxRecoveryPercent = this.Data.CustomValue.Floatings[(int)CustomValueInfo.MAX_RECOVERY_PERCENT];
            float recoveredHPResult = this.Owner.Stat.MaxHP * maxRecoveryPercent;
            float duration = this.Data.Duration;

            while (percent < maxRecoveryPercent)
            {
                delta += Time.deltaTime;
                percent = delta / duration;

                healUpPercent = Mathf.FloorToInt(percent * 100);
                this.Owner.Stat.HP = Recovery(healUpPercent);
                
                Managers.VFX.Percentage(this.Owner, healUpPercent);
                yield return null;
            }

            this.Owner.Stat.HP = recoveredHPResult;
            Burst();            
        }

        private float Recovery(int healUpPercent)
        {
            float maxHp = this.Owner.Stat.MaxHP;
            float recoveryPercentage = healUpPercent / 100f;
            float recoveryAmount = maxHp * recoveryPercentage;
            return recoveryAmount;
        }

        public void Burst()
        {
            _readyLoop.gameObject.SetActive(false);
            EnableParticles(_onGroup, false);
            EnableParticles(_burstGroup, true);

            this.Owner.IsInvincible = false;
            IsReady = false;

            StartCoroutine(CoEndSecondWind());
        }

        private IEnumerator CoEndSecondWind()
        {
            this.Owner.CreatureAnimController.EnterNextState();
            yield return new WaitForSeconds(1f);
            this.Owner.CreatureAnimController.Ready();
            if (this.Data.Grade == InGameGrade.Elite)
            {
                this.Owner.SkillBook.Activate(FixedValue.TemplateID.Skill.Knight_Unique_Mastery);
                yield break;
            }

            float statBuffDuration = this.Data.CustomValue.Floatings[(int)CustomValueInfo.STAT_BUFF_DURATION];
            EnableParticles(_lastBuffs, true);
            this.Owner.SkillBook.Activate(FixedValue.TemplateID.Skill.Knight_Unique_Mastery);
            
            StartCoroutine(this.Owner.Stat.CoBuffMaxArmor(statBuffDuration));
            yield return new WaitForSeconds(statBuffDuration);
            EnableParticles(_lastBuffs, false);
            this.Owner.SkillBook.Deactivate(FixedValue.TemplateID.Skill.Knight_Unique_Elite);
        }
    }
}

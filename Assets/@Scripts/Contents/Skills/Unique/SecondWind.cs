using System;
using System.Collections;
using UnityEngine;

using static STELLAREST_2D.Define;
using STELLAREST_2D.Data;

namespace STELLAREST_2D
{
    /*
        [ Second Wind - Knight Mastery lv.2 ]

        1회 죽음의 위기에서 2.5초 동안 무적 상태가 되고, 체력을 100%까지 회복
        회복이 완료 된 이후, 주변에 있는 적에게 5초간 기절 상태 부여
        이후, 12초 동안 Knight의 이동 속도, 공격 속도가 100% 증가.
    */

    public class SecondWind : UniqueSkill
    {
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
            this.Owner.SkillBook.Deactivate(FixedValue.TemplateID.Skill.KnightMastery);
            this.Owner.ReserveSkillAnimationType(this.Data.AnimationType);
            Owner.CreatureState = Define.CreatureState.Skill;
            
            EnableParticles(_onGroup, true);
            StartCoroutine(CoOnSecondWind());
        }

        private const float DESIRED_RECOVERY_TIME = 2.5f;
        private IEnumerator CoOnSecondWind()
        {
            float delta = 0f;
            float percentage = 0f;
            int percent = 0;

            while (percentage < 1f)
            {
                delta += Time.deltaTime;
                percentage = delta / DESIRED_RECOVERY_TIME;
                percent = Mathf.FloorToInt(percentage * 100);
                this.Owner.Stat.HP = Recovery(percent);
                Managers.VFX.Percentage(this.Owner, percent);

                yield return null;
            }

            this.Owner.Stat.HP = this.Owner.Stat.MaxHP;
            Burst();            
        }

        private float Recovery(int percent)
        {
            float maxHp = this.Owner.Stat.MaxHP;
            float recoveryPercentage = percent / 100f;
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

        private const float FIXED_LAST_BUFF_TIME = 12f;
        private IEnumerator CoEndSecondWind()
        {
            // TODO 1
            // this.Owner.Stat.AddArmorRatio(Define.MAX_ARMOR_RATE);

            KnightAnimationController anim = this.Owner.AnimController.GetComponent<KnightAnimationController>();
            anim.EnterNextState();
            yield return new WaitForSeconds(1f);
            anim.Ready();

            EnableParticles(_lastBuffs, true);
            this.Owner.ReserveSkillAnimationType(Define.SkillAnimationType.Attack);
            this.Owner.CreatureState = Define.CreatureState.Skill;
            this.Owner.SkillBook.Activate(FixedValue.TemplateID.Skill.KnightMastery);

            yield return new WaitForSeconds(FIXED_LAST_BUFF_TIME);
            EnableParticles(_lastBuffs, false);

            // TODO 2
            // this.Owner.Stat.ResetArmor();
            this.Owner.SkillBook.Deactivate(FixedValue.TemplateID.Skill.SecondWind_Elite_Solo);
        }
    }
}

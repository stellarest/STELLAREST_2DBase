using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using STELLAREST_2D.Data;
using UnityEngine;

using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

namespace STELLAREST_2D
{
    /*
        [ Ability Info : Second Wind (Elite Action, Knight) ]
        (Currently Temped Set : lv.3)

        lv.1 : 죽음의 위기에서 2.5초 동안 무적 상태가 되고 100%의 체력을 회복한다. (1회)
            회복이 완료된 이후, 주변에 있는 적에게 충격파를 날려 적을 넉백하고 2초간 기절상태를 부여한다.
            이후, 5초 동안 Knight의 방어력이 최대치(80%)까지 증가한 이후, 스킬은 비활성화 된다.
            (TODO : 5초 동안 공 20%, 방 20% 증가)
        lv.2 : 죽음의 위기에서 2.5초에 동안 무적 상태가 되고 100%의 체력을 회복한다. (1회)
            회복이 완료된 이후, 주변에 있는 적에게 충격파를 날려 적을 넉백하고 3초간 기절상태를 부여한다.
            이후, 6초 동안 Knight의 방어력이 최대치(80%)까지 증가한 이후, 스킬은 비활성화 된다.
            (TODO : 6초 동안 공 30%, 방 30% 증가)
        lv.3 : 죽음의 위기에서 2.5초에 동안 무적 상태가 되고 100%의 체력을 회복한다. (1회)
            회복이 완료된 이후, 주변에 있는 적에게 충격파를 날려 적을 넉백하고 5초간 기절상태를 부여한다.
            이후, 12초 동안 Knight의 방어력이 최대치(80%)까지 증가한 이후, 스킬은 비활성화 된다.
            (TODO : 12초 동안 공 50%, 방 50% 증가)
    */

    public class SecondWind : ActionSkill
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
            this.Owner.SkillBook.Deactivate(SkillTemplate.KnightMastery);
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
                this.Owner.Stat.Hp = Recovery(percent);
                Managers.VFX.Percentage(this.Owner, percent);

                yield return null;
            }

            this.Owner.Stat.Hp = this.Owner.Stat.MaxHp;
            Burst();            
        }

        private float Recovery(int percent)
        {
            float maxHp = this.Owner.Stat.MaxHp;
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
            this.Owner.Stat.AddArmorRatio(Define.MAX_ARMOR_RATE);

            KnightAnimationController anim = this.Owner.AnimController.GetComponent<KnightAnimationController>();
            anim.EnterNextState();
            yield return new WaitForSeconds(1f);
            anim.Ready();

            EnableParticles(_lastBuffs, true);
            this.Owner.ReserveSkillAnimationType(Define.SkillAnimationType.MasteryAction);
            this.Owner.CreatureState = Define.CreatureState.Skill;
            this.Owner.SkillBook.Activate(SkillTemplate.KnightMastery);

            yield return new WaitForSeconds(FIXED_LAST_BUFF_TIME);
            EnableParticles(_lastBuffs, false);

            this.Owner.Stat.ResetArmor();
            this.Owner.SkillBook.Deactivate(SkillTemplate.SecondWind_Elite_Solo);
        }
    }
}

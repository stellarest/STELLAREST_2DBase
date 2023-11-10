using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
            이후, Knight의 방어력이 영구적으로 5% 증가한다.
        lv.2 : 죽음의 위기에서 2.5초에 동안 무적 상태가 되고 100%의 체력을 회복한다. (1회)
            회복이 완료된 이후, 주변에 있는 적에게 충격파를 날려 적을 넉백하고 3초간 기절상태를 부여한다.
            이후, Knight의 방어력이 영구적으로 8% 증가한다.
        lv.3 : 죽음의 위기에서 2.5초에 동안 무적 상태가 되고 100%의 체력을 회복한다. (1회)
            회복이 완료된 이후, 주변에 있는 적에게 충격파를 날려 적을 넉백하고 5초간 기절상태를 부여한다.
            이후, Knight의 방어력이 영구적으로 16% 증가한다.
    */

    public class SecondWind : ActionSkill
    {
        private ParticleSystem _readyLoop = null;
        private ParticleSystem[] _onGroup = null;
        private ParticleSystem[] _burstGroup = null;
        [field: SerializeField] public bool IsReady { get; private set; } = false;

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);

            _readyLoop = transform.GetChild(0).GetComponent<ParticleSystem>();
            _readyLoop.transform.localPosition = Vector3.up;
            _readyLoop.gameObject.SetActive(false);

            _onGroup = transform.GetChild(1).GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            for (int i = 0; i < _onGroup.Length; ++i)
                _onGroup[i].gameObject.SetActive(false);

            _burstGroup = transform.GetChild(2).GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            transform.GetChild(2).GetComponent<SecondWindChild>().Init(owner, data);
            
            for (int i = 0; i < _burstGroup.Length; ++i)
                _burstGroup[i].gameObject.SetActive(false);
        }

        protected override IEnumerator CoStartSkill()
        {
            DoSkillJob();
            yield return null;
        }

        protected override void DoSkillJob(Action callback = null)
        {
            this.Ready();
        }

        public void Ready()
        {
            IsReady = true;
            _readyLoop.gameObject.SetActive(true);
            _readyLoop.Play();
        }

        public void On()
        {
            this.Owner.RendererController.OnFaceBattleHandler();
            this.Owner.SkillBook.Deactivate(SkillTemplate.KnightMastery);
            this.Owner.ReserveSkillAnimationType(this.Data.AnimationType);
            Owner.CreatureState = Define.CreatureState.Skill;

            for (int i = 0; i < _onGroup.Length; ++i)
            {
                _onGroup[i].gameObject.SetActive(true);
                _onGroup[i].Play();
            }

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
            for (int i = 0; i < _onGroup.Length; ++i)
                _onGroup[i].gameObject.SetActive(false);

            for (int i = 0; i < _burstGroup.Length; ++i)
            {
                _burstGroup[i].gameObject.SetActive(true);
                _burstGroup[i].Play();
            }

            this.Owner.IsInvincible = false;
            IsReady = false;

            // this.Owner.CreatureState = Define.CreatureState.Idle;
            // this.Owner.ReserveSkillAnimationType(Define.SkillAnimationType.ExclusiveRepeat);
            // this.Owner.SkillBook.Activate(SkillTemplate.KnightMastery);
            StartCoroutine(CoEndSecondWind());
        }

        private const float ADD_ARMOR_RATIO = 0.16f;
        private IEnumerator CoEndSecondWind()
        {
            KnightAnimationController anim = this.Owner.AnimController.GetComponent<KnightAnimationController>();
            anim.EnterNextState();
            yield return new WaitForSeconds(1f);
            anim.Ready();

            this.Owner.ReserveSkillAnimationType(Define.SkillAnimationType.MasteryAction);
            this.Owner.CreatureState = Define.CreatureState.Skill;
            this.Owner.SkillBook.Activate(SkillTemplate.KnightMastery);

            this.Owner.Stat.AddArmorRatio(ADD_ARMOR_RATIO);
            this.Owner.SkillBook.Deactivate(SkillTemplate.SecondWind_Elite_Solo);
        }
    }
}

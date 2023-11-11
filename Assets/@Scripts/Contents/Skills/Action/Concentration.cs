using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using STELLAREST_2D.Data;
using UnityEngine;

using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;
using CrowdControl = STELLAREST_2D.Define.TemplateIDs.CrowdControl;
using System.Linq;

namespace STELLAREST_2D
{
    /*
        [ Ability Info : Concentration (Elite Action, Arrow Master) ]
        (Currently Temped Set : lv.3)

        lv.1 : 5초 동안 화살의 데미지 50% 증가. 50%의 확률로 1개의 가까운 타겟에게 표식 생성.
            (표식이 생성된 타겟은 100%의 확률로 크리티컬 적용)
        lv.2 : 6초 동안 화살의 데미지 60% 증가. 60%의 확률로 2개의 가까운 타겟에게 표식 생성.
            (표식이 생성된 타겟은 100%의 확률로 크리티컬 적용)
        lv.3 : 8초 동안 화살의 데미지 100% 증가. 100%의 확률로 3개의 가까운 타겟에게 표식 생성.
            (표식이 생성된 타겟은 100%의 확률로 크리티컬 적용)
    */

    public class Concentration : ActionSkill
    {
        private ParticleSystem[] _bursts = null;
        private ParticleSystem[] _buffs = null;

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            this.transform.localScale = Vector3.one * 2.75f;

            for (int i = 0; i < transform.childCount; ++i)
                transform.GetChild(i).gameObject.SetActive(false);

            _bursts = transform.transform.GetChild(0).GetComponentsInChildren<ParticleSystem>();
            _buffs = transform.GetChild(1).GetComponentsInChildren<ParticleSystem>();
        }

        protected override IEnumerator CoStartSkill()
        {
            WaitForSeconds wait = new WaitForSeconds(this.Data.CoolTime);
            while (true)
            {
                IsStopped = false;
                DoSkillJob();
                yield return wait;
            }
        }

        protected override void DoSkillJob(Action callback = null)
        {
            this.Owner.SkillBook.Deactivate(SkillTemplate.ArrowMasterMastery);
            this.Owner.ReserveSkillAnimationType(this.Data.AnimationType);
            Owner.CreatureState = Define.CreatureState.Skill;
            StartCoroutine(CoConcentration());
        }

        private IEnumerator CoConcentration()
        {
            EnableParticles(_bursts, true);
            ApplyTarget();

            bool isPlaying = true;
            while (isPlaying)
            {
                bool isAnyPlaying = false;
                for (int i = 0; i < _bursts.Length; ++i)
                {
                    if (_bursts[i].isPlaying)
                    {
                        isAnyPlaying = true;
                        break;
                    }
                }

                isPlaying = (isAnyPlaying == false) ? false : true;
                yield return null;
            }

            SkillBase skill = this.Owner.SkillBook.GetCanActiveSkillMember(SkillTemplate.ArrowMasterMastery);
            float originMinDamage = skill.Data.MinDamage;
            float originMaxDamage = skill.Data.MaxDamage;

            skill.Data.MinDamage += (skill.Data.MinDamage * this.Data.CrowdControlIntensity);
            skill.Data.MaxDamage += (skill.Data.MaxDamage * this.Data.CrowdControlIntensity);
            
            this.Owner.SkillBook.Activate(SkillTemplate.ArrowMasterMastery);
            EnableParticles(_buffs, true);

            yield return new WaitForSeconds(this.Data.CrowdControlDuration);
            skill.Data.MinDamage = originMinDamage;
            skill.Data.MaxDamage = originMaxDamage;

            EnableParticles(_bursts, false);
            EnableParticles(_buffs, false);
            IsStopped = true;

            //this.Owner.SkillBook.Deactivate(SkillTemplate.Concentration_Elite_Solo);
        }

        private const float FIXED_SEARCH_RANGE_FROM_OWNER = 11f;
        private const int FIXED_MAX_TARGETING_COUNT = 3;
        private void ApplyTarget()
        {
            if (UnityEngine.Random.Range(0f, 1f) < this.Data.CrowdControlRatio)
            {
                var hashMons = FindMonsterTargets(FIXED_SEARCH_RANGE_FROM_OWNER);
                foreach (var mon in hashMons)
                    Managers.CrowdControl.Apply(mon, this);
            }
        }

        private HashSet<MonsterController> FindMonsterTargets(float searchInRange)
        {
            List<MonsterController> monstersInRange = Utils.GetMonstersInRange(this.Owner, FIXED_SEARCH_RANGE_FROM_OWNER);
            HashSet<MonsterController> targets = new HashSet<MonsterController>();

            int targetCount = UnityEngine.Mathf.Min(monstersInRange.Count, FIXED_MAX_TARGETING_COUNT);
            if (targetCount != 0)
            {
                int count = 0;
                int maxAttempts = 100;
                int attempts = 0;
                while (attempts < maxAttempts)
                {
                    int rndIdx = UnityEngine.Random.Range(0, monstersInRange.Count);
                    if (targets.Contains(monstersInRange[rndIdx]))
                    {
                        ++attempts;
                        continue;
                    }
                    else if (monstersInRange[rndIdx].IsValid() == false)
                    {
                        ++attempts;
                        continue;
                    }
                    else if (monstersInRange[rndIdx][CrowdControl.Targeted])
                    {
                        ++attempts;
                        continue;
                    }
                    else if (count < targetCount)
                    {
                        targets.Add(monstersInRange[rndIdx]);
                        ++count;
                    }
                    else if (count >= targetCount)
                        break;
                }
            }

            return targets;
        }
    }
}

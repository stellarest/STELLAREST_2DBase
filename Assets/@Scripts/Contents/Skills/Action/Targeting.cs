using System;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_2D.Data;
using UnityEngine;

using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

namespace STELLAREST_2D
{
    public class Targeting : ActionSkill
    {
        private ParticleSystem[] _particles = null;

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            this.transform.localScale = Vector3.one * 2.75f;

            _particles = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < _particles.Length; ++i)
            {
                _particles[i].GetComponent<ParticleSystemRenderer>().sortingOrder = (int)Define.SortingOrder.Skill;
                _particles[i].gameObject.SetActive(false);
            }
        }

        protected override IEnumerator CoStartSkill()
        {
            WaitForSeconds wait = new WaitForSeconds(this.Data.CoolTime);
            while (true)
            {
                DoSkillJob();
                yield return wait;
            }
        }

        protected override void DoSkillJob(Action callback = null)
        {
            this.Owner.SkillBook.Deactivate(SkillTemplate.ArrowMasterMastery);
            this.Owner.ReserveSkillAnimationType(this.Data.AnimationType);
            Owner.CreatureState = Define.CreatureState.Skill;
            StartCoroutine(CoIsPlayingOffConcentration());
        }

        private IEnumerator CoIsPlayingOffConcentration()
        {
            for (int i = 0; i < _particles.Length; ++i)
            {
                _particles[i].gameObject.SetActive(true);
                _particles[i].Play();
            }

            bool isPlaying = true;
            while (isPlaying)
            {
                bool isAnyPlaying = false;
                for (int i = 0; i < _particles.Length; ++i)
                {
                    if (_particles[i].isPlaying)
                    {
                        isAnyPlaying = true;
                        break;
                    }
                }

                isPlaying = (isAnyPlaying == false) ? false : true;
                yield return null;
            }

            for (int i = 0; i < _particles.Length; ++i)
                _particles[i].gameObject.SetActive(false);

            this.Owner.SkillBook.Deactivate(SkillTemplate.Targeting_Elite_Solo);
            this.Owner.SkillBook.Activate(SkillTemplate.ArrowMasterMastery);
        }
    }
}

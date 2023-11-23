using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using STELLAREST_2D;
using STELLAREST_2D.Data;
using UnityEngine;

using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

namespace STELLAREST_2D
{
    /*
        [ Ability Info : Cloak (Elite Action, Ninja) ]
        (Currently Temped Set : lv.3)

        lv.1 : 3초 동안 Fade Out, 몬스터의 어그로가 사라짐. 혹시 모를 100% 확률로 회피 적용. 이속 증가 100%
        lv.2 : 4초 동안 Fade Out, 몬스터의 어그로가 사라짐. 혹시 모를 100% 확률로 회피 적용. 이속 증가 100%
        lv.3 : 6초 동안 Fade Out, 몬스터의 어그로가 사라짐. 혹시 모를 100% 확률로 회피 적용. 이속 증가 100%
        은폐에서 풀릴 때 닌자 스윙으로 몬스터 공격. 타격시 2초간 기절.
    */

    public class Cloak : ActionSkill
    {
        private ParticleSystem[] _particles = null;

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            _particles = GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            EnableParticles(_particles, false);
        }

        protected override IEnumerator CoStartSkill()
        {
            DoSkillJob(null);
            yield break;
        }

        protected override void DoSkillJob(Action callback = null)
        {
            this.Owner.SkillBook.Deactivate(SkillTemplate.NinjaMastery);
            this.Owner.ReserveSkillAnimationType(this.Data.AnimationType);
            Owner.CreatureState = Define.CreatureState.Skill;
            EnableParticles(_particles, true);
        }
    }
}

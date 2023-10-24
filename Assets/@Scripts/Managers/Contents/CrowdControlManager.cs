using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using Demo_Project;
using STELLAREST_2D.Data;
using UnityEngine;

using CrowdControl = STELLAREST_2D.Define.TemplateIDs.CrowdControl;
using VFXEnv = STELLAREST_2D.Define.TemplateIDs.VFX.Environment;

namespace STELLAREST_2D
{
    public class CrowdControlManager
    {
        public void Apply(CreatureController target, SkillBase from)
        {
            if (target.IsValid() == false)
                return;

            if (target.IsDeadState)
                return;

            target.RequestCrowdControl(from);
        }

        public IEnumerator CoStun(CreatureController target, SkillBase from)
        {
            float delta = 0f;
            float percent = 0f;
            float duration = from.Data.CrowdControlDuration;
            target.CreatureState = Define.CreatureState.Idle;
            
            target.SetDeadHead();
            GameObject goVFX = Managers.VFX.Environment(VFXEnv.Stun, target);
            target[CrowdControl.Stun] = true;
            while (percent < 1f)
            {
                if (target.IsDeadState)
                {
                    target[CrowdControl.Stun] = false;
                    Managers.Resource.Destroy(goVFX);
                    yield break;
                }

                goVFX.transform.position = target.LoadVFXEnvSpawnPos(VFXEnv.Stun);
                delta += Time.deltaTime;
                percent = delta / duration;
                yield return null;
            }
            target[CrowdControl.Stun] = false;
            Managers.Resource.Destroy(goVFX);
            target.SetDefaultHead();

            bool isOnActiveImmediately = true;
            target.StartIdleToAction(isOnActiveImmediately);
        }

        public IEnumerator CoSlow(CreatureController target, SkillBase from)
        {
            float delta = 0f;
            float percent = 0f;
            float duration = from.Data.CrowdControlDuration;
            target.SpeedModifier *= from.Data.CrowdControlIntensity;

            GameObject goVFX = Managers.VFX.Environment(VFXEnv.Slow, target);
            target[CrowdControl.Slow] = true;
            while (percent < 1f)
            {
                goVFX.transform.position = target.LoadVFXEnvSpawnPos(VFXEnv.Slow);
                delta += Time.deltaTime;
                percent = delta / duration;
                yield return null;
            }
            target[CrowdControl.Slow] = false;
            Managers.Resource.Destroy(goVFX);

            target.ResetSpeedModifier();
        }

        private const float GENERATE_VFX_DUST_INTERVAL_TO_PLAYER = 0.2f;
        public IEnumerator CoKnockBack(CreatureController target, SkillBase from)
        {
            float delta = 0f;
            float percent = 0f;
            float duration = from.Data.CrowdControlDuration;
            float intensity = from.Data.CrowdControlIntensity;

            Vector3 knockBackDir = (target.Center.position - from.HitPoint).normalized;
            target[CrowdControl.KnockBack] = true;
            float dustGenPercentage = 0.2f;
            while (percent < 1f)
            {
                if (target.IsPlayer())
                {
                    if (percent > dustGenPercentage)
                    {
                        dustGenPercentage += GENERATE_VFX_DUST_INTERVAL_TO_PLAYER;
                        Managers.VFX.Environment(VFXEnv.Dust, target);
                    }
                }

                Managers.Stage.SetInLimitPos(target);
                target.transform.position += knockBackDir * intensity;
                delta += Time.deltaTime;
                percent = delta / duration;
                yield return null;
            }
            target[CrowdControl.KnockBack] = false;
        }

        public IEnumerator CoSilence(CreatureController target, SkillBase from)
        {
            float delta = 0f;
            float percent = 0f;
            float duration = from.Data.CrowdControlDuration;

            target.SkillBook.DeactivateAll();
            GameObject goVFX = Managers.VFX.Environment(VFXEnv.Silence, target);
            target[CrowdControl.Slience] = true;
            while (percent < 1f)
            {
                if (target.IsDeadState)
                {
                    target[CrowdControl.Slience] = false;
                    Managers.Resource.Destroy(goVFX);
                    yield break;
                }

                goVFX.transform.position = target.LoadVFXEnvSpawnPos(VFXEnv.Silence);
                delta += Time.deltaTime;
                percent = delta / duration;
                yield return null;
            }

            target[CrowdControl.Slience] = false;
            Managers.Resource.Destroy(goVFX);
            target.SkillBook.ActivateAll();
        }
    }
}

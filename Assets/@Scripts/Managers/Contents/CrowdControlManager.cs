using System.Collections;
using System.Collections.Generic;
using Demo_Project;
using UnityEngine;

using CrowdControl = STELLAREST_2D.Define.TemplateIDs.CrowdControl;
using VFXEnv = STELLAREST_2D.Define.TemplateIDs.VFX.Environment;

namespace STELLAREST_2D
{
    public class CrowdControlManager
    {
        public void Apply(SkillBase from, CreatureController target)
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
    }
}

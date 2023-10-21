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
            target[CrowdControl.Stun] = true;
            GameObject goVFX = Managers.VFX.Environment(VFXEnv.Stun, target);
            while (percent < 1f)
            {
                goVFX.transform.position = target.LoadVFXEnvSpawnPos(VFXEnv.Stun);
                delta += Time.deltaTime;
                percent = delta / duration;
                yield return null;
            }
            Managers.Resource.Destroy(goVFX);
            target[CrowdControl.Stun] = false;
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


            target[CrowdControl.Slow] = true;
            GameObject goVFX = Managers.VFX.Environment(VFXEnv.Slow, target);
            while (percent < 1f)
            {
                goVFX.transform.position = target.LoadVFXEnvSpawnPos(VFXEnv.Slow);
                delta += Time.deltaTime;
                percent = delta / duration;
                yield return null;
            }
            Managers.Resource.Destroy(goVFX);
            target[CrowdControl.Slow] = false;


            target.ResetSpeedModifier();
        }
    }
}

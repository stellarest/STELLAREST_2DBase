using System.Collections;
using System.Collections.Generic;
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

            if (target.CreatureState == Define.CreatureState.CrowdControl)
                return;

            switch (from.Data.CrowdControlType)
            {
                case CrowdControl.None:
                    return;

                case CrowdControl.Stun:
                    target.RequestCrowdControl(CrowdControl.Stun, from);
                    break;
            }
        }

        public IEnumerator CoStun(CreatureController target, SkillBase from)
        {
            float delta = 0f;
            float percent = 0f;
            float duration = from.Data.CrowdControlDuration;

            GameObject goVFXStun = Managers.VFX.Environment(VFXEnv.Stun, target);
            target.CreatureState = Define.CreatureState.CrowdControl;
            while (percent < 1f)
            {
                delta += Time.deltaTime;
                percent = delta / duration;
                yield return null;
            }

            target.CreatureState = Define.CreatureState.Idle; // TEMP
            target.CreatureState = Define.CreatureState.Run; // TEMP
            Managers.Resource.Destroy(goVFXStun);
        }
    }
}

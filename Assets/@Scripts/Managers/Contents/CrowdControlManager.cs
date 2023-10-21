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

            if (target.CreatureState == Define.CreatureState.CC_Stun)
                return;

            target.RequestCrowdControl(from.Data.CrowdControlType, from);
        }

        private bool IsAlreadyCCState(CreatureController target, CrowdControl ccType) 
        {
            return false;
        }

        public IEnumerator CoStun(CreatureController target, SkillBase from)
        {
            float delta = 0f;
            float percent = 0f;
            float duration = from.Data.CrowdControlDuration;

            GameObject goVFX = Managers.VFX.Environment(VFXEnv.Stun, target);
            target.CreatureState = Define.CreatureState.CC_Stun;
            while (percent < 1f)
            {
                goVFX.transform.position = target.LoadVFXEnvSpawnPos(VFXEnv.Stun);
                delta += Time.deltaTime;
                percent = delta / duration;
                yield return null;
            }
            Managers.Resource.Destroy(goVFX);

            bool isOnActiveImmediately = true;
            MonsterController mc = target.GetComponent<MonsterController>();
            if (mc != null)
                mc.StartIdleToAction(isOnActiveImmediately);
        }

        // ***** MonsterController::FIXED UPDATE 개선해야할듯 *****
        // CreatureState는 CrowdControl 하나로 퉁쳐야할까나
        // 여기서 Stun 상태라면 Monster Face 바꿔주는등
        public IEnumerator CoSlow(CreatureController target, SkillBase from)
        {
            float delta = 0f;
            float percent = 0f;
            float duration = from.Data.CrowdControlDuration;
            float intensity = from.Data.CrowdControlIntensity; // HOW CAN I SEND IT ?

            // 완성은 했는데, 이제 어떻게 멀티플 CC상태를 관리할것인지? (CreatureState로는 한계가 있음. 로직을 짜는데 다소 복잡해질 가능성이 있기 때문)
            GameObject goVFX = Managers.VFX.Environment(VFXEnv.Slow, target);
            target.SpeedModifier *= intensity;
            while (percent < 1f)
            {
                goVFX.transform.position = target.LoadVFXEnvSpawnPos(VFXEnv.Slow);
                delta += Time.deltaTime;
                percent = delta / duration;
                yield return null;
            }
            target.ResetSpeedModifier();
            Managers.Resource.Destroy(goVFX);

            yield return null;
        }
    }
}

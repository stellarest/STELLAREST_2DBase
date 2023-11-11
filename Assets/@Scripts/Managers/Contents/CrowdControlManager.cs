using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_2D.Data;
using UnityEngine;

using CrowdControl = STELLAREST_2D.Define.TemplateIDs.CrowdControl;
using VFXImpact = STELLAREST_2D.Define.TemplateIDs.VFX.ImpactHit;
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
                if (target.IsDeadState)
                {
                    target[CrowdControl.Slow] = false;
                    Managers.Resource.Destroy(goVFX);
                    yield break;
                }

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
        public IEnumerator CoKnockBack(CreatureController target, SkillBase from, bool isCalledFromContinuous = false)
        {
            float delta = 0f;
            float percent = 0f;
            float duration = from.Data.CrowdControlDuration;
            if (isCalledFromContinuous)
                duration = from.Data.ContinuousCrowdControlDuration;

            float intensity = from.Data.CrowdControlIntensity;
            if (isCalledFromContinuous)
                intensity = from.Data.ContinuousCrowdControlIntensity;

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

        public IEnumerator CoSilence(CreatureController target, SkillBase from, bool isCalledFromContinuous = false)
        {
            float delta = 0f;
            float percent = 0f;
            float duration = 0;
            if (isCalledFromContinuous == false)
                duration = from.Data.CrowdControlDuration;
            else
                duration = from.Data.ContinuousCrowdControlDuration;

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

        public IEnumerator CoTargeted(CreatureController target, SkillBase from, bool isCalledFromContinuous = false)
        {
            Vector3 initScale = Vector3.one;
            Quaternion initRot = Quaternion.identity;
            GameObject goVFX = Managers.VFX.Environment(VFXEnv.Targeted, target);
            goVFX.transform.DORotate(new Vector3(0f, 0f, 360f), 1.5f, RotateMode.FastBeyond360).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            goVFX.transform.DOScale(Vector3.one * 2.5f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

            target[CrowdControl.Targeted] = true;
            yield return new WaitUntil(delegate ()
            {
                if (target.IsDeadState)
                {
                    target[CrowdControl.Targeted] = false;
                    goVFX.transform.localScale = initScale;
                    goVFX.transform.localRotation = initRot;
                    goVFX.transform.DOKill();
                    Managers.Resource.Destroy(goVFX);
                    goVFX = null;
                    return true;
                }

                goVFX.transform.position = target.LoadVFXEnvSpawnPos(VFXEnv.Targeted);
                return from.IsStopped;
            });

            if (goVFX != null)
            {
                target[CrowdControl.Targeted] = false;
                goVFX.transform.localScale = initScale;
                goVFX.transform.localRotation = initRot;
                goVFX.transform.DOKill();
                Managers.Resource.Destroy(goVFX);
            }
        }
    }
}

using System.Collections;
using DG.Tweening;
using UnityEngine;

using static STELLAREST_2D.Define;

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

            target.TryCrowdControl(from);
        }

        public IEnumerator CoStun(CreatureController target, float duration)
        {
            float delta = 0f;
            float percent = 0f;

            GameObject goVFX = Managers.VFX.Environment(VFXEnvType.Stun, target);
            target[CrowdControlType.Stun] = true;
            while (percent < 1f)
            {
                if (target.IsDeadState)
                {
                    target[CrowdControlType.Stun] = false;
                    Managers.Resource.Destroy(goVFX);
                    yield break;
                }

                goVFX.transform.position = target.LoadVFXEnvSpawnPos(VFXEnvType.Stun);
                delta += Time.deltaTime;
                percent = delta / duration;
                yield return null;
            }
            target[CrowdControlType.Stun] = false;
            Managers.Resource.Destroy(goVFX);
        }

        public IEnumerator CoSlow(CreatureController target, float duration, float intensity)
        {
            float delta = 0f;
            float percent = 0f;
            target.SpeedModifier *= intensity;

            GameObject goVFX = Managers.VFX.Environment(VFXEnvType.Slow, target);
            target[CrowdControlType.Slow] = true;
            while (percent < 1f)
            {
                if (target.IsDeadState)
                {
                    target[CrowdControlType.Slow] = false;
                    target.ResetSpeedModifier();
                    Managers.Resource.Destroy(goVFX);
                    yield break;
                }

                goVFX.transform.position = target.LoadVFXEnvSpawnPos(VFXEnvType.Slow);
                delta += Time.deltaTime;
                percent = delta / duration;
                yield return null;
            }
            target[CrowdControlType.Slow] = false;
            Managers.Resource.Destroy(goVFX);
            target.ResetSpeedModifier();

            yield return null;
        }

        private const float GENERATE_VFX_DUST_INTERVAL_TO_PLAYER = 0.2f;
        public IEnumerator CoKnockBack(CreatureController target, float duration, float intensity, Vector3 hitPoint)
        {
            float delta = 0f;
            float percent = 0f;

            Vector3 knockBackDir = (target.Center.position - hitPoint).normalized;
            target[CrowdControlType.KnockBack] = true;
            float dustGenPercentage = 0.2f;
            while (percent < 1f)
            {
                if (target.IsPlayer)
                {
                    if (percent > dustGenPercentage)
                    {
                        dustGenPercentage += GENERATE_VFX_DUST_INTERVAL_TO_PLAYER;
                        Managers.VFX.Environment(VFXEnvType.Dust, target);
                    }
                }

                Managers.Stage.SetInLimitPos(target);
                target.transform.position += knockBackDir * intensity;
                delta += Time.deltaTime;
                percent = delta / duration;
                yield return null;
            }

            target[CrowdControlType.KnockBack] = false;
        }

        public IEnumerator CoSilence(CreatureController target, float duration)
        {
            float delta = 0f;
            float percent = 0f;

            target.SkillBook.DeactivateAll();
            GameObject goVFX = Managers.VFX.Environment(VFXEnvType.Silence, target);
            target[CrowdControlType.Silence] = true;
            while (percent < 1f)
            {
                if (target.IsDeadState)
                {
                    target[CrowdControlType.Silence] = false;
                    Managers.Resource.Destroy(goVFX);
                    yield break;
                }

                goVFX.transform.position = target.LoadVFXEnvSpawnPos(VFXEnvType.Silence);
                delta += Time.deltaTime;
                percent = delta / duration;
                yield return null;
            }

            target[CrowdControlType.Silence] = false;
            Managers.Resource.Destroy(goVFX);
            target.SkillBook.ActivateAll();
        }

        public IEnumerator CoTargeted(CreatureController target, SkillBase from)
        {
            Vector3 initScale = Vector3.one;
            Quaternion initRot = Quaternion.identity;
            GameObject goVFX = Managers.VFX.Environment(VFXEnvType.Targeted, target);
            goVFX.transform.DORotate(new Vector3(0f, 0f, 360f), 1.5f, RotateMode.FastBeyond360).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            goVFX.transform.DOScale(Vector3.one * 2.5f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

            target[CrowdControlType.Targeted] = true;
            yield return new WaitUntil(delegate ()
            {
                if (target.IsDeadState)
                {
                    target[CrowdControlType.Targeted] = false;
                    goVFX.transform.localScale = initScale;
                    goVFX.transform.localRotation = initRot;
                    goVFX.transform.DOKill();
                    Managers.Resource.Destroy(goVFX);
                    goVFX = null;
                    return true;
                }

                goVFX.transform.position = target.LoadVFXEnvSpawnPos(VFXEnvType.Targeted);
                return from.IsStopped;
            });

            if (goVFX != null)
            {
                target[CrowdControlType.Targeted] = false;
                goVFX.transform.localScale = initScale;
                goVFX.transform.localRotation = initRot;
                goVFX.transform.DOKill();
                Managers.Resource.Destroy(goVFX);
            }
        }

        private const float FIXED_MIN_POISON_DOT_DMG_RATIO = 0.2F;
        private const float FIXED_MAX_POISON_DOT_DMG_RATIO = 0.3F;
        private const float FIXED_TAKE_DOT_DMG_INTERVAL = 0.25F;
        public IEnumerator CoPoisoned(CreatureController target, float duration, float minDamage, float maxDamage)
        {
            float delta = 0f;

            target[CrowdControlType.Poisoned] = true;
            target.StartCoroutine(Managers.VFX.CoMatPoison(target, duration, delegate
            {
                target.RendererController.OnFaceDeadHandler();
            }, delegate
            {
                target.RendererController.OnFaceDefaultHandler();
            }));

            float dotPoisionDmgInterval = 0f;
            float minDotDmg = minDamage * FIXED_MIN_POISON_DOT_DMG_RATIO;
            float maxDotDmg = maxDamage * FIXED_MAX_POISON_DOT_DMG_RATIO;
            while (delta <= duration)
            {
                if (target.IsDeadState)
                {
                    target[CrowdControlType.Poisoned] = false;
                    yield break;
                }

                delta += Time.deltaTime;
                dotPoisionDmgInterval += Time.deltaTime;
                if (dotPoisionDmgInterval >= FIXED_TAKE_DOT_DMG_INTERVAL)
                {
                    target.OnFixedDamaged(Random.Range(minDotDmg, maxDotDmg), CrowdControlType.Poisoned);
                    dotPoisionDmgInterval = 0f;
                }

                yield return null;
            }

            target[CrowdControlType.Poisoned] = false;
        }
    }
}

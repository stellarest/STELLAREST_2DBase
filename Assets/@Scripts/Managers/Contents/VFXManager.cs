using System.Collections;
using DamageNumbersPro;
using SpriteTrail;
using UnityEngine;

using static STELLAREST_2D.Define;

namespace STELLAREST_2D
{
    public class VFXManager
    {
        public Material MatHit_Monster { get; private set; } = null;
        public Material MatHit_Player { get; private set; } = null;
        public Material Mat_Hologram { get; private set; } = null;
        public Material Mat_Fade { get; private set; } = null;
        public Material Mat_StrongTint { get; private set; } = null;
        public Material Mat_InnerOutline { get; private set; } = null;
        public Material Mat_Poison { get; private set; } = null;

        public readonly int SHADER_HOLOGRAM_FADE = Shader.PropertyToID("_HologramFade");
        public readonly int SHADER_FADE_ALPHA = Shader.PropertyToID("_CustomFadeAlpha");
        public readonly int SHADER_STRONG_TINT_FADE = Shader.PropertyToID("_StrongTintFade");
        public readonly int SHADER_STRONG_TINT_COLOR = Shader.PropertyToID("_StrongTintTint");
        public readonly int SHADER_INNER_OUTLINE_FADE = Shader.PropertyToID("_InnerOutlineFade");
        public readonly int SHADER_POISON_FADE = Shader.PropertyToID("_PoisonFade");

        public TrailPreset SO_SPT_BLOB { get; private set; } = null;
        public TrailPreset SO_SPT_POS { get; private set; } = null;

        public void Init()
        {
            MatHit_Monster = Managers.Resource.Load<Material>(FixedValue.Load.MAT_HIT_WHITE);
            MatHit_Player = Managers.Resource.Load<Material>(FixedValue.Load.MAT_HIT_RED);
            Mat_Hologram = Managers.Resource.Load<Material>(FixedValue.Load.MAT_HOLOGRAM);
            Mat_Fade = Managers.Resource.Load<Material>(FixedValue.Load.MAT_FADE);
            Mat_StrongTint = Managers.Resource.Load<Material>(FixedValue.Load.MAT_STRONG_TINT);
            Mat_InnerOutline = Managers.Resource.Load<Material>(FixedValue.Load.MAT_INNER_OUTLINE);
            Mat_Poison = Managers.Resource.Load<Material>(FixedValue.Load.MAT_POISON);

            SO_SPT_BLOB = Managers.Resource.Load<TrailPreset>(FixedValue.Load.SO_SPT_BLOB);
            SO_SPT_POS = Managers.Resource.Load<TrailPreset>(FixedValue.Load.SO_SPT_POS);


        }

        public IEnumerator CoMatHit(BaseController bc, System.Action startCallback = null, System.Action endCallback = null)
        {
            if (bc?.IsValid() == false)
                yield break;

            if (IsChangingMaterial(bc))
                yield break;

            switch (bc.ObjectType)
            {
                case Define.ObjectType.Player:
                case Define.ObjectType.Monster:
                    {
                        startCallback?.Invoke();
                        if (bc.ObjectType == Define.ObjectType.Player)
                            bc.RendererController.SetMaterial(MatHit_Player);
                        else if (bc.ObjectType == Define.ObjectType.Monster)
                            bc.RendererController.SetMaterial(MatHit_Monster);

                        yield return new WaitForSeconds(FixedValue.Numeric.MAT_HIT_DURATION);
                        bc.RendererController.ResetMaterial();
                        endCallback?.Invoke();
                    }
                    break;

                default:
                    yield break;
            }

            yield break;
        }

        // 플레이어 혼자 쓸거면 clone 안해도 ㄱㅊ
        public IEnumerator CoMatHologram(BaseController bc, System.Action startCallback = null, System.Action endCallback = null)
        {
            if (bc?.IsValid() == false)
                yield break;

            if (IsChangingMaterial(bc))
                yield break;

            switch (bc.ObjectType)
            {
                case Define.ObjectType.Player:
                    {
                        startCallback?.Invoke();
                        bc.RendererController.SetMaterial(Mat_Hologram);

                        {
                            float percent = 0f;
                            yield return new WaitUntil(() =>
                            {
                                while (percent < 1f)
                                {
                                    Mat_Hologram.SetFloat(SHADER_HOLOGRAM_FADE, percent);
                                    percent += Time.deltaTime * FixedValue.Numeric.HOLOGRAM_SPEED_POWER;
                                    return false;
                                }

                                return true;
                            });

                            percent = 1f;
                            float elapsedTime = 0f;
                            yield return new WaitUntil(() =>
                            {
                                while (percent > 0f)
                                {
                                    Mat_Hologram.SetFloat(SHADER_HOLOGRAM_FADE, percent);
                                    elapsedTime += Time.deltaTime;
                                    percent = 1f - (elapsedTime * FixedValue.Numeric.HOLOGRAM_SPEED_POWER);
                                    return false;
                                }

                                return true;
                            });
                        }

                        bc.RendererController.ResetMaterial();
                        endCallback?.Invoke();
                    }
                    break;

                default:
                    yield break;
            }

            yield break;
        }

        // 이것도 HIt처럼 매터리얼 클론해야함..
        public IEnumerator CoMatFadeOut(BaseController bc, System.Action startCallback = null, System.Action endCallback = null)
        {
            if (bc?.IsValid() == false)
                yield break;

            switch (bc.ObjectType)
            {
                case Define.ObjectType.Player:
                case Define.ObjectType.Monster:
                    {
                        startCallback?.Invoke();
                        Material clonedMat = MakeClonedMaterial(Mat_Fade);
                        clonedMat.SetFloat(SHADER_FADE_ALPHA, 1f);
                        bc.RendererController.SetMaterial(clonedMat);

                        float delta = 0f;
                        float percent = 1f;
                        while (percent > 0f)
                        {
                            delta += Time.deltaTime;
                            percent = 1f - (delta / FixedValue.Numeric.FADE_OUT_DURATION);
                            clonedMat.SetFloat(SHADER_FADE_ALPHA, percent);
                            yield return null;
                        }

                        bc.RendererController.ResetMaterial();
                        endCallback?.Invoke();
                    }
                    break;
            }

            yield break;
        }

        public IEnumerator CoMatFadeOutInstantly(BaseController bc, float duration, System.Action startCallback = null, System.Action endCallback = null)
        {
            if (bc?.IsValid() == false)
                yield break;

            startCallback?.Invoke();
            Material clonedMat = MakeClonedMaterial(Mat_Fade);
            clonedMat.SetFloat(SHADER_FADE_ALPHA, FixedValue.Numeric.INSTANT_FADE_ALPHA);
            bc.RendererController.SetMaterial(clonedMat);

            float delta = 0f;
            float percent = 0f;
            while (percent < 1f)
            {
                delta += Time.deltaTime;
                percent = delta / duration;
                yield return null;
            }

            bc.RendererController.ResetMaterial();
            endCallback?.Invoke();
        }

        public IEnumerator CoMatStrongTint(MaterialColor matColor, BaseController bc, SpriteRenderer spr, float desiredTime, System.Action callback = null)
        {
            if (bc?.IsValid() == false)
                yield break;

            Material matOrigin = spr.material;
            // NEED TO MAKE NEW MAT
            Material clonedMat = new Material(Mat_StrongTint);

            switch (matColor)
            {
                case MaterialColor.UsePreset:
                    break;

                case MaterialColor.White:
                    clonedMat.SetColor(SHADER_STRONG_TINT_COLOR, Color.white);
                    break;

                case MaterialColor.Red:
                    clonedMat.SetColor(SHADER_STRONG_TINT_COLOR, Color.red);
                    break;

                case MaterialColor.Green:
                    clonedMat.SetColor(SHADER_STRONG_TINT_COLOR, Color.green);
                    break;
            }

            spr.material = clonedMat;
            float percent = 0f;
            while (percent < 1f)
            {
                percent += Time.deltaTime / desiredTime;
                clonedMat.SetFloat(SHADER_STRONG_TINT_FADE, percent);
                yield return null;
            }

            callback?.Invoke(); // START BOMB
            percent = 0f;
            while (percent < 0.1f)
            {
                percent += Time.deltaTime;
                yield return null;
            }

            spr.material = matOrigin; // RESET MATERIAL
        }

        public IEnumerator CoMatInnerOutline(SpriteRenderer spr, float duration, System.Action callback = null)
        {
            if (spr.sprite == null)
            {
                Utils.Log("Failed to find sprite in SpriteRenderer.");
                yield break;
            }

            Material matOrigin = spr.material;
            Material matCloned = new Material(Mat_InnerOutline);
            matCloned.SetFloat(SHADER_INNER_OUTLINE_FADE, 0f);
            spr.material = matCloned;

            float delta = 0f;
            float deltaForDuration = 0f;

            float percent = 0f;
            bool isIncreasing = true;
            while (deltaForDuration < duration)
            {
                deltaForDuration += Time.deltaTime;
                delta += Time.deltaTime;

                if (isIncreasing)
                    percent = delta / FixedValue.Numeric.INNER_OUTLINE_FADE_PING_PONG_INTERVAL;
                else
                    percent = 1f - (delta / FixedValue.Numeric.INNER_OUTLINE_FADE_PING_PONG_INTERVAL);

                if (percent <= 1f && percent >= 0f)
                    matCloned.SetFloat(SHADER_INNER_OUTLINE_FADE, percent);
                else
                {
                    isIncreasing = !isIncreasing;
                    delta = 0f;
                }

                yield return null;
            }

            callback?.Invoke();
            matCloned.SetFloat(SHADER_INNER_OUTLINE_FADE, 0f);
            spr.material = matOrigin;
        }

        private bool IsChangingMaterial(BaseController bc) =>
                        (bc.RendererController.IsChangingMaterial || bc.RendererController == null);

        public IEnumerator CoMatPoison(BaseController bc, float duration, System.Action startCallback = null, System.Action endCallback = null)
        {
            if (bc?.IsValid() == false)
                yield break;

            if (IsChangingMaterial(bc))
                yield break;

            Material clonedMat = MakeClonedMaterial(Mat_Poison);
            bc.RendererController.SetMaterial(clonedMat);
            clonedMat.SetFloat(SHADER_POISON_FADE, 0f);

            float delta = 0f;
            float deltaForDuration = 0f;

            float percent = 0f;
            bool isIncreasing = true;
            startCallback?.Invoke();

            CreatureController cc = bc.GetComponent<CreatureController>();
            while (deltaForDuration < duration)
            {
                if (cc != null && cc.IsDeadState)
                {
                    // 어차피 Fade로 바뀌니까...
                    //cc.RendererController.ResetMaterial();
                    endCallback?.Invoke(); // 중복호출일수도 있지만 혹시 모르니
                    yield break;
                }

                deltaForDuration += Time.deltaTime;
                delta += Time.deltaTime;

                if (isIncreasing)
                    percent = delta / FixedValue.Numeric.POISON_PING_PONG_INTERVAL;
                else
                    percent = 1f - (delta / FixedValue.Numeric.POISON_PING_PONG_INTERVAL);

                if (percent >= 0f && percent <= 1f)
                    clonedMat.SetFloat(SHADER_POISON_FADE, percent);
                else
                {
                    isIncreasing = !isIncreasing;
                    delta = 0f;
                }

                yield return null;
            }

            bc.RendererController.ResetMaterial();
            endCallback?.Invoke();
        }

        //=======================================================================================================
        //=======================================================================================================
        //=======================================================================================================
        public void Muzzle(VFXMuzzleType muzzleType, CreatureController target)
        {
            Vector3 muzzlePoint = Vector3.zero;
            switch (muzzleType)
            {
                case VFXMuzzleType.None:
                    return;

                case VFXMuzzleType.White:
                    GameObject go = Managers.Resource.Instantiate(FixedValue.Load.VFX_MUZZLE_WHITE, null, true);
                    go.transform.position = target.FireSocketPosition;
                    MovementToOwner movementToOwner = go.GetComponent<MovementToOwner>();
                    movementToOwner.IsOnFireSocket = true;
                    movementToOwner.Owner = target;

                    // if (go.GetComponent<MovementToOwner>().Owner == null)
                    // {
                    //     go.GetComponent<MovementToOwner>().Owner = target;
                    //     go.GetComponent<MovementToOwner>().Owner = target;
                    // }
                    break;
            }
        }

        public GameObject ImpactHit(VFXImpactHitType vfxImpactHitType, CreatureController target, SkillBase from)
        {
            Vector3 impactPoint = Vector3.zero;
            if (from.Data.IsVFXImpactPointOnTarget)
            {
                float additionalPointX = UnityEngine.Random.Range(0f, 0.5f);
                float additionalPointY = UnityEngine.Random.Range(0f, 1f);
                impactPoint = target.Center.position + new Vector3(additionalPointX, additionalPointY, 0f);
            }
            else
                impactPoint = from.transform.position;

            // VFX_IMPACT_HIT
            GameObject goImpactHit = null;
            switch (vfxImpactHitType)
            {
                case VFXImpactHitType.None:
                    return goImpactHit;

                case VFXImpactHitType.Hit:
                    goImpactHit = Managers.Resource.Instantiate(FixedValue.Load.VFX_IMPACT_HIT_DEFAULT, null, true);
                    break;

                case VFXImpactHitType.Leaves:
                    goImpactHit = Managers.Resource.Instantiate(FixedValue.Load.VFX_IMPACT_HIT_LEAVES, null, true);
                    break;

                case VFXImpactHitType.Light:
                    goImpactHit = Managers.Resource.Instantiate(FixedValue.Load.VFX_IMPACT_HIT_LIGHT, null, true);
                    break;

                case VFXImpactHitType.SmokePuff:
                    goImpactHit = Managers.Resource.Instantiate(FixedValue.Load.VFX_IMPACT_HIT_SMOKE_PUFF, null, true);
                    break;

                case VFXImpactHitType.Incinvible:
                    goImpactHit = Managers.Resource.Instantiate(FixedValue.Load.VFX_IMPACT_HIT_INVINCIBLE, null, true);
                    break;

                case VFXImpactHitType.Poison:
                    goImpactHit = Managers.Resource.Instantiate(FixedValue.Load.VFX_IMPACT_HIT_POISON, null, true);
                    break;
            }

            goImpactHit.transform.position = impactPoint;
            return goImpactHit;
        }

        public GameObject Trail(VFXTrailType trailType, BaseController targetInSocket, CreatureController owner)
        {
            if (targetInSocket.TrailSocket == null)
                Utils.LogCritical(nameof(VFXManager), nameof(Trail), "You have to set TrailSocket in advance if you want to use Trail.");

            GameObject goTrail = null;
            switch (trailType)
            {
                case VFXTrailType.None:
                    return goTrail;

                case VFXTrailType.Wind:
                    {
                        goTrail = Managers.Resource.Instantiate(FixedValue.Load.VFX_TRAIL_WIND, null, true);
                        TrailTarget trailTarget = goTrail.GetComponent<TrailTarget>();
                        trailTarget.transform.position = owner.FireSocketPosition;
                        trailTarget.Owner = owner;
                        trailTarget.Target = targetInSocket;
                    }
                    break;

                case VFXTrailType.Light:
                    {
                        goTrail = Managers.Resource.Instantiate(FixedValue.Load.VFX_TRAIL_LIGHT, null, true);
                        TrailTarget trailTarget = goTrail.GetComponent<TrailTarget>();
                        trailTarget.transform.position = owner.FireSocketPosition;
                        trailTarget.Owner = owner;
                        trailTarget.Target = targetInSocket;
                    }
                    break;
            }

            return goTrail;
        }

        public void DamageOnShield(CreatureController cc, float damage)
            => Managers.Resource.Load<GameObject>(FixedValue.Load.VFX_ENV_DAMAGE_TO_PLAYER_SHIELD)
                       .GetComponent<DamageNumber>().Spawn(cc.LoadVFXEnvSpawnPos(VFXEnvType.Damage), damage);

        public void Damage(CreatureController cc, float damage, bool isCritical)
        {
            if (cc.IsValid() == false)
                return;
            if (damage <= 0f)
                return;

            Vector3 spawnPos = cc.LoadVFXEnvSpawnPos(VFXEnvType.Damage);
            if (cc.IsMonster)
            {
                if (isCritical)
                {
                    Managers.Resource.Load<GameObject>(FixedValue.Load.VFX_ENV_DAMAGE_TO_MONSTER_CRITICAL_FONT)
                            .GetComponent<DamageNumber>().Spawn(spawnPos);

                    Managers.Resource.Load<GameObject>(FixedValue.Load.VFX_ENV_DAMAGE_TO_MONSTER_CRITICAL)
                            .GetComponent<DamageNumber>().Spawn(spawnPos, damage);
                }
                else
                    Managers.Resource.Load<GameObject>(FixedValue.Load.VFX_ENV_DAMAGE_TO_MONSTER)
                            .GetComponent<DamageNumber>().Spawn(spawnPos, damage);
            }
            else
            {
                Managers.Resource.Load<GameObject>(FixedValue.Load.VFX_ENV_DAMAGE_TO_PLAYER)
                        .GetComponent<DamageNumber>().Spawn(spawnPos, damage);
            }
        }

        public void PoisonDamage(CreatureController target, float damage)
        {
            if (target.IsValid() == false)
                return;

            Vector3 spawnPos = (target?.IsPlayer == false && target.GetComponent<MonsterController>() != null)
                                ? target.GetComponent<MonsterController>().LoadVFXEnvSpawnPos(VFXEnvType.Poison)
                                : target.GetComponent<PlayerController>().LoadVFXEnvSpawnPos(VFXEnvType.Damage);

            Managers.Resource.Load<GameObject>(FixedValue.Load.VFX_ENG_DAMAGE_POISON)
                            .GetComponent<DamageNumber>().Spawn(spawnPos, damage, followedTransform: target.Center);
        }

        public void Percentage(CreatureController target, int percent)
        {
            Vector3 spawnPos = target.Center.transform.position + (Vector3.up * 3.5f);
            DamageNumber dmgNumber = Managers.Resource.Load<GameObject>(FixedValue.Load.VFX_ENV_FONT_PERCENTAGE)
                 .GetComponent<DamageNumber>().Spawn(spawnPos, percent);
            if (percent < 100)
                dmgNumber.lifetime = 0.15f;
            else
                dmgNumber.lifetime = 0.75f;
        }

        public GameObject Environment(VFXEnvType vfxEnvType, CreatureController target)
        {
            GameObject goVFX = null;

            // Vector3 spawnScale = Vector3.one;
            // spawnScale = (target?.IsPlayer == false && target.GetComponent<MonsterController>() != null)
            //             ? target.GetComponent<MonsterController>().LoadVFXEnvSpawnScale(templateOrigin)
            //             : target.GetComponent<PlayerController>().LoadVFXEnvSpawnScale(templateOrigin);
            Vector3 spawnScale = target.LoadVFXEnvSpawnScale(vfxEnvType);

            // Vector3 spawnPos = Vector3.zero;
            // spawnPos = (target?.IsPlayer == false && target.GetComponent<MonsterController>() != null)
            //             ? target.GetComponent<MonsterController>().LoadVFXEnvSpawnPos(templateOrigin)
            //             : target.GetComponent<PlayerController>().LoadVFXEnvSpawnPos(templateOrigin);
            Vector3 spawnPos = target.LoadVFXEnvSpawnPos(vfxEnvType);

            switch (vfxEnvType)
            {
                case VFXEnvType.Spawn:
                    {
                        goVFX = Managers.Resource.Instantiate(FixedValue.Load.VFX_ENV_SPAWN, null, true);
                        goVFX.transform.localScale = spawnScale;
                        goVFX.transform.position = spawnPos;
                    }
                    break;

                case VFXEnvType.Dodge:
                    {
                        Managers.Resource.Load<GameObject>(FixedValue.Load.VFX_ENV_DAMAGE_TO_PLAYER_DODGE_FONT)
                                         .GetComponent<DamageNumber>().Spawn(spawnPos);
                    }
                    break;

                case VFXEnvType.Skull:
                    {
                        goVFX = Managers.Resource.Instantiate(FixedValue.Load.VFX_ENV_SKULL, null, true);
                        goVFX.transform.localScale = spawnScale;
                        goVFX.transform.position = spawnPos;
                    }
                    break;

                case VFXEnvType.Dust:
                    {
                        goVFX = Managers.Resource.Instantiate(FixedValue.Load.VFX_ENV_DUST, null, true);
                        goVFX.transform.localScale = spawnScale;
                        goVFX.transform.position = spawnPos;
                    }
                    break;

                case VFXEnvType.Stun:
                    {
                        goVFX = Managers.Resource.Instantiate(FixedValue.Load.VFX_ENV_STUN, null, true);
                        goVFX.transform.localScale = spawnScale;
                        goVFX.transform.position = spawnPos;
                    }
                    break;

                case VFXEnvType.Slow:
                    {
                        goVFX = Managers.Resource.Instantiate(FixedValue.Load.VFX_ENV_SLOW, null, true);
                        goVFX.transform.localScale = spawnScale;
                        goVFX.transform.position = spawnPos;
                    }
                    break;

                case VFXEnvType.Silence:
                    {
                        goVFX = Managers.Resource.Instantiate(FixedValue.Load.VFX_ENV_SILENCE, null, true);
                        goVFX.transform.localScale = spawnScale;
                        goVFX.transform.position = spawnPos;
                    }
                    break;

                case VFXEnvType.Targeted:
                    {
                        goVFX = Managers.Resource.Instantiate(FixedValue.Load.VFX_ENV_TARGETED, null, true);
                        goVFX.transform.localScale = spawnScale;
                        goVFX.transform.position = spawnPos;
                    }
                    break;

                case VFXEnvType.QuestionMark:
                    {
                        goVFX = Managers.Resource.Instantiate(FixedValue.Load.VFX_ENV_QUESTION_MARK, null, true);
                        goVFX.transform.localScale = spawnScale;
                        goVFX.transform.position = spawnPos;
                    }
                    break;
            }

            return goVFX;
        }

        public GameObject Environment(VFXEnvType vfxEnvType, Vector3 spawnPos)
        {
            GameObject goVFX = null;
            switch (vfxEnvType)
            {
                case VFXEnvType.GemGather:
                    {
                        goVFX = Managers.Resource.Instantiate(FixedValue.Load.VFX_ENV_GEM_GATHER, null, true);
                        goVFX.transform.position = spawnPos;
                    }
                    break;

                case VFXEnvType.GemExplosion:
                    {
                        goVFX = Managers.Resource.Instantiate(FixedValue.Load.VFX_ENV_GEM_EXPLOSION, null, true);
                        goVFX.transform.position = spawnPos;
                    }
                    break;
            }

            return goVFX;
        }

        private Material MakeClonedMaterial(Material matTarget) => new Material(matTarget);
    }
}

// ===================================================================================================================
//         public void Damage(CreatureController cc, float damage, bool isCritical)
//         {
//             if (cc.IsValid() == false)
//                 return;
//             if (damage <= 0f)
//                 return;

//             Vector3 spawnPos = (cc?.IsPlayer == false && cc.GetComponent<MonsterController>() != null)
//                                 ? cc.GetComponent<MonsterController>().LoadVFXEnvSpawnPos(VFXEnvType.Damage)
//                                 : cc.GetComponent<PlayerController>().LoadVFXEnvSpawnPos(VFXEnvType.Damage);

// #if UNITY_EDITOR
//             if (spawnPos == Vector3.zero)
//                 Utils.LogCritical(nameof(VFXManager), nameof(Environment), "Failed to load VFX Env Spawn Pos.");
// #endif
//             if (cc.IsMonster) // 현재 크티티컬은 몬스터만 받아서 필요 없을수도
//             {
//                 if (isCritical)
//                 {
//                     Managers.Resource.Load<GameObject>(FixedValue.Load.VFX_ENV_DAMAGE_TO_MONSTER_CRITICAL_FONT)
//                                      .GetComponent<DamageNumber>().Spawn(spawnPos);

//                     Managers.Resource.Load<GameObject>(FixedValue.Load.VFX_ENV_DAMAGE_TO_MONSTER_CRITICAL)
//                                      .GetComponent<DamageNumber>().Spawn(spawnPos, damage);
//                 }
//                 else
//                 {
//                     Managers.Resource.Load<GameObject>(FixedValue.Load.VFX_ENV_DAMAGE_TO_MONSTER)
//                                      .GetComponent<DamageNumber>().Spawn(spawnPos, damage);
//                 }
//             }
//             else // Damage to Player
//             {
//                 if (cc.SkillBook.IsOnShield == false)
//                 {
//                     Managers.Resource.Load<GameObject>(FixedValue.Load.VFX_ENV_DAMAGE_TO_PLAYER)
//                                     .GetComponent<DamageNumber>().Spawn(spawnPos, damage);
//                 }
//                 else
//                 {
//                     Managers.Resource.Load<GameObject>(FixedValue.Load.VFX_ENV_DAMAGE_TO_PLAYER_SHIELD)
//                                      .GetComponent<DamageNumber>().Spawn(spawnPos, damage);
//                 }
//             }
//         }
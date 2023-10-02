using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DamageNumbersPro;
using UnityEngine;

using ImpactTemplate = STELLAREST_2D.Define.TemplateIDs.VFX.Impact;
using EnvTemplate = STELLAREST_2D.Define.TemplateIDs.VFX.Environment;
using Unity.VisualScripting;

namespace STELLAREST_2D
{
    public class VFXManager
    {
        public Material MatHit_Monster { get; private set; } = null;
        public Material MatHit_Player { get; private set; } = null;

        public void Init()
        {
            MatHit_Monster = Managers.Resource.Load<Material>(Define.Labels.Materials.MAT_HIT_WHITE);
            MatHit_Player = Managers.Resource.Load<Material>(Define.Labels.Materials.MAT_HIT_RED);
        }

        private const float HIT_RESET_DELAY = 0.1f;
        public void Hit(CreatureController cc)
        {
            if (cc.IsValid() == false)
                return;

            if (cc.IsMonster())
                cc.RendererController.ChangeMaterial(MatHit_Monster, HIT_RESET_DELAY);
            else
                cc.RendererController.ChangeMaterial(MatHit_Player, HIT_RESET_DELAY);
        }

        // Critical Ratio of all of monsters is zero.
        public void DamageFont(CreatureController cc, float damage, bool isCritical)
        {
            if (cc.IsValid() == false)
                return;

            Vector3 spawnPos = GetSpawnPosForDamageFont(cc);
            if (cc.IsMonster())
            {
                if (isCritical)
                {
                    Managers.Resource.Load<GameObject>(Define.Labels.Prefabs.VFX_ENV_DMG_TEXT_TO_MONSTER_CRITICAL)
                                     .GetComponent<DamageNumber>().Spawn(spawnPos);

                    Managers.Resource.Load<GameObject>(Define.Labels.Prefabs.VFX_ENV_DMG_NUMBER_TO_MONSTER_CRITICAL)
                                     .GetComponent<DamageNumber>().Spawn(spawnPos, damage);
                }
                else
                {
                    Managers.Resource.Load<GameObject>(Define.Labels.Prefabs.VFX_ENV_DMG_NUMBER_TO_MONSTER)
                                     .GetComponent<DamageNumber>().Spawn(spawnPos, damage);
                }
            }
            else
            {
                Managers.Resource.Load<GameObject>(Define.Labels.Prefabs.VFX_ENV_DMG_NUMBER_TO_PLAYER)
                                 .GetComponent<DamageNumber>().Spawn(spawnPos, damage);
            }
        }

        // 어뜨케할까?
        public void Impact(ImpactTemplate templateOrigin, CreatureController target, SkillBase from)
        {
            Vector3 impactPoint = Vector3.zero;
            if (from.Data.IsImpactPointOnTarget)
            {
                float additionalPointX = UnityEngine.Random.Range(0f, 0.5f);
                float additionalPointY = UnityEngine.Random.Range(0f, 1f);
                impactPoint = target.Center.position + new Vector3(additionalPointX, additionalPointY, 0f);
            }
            else
                impactPoint = from.transform.position;

            switch (templateOrigin)
            {
                case ImpactTemplate.Hit:
                {
                    GameObject go = Managers.Resource.Instantiate(Define.Labels.Prefabs.IMPACT_CRITICAL_HIT_EFFECT, null, true);
                    go.transform.position = impactPoint;
                }
                break;
            }
        }

        public void Environment(EnvTemplate templateOrigin, CreatureController target)
        {
            Vector3 spawnPos = GetSpawnPosForEnvironment(templateOrigin, target);
            switch (templateOrigin)
            {
                case EnvTemplate.Spawn:
                    GameObject go = Managers.Resource.Instantiate(Define.Labels.Prefabs.VFX_ENV_SPAWN, null, true);
                    go.transform.position = spawnPos;
                    break;
            }
        }

        private const float DEFAULT_DMG_SPAWN_HEIGHT = 2.5f;
        private Vector3 GetSpawnPosForDamageFont(CreatureController cc)
        {
            Vector3 spawnPos = cc.transform.position + (Vector3.up * DEFAULT_DMG_SPAWN_HEIGHT);
            if (cc?.IsPlayer() == false)
            {
                MonsterController mc = cc.GetComponent<MonsterController>();
                switch (mc.MonsterType)
                {
                    case Define.MonsterType.Chicken:
                        return (spawnPos -= Vector3.up);
                }
            }

            return spawnPos;
        }

        // TODO
        private const float DEFAULT_SPAWN_HEIGHT = 2.5f;
        private const float ADDITIONAL_SPAWH_WIDTH_FOR_CHICKEN = 0.1f;
        private const float ADDITIONAL_SPAWH_HEIGHT_FOR_CHICKEN = 1.2f;
        private Vector3 GetSpawnPosForEnvironment(EnvTemplate templateOrigin, CreatureController cc)
        {
            MonsterController mc = cc.GetComponent<MonsterController>();
            if (mc != null)
            {
                Vector3 spawnedPos = mc.transform.position + (Vector3.up * DEFAULT_SPAWN_HEIGHT);
                switch (mc.MonsterType)
                {
                    case Define.MonsterType.Chicken:
                        return new Vector3(spawnedPos.x + ADDITIONAL_SPAWH_WIDTH_FOR_CHICKEN, spawnedPos.y + ADDITIONAL_SPAWH_HEIGHT_FOR_CHICKEN, 1f);

                    default:
                        return spawnedPos;
                }
            }

            return Vector3.zero;
        }
    }
}

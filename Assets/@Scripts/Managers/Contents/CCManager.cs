using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Threading.Tasks;
using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    public static class CCStun
    {
        public const float CHICKEN_FOR_WIDTH = 2f;
        public const float CHICKEN_FOR_HEIGHT = 1.7f;
    }

    public class CCManager
    {
        // Chicen : y - 7.83, scale : 2, 2
        // +++ 주의 사항 : CC만 담당, 데미지 X +++
        public void ApplyCC<T>(T creature, SkillData skillData, ProjectileController projectileAttacker = null) where T : CreatureController
        {
            if (creature?.IsValid() == false)
                return;

            switch (skillData.CCType)
            {
                case Define.CCType.Stun:
                    ApplyStun<T>(creature, skillData.CCDuration);
                    break;

                case Define.CCType.KnockBack:
                    {
                        Vector3 attackerShootDir = projectileAttacker.ShootDir;
                        ApplyKnockBack<T>(creature, attackerShootDir, skillData.CCDuration);
                    }
                    break;
            }
        }

        private void ApplyStun<T>(T creature, float duration) where T : CreatureController
        {
            if (creature?.IsValid() == false)
                return;

            if (creature[Define.CCType.Stun])
                return;

            CreatureController cc = creature.GetComponent<CreatureController>();
            GameObject stunEffect = Managers.Effect.ShowStunEffect();
            stunEffect.transform.localScale = ApplyCCEffectScale(cc, Define.CCType.Stun);
            cc.CoCCStun(cc, stunEffect, duration); // StartCoroutine만 cc에서 해주는것이다
        }

        private void ApplyKnockBack<T>(T creature, Vector3 dir, float duration) where T : CreatureController
        {
            if (creature?.IsValid() == false)
                return;

            if (creature[Define.CCType.KnockBack])
                return;

            CreatureController cc = creature.GetComponent<CreatureController>();
            cc.CoCCKnockBack(cc, dir, duration);
        }

        public IEnumerator CoStun(CreatureController cc, GameObject goCCEffect, float duration)
        {
            cc.GoCCEffect = goCCEffect;
            cc[Define.CCType.Stun] = true; // !!! 인덱서로 Set 할것. 반드시 !!!
            MonsterController mc = null;
            if (cc?.IsMonster() == true)
            {
                mc = cc.GetComponent<MonsterController>();
                Managers.Sprite.SetMonsterFace(mc, Define.MonsterFace.Death);
            }

            float t = 0f;
            float percent = 0f;
            while (percent < 1f)
            {
                goCCEffect.transform.position = ApplyCCEffectPosition(cc, Define.CCType.Stun);
                t += Time.deltaTime;
                percent = t / duration;
                yield return null;
            }

            cc.GoCCEffect = null;
            Managers.Resource.Destroy(goCCEffect);
            cc[Define.CCType.Stun] = false;

            if (cc?.IsMonster() == true)
            {
                Managers.Sprite.SetMonsterFace(mc, Define.MonsterFace.Normal);
                mc.CoStartReadyToAction(false);
            }
        }

        public IEnumerator CoKnockBack(CreatureController cc, Vector3 attackerShootDir, float duration)
        {
            float t = 0f;
            float percent = 0f;
            cc[Define.CCType.KnockBack] = true;
            while (percent < 1f)
            {
                t += Time.deltaTime;
                percent = t / duration;
                //cc.transform.position += (attackerShootDir * 0.5f);
                //cc.transform.position += (attackerShootDir * 0.35f);
                Vector2 pos = cc.transform.position;
                if (Managers.Stage.IsOutOfPos(pos))
                    continue;
                else
                    cc.transform.position += attackerShootDir * 0.05f;

                yield return null;
            }
            cc[Define.CCType.KnockBack] = false;
        }

        private Vector3 ApplyCCEffectScale(CreatureController creature, Define.CCType ccType)
        {
            Define.CreatureType type = creature.CreatureType;
            if (ccType == Define.CCType.Stun)
            {
                switch (type)
                {
                    case Define.CreatureType.Chicken:
                        return new Vector3(CCStun.CHICKEN_FOR_WIDTH, CCStun.CHICKEN_FOR_WIDTH, 1f);
                }
            }

            return Vector3.zero;
        }

        private Vector2 ApplyCCEffectPosition(CreatureController cc, Define.CCType ccType)
        {
            Define.CreatureType type = cc.CreatureType;
            if (ccType == Define.CCType.Stun)
            {
                switch (type)
                {
                    case Define.CreatureType.Chicken:
                        return new Vector2(cc.transform.position.x, cc.transform.position.y + CCStun.CHICKEN_FOR_HEIGHT);
                }
            }

            return Vector2.zero;
        }

        // public void ApplyKnockBack<T>(T c, Vector2 hitPoint, Vector2 knockBackDir, float duration, float intensity) where T : CreatureController
        // {
        //     if (c?.IsValid() == false)
        //         return;

        //     CreatureController cc = c.GetComponent<CreatureController>();
        //     cc.CoStartKnockBack(cc, hitPoint, knockBackDir, duration, intensity);
        // }

        // // 스턴과 다르게 히트 할때마다 수시로 발동해야되서 스턴보다 무조건 구려야함
        // // 그래서 넉백은 mc.CoStartReadyToAction(false) 이런거 절대하면 안됨
        // public IEnumerator CoStartKnockBack(CreatureController cc, Vector2 hitPoint, Vector2 knockBackDir, float duration, float intensity)
        // {
        //     if (cc?.IsValid() == false)
        //         yield break;

        //     cc[Define.CCType.KnockBack] = true;
        //     float t = 0f;
        //     float percent = 0f;

        //     Vector2 startPos = Vector2.zero;
        //     Vector2 endPos = startPos + (knockBackDir * intensity);
        //     while (percent < 1f)
        //     {
        //         startPos = cc.transform.position;
        //         t += Time.deltaTime;
        //         percent = t / duration;
        //         cc.transform.position = Vector2.MoveTowards(startPos, endPos, Time.deltaTime * intensity);
        //     }

        //     cc[Define.CCType.KnockBack] = false;
        //     yield return null;
        // }



        // LEGACY
        // 뭔가 순서가 잘못된듯.
        // OnTrigerEnter에서 맞혔으면, 거기서 StartCoRoutine을 실행시켜야할듯.
        // ApplyCC가 아닌,,, 이거 Legacy 될 듯
        // public void ApplyCC<T>(T c, Define.CCState ccState, float duration, float knockBackIntensity = 1f) where T : CreatureController
        // {
        //     if (c?.IsValid() == false)
        //         return;

        //     CreatureController cc = c.GetComponent<CreatureController>();
        //     cc[ccState] = true;
        //     if (cc[ccState])
        //     {
        //         switch (ccState)
        //         {
        //             case Define.CCState.Stun:
        //                 {
        //                     GameObject stunEffect = Managers.Effect.ShowStunEffect();
        //                     //stunEffect.transform.position = ApplyPosition(cc);
        //                     stunEffect.transform.localScale = ApplyCCEffectScale(cc);
        //                     cc.CoStartStun(cc, stunEffect, duration); // StartCoroutine만 cc에서 해주는것이다
        //                 }
        //                 break;

        //             case Define.CCState.KnockBack:
        //                 {
        //                 }
        //                 break;
        //         }
        //     }
        // }
    }
}

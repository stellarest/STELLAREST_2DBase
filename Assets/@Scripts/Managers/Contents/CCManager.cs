using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

namespace STELLAREST_2D
{
    public class CCManager
    {
        private const float CHICKEN_FOR_WIDTH = 2f;
        private const float CHICKEN_FOR_HEIGHT = 1.7f;

        // Chicen : y - 7.83, scale : 2, 2
        // +++ 주의 사항 : CC만 담당한다 +++
        // 크리쳐의 HP가 몇 이상일때 적용하거나 안하는건 데미지를 전달하는 Trigger쪽에서 결정할일이다.
        public void ApplyStun<T>(T c, float duration) where T : CreatureController
        {
            if (c?.IsValid() == false)
                return;

            CreatureController cc = c.GetComponent<CreatureController>();
            GameObject stunEffect = Managers.Effect.ShowStunEffect();
            stunEffect.transform.localScale = ApplyCCEffectScale(cc);
            cc.CoStartStun(cc, stunEffect, duration); // StartCoroutine만 cc에서 해주는것이다
        }

        public IEnumerator CoStartStun(CreatureController cc, GameObject goCCEffect, float duration)
        {
            cc.GoCCEffect = goCCEffect;
            cc[Define.CCState.Stun] = true;
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
                goCCEffect.transform.position = ApplyCCEffectPosition(cc);
                t += Time.deltaTime;
                percent = t / duration;
                yield return null;
            }

            cc.GoCCEffect = null;
            Managers.Resource.Destroy(goCCEffect);
            cc[Define.CCState.Stun] = false;
            if (cc?.IsMonster() == true)
            {
                Managers.Sprite.SetMonsterFace(mc, Define.MonsterFace.Normal);
                mc.CoStartReadyToAction(false);
            }
        }

        public void ApplyKnockBack<T>(T c, Vector2 hitPoint, Vector2 knockBackDir, float duration, float intensity) where T : CreatureController
        {
            if (c?.IsValid() == false)
                return;

            CreatureController cc = c.GetComponent<CreatureController>();
            cc.CoStartKnockBack(cc, hitPoint, knockBackDir, duration, intensity);
        }

        // 스턴과 다르게 히트 할때마다 수시로 발동해야되서 스턴보다 무조건 구려야함
        // 그래서 넉백은 mc.CoStartReadyToAction(false) 이런거 절대하면 안됨
        public IEnumerator CoStartKnockBack(CreatureController cc, Vector2 hitPoint, Vector2 knockBackDir, float duration, float intensity)
        {
            if (cc?.IsValid() == false)
                yield break;

            cc[Define.CCState.KnockBack] = true;
            float t = 0f;
            float percent = 0f;

            Vector2 startPos = Vector2.zero;
            Vector2 endPos = startPos + (knockBackDir * intensity);
            while (percent < 1f)
            {
                startPos = cc.transform.position;
                t += Time.deltaTime;
                percent = t / duration;
                cc.transform.position = Vector2.MoveTowards(startPos, endPos, Time.deltaTime * intensity);
            }

            cc[Define.CCState.KnockBack] = false;
            yield return null;
        }

        private Vector3 ApplyCCEffectScale(CreatureController cc)
        {
            Define.CreatureType type = cc.CreatureType;
            switch (type)
            {
                case Define.CreatureType.Chicken:
                    return new Vector3(CHICKEN_FOR_WIDTH, CHICKEN_FOR_WIDTH, 1f);
            }

            return Vector3.zero;
        }

        private Vector2 ApplyCCEffectPosition(CreatureController cc)
        {
            Define.CreatureType type = cc.CreatureType;
            switch (type)
            {
                case Define.CreatureType.Chicken:
                    return new Vector2(cc.transform.position.x, cc.transform.position.y + CHICKEN_FOR_HEIGHT);
            }

            return Vector2.zero;
        }



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

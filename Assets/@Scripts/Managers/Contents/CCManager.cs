using System.Collections;
using System.Collections.Generic;
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
        public void ApplyCC(CreatureController cc, Define.CCStatus ccStatus, float duration)
        {
            if (cc?.IsValid() == false)
                return;

            switch (ccStatus)
            {
                case Define.CCStatus.Stun:
                {
                    GameObject stunEffect = Managers.Effect.ShowStunEffect();
                    //stunEffect.transform.position = ApplyPosition(cc);
                    stunEffect.transform.localScale = ApplyScale(cc);
                    cc.CoStartStun(cc, stunEffect, duration); // StartCoroutine만 cc에서 해주는것이다
                }
                break;
            }
        }

        // CC를 동시에 중복 적용 시켜야는데,,,
        public IEnumerator CoStartStun(CreatureController cc, GameObject goCCEffect, float duration)
        {
            cc.GoCCEffect = goCCEffect;
            cc.CCStatus = Define.CCStatus.Stun;

            MonsterController mc = null;
            if (cc?.IsMonster() == true)
            {
                mc = cc.GetComponent<MonsterController>();
                Managers.Sprite.SetMonsterFace(mc, Define.SpriteLabels.MonsterFace.Death);
            }
            
            float t = 0f;
            float percent = 0f;
            
            while (percent < 1f)
            {
                goCCEffect.transform.position = ApplyPosition(cc);
                t += Time.deltaTime;
                percent = t / duration;
                yield return null;
            }
            
            cc.GoCCEffect = null;
            Managers.Resource.Destroy(goCCEffect);
            cc.CCStatus = Define.CCStatus.None;

            if (cc?.IsMonster() == true)
            {
                Managers.Sprite.SetMonsterFace(mc, Define.SpriteLabels.MonsterFace.Normal);
                mc.CoStartReadyToAction(false);
            }
        }

        private Vector3 ApplyScale(CreatureController cc)
        {
            Define.CreatureType type = cc.CreatureType;
            switch (type)
            {
                case Define.CreatureType.Chicken:
                    return new Vector3(CHICKEN_FOR_WIDTH, CHICKEN_FOR_WIDTH, 1f);
            }

            return Vector3.zero;
        }

        private Vector2 ApplyPosition(CreatureController cc)
        {
            Define.CreatureType type = cc.CreatureType;
            switch (type)
            {
                case Define.CreatureType.Chicken:
                    return new Vector2(cc.transform.position.x, cc.transform.position.y + CHICKEN_FOR_HEIGHT);
            }

            return Vector2.zero;
        }
    }
}

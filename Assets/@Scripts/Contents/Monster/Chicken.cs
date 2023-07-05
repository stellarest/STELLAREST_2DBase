using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class Chicken : MonsterController
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;
            Debug.Log("### CHICKEN INIT ###");

            MonsterState = Define.MonsterState.Run;
            _attackRange = 4f;

            AttackCol = Utils.FindChild(gameObject, "Body").GetComponent<CircleCollider2D>();
            SetInitAttackCollider(AttackCol);
            
            // Debug.Log("Parent Of Attack Col : " + AttackCol.name);
            // SkillBook.AddSkill<MeleeAttack>(transform.position); // Init에서 먼저 가져와서 null인듯
            return true;
        }

        // 몸땡아리 콜라이더와 어택 콜라이더 따로 있어야 할듯
        public override void InitMonsterSkill() // 매서드 이름 Late Init으로 바꿔야할지도
        {
            base.InitMonsterSkill();
            SkillBook.AddSkill<BodyAttack>(this);

            // fadePropertyID = Shader.PropertyToID("_StrongTintFade");
            // fadePropertyID = Shader.PropertyToID("_HologramFade");

            /*
            if (bChange)
            {
                foreach (var sp in renderers)
                {
                    sp.material = testMat;
                    sp.material.SetFloat(fadePropertyID, intensity);
                }
            }
            else
            {
                foreach (var sp in renderers)
                    sp.material = defaultMat;
            }
            */
        }

        // private Coroutine _coHitEffect;
        public override void OnDamaged(BaseController attacker, SkillBase skill, int damage)
        {
            // 히트 처리
            // 데미지 폰트 처리

            base.OnDamaged(attacker, skill, damage);

            // if (_coHitEffect != null)
            //     StopCoroutine(_coHitEffect);

            // _coHitEffect = StartCoroutine(CoHitEffect());


            // int rand = Random.Range(0, 3); // 0 ~ 4
            // if (rand < 2)
            //     Managers.Object.ShowDamageNumber(Define.PrefabLabels.DMG_NUMBER_TO_MONSTER, transform.position + (Vector3.up * 1.5f), damage);
            // else
            // {
            //     Managers.Object.ShowDamageNumber(Define.PrefabLabels.DMG_NUMBER_TO_MONSTER_CRITICAL, transform.position + (Vector3.up * 1.5f), damage, null, true);
            //     Managers.Object.ShowDamageText(Define.PrefabLabels.DMG_TEXT_TO_MONSTER_CRITICAL, transform.position + (Vector3.up * 1.5f), "Critical !!");
            // }
        }

        // private IEnumerator CoHitEffect()
        // {
        //     Managers.Effect.StartHitEffect(gameObject);
        //     yield return new WaitForSeconds(0.1f);
        //     Managers.Effect.EndHitEffect(gameObject);
        // }
    }
}

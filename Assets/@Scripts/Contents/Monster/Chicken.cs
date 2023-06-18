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

            // SkillBook.AddSkill<MeleeAttack>(transform.position); // Init에서 먼저 가져와서 null인듯
            return true;
        }

        List<SpriteRenderer> renderers = new List<SpriteRenderer>();
        Material testMat;
        Material defaultMat;
        GameObject body;
        public override void InitMonsterSkill() // 매서드 이름 Late Init으로 바꿔야할지도
        {
            //base.InitMonsterSkill();
            SkillBook.AddSkill<BodyAttack>(this);
            testMat = Resources.Load<Material>("Materials/MatTest");
            defaultMat = Utils.FindChild(gameObject, "Body").GetComponent<SpriteRenderer>().material;

            // fadePropertyID = Shader.PropertyToID("_StrongTintFade");
            fadePropertyID = Shader.PropertyToID("_HologramFade");


            foreach (var sp in GetComponentsInChildren<SpriteRenderer>())
            {
                renderers.Add(sp);
            }
        }

        bool bChange = false;
        int fadePropertyID = -1;
        public float intensity = 0f;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                bChange = !bChange;
            }

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
                {
                    sp.material = defaultMat;
                }
            }
        }
    }
}

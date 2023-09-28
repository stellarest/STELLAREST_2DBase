using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace STELLAREST_2D
{
    // 사실 이거까지 필요한지 모르겠,,, 없어도 되긴 할듯??
    public class ChickenController : MonsterController
    {
        public override void Init(int templateID)
        {
            base.Init(templateID);
            CreatureState = Define.CreatureState.Idle;

            // MonsterType = Define.MonsterType.Chicken;
            
            // ResetCCStates();
            // if (GoCCEffect != null)
            // {
            //     Managers.Resource.Destroy(GoCCEffect);
            //     GoCCEffect = null;
            // }

            // Managers.Sprite.SetMonsterFace(this, Define.MonsterFace.Normal);
            // RigidBody.simulated = true;
            // RigidBody.velocity = Vector2.zero;
            // SkillBook.Stopped = false;
            // BodyCol.isTrigger = false;
            // IsThrowingStarHit = false;
            // IsLazerBoltHit = false;

            // //CreatureType = Define.CreatureType.Monster;
            // if (SkillBook.SequenceSkills.Count != 0)
            //     SkillBook.SequenceSkills[(int)Define.InGameGrade.Normal - 1].gameObject.SetActive(true);
        }

        [ContextMenu("TEST_CC")]
        private void TEST_CC()
        {
            //Managers.CC.ApplyCC(this, Define.CCStatus.Stun, 5f);
        }

        protected override void OnDead()
        {
            //base.OnDead();
        }
    }
}

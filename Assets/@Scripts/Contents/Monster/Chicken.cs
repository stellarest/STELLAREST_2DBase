using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace STELLAREST_2D
{
    public class Chicken : MonsterController
    {
        public override bool Init()
        {
            base.Init();

            CreatureState = Define.CreatureState.Idle;
            ResetCCStates();

            if (GoCCEffect != null)
            {
                Managers.Resource.Destroy(GoCCEffect);
                GoCCEffect = null;
            }

            Managers.Sprite.SetMonsterFace(this, Define.MonsterFace.Normal);
            RigidBody.simulated = true;
            RigidBody.velocity = Vector2.zero;
            SkillBook.Stopped = false;
            BodyCol.isTrigger = false;
            IsThrowingStarHit = false;
            IsLazerBoltHit = false;

            CreatureType = Define.CreatureType.Chicken;

            if (SkillBook.SequenceSkills.Count != 0)
                SkillBook.SequenceSkills[(int)Define.InGameGrade.Normal - 1].gameObject.SetActive(true);

            return true;
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

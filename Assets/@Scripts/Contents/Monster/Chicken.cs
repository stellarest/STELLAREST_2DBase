using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class Chicken : MonsterController
    {
        public override bool Init()
        {
            base.Init();

            CreatureState = Define.CreatureState.Idle;
            Managers.Sprite.SetMonsterFace(this, Define.SpriteLabels.MonsterFace.Normal);
            RigidBody.simulated = true;
            RigidBody.velocity = Vector2.zero;
            SkillBook.Stopped = false;
            BodyCol.isTrigger = false;
            IsThrowingStarBounceHit = false;

            if (SkillBook.SequenceSkills.Count != 0)
                SkillBook.SequenceSkills[(int)Define.InGameGrade.Normal - 1].gameObject.SetActive(true);

            return true;
        }

        protected override void OnDead()
        {
            //base.OnDead();
        }
    }
}

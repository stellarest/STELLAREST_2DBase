using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    // SequenceSkill은 Active 스킬임.
    // 하나의 스킬만 발동 가능
    public abstract class SequenceSkill : SkillBase
    {
        protected PlayerController pc;
        protected PlayerAnimationController pac;

        protected MonsterController mc;
        protected MonsterAnimationController mac;

        protected AnimationCurve _curve;

        private void Start()
        {
            // Sequence 스킬을 사용하는것은 일단 몬스터와 플레이어밖에 없음
            if (Owner.GetComponent<CreatureController>().ObjectType == Define.ObjectType.Monster)
            {
                mc = Owner.GetComponent<MonsterController>();
                mac = mc.MAC;
            }
            else
            {
                pc = Owner.GetComponent<PlayerController>();
                pac = pc.PAC;
            }

            _curve = new AnimationCurve();
        }

        // 하나의 스킬을 쓸때, 다른걸 못쓰니까 Action으로 콜백을 받는 방식으로하면 굉장히 편해질수 있음
        public abstract void DoSkill(System.Action callback = null);
    }
}

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

        private void Start()
        {
            if (Owner?.IsMonster() == false)
            {
                pc = Owner.GetComponent<PlayerController>();
                pac = pc.PlayerAnim;

            }
            else
            {
                mc = Owner.GetComponent<MonsterController>();
                mac = mc.MAC;
            }
        }

        // 하나의 스킬을 쓸때, 다른걸 못쓰니까 Action으로 콜백을 받는 방식으로하면 굉장히 편해질수 있음
        public abstract void DoSkill(System.Action callback = null);
    }
}

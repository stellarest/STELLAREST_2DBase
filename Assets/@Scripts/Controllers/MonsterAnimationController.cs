using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.FantasyMonsters.Scripts;

namespace STELLAREST_2D
{
    public class MonsterAnimationController : AnimationController
    {
        public Monster MonsterBase { get; protected set; }
        private readonly int IDLE = Animator.StringToHash(""); // 보스 전용
        private readonly int WALK = Animator.StringToHash("");

        public override bool Init()
        {
            base.Init();

            MonsterBase = GetComponent<Monster>();

            return true;
        }
    }
}


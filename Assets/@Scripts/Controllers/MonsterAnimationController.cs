using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.FantasyMonsters.Scripts;

namespace STELLAREST_2D
{
    public class MonsterAnimationController : AnimationController
    {
        public Monster MonsterBase { get; protected set; }
        private readonly int IDLE = Animator.StringToHash("Idle");
        private readonly int RUN = Animator.StringToHash("Run");
        private readonly int ATTACK = Animator.StringToHash("Attack");
        private readonly int DEATH = Animator.StringToHash("Death");

        public void Idle() => AnimController.Play(IDLE);
        public void Run() => AnimController.Play(RUN);
        public void Attack() => AnimController.Play(ATTACK);
        public void Death() => AnimController.Play(DEATH);

        public override bool Init()
        {
            base.Init();
            MonsterBase = GetComponent<Monster>();
            return true;
        }
    }
}


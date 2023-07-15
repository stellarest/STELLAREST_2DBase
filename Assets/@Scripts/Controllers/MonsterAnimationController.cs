using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.FantasyMonsters.Scripts;

namespace STELLAREST_2D
{
    public class MonsterAnimationController : AnimationController
    {
        public Monster MonsterBase { get; protected set; }
        private readonly int ATTACK = Animator.StringToHash("Attack");
        private readonly int ACTION = Animator.StringToHash("Action");
        private readonly int STATE = Animator.StringToHash("State");
        private readonly int BODY_SPEED = Animator.StringToHash("BodySpeed");

        private readonly int FACE_DEFAULT = 0;
        private readonly int FACE_ANGRY = 1;
        private readonly int FACE_DEAD = 2;

        public override bool Init()
        {
            base.Init();

            MonsterBase = GetComponent<Monster>();

            return true;
        }

        public override void Idle()
        {
            _animController.SetInteger(STATE, (int)Define.MonsterState.Idle);
        }

        public override void Attack()
        {
            _animController.SetTrigger(ATTACK);
        }

        public override void Run(float speed = 1f)
        {
            _animController.SetFloat(BODY_SPEED, speed);
            _animController.SetInteger(STATE, (int)Define.MonsterState.Run);
        }

        public override void DefaultFace()
        {
            MonsterBase.SetHead(FACE_DEFAULT);
        }

        public override void AngryFace()
        {
            MonsterBase.SetHead(FACE_ANGRY);
        }

        public override void DeadFace()
        {
            MonsterBase.SetHead(FACE_DEAD);
        }
    }
}


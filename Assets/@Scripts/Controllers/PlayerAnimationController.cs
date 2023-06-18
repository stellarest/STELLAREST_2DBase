using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;

namespace STELLAREST_2D
{
    public class PlayerAnimationController : AnimationController
    {
        public Character CharaBase { get; protected set; }

        protected readonly int IDLE = Animator.StringToHash("Idle");
        protected readonly int READY = Animator.StringToHash("Ready");
        protected readonly int RUN = Animator.StringToHash("Run");
        protected readonly int STUN = Animator.StringToHash("Stun");
        protected readonly int SLASH = Animator.StringToHash("Slash");
        protected readonly int JAB = Animator.StringToHash("Jab");
        protected readonly int UPPER_BODY_SPEED = Animator.StringToHash("UpperBodySpeed");
        protected readonly int ATTACK_SPEED = Animator.StringToHash("AttackSpeed");

        public readonly string EXPRESSION_DEFAULT = "Default";
        public readonly string EXPRESSION_ANGRY = "Angry";
        public readonly string EXPRESSION_DEAD = "Dead";
        public readonly string EXPRESSION_STUN = "Stun";

        public override bool Init()
        {
            base.Init();

            CharaBase = GetComponent<Character>();

            return true;
        }

        public override void Idle()
        {
            _animController.Play(IDLE);
        }

        public override void Ready()
        {
            _animController.SetBool(READY, true);
        }

        public override void Run(float speed = 1f)
        {
            _animController.Play(RUN);
        }

        public override void Stun()
        {
            _animController.Play(STUN);
            CharaBase.SetExpression("Stun");
        }

        public override void MeleeSlash(float attackSpeed = 1f)
        {
            _animController.SetFloat(ATTACK_SPEED, attackSpeed);
            _animController.SetTrigger(SLASH);
        }

        public override void MeleeJab(float attackSpeed = 1f)
        {
            _animController.SetFloat(ATTACK_SPEED, attackSpeed);
            _animController.SetTrigger(JAB);
        }
    }
}


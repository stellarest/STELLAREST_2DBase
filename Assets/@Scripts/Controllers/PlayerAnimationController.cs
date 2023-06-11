using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;

namespace STELLAREST_2D
{
    public class PlayerAnimationController : AnimationController
    {
        public Character CharaBase { get; protected set; }

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

        public override void Run()
        {
            _animController.Play(RUN);
        }

        public override void Stun()
        {
            _animController.Play(STUN);
            CharaBase.SetExpression("Stun");
        }

        public override void MeleeSlash(float upperBodySpeed = 1f)
        {
            _animController.SetFloat(UPPER_BODY_SPEED, upperBodySpeed);
            _animController.SetTrigger(SLASH);
        }

        public override void MeleeJab(float upperBodySpeed = 1f)
        {
            _animController.SetFloat(UPPER_BODY_SPEED, upperBodySpeed);
            _animController.SetTrigger(JAB);
        }
    }
}


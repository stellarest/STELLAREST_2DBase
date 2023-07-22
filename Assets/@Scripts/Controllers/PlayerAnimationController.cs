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

        protected readonly int DEATH_BACK = Animator.StringToHash("DeathBack");
        protected readonly int DEATH_FRONT = Animator.StringToHash("DeathFront");
        
        protected readonly int ATTACK_ANIM_SPEED = Animator.StringToHash("AttackAnimSpeed");
        protected readonly int UPPER_BODY_ANIM_SPEED = Animator.StringToHash("UpperBodyAnimSpeed");
        protected readonly int LOWER_BODY_ANIM_SPEED = Animator.StringToHash("LowerBodyAnimSpeed");

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

        public override void Run()
        {
            _animController.SetFloat(LOWER_BODY_ANIM_SPEED, 1f);
            _animController.Play(RUN);
        }

        public override void Stun()
        {
            _animController.Play(STUN);
            CharaBase.SetExpression("Stun");
        }

        public override void Slash1H()
        {
            _animController.SetFloat(ATTACK_ANIM_SPEED, 1f);
            _animController.SetTrigger(SLASH);
        }

        public override void Jab1H()
        {
            _animController.SetFloat(ATTACK_ANIM_SPEED, 1f);
            _animController.SetTrigger(JAB);
        }

        public void DieBack()
        {
            Managers.Sprite.SetPlayerEmotion(Define.PlayerEmotion.Die);
            _animController.Play(DEATH_BACK);
        }

        public void DieFront()
        {
            Managers.Sprite.SetPlayerEmotion(Define.PlayerEmotion.Die);
            _animController.Play(DEATH_FRONT);
        }
    }
}


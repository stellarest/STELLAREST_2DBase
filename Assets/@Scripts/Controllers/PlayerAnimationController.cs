using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;

namespace STELLAREST_2D
{
    public class PlayerAnimationController : AnimationController
    {
        public Character CharaBase { get; protected set; }
        private readonly int READY = Animator.StringToHash("Ready");
        private readonly int IDLE = Animator.StringToHash("Stand");
        private readonly int WALK = Animator.StringToHash("Walk");
        private readonly int RUN = Animator.StringToHash("Run");

        private readonly int SLASH_1H = Animator.StringToHash("SlashMelee1H");
        private readonly int JAB_1H = Animator.StringToHash("JabMelee1H");

        private readonly int SLASH_2H = Animator.StringToHash("SlashMelee2H");
        private readonly int JAB_2H = Animator.StringToHash("JabMelee2H");

        private readonly int SLASH_PAIRED = Animator.StringToHash("SlashMeleePaired");
        private readonly int JAB_PAIRED = Animator.StringToHash("JabMeleePaired");

        private readonly int DEATH_BACK = Animator.StringToHash("DeathBack");
        private readonly int DEATH_FRONT = Animator.StringToHash("DeathFront");
        
        private readonly int ATTACK_ANIM_SPEED = Animator.StringToHash("AttackAnimSpeed");
        private readonly int UPPER_BODY_ANIM_SPEED = Animator.StringToHash("UpperBodyAnimSpeed");
        private readonly int LOWER_BODY_ANIM_SPEED = Animator.StringToHash("LowerBodyAnimSpeed");

        public void ResetAttackAnimSpeed() => AnimController.SetFloat(ATTACK_ANIM_SPEED, 1f);
        public void SetAttackAnimSpeed(float speed) => AnimController.SetFloat(ATTACK_ANIM_SPEED, speed);

        public void ResetUpperBodyAnimSpeed() => AnimController.SetFloat(UPPER_BODY_ANIM_SPEED, 1f);
        public void SetUpperBodyAnimSpeed(float speed) => AnimController.SetFloat(UPPER_BODY_ANIM_SPEED, speed);

        public void ResetLowerBodyAnimSpeed() => AnimController.SetFloat(LOWER_BODY_ANIM_SPEED, 1f);
        public void SetLowerBodyAnimSpeed(float speed) => AnimController.SetFloat(LOWER_BODY_ANIM_SPEED, speed);

        public override bool Init()
        {
            base.Init();
            CharaBase = GetComponent<Character>();
            return true;
        }

        public void Ready() => AnimController.SetBool(READY, true);
        public void Idle() => AnimController.Play(IDLE);
        public void Walk() => AnimController.Play(WALK);
        public void Run() => AnimController.Play(RUN);

        public void Slash1H() => AnimController.Play(SLASH_1H);
        public void Jab1H() => AnimController.Play(JAB_1H);

        public void Slash2H() => AnimController.Play(SLASH_2H);
        public void Jab2H() => AnimController.Play(JAB_2H);

        public void SlashPaired() => AnimController.Play(SLASH_PAIRED);
        public void JabPaired() => AnimController.Play(JAB_PAIRED);
        
        public void DeathBack() => AnimController.Play(DEATH_BACK);
        public void DeathFront() => AnimController.Play(DEATH_FRONT);
    }
}


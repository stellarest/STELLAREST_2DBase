using UnityEngine;

namespace STELLAREST_2D
{
    public class PlayerAnimationController : AnimationController
    {
        public readonly int READY = Animator.StringToHash("Ready");
        public readonly int READY_BOW = Animator.StringToHash("ReadyBow");

        public readonly int IDLE = Animator.StringToHash("Stand");
        public readonly int WALK = Animator.StringToHash("Walk");
        public readonly int RUN = Animator.StringToHash("Run");

        public readonly int SLASH_1H = Animator.StringToHash("SlashMelee1H");
        public readonly int SLASH_2H = Animator.StringToHash("SlashMelee2H");
        public readonly int SLASH_PAIRED = Animator.StringToHash("SlashMeleePaired");
        public readonly int SLASH_DOUBLE = Animator.StringToHash("SlashMeleeDouble");

        public readonly int JAB_1H = Animator.StringToHash("JabMelee1H");
        public readonly int JAB_1H_LEFT = Animator.StringToHash("JabMelee1HLeft");
        public readonly int JAB_2H = Animator.StringToHash("JabMelee2H");
        public readonly int JAB_PAIRED = Animator.StringToHash("JabMeleePaired");

        public readonly int SIMPLE_BOW_SHOT = Animator.StringToHash("SimpleBowShot");
        public readonly int CAST_1H = Animator.StringToHash("Cast1H");

        public readonly int DEATH_BACK = Animator.StringToHash("DeathBack");
        public readonly int DEATH_FRONT = Animator.StringToHash("DeathFront");
        
        public readonly int ATTACK_ANIM_SPEED = Animator.StringToHash("AttackAnimSpeed");
        public readonly int UPPER_BODY_ANIM_SPEED = Animator.StringToHash("UpperBodyAnimSpeed");
        public readonly int LOWER_BODY_ANIM_SPEED = Animator.StringToHash("LowerBodyAnimSpeed");

        public void ResetAttackAnimSpeed() => AnimController.SetFloat(ATTACK_ANIM_SPEED, 1f);
        public void AttackAnimSpeed(float speed) => AnimController.SetFloat(ATTACK_ANIM_SPEED, speed);

        public void ResetUpperBodyAnimSpeed() => AnimController.SetFloat(UPPER_BODY_ANIM_SPEED, 1f);
        public void SetUpperBodyAnimSpeed(float speed) => AnimController.SetFloat(UPPER_BODY_ANIM_SPEED, speed);

        public void ResetLowerBodyAnimSpeed() => AnimController.SetFloat(LOWER_BODY_ANIM_SPEED, 1f);
        public void SetLowerBodyAnimSpeed(float speed) => AnimController.SetFloat(LOWER_BODY_ANIM_SPEED, speed);

        // public void OnReady() => AnimController.SetBool(READY, true);
        // public void OffReady() => AnimController.SetBool(READY, false);

        public void Ready(bool isOn) => AnimController.SetBool(READY, isOn);

        public void Idle() => AnimController.Play(IDLE);
        public void ReadyBow() => AnimController.Play(READY_BOW);

        public void Walk() => AnimController.Play(WALK);
        public void Run() => AnimController.Play(RUN);

        public void Slash1H() => AnimController.Play(SLASH_1H);
        public void Slash2H() => AnimController.Play(SLASH_2H);
        public void SlashPaired() => AnimController.Play(SLASH_PAIRED);
        public void SlashDouble() => AnimController.Play(SLASH_DOUBLE);

        public void Jab1H() => AnimController.Play(JAB_1H);
        public void Jab1HLeft() => AnimController.Play(JAB_1H_LEFT);
        public void Jab2H() => AnimController.Play(JAB_2H);
        public void JabPaired() => AnimController.Play(JAB_PAIRED);

        public void SimpleBowShot() => AnimController.Play(SIMPLE_BOW_SHOT);
        public void Cast1H() => AnimController.Play(CAST_1H);
        public void DeathBack() => AnimController.Play(DEATH_BACK);
        public void DeathFront() => AnimController.Play(DEATH_FRONT);
    }
}


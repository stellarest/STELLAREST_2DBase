using System;
using System.Collections;
using UnityEngine;

using static STELLAREST_2D.Define;
using STELLAREST_2D.Data;

namespace STELLAREST_2D
{
    /*
        [ Ability Info : Cloak (Elite Action, Ninja) ]
        (Currently Temped Set : lv.3)

        lv.1 : 3초 동안 Fade Out, 몬스터 어그로 사라짐. 이속 증가 50%, Fade Out 종료시 Ninja Swing(+50% 데미지, 1초 스턴)    (100% 회피율, 혹시 모를 상황 대비)
        lv.2 : 3초 동안 Fade Out, 몬스터 어그로 사라짐. 이속 증가 50%, Fade Out 종료시 Ninja Swing(+60% 데미지, 1.5초 스턴)  (100% 회피율, 혹시 모를 상황 대비) 
        lv.3 : 3초 동안 Fade Out, 몬스터 어그로 사라짐. 이속 증가 50%, Fade Out 종료시 Ninja Swing(+100% 데미지, 3초 스턴)   (100% 회피율, 혹시 모를 상황 대비)
        은폐에서 풀릴 때 닌자 스윙으로 몬스터 공격. 타격시 2초간 기절.

        또는, 죽음의 위기에서 100%의 확률 -> 85%의 확률 -> 60% -> 45% -> 30% -> 15%(고정)
    */
    public class Cloak : UniqueSkill
    {
        private ParticleSystem[] _burstGroup = null;
        private KennethController _ownerController = null;
        private Transform _parentOrigin = null;

        private const string FIXED_PARENT_TARGET = "@ActionSkills";
        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            _burstGroup = GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            EnableParticles(_burstGroup, false);
            _ownerController = owner.GetComponent<KennethController>();
            _parentOrigin = Utils.FindChild<Transform>(owner.gameObject, FIXED_PARENT_TARGET, true);
        }

        private const float FIXED_AFTER_COMPLETED_WAIT_TIME = 0.3f;
        protected override IEnumerator CoStartSkill()
        {
            // If nija is hitted from monster
            DoSkillJob(null);
            yield break;
        }

        private const float FIXED_ADD_MOVEMENT_RATIO = 0.5F;
        protected override void DoSkillJob(Action callback = null)
        {
            _ownerController.PlayerAnimController.EnterNextState(false);
            this.Owner.SkillBook.Deactivate(FixedValue.TemplateID.Skill.NinjaMastery);
            this.Owner.CreatureRendererController.HideWeapons(true);

            this.Owner.CreatureSkillAnimType = this.Data.AnimationType;
            this.Owner.CreatureState = CreatureState.Skill;
            callback?.Invoke();

            TakeOffParticlesFromParent(_burstGroup);
            EnableParticles(_burstGroup, true);
            StartCoroutine(Managers.VFX.CoMatFadeOutInstantly(this.Owner, this.Data.Duration,
                startCallback: () => StartCloak(),
                endCallback: () => EndCloak()));
        }

        private void StartCloak()
        {
            Managers.Game.OnStopAction?.Invoke(true);
            Managers.Game.OnVFXEnvTarget?.Invoke(VFXEnvType.QuestionMark);

            this.Owner.CreatureRendererController.OnFaceBunny();
            this.Owner.Stat.AddMovementSpeedRatio(FIXED_ADD_MOVEMENT_RATIO);
        }

        private void EndCloak() 
        {
            TakeOnParticlesFromParent(_burstGroup, _parentOrigin);
            _ownerController.PlayerAnimController.EnterNextState(true);
            this.Owner.CreatureRendererController.OnFaceDefaultHandler();
            StartCoroutine(CoNinjaSlash());
        }

        private const float FIXED_NINJA_SLASH_START_DASH_SPEED = 60F;
        private const float FIXED_NINJA_SLASH_DASH_SPEED_LOG_DECAY_FACTOR = 2F;
        private const float FIXED_NINJA_SLASH_MINIMUM_DASH_SPEED = 1F;
        private const float FIXED_NINJA_SLASH_ADDITIONAL_END_TIME = 0.25F;
        private IEnumerator CoNinjaSlash()
        {
            this.Owner.CreatureRendererController.HideWeapons(false);
            //_ownerController.CreatureRendererController.ChangeWeapon(SpriteManager.WeaponType.NinjaSword); 이거 의미가 없는것같기도 하고

            SkillBase ninjaSlash = this.Owner.SkillBook.ForceGetSkillMember(FixedValue.TemplateID.Skill.Ninja_Unique_Elite_C1, 0);
            if (ninjaSlash.IsLearned == false)
            {
                this.Owner.SkillBook.LevelUp(FixedValue.TemplateID.Skill.Ninja_Unique_Elite_C1);
                this.Owner.SkillBook.Activate(FixedValue.TemplateID.Skill.Ninja_Unique_Elite_C1);
            }
            else
                this.Owner.SkillBook.Activate(FixedValue.TemplateID.Skill.Ninja_Unique_Elite_C1);

            this.Owner.RigidBody.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
            this.Owner.RigidBody.constraints &= ~RigidbodyConstraints2D.FreezePositionY;

            this.Owner.RendererController.EnableSpriteTrails(true);
            float dashSpeed = FIXED_NINJA_SLASH_START_DASH_SPEED;
            while (dashSpeed > 0f)
            {
                Vector2 dashDirection = Owner.MoveDir;
                //Vector2 dashDirection = Owner.LastMovementDir; 화면 밖으로 넘어감

                float timeFactor = Mathf.Log(FIXED_NINJA_SLASH_DASH_SPEED_LOG_DECAY_FACTOR + 1) / Time.deltaTime;
                if (dashSpeed <= 1f)
                {
                    this.Owner.RendererController.EnableSpriteTrails(false);
                    dashSpeed = FIXED_NINJA_SLASH_START_DASH_SPEED;
                    Owner.RigidBody.velocity = Vector2.zero;
                    this.Owner.RigidBody.constraints |= RigidbodyConstraints2D.FreezePositionX;
                    this.Owner.RigidBody.constraints |= RigidbodyConstraints2D.FreezePositionY;
                    break;
                }
                else
                    dashSpeed -= timeFactor * Time.deltaTime;

                // 최소 속도 제한 (원하는 값에 따라 조정)
                dashSpeed = Mathf.Max(dashSpeed, FIXED_NINJA_SLASH_MINIMUM_DASH_SPEED);
                Owner.RigidBody.velocity = dashDirection * dashSpeed;
                yield return null;
            }

            yield return new WaitForSeconds(ninjaSlash.Data.Duration + FIXED_NINJA_SLASH_ADDITIONAL_END_TIME);
            this.Owner.Stat.ResetMovementSpeed();
            Managers.Game.OnStopAction?.Invoke(false);

            this.Owner.SkillBook.Deactivate(FixedValue.TemplateID.Skill.Ninja_Unique_Elite_C1);
            this.Owner.SkillBook.Deactivate(FixedValue.TemplateID.Skill.Ninja_Unique_Elite);
            this.Owner.SkillBook.Activate(FixedValue.TemplateID.Skill.NinjaMastery);
        }
    }
}

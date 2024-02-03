using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

using static STELLAREST_2D.Define;

namespace STELLAREST_2D
{
    [System.Serializable]
    public class PlayerBodyParts
    {
        public PlayerBodyParts(Transform hair, Transform armLeft, Transform armRight,
                                Transform handLeft, Transform handRight, Transform legLeft, Transform legRight)
        {
            this.Hair = hair;
            this.ArmLeft = armLeft;
            this.ArmRight = armRight;

            this.HandLeft = handLeft;
            this.HandRight = handRight;

            this.LegLeft = legLeft;
            this.LegRight = legRight;

            if (Hair == null || ArmLeft == null || ArmRight == null || HandLeft == null || HandRight == null || LegLeft == null || LegRight == null)
                Utils.LogCritical(nameof(PlayerController), nameof(PlayerBodyParts));
        }

        #region Public Settings All Player Characters
        [field: SerializeField] public Transform Hair { get; private set; } = null;
        [field: SerializeField] public Transform ArmLeft { get; private set; } = null;
        [field: SerializeField] public Transform ArmRight { get; private set; } = null;

        [field: SerializeField] public Transform HandLeft { get; private set; } = null;
        [field: SerializeField] public Transform HandRight { get; private set; } = null;

        [field: SerializeField] public Transform LegLeft { get; private set; } = null;
        [field: SerializeField] public Transform LegRight { get; private set; } = null;
        #endregion

        #region Settings Manually on Player Characters
        [field: SerializeField] public Transform HandLeft_MeleeWeapon { get; set; } = null;
        [field: SerializeField] public Transform HandRight_MeleeWeapon { get; set; } = null;
        [field: SerializeField] public Transform RangedWeapon { get; set; } = null;
        #endregion
    }

    public class PlayerController : CreatureController
    {
        protected float _armBowFixedAngle = 110f;
        private float _armRifleFixedAngle = 146f;
        [field: SerializeField] public PlayerBodyParts BodyParts { get; protected set; } = null;
        public PlayerAnimationController PlayerAnimController { get; private set; } = null;

        public override bool this[CrowdControlType crowdControlType]
        {
            get => base[crowdControlType];
            set
            {
                base[crowdControlType] = value;
                switch (crowdControlType)
                {
                    case CrowdControlType.Stun:
                        {
                            if (base[crowdControlType])
                            {
                                this.MoveDir = Vector3.zero;
                                SkillBook.DeactivateAll();
                                PlayerAnimController.Stun();
                            }
                            else
                            {
                                if (this.IsDeadState)
                                    return;

                                SkillBook.ActivateAll();
                                //PlayerAnimController.Stand();
                                PlayerAnimController.Idle();
                            }
                        }
                        break;
                }
            }
        }

        // TODO
        public override float SpeedModifier
        {
            get => base.SpeedModifier;
            set
            {
                base.SpeedModifier = value;
                PlayerAnimController.SetAnimationSpeed(base.SpeedModifier);
            }
        }

        public override void ResetSpeedModifier()
        {
            base.ResetSpeedModifier();
            PlayerAnimController.SetAnimationSpeed(base.SpeedModifier);
        }


        private Define.LookAtDirection _prevLookAtDir = Define.LookAtDirection.Right;
        public override void Init(int templateID)
        {
            if (this.IsFirstPooling)
            {
                base.Init(templateID);
                if (PlayerAnimController == null)
                    PlayerAnimController = CreatureAnimController as PlayerAnimationController;

                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerBody);
                AddCallbacks();
                this.IsFirstPooling = false;

#if UNITY_EDITOR
                // ############## TEST ##############
                DebugDrawLines drawLines = gameObject.AddComponent<DebugDrawLines>();
                drawLines.DebugTarget = this.gameObject;
                //PQueueTest();
#endif
            }

            CreatureState = Define.CreatureState.Idle;
            PlayerAnimController.Ready();

            this.LookAtDir = Define.LookAtDirection.Right;
            _prevLookAtDir = this.LookAtDir;
        }

        protected override void InitChild(int templateID)
        {
            base.InitChild(templateID);
            this.BodyParts = new PlayerBodyParts(
                hair: Utils.FindChild<Transform>(this.gameObject, FixedValue.Find.PLAYER_HAIR, recursive: true),
                armLeft: Utils.FindChild<Transform>(this.gameObject, FixedValue.Find.PLAYER_ARM_LEFT, recursive: true),
                armRight: Utils.FindChild<Transform>(this.gameObject, FixedValue.Find.PLAYER_ARM_RIGHT, recursive: true),
                handLeft: Utils.FindChild<Transform>(this.gameObject, FixedValue.Find.PLAYER_HAND_LEFT, recursive: true),
                handRight: Utils.FindChild<Transform>(this.gameObject, FixedValue.Find.PLAYER_HAND_RIGHT, recursive: true),
                legLeft: Utils.FindChild<Transform>(this.gameObject, FixedValue.Find.PLAYER_LEG_LEFT, recursive: true),
                legRight: Utils.FindChild<Transform>(this.gameObject, FixedValue.Find.PLAYER_LEG_RIGHT, recursive: true));

            Center = Utils.FindChild<Transform>(AnimTransform.gameObject, FixedValue.Find.PLAYER_PELVIS, true);
            SetPlayerWeapon(templateID);
        }

        private void SetPlayerWeapon(int templateID)
        {
            switch (templateID)
            {
                case (int)FixedValue.TemplateID.Creatures.Gary_Paladin:
                    this.BodyParts.HandLeft_MeleeWeapon = Utils.FindChild<Transform>(this.BodyParts.HandLeft.gameObject, FixedValue.Find.PLAYER_SHIELD);
                    this.BodyParts.HandRight_MeleeWeapon = Utils.FindChild<Transform>(this.BodyParts.HandRight.gameObject, FixedValue.Find.PLAYER_MELEE_WEAPON);
                    break;

                case (int)FixedValue.TemplateID.Creatures.Gary_Knight:
                case (int)FixedValue.TemplateID.Creatures.Gary_PhantomKnight:
                    this.BodyParts.HandRight_MeleeWeapon = Utils.FindChild<Transform>(this.BodyParts.HandRight.gameObject, FixedValue.Find.PLAYER_MELEE_WEAPON);
                    break;

                case (int)FixedValue.TemplateID.Creatures.Reina_ArrowMaster:
                case (int)FixedValue.TemplateID.Creatures.Reina_ElementalArcher:
                case (int)FixedValue.TemplateID.Creatures.Reina_ForestGuardian:
                    GameObject foreArmL2 = Utils.FindChild(this.BodyParts.ArmLeft.gameObject, FixedValue.Find.PLAYER_FOREARM_LEFT_2, recursive: true);
                    this.BodyParts.RangedWeapon = Utils.FindChild<Transform>(foreArmL2, FixedValue.Find.PLAYER_BOW, recursive: true);
                    break;

                case (int)FixedValue.TemplateID.Creatures.Kenneth_Assassin:
                case (int)FixedValue.TemplateID.Creatures.Kenneth_Ninja:
                    this.BodyParts.HandLeft_MeleeWeapon = Utils.FindChild<Transform>(this.BodyParts.HandLeft.gameObject, FixedValue.Find.PLAYER_MELEE_WEAPON);
                    this.BodyParts.HandRight_MeleeWeapon = Utils.FindChild<Transform>(this.BodyParts.HandRight.gameObject, FixedValue.Find.PLAYER_MELEE_WEAPON);
                    break;
            }
        }

        private void AddCallbacks()
        {
            if (Managers.Game != null)
            {
                Managers.Game.OnMoveDirChanged += OnMoveDirChangedHandler;
                Utils.Log("Add Event : Managers.Game.OnMoveDirChanged += OnMoveDirChangedHandler");

                Managers.Game.OnGameStart += OnGameStartHandler;
                Utils.Log("Add Event : Managers.Game.OnGameStart += OnGameStartHandler");
            }
        }

        private void RemoveCallbacks()
        {
            if (Managers.Game != null)
            {
                Managers.Game.OnMoveDirChanged -= OnMoveDirChangedHandler;
                Utils.Log("Release Event : Managers.Game.OnMoveDirChanged -= OnMoveDirChangedHandler;");

                Managers.Game.OnGameStart -= OnGameStartHandler;
                Utils.Log("Release Event : Managers.Game.OnGameStart -= OnGameStartHandler");
            }
        }

        public void OnMoveDirChangedHandler(Vector3 moveDir)
        {
            if (this.IsDeadState)
                return;

            if (this[CrowdControlType.Stun])
                return;

            this.MoveDir = moveDir;
            if (moveDir == Vector3.zero)
                CreatureState = Define.CreatureState.Idle;
            else
            {
                CreatureState = Define.CreatureState.Run;
                this.LastMovementDir = moveDir;
            }
        }

        public void OnGameStartHandler() => StartCoroutine(CoPlayerGameStart());
        private const float PLAYER_GAME_START_TIME = 1.75f;
        private IEnumerator CoPlayerGameStart()
        {
            yield return new WaitForSeconds(PLAYER_GAME_START_TIME);
            //SkillBook.LevelUp(SkillBook.UniqueMasteryTemplate);
            SkillBook.Activate(SkillBook.UniqueDefaultTemplate);
        }

        [Conditional("UNITY_EDITOR")]
        private void DevSkillFlag(FixedValue.TemplateID.Skill skillTemplateID)
        {
            SkillBook.LevelUp(skillTemplateID);
            SkillBook.Activate(skillTemplateID);

            if (skillTemplateID == SkillBook.UniqueDefaultTemplate)
            {
                SkillBase skill = SkillBook.GetLastLearnedSkillMember(skillTemplateID);
                if (skill.Data.Grade > Define.InGameGrade.Default)
                    this.RendererController.OnRefreshRenderer?.Invoke(skill.Data.Grade);
            }
        }

        public float TESST_ATTACK_SPEED_RATIO = 0f;
        private void Update()
        {
            if (this.IsDeadState)
                return;

#if UNITY_EDITOR
            DEV_CLEAR_LOG();
            if (Input.GetKeyDown(KeyCode.Q))
                DevSkillFlag(SkillBook.UniqueDefaultTemplate);
            if (Input.GetKeyDown(KeyCode.W))
                DevSkillFlag(SkillBook.UniqueEliteTemplate);

            if (Input.GetKeyDown(KeyCode.Alpha1))
                DevSkillFlag(FixedValue.TemplateID.Skill.ThrowingStar);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                DevSkillFlag(FixedValue.TemplateID.Skill.Boomerang);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                DevSkillFlag(FixedValue.TemplateID.Skill.LazerBolt);
            if (Input.GetKeyDown(KeyCode.Alpha4))
                DevSkillFlag(FixedValue.TemplateID.Skill.Spear);
            if (Input.GetKeyDown(KeyCode.Alpha5))
                DevSkillFlag(FixedValue.TemplateID.Skill.BombTrap);

            // Face Test
            // if (Input.GetKeyDown(KeyCode.Alpha8))
            //     this.RendererController.OnFaceDefaultHandler();
            // if (Input.GetKeyDown(KeyCode.Alpha9))
            //     this.RendererController.OnFaceCombatHandler();
            // if (Input.GetKeyDown(KeyCode.Alpha0))
            //     this.RendererController.OnFaceDeadHandler();

            if (Input.GetKeyDown(KeyCode.K))
                SkillBook.ActivateAll();
            if (Input.GetKeyDown(KeyCode.L))
                SkillBook.DeactivateAll();

            if (Input.GetKeyDown(KeyCode.T))
            {
                // this.Stat.OnSetMasteryAttackSpeed?.Invoke(MasteryAtkSpeedTest);
                //this.Stat.OnAddSkillCooldownRatio?.Invoke(FixedValue.TemplateID.Skill.Paladin_Unique_Mastery, 0f);

                this.Stat.AddMasteryAttackSpeedRatio = TESST_ATTACK_SPEED_RATIO;
                
                // CreatureRendererController.OnFaceBunny();
                // this.CreatureRendererController.HideWeapons(testHideWeapon);
                // testHideWeapon = !testHideWeapon;
                // CreatureRendererController.ChangeWeapon(SpriteManager.WeaponType.NinjaSword);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                // this.Stat.OnResetSkillCooldown?.Invoke(TestSkillTemplate);
                // Utils.Log($"Current Mastery : {this.SkillBook.GetCurrentMasterySkill().Data.Name}");
            }
#endif
            MoveByJoystick();
            CollectGems();
            // foreach (var mon in Utils.GetMonstersInRange(this, 11f))
            //     Debug.DrawLine(this.Center.position, mon.Center.position, Color.magenta, -1f);
        }

        protected override void UpdateAnimation()
        {
            switch (CreatureState)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;

                case CreatureState.Run:
                    UpdateRun();
                    break;

                case CreatureState.Skill:
                    UpdateSkill();
                    break;

                case CreatureState.Dead:
                    OnDead();
                    break;
            }
        }

        protected override void UpdateIdle() => PlayerAnimController.Idle();
        protected override void UpdateRun() => PlayerAnimController.Run();
        protected override void UpdateSkill() => PlayerAnimController.Skill(this.CreatureSkillAnimType);

        private void MoveByJoystick()
        {
            Vector3 dir = MoveDir.normalized * (Stat.MovementSpeed * this.SpeedModifier) * Time.deltaTime;
            if (this[CrowdControlType.KnockBack] == false)
                transform.position += dir;

            if (IsMoving)
            {
                float degree = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;
                Indicator.localRotation = Quaternion.Euler(0, 0, degree);

                Turn(degree);
                Managers.Stage.SetInLimitPos(this);
            }
        }

        private void Turn(float angle)
        {
            if (Mathf.Sign(angle) < 0)
            {
                LookAtDir = Define.LookAtDirection.Right;
                if (_prevLookAtDir != LookAtDir)
                {
                    this.OnLookAtDirChanged?.Invoke(this.LookAtDir);
                    _prevLookAtDir = LookAtDir;
                }
                _armBowFixedAngle = 110f;
            }
            else
            {
                LookAtDir = Define.LookAtDirection.Left;
                if (_prevLookAtDir != LookAtDir)
                {
                    this.OnLookAtDirChanged?.Invoke(this.LookAtDir);
                    _prevLookAtDir = LookAtDir;
                }
                _armBowFixedAngle = -110f;
            }

            LocalScale = new Vector3((int)LookAtDir * FixedValue.Numeric.PLAYER_LOCAL_SCALE_X * -1f, FixedValue.Numeric.PLAYER_LOCAL_SCALE_Y, 1);
        }
        private void CollectGems()
        {
            float sqrCollectDist = Stat.CollectRange * Stat.CollectRange;
            // var allSpawnedGems = Managers.Object.Gems.ToList();
            //var findGems = Managers.Object.GridController.GatherObjects(transform.position, GEM_COLLECTION_FIXED_DIST).ToList();
            var findGems = Managers.Object.GridController.Gather(Define.ObjectType.Gem, this.Center.position, FixedValue.Numeric.PLAYER_MINIMUM_ENV_COLLECT_RANGE).ToList();
            foreach (var gem in findGems)
            {
                GemController gc = gem.GetComponent<GemController>();
                Vector3 dir = gem.transform.position - transform.position;
                if (dir.sqrMagnitude <= sqrCollectDist)
                {
                    gc.GetGem();
                }
            }
            //Debug.Log($"Find Gem : {findGems.Count} / Total Gem : {allSpawnedGems.Count}");
        }

        public override Vector3 LoadVFXEnvSpawnScale(VFXEnvType vfxEnvType)
        {
            switch (vfxEnvType)
            {
                case VFXEnvType.Skull:
                    return Vector3.one * 2f;

                case VFXEnvType.Stun:
                    return Vector3.one * 2.5f;

                case VFXEnvType.Slow:
                    return new Vector3(2.25f, 1f, 1f);

                case VFXEnvType.Silence:
                    return Vector3.one * 1.25f;

                default:
                    return base.LoadVFXEnvSpawnScale(vfxEnvType);
            }
        }

        public override Vector3 LoadVFXEnvSpawnPos(VFXEnvType vfxEnvType)
        {
            switch (vfxEnvType)
            {
                case VFXEnvType.Spawn:
                    return (transform.position + (Vector3.up * 2.5f));

                case VFXEnvType.Damage:
                    return (transform.position + (Vector3.up * 2.5f));

                case VFXEnvType.Dodge:
                    return (transform.position + (Vector3.up * 2.95f));

                case VFXEnvType.Dust:
                    return this.BodyParts.LegRight.position + (Vector3.down * 0.35f);

                case VFXEnvType.Stun:
                    return (transform.position + (Vector3.up * 1.83f));

                case VFXEnvType.Slow:
                    return (transform.position + new Vector3(0f, -1.25f, 0f));

                case VFXEnvType.Silence:
                    return (Center.position + new Vector3(-1.65f, 2.55f, 0f));

                case VFXEnvType.Poison:
                    return (transform.position + (Vector3.up * 2.5f));

                default:
                    return base.LoadVFXEnvSpawnPos(vfxEnvType);
            }
        }

        protected override void SetSortingOrder()
            => GetComponent<SortingGroup>().sortingOrder = (int)Define.SortingOrder.Player;

        public override void OnDamaged(CreatureController attacker, SkillBase from)
        {
            base.OnDamaged(attacker, from);
        }

        protected override void OnDead()
        {
            base.OnDead();
            PlayerAnimController.Dead();
            this.RendererController.OnFaceDeadHandler();
            Managers.Game.OnPlayerIsDead?.Invoke();
#if UNITY_EDITOR
            IsStillDeadEyes();
#endif
        }
        private void OnDestroy()
            => RemoveCallbacks();

#if UNITY_EDITOR
        [ContextMenu("UNITY_EDITOR")]
        private void DEV_CLEAR_LOG()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Utils.ClearLog();
        }

        [ContextMenu("UNITY_EDITOR")]
        private void PQueueTest()
        {
            PriorityQueue<int> pQueueTest = new PriorityQueue<int>();
             Utils.Log("<color=cyan>=== ENQUEUE ===</color>");
            pQueueTest.Enqueue(13);
            pQueueTest.Enqueue(23);
            pQueueTest.Enqueue(10);
            pQueueTest.Enqueue(8);
            pQueueTest.Enqueue(9);
            pQueueTest.Enqueue(5);
            pQueueTest.Enqueue(3);
            pQueueTest.CheckValue();

            var ret = pQueueTest.Dequeue();
            Utils.Log($"<color=cyan>RET HEAD(DEQUEUE) : {ret}</color>");
            pQueueTest.CheckValue();

            ret = pQueueTest.Dequeue();
            Utils.Log($"<color=cyan>RET HEAD(DEQUEUE) : {ret}</color>");
            pQueueTest.CheckValue();

            ret = pQueueTest.Dequeue();
            Utils.Log($"<color=cyan>RET HEAD(DEQUEUE) : {ret}</color>");
            pQueueTest.CheckValue();

            ret = pQueueTest.Dequeue();
            Utils.Log($"<color=cyan>RET HEAD(DEQUEUE) : {ret}</color>");
            pQueueTest.CheckValue();

            ret = pQueueTest.Dequeue();
            Utils.Log($"<color=cyan>RET HEAD(DEQUEUE) : {ret}</color>");
            pQueueTest.CheckValue();

            ret = pQueueTest.Dequeue();
            Utils.Log($"<color=cyan>RET HEAD(DEQUEUE) : {ret}</color>");
            pQueueTest.CheckValue();

            ret = pQueueTest.Dequeue();
            Utils.Log($"<color=cyan>RET HEAD(DEQUEUE) : {ret}</color>");
            pQueueTest.CheckValue();

            ret = pQueueTest.Dequeue(); // FAILED
            Utils.Break();
        }

        private void IsStillDeadEyes() => StartCoroutine(CoCheckDeadEyes());
        private IEnumerator CoCheckDeadEyes()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 3f));
            // if (this.RendererController.IsPlayerDeadEyes() == false)
            //     Utils.LogStrong(nameof(PlayerController), nameof(CoCheckDeadEyes), $"Player Eyes is not dead eyes.", true);
            // else
            //     Utils.Log("### STILL DEAD EYES ###");
        }
#endif
    }
}

// -------------------------------------------------------------------------------------
// -------------------------------------------------------------------------------------
// this.BodyParts.HandLeft_MeleeWeapon = Utils.PlayerBodyPartsFinder.Find(this.BodyParts.HandLeft.gameObject, Utils.PlayerBodyPartsFinder.MELEE_WEAPON);
// this.BodyParts.HandRight_MeleeWeapon = Utils.PlayerBodyPartsFinder.Find(this.BodyParts.HandRight.gameObject, Utils.PlayerBodyPartsFinder.MELEE_WEAPON);

// #if UNITY_EDITOR
//             if (this.BodyParts.Hair == null || this.BodyParts.ArmLeft == null || this.BodyParts.ArmRight == null ||
//                 this.BodyParts.HandLeft == null || this.BodyParts.HandRight == null || this.BodyParts.LegLeft == null || this.BodyParts.LegRight == null ||
//                 this.BodyParts.HandLeft_MeleeWeapon == null || this.BodyParts.HandRight_MeleeWeapon == null)
//                 Utils.LogCritical(nameof(PlayerController), nameof(InitChildObject), "Failed to init Body Parts.");
// #endif

// private void LateUpdate()
// {
//     // if (Managers.Game.IsGameStart)
//     // {
//     //     switch (CreatureStat.TemplateID)
//     //     {
//     //         case (int)Define.TemplateIDs.Creatures.Player.Reina_ArrowMaster:
//     //         case (int)Define.TemplateIDs.Creatures.Player.Reina_ElementalArcher:
//     //         case (int)Define.TemplateIDs.Creatures.Player.Reina_ForestWarden:
//     //             {
//     //                 float modifiedAngle = (Indicator.eulerAngles.z + _armBowFixedAngle);
//     //                 if (LocalScale.x < 0)
//     //                     modifiedAngle = 360f - modifiedAngle;

//     //                 //ArmL.transform.localRotation = Quaternion.Euler(0, 0, modifiedAngle);
//     //                 BodyParts.ArmLeft.localRotation = Quaternion.Euler(0, 0, modifiedAngle);
//     //             }
//     //             break;


//     //         case (int)Define.TemplateIDs.Creatures.Player.Christian_Hunter:
//     //         case (int)Define.TemplateIDs.Creatures.Player.Christian_Desperado:
//     //         case (int)Define.TemplateIDs.Creatures.Player.Christian_Destroyer:
//     //             {
//     //                 float modifiedAngle = (Indicator.eulerAngles.z + _armRifleFixedAngle);
//     //                 if (LocalScale.x < 0)
//     //                 {
//     //                     modifiedAngle = 360f - modifiedAngle - 65f;
//     //                     //ArmR.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Clamp(modifiedAngle, 15f, 91f));
//     //                     BodyParts.ArmRight.localRotation = Quaternion.Euler(0, 0, Mathf.Clamp(modifiedAngle, 15f, 91f));
//     //                 }
//     //                 else
//     //                 {
//     //                     //ArmR.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Clamp(modifiedAngle, 378f, 450f));
//     //                     BodyParts.ArmRight.localRotation = Quaternion.Euler(0, 0, Mathf.Clamp(modifiedAngle, 378f, 450f));
//     //                 }
//     //             }
//     //             break;
//     //     }
//     // }
// }

// private void InGameLimitPos(Vector3 position)
// {
//     // Min
//     if (position.x <= Managers.Stage.LeftBottom.x)
//         transform.position = new Vector2(Managers.Stage.LeftBottom.x, transform.position.y);
//     if (position.y <= Managers.Stage.LeftBottom.y)
//         transform.position = new Vector2(transform.position.x, Managers.Stage.LeftBottom.y);

//     // Max
//     if (position.x >= Managers.Stage.RightTop.x)
//         transform.position = new Vector2(Managers.Stage.RightTop.x, transform.position.y);
//     if (position.y >= Managers.Stage.RightTop.y)
//         transform.position = new Vector2(transform.position.x, Managers.Stage.RightTop.y);
// }

// public bool IsInLimitPos()
// {
//     if (Mathf.Abs(transform.position.x - Managers.Stage.LeftBottom.x) < Mathf.Epsilon ||
//         Mathf.Abs(transform.position.y - Managers.Stage.LeftBottom.y) < Mathf.Epsilon ||
//         Mathf.Abs(transform.position.x - Managers.Stage.RightTop.x) < Mathf.Epsilon ||
//         Mathf.Abs(transform.position.y - Managers.Stage.RightTop.y) < Mathf.Epsilon)
//         return true;
//     else
//         return false;
// }

// private IEnumerator CoIsIdle()
// {
//     while (Anima)
// }

// private IEnumerator CoChangePlayerAppearance(SkillBase newSkill)
// {
//     SkillBook.StopSkills(); // 이걸로하면 나중에 Sequence Skill이 Active가 안될텐뎅
//     // StopPlayerDefaultSkill로 바꿔야함.
//     PlayerAnim.Ready(false);

//     // +++ TEMP +++
//     // if (IsChristian(CreatureStat.TemplateID) == false)
//     // {
//     //     while (PlayerAnim.AnimController.GetCurrentAnimatorStateInfo(0).IsName("IdleMelee") == false)
//     //     {
//     //         // 이 while 문은 공격 중일때 들어오는 부분인데, 현재 아직 공격중인 크리스티앙 애니메이션이 없음.
//     //         yield return null;
//     //     }
//     // }

//     // +++ ENABLE ASSASSIN DOUBLE WEAPON +++
//     // if (IsAssassin(newSkill.SkillData.OriginTemplateID) && newSkill.SkillData.InGameGrade == Define.InGameGrade.Legendary)
//     //     LeftHandMeleeWeapon.GetComponent<SpriteRenderer>().enabled = true;

//     // // +++ FIND OTHER HAND REINA BOW +++
//     // if (IsReina(CreatureData.TemplateID) || IsChristian(CreatureData.TemplateID))
//     //     Managers.Sprite.UpgradePlayerAppearance(this, newSkill.SkillData.InGameGrade, true);
//     // else
//     //     Managers.Sprite.UpgradePlayerAppearance(this, newSkill.SkillData.InGameGrade, false);

//     // // +++ ADJUST THIEF HAIR MASK +++
//     // if (IsThief(newSkill.SkillData.OriginTemplateID) && newSkill.SkillData.InGameGrade > Define.InGameGrade.Rare)
//     //     Hair.GetComponent<SpriteMask>().isCustomRangeActive = false;

//     Managers.Sprite.PlayerExpressionController.UpdateDefaultFace(this);

//     PlayerAnim.Ready(true);
//     newSkill.Activate();
// }

// switch (CreatureState)
// {
//     case Define.CreatureState.Idle:
//         {
//             PlayerAnim.Idle();
//         }
//         break;

//     case Define.CreatureState.Walk:
//         {
//             PlayerAnim.Walk();
//         }
//         break;

//     case Define.CreatureState.Run:
//         {
//             PlayerAnim.Run();
//         }
//         break;

//     case Define.CreatureState.Attack:
//         {
//             AttackStartPoint = transform.position;
//             PlayerAnim.Slash1H();

//             // 애초에 Weapon Type을 받아와서 재생하는게 더 깔끔할수도 있음.
//             // 근데 이것도 ㄱㅊ
//             // switch (CreatureData.TemplateID)
//             // {
//             //     case (int)Define.TemplateIDs.Creatures.Player.Gary_Paladin:
//             //         {
//             //             PAC.Slash1H();
//             //         }
//             //         break;
//             //     case (int)Define.TemplateIDs.Creatures.Player.Gary_Knight:
//             //         {
//             //             PAC.Slash2H();
//             //         }
//             //         break;
//             //     case (int)Define.TemplateIDs.Creatures.Player.Gary_PhantomKnight:
//             //         {
//             //             PAC.Slash1H();
//             //         }
//             //         break;

//             //     case (int)Define.TemplateIDs.Creatures.Player.Reina_ArrowMaster:
//             //         {
//             //             // SkillData skillData = SkillBook.GetCurrentPlayerDefaultSkill.SkillData;
//             //             // if (skillData.InGameGrade >= Define.InGameGrade.Epic)
//             //             //     PAC.AttackAnimSpeed(ATTACK_SPEED_TEST);
//             //             PAC.SimpleBowShot();
//             //         }
//             //         break;
//             //     case (int)Define.TemplateIDs.Creatures.Player.Reina_ElementalArcher:
//             //         {
//             //             PAC.SimpleBowShot();
//             //         }
//             //         break;
//             //     case (int)Define.TemplateIDs.Creatures.Player.Reina_ForestWarden:
//             //         {
//             //             RepeatSkillData skillData = SkillBook.GetCurrentPlayerDefaultSkill.SkillData;
//             //             if (skillData.InGameGrade >= Define.InGameGrade.Epic)
//             //                 PAC.AttackAnimSpeed(1.3f);
//             //             PAC.SimpleBowShot();
//             //         }
//             //         break;

//             //     case (int)Define.TemplateIDs.Creatures.Player.Kenneth_Assassin:
//             //         {
//             //             RepeatSkillData skillData = SkillBook.GetCurrentPlayerDefaultSkill.SkillData;

//             //             // Bonus Stat으로 바꾸기.
//             //             //float animSpeed = skillData.AnimationSpeed;
//             //             //PAC.AttackAnimSpeed(animSpeed);
//             //             if (skillData.InGameGrade < Define.InGameGrade.Legendary)
//             //                 PAC.Jab1H();
//             //             else
//             //                 PAC.JabPaired();
//             //         }
//             //         break;
//             //     case (int)Define.TemplateIDs.Creatures.Player.Kenneth_Thief:
//             //         {
//             //             RepeatSkillData skillData = SkillBook.GetCurrentPlayerDefaultSkill.SkillData;
//             //             if (skillData.InGameGrade > Define.InGameGrade.Epic)
//             //                 PAC.SlashDouble();
//             //             else
//             //                 PAC.SlashPaired();
//             //         }
//             //         break;

//             //     case (int)Define.TemplateIDs.Creatures.Player.Lionel_Warrior:
//             //         {
//             //             PAC.Jab2H();
//             //         }
//             //         break;
//             //     case (int)Define.TemplateIDs.Creatures.Player.Lionel_Berserker:
//             //         {
//             //             PAC.SlashPaired();
//             //         }
//             //         break;

//             //     case (int)Define.TemplateIDs.Creatures.Player.Stigma_SkeletonKing:
//             //         {
//             //             PAC.Slash2H();
//             //         }
//             //         break;
//             //     case (int)Define.TemplateIDs.Creatures.Player.Stigma_Pirate:
//             //         {
//             //             RepeatSkillData skillData = SkillBook.GetCurrentPlayerDefaultSkill.SkillData;
//             //             if (skillData.InGameGrade < Define.InGameGrade.Legendary)
//             //                 PAC.Jab1HLeft();
//             //             else
//             //                 PAC.SlashPaired();
//             //         }
//             //         break;

//             //     case (int)Define.TemplateIDs.Creatures.Player.Eleanor_Queen:
//             //         {
//             //             PAC.Slash1H();
//             //         }
//             //         break;
//             // }

//             //StartAttackPos = transform.position;
//         }
//         break;

//     case Define.CreatureState.Death:
//         {
//             //Utils.LogStrong("### INVINCIBLA PLAYER NOW ###");
//         }
//         break;
// }

/*
Paladin
> Shield
> Heaven's of judgement

Knight : Second Wind
> Second Wind
> 

Phantom Knight
> Soul Eater
> King of Darkness

Arrow Master
> Leg Shot
> Arrow Time

Elemental Archer
> Elemental Spikes
> Elemental Charge

Forest Guardian
> ???
> Summon : Black Panther

Assassin
> Shadow Step
> ???

Thief
> Let's Sweep
> Lucky Strike

Ninja 
> Ninja Slash
> Cloned Technique


            캐릭터 별로, 하는것이 좋을 것 같음.
            Gary (캐릭터 공용 스킬)
            - Endurance
            > Gary가 매 웨이브를 클리어할 때 마다 방어력 1% 증가 (최대 + 10%) (O)
            > Gary가 매 웨이브를 클리어할 때 마다 방어력 1% 증가 (최대 + 20%) (V)
            > Gary가 매 웨이브를 클리어할 때 마다 방어력 2% 증가 (최대 + 20%) (V)
            > Gary가 매 웨이브를 클리어할 때 마다 방어력 2% 증가 (최대 + 40%) (V)

            +++++ 사실, 게임 자체는 이게 끝임 +++++
            웨이브당 시간 : 30초 ~ 100초
            Normal (20 waves) - Forest
            - Clear Gary : Unlock Knight
            Normal (Master) (30 waves) - Volcano
            - Clear Gary : The load of swordman + lv.2

            Hard (20 waves) - Forest
            - Clear Gary : Unlock Phantom Knight
            Hard (Master) (30 waves) - Volcano
            - Clear Gary : The load of swordman + lv.3

            Expert (20 waves) - Forest
            - Clear Gary : 모든 캐릭터에게 전용 스킬 전승("수호자의 방패", "재정비", "차가운 심장")
            * 동시에 여러개 배울 수 있음. 근데 비용이 가능할까? 캐릭터 전용 스킬은 매우 비쌈.
            * 자신의 스킬은 원래부터 똑같은 가격, 그러나 다른 캐릭터의 스킬 가격은 100% 증가된 가격으로 구매 가능.
            * 모든 캐릭터에게 전승되는 것이므로 모든 캐릭터가 사용할 수 있는 스킬인지 확인

            Exper (Master) (30 waves) - Volcano
            - Clear Gary : The load of swordman + lv.4

            Extreme (Endless Mode) - 일반, Master 난이도를 모두 클리어시 활성화되는 최종 난이도
            - Forest, Volcano 중에서 선택 (Boss Arena On / Off)

            Paladin Mastery
            > 방어력 +20%에서 게임 시작 (O)
            > 검기 1개 추가 (V)
            > "수호자의 쉴드" 활성화 (V)
            > 얼티밋 팔라딘, "하늘의 심판" 궁극기 활성화 (V)

            Paladin Mastery + Lv.2
            > 방어력 +20%, 최대 체력 +20%에서 게임 시작 (O)
            > 검기 1개 추가 (O)
            > "수호자의 쉴드" 활성화 (V)
            > 얼티밋 팔라딘, "하늘의 심판" 궁극기 활성화 (V)

            Paladin Mastery + Lv.3
            > 방어력 +10, 최대 체력 +20%에서 게임 시작 (O)
            > 검기 1개 추가 (O)
            > "수호자의 쉴드" 활성화 (O)
            > 얼티밋 팔라딘, "하늘의 심판" 궁극기 활성화 (V)
    
            Paladin Mastery + Lv.4
            > 방어력 +10, 최대 체력 +20%에서 게임 시작 (O)
            > 검기 1개 추가 (O)
            > "수호자의 쉴드" 활성화 (O)
            > 얼티밋 팔라딘, "하늘의 심판" 궁극기 활성화 (O)

            * 디스플레이 명칭 "캐릭터 공용 스킬" : 인내심
            * 디스플레이 명칭 "캐릭터 전용 스킬" : 수호자의 쉴드
            * 디스플레이 명칭 "캐릭터 전용 궁극기 스킬" : 하늘의 심판

            Knight Mastery
            > 나이트의 기본 공격은 몬스터의 방어력을 무시
            > 검기의 크기 증가
            > "재정비" 활성화 (V)
            > 얼티밋 나이트, "슬래셔" 궁극기 활성화 (V)

            Phantom Knight Mastery
            > 3번 히트시 불안정한 불안정한 추가 데미지(1 ~ 99)
            > 불안정한 검기의 모양
            > "차가운 심장" 활성화 (V)
            > 얼티밋 팬텀 나이트, "공포의 군주" 궁극기 활성화 (V)

            Reina (캐릭터 공용 스킬)
            > 매 웨이브를 클리어할 때 마다 기본 스킬 데미지 1% 증가 (최대 + 10%) (V)
            > 매 웨이브를 클리어할 때 마다 기본 스킬 데미지 1% 증가 (최대 + 20%) (V)
            > 매 웨이브를 클리어할 때 마다 기본 스킬 데미지 2% 증가 (최대 + 20%) (V)
            > 매 웨이브를 클리어할 때 마다 기본 스킬 데미지 2% 증가 (최대 + 40%) (V)

            Arrow Master Mastery
            > 화살이 가장 가까운 몬스터에게 오토 에이밍
            > 화살의 개수 추가
            > "집중" 활성화 (V)
            > Ultimate Arrow Master, "연발 사격" 궁극기 활성화 (V)

            Elemental Archer Mastery
            > 5초 마다 몬스터를 넉백하는 화살을 발사
            > 검기 1개 추가 (V)
            > "수호자의 쉴드" 활성화 (V)
            > 얼티밋 팔라딘, "하늘의 심판" 궁극기 활성화 (V)

            Elemental Archer Mastery
            > 최대 체력 +20%, 방어력 +10%에서 게임 시작 (O)
            > 검기 1개 추가 (V)
            > "수호자의 쉴드" 활성화 (V)
            > 얼티밋 팔라딘, "하늘의 심판" 궁극기 활성화 (V)

            Kenneth (캐릭터 공용 스킬)
            > 매 웨이브를 클리어할 때 마다 기본 공격 속도 1% 증가 (최대 + 10%) (O)
            > 매 웨이브를 클리어할 때 마다 기본 공격 속도 1% 증가 (최대 + 20%) (V)
            > 매 웨이브를 클리어할 때 마다 기본 공격 속도 2% 증가 (최대 + 20%) (V)
            > 매 웨이브를 클리어할 때 마다 기본 공격 속도 2% 증가 (최대 + 40%) (V)


            * 마스터리 스킬은 레벨업 할 때 마다 외형이 변경됨

            Paladin Mastery
            > 방어력 +20%에서 게임 시작 (O)
            > 검기 1개 추가 (V)
            > "Guardian's Shield"
            > 얼티밋 팔라딘, "하늘의 심판" 궁극기 활성화 (V)

            Knight Mastery
            > 나이트의 기본 공격은 몬스터의 방어력을 무시
            > 검기의 크기 증가
            > "Second Wind"
            > 얼티밋 나이트, "슬래셔" 궁극기 활성화 (V)

            Phantom Knight Mastery
            > 랜덤한 쿨타임(5초 ~ 20초)에 불안정한 추가 데미지를 입힘(1 ~ 99)
            > 검기의 모양이 불안정하게 변경(최대 3회 추가 타격)
            > "Soul Eater" 적을 처치하면 최대 체력의 1~3% 회복 (랜덤) / 1% ~ 2% (60%), 2% ~ 3% (40%)
            >>> "Phantom Blade" 매 쿨타임, 지속시간마다 랜덤한 확률의 추가 데미지 적용
            >>> 아니면 때릴때마다 소울을 흡수하고 이 소울이 20개 이상 모이면 검이 반짝거림
            >>> 이때 지속시간동안 적을 처치하면 적 하나당 1~3%의 체력 흡수
            >>> 이상태에서 적을 처치하면 소울이 적 뒤로 갔다가 랜덤한 JumpDir 방향으로 플레이에어게 접근
            >>> 매우 빠르게 접근해야 되려나

            > 얼티밋 팬텀 나이트, "공포의 군주" 궁극기 활성화 (V)

            Arrow Master Mastery
            > 화살이 가장 가까운 몬스터에게 오토 에이밍
            > 화살의 개수 추가
            > "Concentration"
            > Ultimate Arrow Master, "연발 사격" 궁극기 활성화 (V)

            Elemental Archer Mastery
            > 몬스터를 넉백하는 화살을 발사
            > 넉백 거리 증가
            > ""
            > Ultimate Elemental Archer, "Elemental Arrow" 궁극기 활성화 (V), Elemental Trail + Elemental Explosion

            "Raining Cloud" : 맵 전체에 비가 내림. 스폰된 전체 몬스터의 방어력 -10% 감소 (Archmage)

            Forest Warden Mastery
            > 10%확률로 적을 2초간 기절시키는 깃털 화살 발사
            > "Shield of leaves"
            > Ultimate Forest Warden, "Summon Black Phanther" 활성화

            Assassin Mastery
            > Dodge +20%
            > 찌르기 공격 1회 추가
            > "Shadow Step"
            > Ultimate Assassin, "Shadow Strike" 궁극기 활성화 (V)
            * Shadow Strike : 하나의 적에게 근접했을 때 다양한 각도로 여러번 텔포 타면서 빠르게 근접 공격. 하나의 대상만 여러번 공격을 가함.

            Thief Mastery
            > Luck +30%
            > 검기 1회 추가
            > "Let's Sweep" : 웨이브 종료시 못잡은 몬스터 한마리당 1GEM 획득
            > Ultimate Thief, "Plunder" 궁극기 활성화 (V)

            Ninja Mastery
            > MoveSpeed +50%
            > 표창 2개 추가
            > "Shadow Clone Technique" : 체력이 50% 이하일 때 발동, 20%의 확률로 몬스터가 밀집되어 있지 않은 지역으로 텔레포트, 쿨타임 30초
            > Ultimate Thief, "질주" 궁극기 활성화 (V)

            Pirate Mastery
            > 타격을 1회씩 할 때 마다 몬스터에게 저주를 걸 확률이 1% 증가(성공시 확률 초기화)
            > 검기의 모양 변경
            > "Plunder" 활성화
            > Ultimate Pirate, "Pirate's Bomb Cannon" 궁극기 활성화 (V)

            Paladin - "Guardian's Sheild"
            Knight - "BUCKLE UP" (재정비)
            Phantom Knight - "Soul Eater" 적을 처치하면 최대 체력의 1~3% 회복 (랜덤) /  1% ~ 2% (60%), 2% ~ 3% (40%)

            Arrow Master - "Concentration"
            Elemental Archer - "Rainning Cloud"
            Forest Warden - "Shield of leaves"

            Assassin - "Shadow Step" (던파 소울브링어 잠깐씩 반짝거리는거처럼 아주 잠깐 공격을 회피)
            Thief - "Let's Sweep"
            Ninja - "Shadow Clone Technique"
            Ninja Rare Mastery : 가까이 다가오는 적에게 검기를 날림
            Mastery Level 마다 스킬의 네이밍 디스플레이가 있어야할 것 같음
            (ex) Ninja Master + Lv.2 : Ninja Slash
            WindlinesStormy

            Warrior - "Wind Blade" (SwordWhirlwindBlue)
            Barbarian - "Wild Stance"
            Berserker - "Revenger"

            Hunter - "Head Shot" (1% 확률로 즉사, 보스에게 +500%의 데미지, 쿨타임 20초)
            Desperado - "Technical Dexterity"
            Destroyer - "Rocket Explosion"

            Archmage - "Raining Cloud"  
            Trickster - "Magical Hit" (Impact_Cartoon_Hit_V1 ~ V3 - 전용 스킬, Impact_Cartoon_Hit_V4, V5 - 궁)
            Frost Weaver - "Frozen Heart"

            SkeletonKing - "Summon : Skeleton Warriors" 
            Pirate - "Dark Smoke" (자신의 모습을 5초간 감춤, 쿨타임 30초) (CFXR4 Explosion Purple (HDR) + Dark Smoke)
            Mutant - "Zombie Virus" (Venom Explosion)

            Queen - "For the queen" (고위 기사 3명 소환)
            // WindlinesStormy : 보스 스킬로 하면 될듯
*/


// Pref MoveByJoystick
// Get Degrees = 180f / PI = Rad2Deg
// if (_moveDir != Vector2.zero)
//     _indicator.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * 180f / Mathf.PI);
// if (MoveDir != Vector3.zero)
// {
//     float degree = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;
//     //_indicator.eulerAngles = new Vector3(0, 0, degree);
//     //_indicator.rotation = Quaternion.Euler(0, 0, degree);
//     Indicator.localRotation = Quaternion.Euler(0, 0, degree);
//     Turn(degree);
//     Managers.Stage.SetInLimitPos(this);
// }

//SkillBook.Activate(SkillBook.DefaultRepeatSkillType);
//RendererController.Reset();

// SpriteRenderer[] SPRs = RendererController.GetSpriteRenderers(this, Grade);
// foreach (var spr in SPRs)
// {
//     Utils.Log("ROOT : " + spr.transform.root.name);
//     Debug.Log(spr.gameObject.name);
// }
// Utils.Log("=========================");

// Managers.Pool.ResetPools();
// 바뀌는거 확인했었음

// // PALADIN NORMAL LENGTH : 81
// SpriteRenderer[] currentSPRs = RendererController.GetSpriteRenderers(this, Define.InGameGrade.Normal);
// // PALADIN RARE, EPIC, LEGENDARY : 80
// // 스프라이트 바꿀 때 부위 옵션주거나 맞춰야할듯
// SpriteRenderer[] nextSPRs =  RendererController.GetSpriteRenderers(this, Define.InGameGrade.Rare);
// int length = Mathf.Max(currentSPRs.Length, nextSPRs.Length);
// for (int i = 0; i < length; ++i)
// {
//     // Prevent out of idx
//     if (i < currentSPRs.Length && i < nextSPRs.Length)
//     {
//         currentSPRs[i].sprite = nextSPRs[i].sprite;
//         currentSPRs[i].color = nextSPRs[i].color;
//     }
//     else
//         Utils.LogStrong("OOPS !!");
// }



// TEMP
// if (Input.GetKeyDown(KeyCode.O))
// {
//     Shield shield = SkillBook.GetCanActiveSkillMember(SkillTemplate.Shield).GetComponent<Shield>();
//     shield.Hit();
//     // SkillBase shield = SkillBook.GetCanActiveSkillMember(SkillTemplate.Shield);
//     // shield.GetComponent<Shield>().OnShield();
// }

// if (Input.GetKeyDown(KeyCode.P))
// {
//     SkillBase shield = SkillBook.GetCanActiveSkillMember(SkillTemplate.Shield);
//     shield.GetComponent<Shield>().OffShield();
// }

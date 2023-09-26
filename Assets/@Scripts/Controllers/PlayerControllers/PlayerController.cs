using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering;

using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

namespace STELLAREST_2D
{
    public class PlayerController : CreatureController
    {
        private readonly float ENV_COLLECTION_FIXED_DIST = 5f; // +++ NO DATE SHEET +++
        private float _armBowFixedAngle = 110f; // REINA에서 뺴야함
        private float _armRifleFixedAngle = 146f; // CHRISTIAN에서 빼야함
        public class PlayerBody
        {
            public PlayerBody(Transform hair, Transform armLeft, Transform armRight, Transform leftHandMeleeWeapon,
                            Transform legLeft, Transform legRight)
            {
                this.Hair = hair;
                this.ArmLeft = armLeft;
                this.ArmRight = armRight;
                this.LeftHandMeleeWeapon = leftHandMeleeWeapon;
                this.LegLeft = legLeft;
                this.LegRight = legRight;

                if (Hair == null || ArmLeft == null || ArmRight == null || LeftHandMeleeWeapon == null ||
                    LegLeft == null || LegRight == null)
                    Utils.LogCritical(nameof(PlayerController), nameof(PlayerBody));
            }

            public Transform Hair { get; private set; } = null;
            public Transform ArmLeft { get; private set; } = null;
            public Transform ArmRight { get; private set; } = null;
            public Transform LeftHandMeleeWeapon { get; private set; } = null;
            public Transform LegLeft { get; private set; } = null;
            public Transform LegRight { get; private set; } = null;
        }
        public PlayerBody BodyParts { get; protected set; } = null;
        public PlayerAnimationController PlayerAnimController { get; private set; } = null;

        public override void Init(int templateID)
        {
            base.Init(templateID);
            if (PlayerAnimController == null)
            {
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerBody);
                PlayerAnimController = AnimController as PlayerAnimationController;
                AddCallbacks();
            }

            CreatureState = Define.CreatureState.Idle;
        }

        protected override void InitChildObject()
        {
            base.InitChildObject();
            Transform hair = Utils.FindChild<Transform>(gameObject, "Hair", true);
            Transform armL = Utils.FindChild<Transform>(gameObject, "ArmL", true);
            Transform armR = Utils.FindChild<Transform>(gameObject, "ArmR[1]", true);
            Transform leftHandMeleeWeapon = Utils.FindChild<Transform>(armL.gameObject, "MeleeWeapon", true);
            Transform legL = Utils.FindChild<Transform>(gameObject, "Leg[L]", true);
            Transform legR = Utils.FindChild<Transform>(gameObject, "Leg[R]", true);
            BodyParts = new PlayerBody(hair: hair, armLeft: armL, armRight: armR, leftHandMeleeWeapon: leftHandMeleeWeapon,
                                    legLeft: legL, legRight: legR);
        }

        private void AddCallbacks()
        {
            if (Managers.Game != null)
                Managers.Game.OnMoveDirChanged += OnMoveDirChangedHandler;
        }

        private void RemoveCallbacks()
        {
            if (Managers.Game != null)
                Managers.Game.OnMoveDirChanged -= OnMoveDirChangedHandler;

            // if (this.IsValid())
            //     AnimCallback.OnAttackAnimSkillEnable -= SkillBook.Activate;
        }


        private void OnMoveDirChangedHandler(Vector3 moveDir)
        {
            this.MoveDir = moveDir;
            if (moveDir == Vector3.zero)
                CreatureState = Define.CreatureState.Idle;
            else
            {
                CreatureState = Define.CreatureState.Run;
                if (Managers.Game.IsGameStart == false)
                {
                    PlayerAnimController.Ready();
                    SkillBook.LevelUp(SkillBook.FirstExclusiveSkill);
                    SkillBook.Activate(SkillBook.FirstExclusiveSkill);
                    Managers.Game.GAME_START();
                }
            }
        }

#if UNITY_EDITOR
        private bool flag1 = false;
        private void Flag1()
        {
            flag1 = !flag1;
            if (flag1)
                SkillBook.Activate(SkillBook.FirstExclusiveSkill);
            else
                SkillBook.Deactivate(SkillBook.FirstExclusiveSkill);
        }

        private bool flag2 = false;
        private void Flag2()
        {
            flag2 = !flag2;
            if (flag2)
                SkillBook.Activate(SkillTemplate.ThrowingStar);
            else
                SkillBook.Deactivate(SkillTemplate.ThrowingStar);
        }
        private void Update()
        {
            DEV_CLEAR_LOG();
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SkillBook.LevelUp(SkillTemplate.PaladinMastery);
            if (Input.GetKeyDown(KeyCode.Q))
                Flag1();

            if (Input.GetKeyDown(KeyCode.Alpha2))
                SkillBook.LevelUp(SkillTemplate.ThrowingStar);
            if (Input.GetKeyDown(KeyCode.W))
                Flag2();
#endif
            MoveByJoystick(); // ERROR
            CollectEnv();
        }

        public override void UpdateAnimation()
        {
            switch (CreatureState)
            {
                case Define.CreatureState.Idle:
                    PlayerAnimController.Stand();
                    break;

                case Define.CreatureState.Run:
                    PlayerAnimController.Run();
                    break;

                case Define.CreatureState.Attack:
                    PlayerAnimController.Attack();
                    break;
            }
        }

        public Define.ExpressionType Testxpression = Define.ExpressionType.Default;
        private void TEST_EXPRESSION()
        {
            Managers.Sprite.PlayerExpressionController.Expression(Testxpression);
        }
        private void MoveByJoystick()
        {
            Vector3 dir = MoveDir.normalized * CreatureStat.MovementSpeed * Time.deltaTime;
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
                _armBowFixedAngle = 110f;
            }
            else
            {
                LookAtDir = Define.LookAtDirection.Left;
                _armBowFixedAngle = -110f;
            }

            LocalScale = new Vector3((int)LookAtDir * Define.PLAYER_LOCAL_SCALE_X * -1f, Define.PLAYER_LOCAL_SCALE_Y, 1);
        }
        private void CollectEnv()
        {
            // float sqrCollectDist = EnvCollectDist * EnvCollectDist;
            float sqrCollectDist = CreatureStat.CollectRange * CreatureStat.CollectRange;

            // var allSpawnedGems = Managers.Object.Gems.ToList();
            var findGems = Managers.Object.GridController.
                            GatherObjects(transform.position, ENV_COLLECTION_FIXED_DIST).ToList();

            foreach (var findGem in findGems)
            {
                GemController gc = findGem.GetComponent<GemController>();
                // gc.Alive = true;

                Vector3 dir = findGem.transform.position - transform.position;
                if (dir.sqrMagnitude <= sqrCollectDist)
                {
                    //Managers.Game.Gem += 1;
                    // Debug.Log("GEM SIZE : " + gc.GemSize);
                    // Managers.Game.Gem += (int)gc.GemSize;
                    // Managers.Game.Gem = (int)gc.GemSize;
                    gc.GetGem();
                }
            }
            //Debug.Log($"Find Gem : {findGems.Count} / Total Gem : {allSpawnedGems.Count}");
        }

        protected override void SetSortingGroup()
            => GetComponent<SortingGroup>().sortingOrder = (int)Define.SortingOrder.Player;

        public override void OnDamaged(BaseController attacker, SkillBase skill)
            => base.OnDamaged(attacker, skill);

        public void Expression(Define.ExpressionType expression, float duration)
            => StartCoroutine(Managers.Sprite.PlayerExpressionController.CoExpression(expression, duration));

        private void OnDestroy()
            => RemoveCallbacks();

        public bool AllStopAction()
        {
            //SkillBook.ActivateAll(Define.SkillType.Repeat, false);
            return false;
        }

        private IEnumerator CoAllStopAction()
        {
            yield return null;
        }

        public void ChangePlayerAppearance(SkillBase newSkill)
        {
            //StartCoroutine(CoChangePlayerAppearance(newSkill));
        }


#if UNITY_EDITOR
        [ContextMenu("UNITY_EDITOR")]
        private void DEV_CLEAR_LOG()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Utils.ClearLog();
        }
#endif
    }
}

// -------------------------------------------------------------------------------------
// -------------------------------------------------------------------------------------
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
            Phantom Knight - "Soul Eater" 적을 처치하면 최대 체력의 1~3% 회복 (랜덤) / 1% ~ 2% (60%), 2% ~ 3% (40%)

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
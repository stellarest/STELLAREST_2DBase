using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine.Rendering;
using STELLAREST_2D.Data;

namespace STELLAREST_2D
{
    public class PlayerController : CreatureController
    {
        public PlayerAnimationController PAC { get; protected set; }
        public float EnvCollectDist { get; private set; } = 5f; // 이건 데이터 시트로 안빼도 됨

        private Transform _indicator;
        public Transform Indicator => _indicator;

        private Transform _fireSocket;
        public Vector3 FireSocket => _fireSocket.position;
        public Transform GetFireSocket => _fireSocket;

        public SpriteRenderer FireSocketSpriteRenderer { get; private set; } = null;
        public Vector3 ShootDir => (_fireSocket.position - _indicator.position).normalized;

        public Transform LegR { get; private set; } = null;
        public Transform ArmL { get; private set; } = null;
        public Transform ArmR { get; private set; } = null;

        public GameObject LeftHandMeleeWeapon { get; private set; } = null;
        public GameObject Hair { get; private set; } = null;


        [field: SerializeField]
        public float TurningAngle { get; private set; }
        public Define.LookAtDirection LookAtDir { get; private set; } = Define.LookAtDirection.Right;

        private GameObject _animChildObject;
        public Vector3 AnimationLocalScale => _animChildObject.transform.localScale;
        public AnimationEvents AnimEvents { get; private set; }

        public float ATTACK_SPEED_TEST = 2f;

        public override void UpdateAnimation()
        {
            switch (_cretureState)
            {
                case Define.CreatureState.Idle:
                    {
                        PAC.Idle();
                    }
                    break;

                case Define.CreatureState.Walk:
                    {
                        PAC.Walk();
                    }
                    break;

                case Define.CreatureState.Run:
                    {
                        PAC.Run();
                    }
                    break;

                case Define.CreatureState.Attack:
                    {
                        // 애초에 Weapon Type을 받아와서 재생하는게 더 깔끔할수도 있음.
                        // 근데 이것도 ㄱㅊ
                        switch (CharaData.TemplateID)
                        {
                            case (int)Define.TemplateIDs.Player.Gary_Paladin:
                                {
                                    PAC.Slash1H();
                                }
                                break;
                            case (int)Define.TemplateIDs.Player.Gary_Knight:
                                {
                                    PAC.Slash2H();
                                }
                                break;
                            case (int)Define.TemplateIDs.Player.Gary_PhantomKnight:
                                {
                                    PAC.Slash1H();
                                }
                                break;



                            case (int)Define.TemplateIDs.Player.Reina_ArrowMaster:
                                {
                                    // SkillData skillData = SkillBook.GetCurrentPlayerDefaultSkill.SkillData;
                                    // if (skillData.InGameGrade >= Define.InGameGrade.Epic)
                                    //     PAC.AttackAnimSpeed(ATTACK_SPEED_TEST);
                                    PAC.SimpleBowShot();
                                }
                                break;
                            case (int)Define.TemplateIDs.Player.Reina_ElementalArcher:
                                {
                                    PAC.SimpleBowShot();
                                }
                                break;
                            case (int)Define.TemplateIDs.Player.Reina_ForestWarden:
                                {
                                    SkillData skillData = SkillBook.GetCurrentPlayerDefaultSkill.SkillData;
                                    if (skillData.InGameGrade >= Define.InGameGrade.Epic)
                                        PAC.AttackAnimSpeed(1.3f);
                                    PAC.SimpleBowShot();
                                }
                                break;



                            case (int)Define.TemplateIDs.Player.Kenneth_Assassin:
                                {
                                    SkillData skillData = SkillBook.GetCurrentPlayerDefaultSkill.SkillData;
                                    float animSpeed = skillData.AnimationSpeed;
                                    PAC.AttackAnimSpeed(animSpeed);
                                    if (skillData.InGameGrade < Define.InGameGrade.Legendary)
                                        PAC.Jab1H();
                                    else
                                        PAC.JabPaired();
                                }
                                break;
                            case (int)Define.TemplateIDs.Player.Kenneth_Thief:
                                {
                                    SkillData skillData = SkillBook.GetCurrentPlayerDefaultSkill.SkillData;
                                    if (skillData.InGameGrade > Define.InGameGrade.Epic)
                                        PAC.SlashDouble();
                                    else
                                        PAC.SlashPaired();
                                }
                                break;



                            case (int)Define.TemplateIDs.Player.Lionel_Warrior:
                                {
                                    PAC.Jab2H();
                                }
                                break;
                            case (int)Define.TemplateIDs.Player.Lionel_Berserker:
                                {
                                    PAC.SlashPaired();
                                }
                                break;



                            case (int)Define.TemplateIDs.Player.Stigma_SkeletonKing:
                                {
                                    PAC.Slash2H();
                                }
                                break;
                            case (int)Define.TemplateIDs.Player.Stigma_Pirate:
                                {
                                    SkillData skillData = SkillBook.GetCurrentPlayerDefaultSkill.SkillData;
                                    if (skillData.InGameGrade < Define.InGameGrade.Legendary)
                                        PAC.Jab1HLeft();
                                    else
                                        PAC.SlashPaired();
                                }
                                break;



                            case (int)Define.TemplateIDs.Player.Eleanor_Queen:
                                {
                                    PAC.Slash1H();
                                }
                                break;
                        }
        
                        StartAttackPos = transform.position;
                    }
                    break;

                case Define.CreatureState.Death:
                    {
                        Utils.LogStrong("### INVINCIBLA PLAYER NOW ###");
                    }
                    break;
            }
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = Define.ObjectType.Player;
            CreatureType = Define.CreatureType.Gary; // 조정 필요

            Managers.Game.OnMoveDirChanged += OnMoveDirChangedHandler;

            _animChildObject = Utils.FindChild(gameObject, "Animation");
            AnimEvents = _animChildObject.GetComponent<AnimationEvents>();
            PAC = gameObject.GetOrAddComponent<PlayerAnimationController>();
            PAC.Owner = this;

            GameObject LegR = Utils.FindChild(gameObject, "Leg[R]", true);
            this.LegR = Utils.FindChild(LegR, "Shin").transform;

            ArmL = Utils.FindChild(gameObject, "ArmL", true).transform;
            ArmR = Utils.FindChild(gameObject, "ArmR[1]", true).transform;

            LeftHandMeleeWeapon = Utils.FindChild(ArmL.gameObject, "MeleeWeapon", true);
            Hair = Utils.FindChild(gameObject, "Hair", true);

            CreatureState = Define.CreatureState.Idle;

            // TODO : 스킬은 처음에 UI에서 고르던 다른 방식으로 하던 지금처럼 하던 마음대로 채택
            Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerBody);
            return true;
        }

        public override void SetInfo(int templateID)
        {
            base.SetInfo(templateID);
            GetIndicator();

            // ++++++++++++++++++++++++++++++++++++++++++++++++++
            // +++++ Set Player Default Skill Automatically +++++
            // ++++++++++++++++++++++++++++++++++++++++++++++++++
            this.SkillBook.PlayerDefaultSkill = (Define.TemplateIDs.SkillType)SkillBook.GetPlayerDefaultSkill(Define.InGameGrade.Normal).SkillData.TemplateID;
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!! : " + SkillBook.PlayerDefaultSkill);

            // TODO
            // Define.TemplateIDs.SkillType.PaladinSwing : UI에서 최초 캐릭터 셀렉트시 결정 
            //AnimEvents.PlayerDefaultAttack = Define.TemplateIDs.SkillType.PaladinMeleeSwing;
            AnimEvents.PlayerDefaultSkill = this.SkillBook.PlayerDefaultSkill;
            AnimEvents.OnRepeatAttack += SkillBook.GeneratePlayerAttack;
        }

        private void GetIndicator()
        {
            if (_indicator == null)
                _indicator = Utils.FindChild<Transform>(this.gameObject,
                    Define.Player.INDICATOR, true);

            if (_fireSocket == null)
            {
                _fireSocket = Utils.FindChild<Transform>(this.gameObject,
                    Define.Player.FIRE_SOCKET, true);

                FireSocketSpriteRenderer = _fireSocket.GetComponent<SpriteRenderer>();
            }

            // _indicator.gameObject.SetActive(false);
            _indicator.gameObject.SetActive(true);

            GetComponent<CircleCollider2D>().enabled = true;
        }

        protected override void SetSortingGroup()
                => GetComponent<SortingGroup>().sortingOrder = (int)Define.SortingOrder.Player;

        public Vector3 StartAttackPos { get; private set; }
        public Vector3 EndAttackPos { get; set; }
        public float MovementPower
                => (EndAttackPos - StartAttackPos).magnitude;

        private bool _getReady = false;

        public void MoveByJoystick()
        {
            Vector3 dir = MoveDir.normalized * CharaData.MoveSpeed * Time.deltaTime;
            transform.position += dir;

            // Get Degrees = 180f / PI = Rad2Deg
            // if (_moveDir != Vector2.zero)
            //     _indicator.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * 180f / Mathf.PI);
            if (MoveDir != Vector2.zero)
            {
                if (_getReady == false)
                {
                    _getReady = true;

                    if (CharaData.TemplateID != (int)Define.TemplateIDs.Player.Stigma_Mutant)
                        SkillBook.UpgradeRepeatSkill((int)SkillBook.PlayerDefaultSkill);
                    else
                        SkillBook.UpgradeRepeatSkill((int)Define.TemplateIDs.SkillType.DeathClaw);

                    PAC.OnReady();
                }

                float degree = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;
                //_indicator.eulerAngles = new Vector3(0, 0, degree);
                //_indicator.rotation = Quaternion.Euler(0, 0, degree);
                _indicator.localRotation = Quaternion.Euler(0, 0, degree);                

                Turn(degree);
                // InGameLimitPos(transform.position);
                Managers.Stage.SetInLimitPos(this);
            }
            //RigidBody.velocity = Vector3.zero;
        }

        public bool IsFacingRight
                => (_animChildObject.transform.localScale.x != Define.Player.LOCAL_SCALE_X * -1f) ? true : false;
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

            Vector3 turnChara = new Vector3((int)LookAtDir * Define.Player.LOCAL_SCALE_X * -1f,
                                        Define.Player.LOCAL_SCALE_Y, 1);
            _animChildObject.transform.localScale = turnChara;
        }

        private void InGameLimitPos(Vector3 position)
        {
            // Min
            if (position.x <= Managers.Stage.LeftBottom.x)
                transform.position = new Vector2(Managers.Stage.LeftBottom.x, transform.position.y);
            if (position.y <= Managers.Stage.LeftBottom.y)
                transform.position = new Vector2(transform.position.x, Managers.Stage.LeftBottom.y);

            // Max
            if (position.x >= Managers.Stage.RightTop.x)
                transform.position = new Vector2(Managers.Stage.RightTop.x, transform.position.y);
            if (position.y >= Managers.Stage.RightTop.y)
                transform.position = new Vector2(transform.position.x, Managers.Stage.RightTop.y);
        }

        public bool IsInLimitPos()
        {
            if (Mathf.Abs(transform.position.x - Managers.Stage.LeftBottom.x) < Mathf.Epsilon ||
                Mathf.Abs(transform.position.y - Managers.Stage.LeftBottom.y) < Mathf.Epsilon ||
                Mathf.Abs(transform.position.x - Managers.Stage.RightTop.x) < Mathf.Epsilon ||
                Mathf.Abs(transform.position.y - Managers.Stage.RightTop.y) < Mathf.Epsilon)
                return true;
            else
                return false;
        }

        public bool IsInLimitMaxPosX => Mathf.Abs(transform.position.x - Managers.Stage.RightTop.x) < Mathf.Epsilon ||
                                        Mathf.Abs(transform.position.x - Managers.Stage.LeftBottom.x) < Mathf.Epsilon;

        public bool IsInLimitMaxPosY => Mathf.Abs(transform.position.y - Managers.Stage.RightTop.y) < Mathf.Epsilon ||
                                        Mathf.Abs(transform.position.y - Managers.Stage.LeftBottom.y) < Mathf.Epsilon;

        private void CollectEnv()
        {
            // float sqrCollectDist = EnvCollectDist * EnvCollectDist;
            float sqrCollectDist = CharaData.CollectRange * CharaData.CollectRange;

            // var allSpawnedGems = Managers.Object.Gems.ToList();
            var findGems = Managers.Object.GridController.
                            GatherObjects(transform.position, EnvCollectDist).ToList();

            // 맵안에 있는 잼들은 디폴트로 시간이 지나면 다 죽임.
            // foreach (GemController allSpawnedGem in allSpawnedGems)
            //     allSpawnedGem.Alive = false;

            // 플레이어가 이동하다가 발견된 잼은 살림
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

        private void OnMoveDirChangedHandler(Vector2 moveDir)
        {
            this.MoveDir = moveDir;
            if (moveDir == Vector2.zero)
            {
                CreatureState = Define.CreatureState.Idle;
                // _indicator.gameObject.SetActive(false);
                FireSocketSpriteRenderer.enabled = false;
                //FireSocketSpriteRenderer.enabled = true;

            }
            else
            {
                CreatureState = Define.CreatureState.Run;
                // _indicator.gameObject.SetActive(true);
                // FireSocketSpriteRenderer.enabled = true;
                FireSocketSpriteRenderer.enabled = false; // 냐중에 어떻게 해야할지 고쳐야함
                //FireSocketSpriteRenderer.enabled = true;
            }
        }

        public void UpgradePlayerAppearance(SkillBase newSkill)
        {
            StartCoroutine(CoUpgradePlayerAppearance(newSkill));
        }

        private IEnumerator CoUpgradePlayerAppearance(SkillBase newSkill)
        {
            SkillBook.StopSkills(); // 이걸로하면 나중에 Sequence Skill이 Active가 안될텐뎅
            // StopPlayerDefaultSkill로 바꿔야함.
            PAC.OffReady();

            // +++ TEMP +++
            if (IsChristian(CharaData.TemplateID) == false)
            {
                while (PAC.AnimController.GetCurrentAnimatorStateInfo(0).IsName("IdleMelee") == false)
                {
                    // 이 while 문은 공격 중일때 들어오는 부분인데, 현재 아직 공격중인 크리스티앙 애니메이션이 없음.
                    yield return null;
                }
            }

            // +++ ENABLE ASSASSIN DOUBLE WEAPON +++
            if (IsAssassin(newSkill.SkillData.OriginTemplateID) && newSkill.SkillData.InGameGrade == Define.InGameGrade.Legendary)
                LeftHandMeleeWeapon.GetComponent<SpriteRenderer>().enabled = true;

            // +++ FIND OTHER HAND REINA BOW +++
            if (IsReina(CharaData.TemplateID) || IsChristian(CharaData.TemplateID))
                Managers.Sprite.UpgradePlayerAppearance(this, newSkill.SkillData.InGameGrade, true);
            else
                Managers.Sprite.UpgradePlayerAppearance(this, newSkill.SkillData.InGameGrade, false);

            // +++ ADJUST THIEF HAIR MASK +++
            if (IsThief(newSkill.SkillData.OriginTemplateID) && newSkill.SkillData.InGameGrade > Define.InGameGrade.Rare)
                Hair.GetComponent<SpriteMask>().isCustomRangeActive = false;

            Managers.Sprite.PlayerExpressionController.UpdateDefaultFace(this);

            PAC.OnReady();
            newSkill.ActivateSkill();
        }

        private bool IsAssassin(int templateID)
            => templateID == 200124;

        private bool IsThief(int templateID)
            => templateID == 200128;

        public bool IsReina(int templateID) 
            => templateID == 100103 || templateID == 100104 || templateID == 100105 ? true : false;

        private bool IsChristian(int templateID)
            => templateID == 100112 || templateID == 100113 || templateID == 100114 ? true : false;
        

        public override void OnDamaged(BaseController attacker, SkillBase skill)
                        => base.OnDamaged(attacker, skill);

        // bool bChange = false;
        public Define.ExpressionType MyExpression = Define.ExpressionType.Default;

        private void TEST_EXPRESSION()
        {
            Managers.Sprite.PlayerExpressionController.Expression(MyExpression);
        }

        private void Update()
        {
            // Debug.Log(Mathf.Abs(transform.position.x - Managers.Stage.LeftBottom.x));
            // Debug.Log(Managers.Stage.IsInLimitPos(this.transform));

            if (Input.GetKeyDown(KeyCode.T))
            {
                // Managers.Sprite.PlayerEmotion.Sick();
                // Debug.Log(SkillBook.GetPlayerDefaultSkill(Define.InGameGrade.Normal).SkillData.ModelingLabel);
                // Debug.Log(SkillBook.GetPlayerDefaultSkill(Define.InGameGrade.Rare).SkillData.ModelingLabel);
                // Debug.Log(SkillBook.GetPlayerDefaultSkill(Define.InGameGrade.Epic).SkillData.ModelingLabel);
                // Debug.Log(SkillBook.GetPlayerDefaultSkill(Define.InGameGrade.Legendary).SkillData.ModelingLabel);

                // CoGlitchEffect(CreatureData.TemplateID);
                // bChange = !bChange;
                // Managers.Sprite.SetPlayerEmotion(bChange ? Define.PlayerEmotion.Sick : Define.PlayerEmotion.Default);
                // Managers.Sprite.SetPlayerEmotion(emotion);
                // CoFadeEffect(CreatureData.TemplateID + (int)Define.InGameGrade.Legendary);
                // Debug.Log(SkillBook.RepeatCurrentGrade(Define.TemplateIDs.SkillType.PaladinSwing));
                // PAC.DieFront();
                // Managers.Effect.ShowDodgeText(this);
                // PAC.Jab1H();
                // PAC.DeathBack();
                // PAC.Slash1H();

                // CoEffectHologram();
                // Managers.Sprite.SetPlayerEmotion(Define.PlayerEmotion.Kitty);
                // Vector3 spawnEffectPos = new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z);
                // Managers.Effect.ShowSpawnEffect(Define.PrefabLabels.SPAWN_EFFECT, spawnEffectPos);

                // for (int i = 0; i < 100; ++i)
                // {
                //     Vector3 randPos = Utils.GenerateMonsterSpawnPosition(Managers.Game.Player.transform.position, 10f, 20f);
                //     GemController gc = Managers.Object.Spawn<GemController>(randPos);
                //     gc.GemSize = UnityEngine.Random.Range(0, 2) == 0 ? gc.GemSize = GemSize.Normal : gc.GemSize = GemSize.Large;
                // }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                TEST_EXPRESSION();
                // PAC.DeathFront();
                // var findGems = Managers.Object.GridController.
                // GatherObjects(transform.position, EnvCollectDist + 99f).ToList();
                // foreach (var gem in findGems)
                // {
                //     GemController gc = gem.GetComponent<GemController>();
                //     gc.GetGem();
                // }
            }

            MoveByJoystick();
            CollectEnv();

            if (Input.GetKeyDown(KeyCode.Alpha1))
                SkillBook.UpgradeRepeatSkill((int)Define.TemplateIDs.SkillType.ThrowingStar);

            if (Input.GetKeyDown(KeyCode.Alpha2))
                SkillBook.UpgradeRepeatSkill((int)Define.TemplateIDs.SkillType.LazerBolt);

            if (Input.GetKeyDown(KeyCode.Alpha3))
                SkillBook.UpgradeRepeatSkill((int)Define.TemplateIDs.SkillType.Boomerang);

            if (Input.GetKeyDown(KeyCode.Alpha4))
                SkillBook.UpgradeRepeatSkill((int)Define.TemplateIDs.SkillType.Spear);

            if (Input.GetKeyDown(KeyCode.Alpha5))
                SkillBook.UpgradeRepeatSkill((int)Define.TemplateIDs.SkillType.BombTrap);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                //SkillBook.UpgradeRepeatSkill((int)Define.TemplateIDs.SkillType.PaladinMeleeSwing);
                //SkillBook.UpgradeRepeatSkill((int)Define.TemplateIDs.SkillType.KnightMeleeSwing);

                if (CharaData.TemplateID != (int)Define.TemplateIDs.Player.Stigma_Mutant)
                    SkillBook.UpgradeRepeatSkill((int)SkillBook.PlayerDefaultSkill);
                else
                    SkillBook.UpgradeRepeatSkill((int)Define.TemplateIDs.SkillType.DeathClaw);
            }
        }

        private float _armBowFixedAngle = 110f;
        private float _armRifleFixedAngle = 146f;
        public float TEST_ANGLE = 0f;
        private void LateUpdate()
        {
            //Debug.Log("Angle : " + Vector2.Angle(ArmL.transform.localPosition, Indicator.localPosition));
            if (_getReady)
            {

                switch (CharaData.TemplateID)
                {
                    case (int)Define.TemplateIDs.Player.Reina_ArrowMaster:
                    case (int)Define.TemplateIDs.Player.Reina_ElementalArcher:
                    case (int)Define.TemplateIDs.Player.Reina_ForestWarden:
                        {
                            float modifiedAngle = (Indicator.eulerAngles.z + _armBowFixedAngle);
                            if (AnimationLocalScale.x < 0)
                                modifiedAngle = 360f - modifiedAngle;

                            ArmL.transform.localRotation = Quaternion.Euler(0, 0, modifiedAngle);
                        }
                        break;


                    case (int)Define.TemplateIDs.Player.Christian_Hunter:
                    case (int)Define.TemplateIDs.Player.Christian_Desperado:
                    case (int)Define.TemplateIDs.Player.Christian_Destroyer:
                        {
                            float modifiedAngle = (Indicator.eulerAngles.z + _armRifleFixedAngle);
                            if (AnimationLocalScale.x < 0)
                            {
                                modifiedAngle = 360f - modifiedAngle - 65f;
                                ArmR.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Clamp(modifiedAngle, 15f, 91f));
                            }
                            else
                                ArmR.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Clamp(modifiedAngle, 378f, 450f));
                        }
                        break;

                }
            }
        }

        private void OnDestroy()
        {
            if (Managers.Game != null)
                Managers.Game.OnMoveDirChanged -= OnMoveDirChangedHandler;

            if (this.IsValid())
                AnimEvents.OnRepeatAttack -= SkillBook.GeneratePlayerAttack;
        }

        public void CoExpression(Define.ExpressionType expression, float duration)
                => StartCoroutine(Managers.Sprite.PlayerExpressionController.CoExpression(expression, duration));
    }
}

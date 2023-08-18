using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine.Rendering;
using System.Reflection;
using Unity.VisualScripting;

namespace STELLAREST_2D
{
    public class PlayerController : CreatureController
    {
        public enum LookAtDirection
        {
            Left = 1,
            Right = -1
        }

        public PlayerAnimationController PAC { get; protected set; }
        public float EnvCollectDist { get; private set; } = 5f; // 이건 데이터 시트로 안빼도 됨

        private Transform _indicator;
        public Transform Indicator => _indicator;

        private Transform _fireSocket;
        public Vector3 FireSocket => _fireSocket.position;
        public SpriteRenderer FireSocketSpriteRenderer { get; private set; } = null;
        public Vector3 ShootDir => (_fireSocket.position - _indicator.position).normalized;

        public Transform LegR { get; private set; } = null;
    

        [field: SerializeField]
        public float TurningAngle { get; private set; }
        public LookAtDirection LookAtDir { get; private set; } = LookAtDirection.Right;

        private GameObject _animChildObject;
        public Vector3 AnimationLocalScale => _animChildObject.transform.localScale;
        public AnimationEvents AnimEvents { get; private set; }
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
                            case (int)Define.TemplateIDs.Player.Gary_PhantomKnight:
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
                        }

                        // PAC.Slash1H();
                        // PAC.Slash2H();
                        // PAC.Cast1H();

                        StartAttackPos = transform.position;
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

            CreatureState = Define.CreatureState.Idle;

            // TODO : 스킬은 처음에 UI에서 고르던 다른 방식으로 하던 지금처럼 하던 마음대로 채택
            Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerBody);
            return true;
        }

        public override void SetInfo(int templateID)
        {
            base.SetInfo(templateID);
            GetIndicator();

            // +++++ Set Player Default Skill Automatically +++++
            this.SkillBook.PlayerDefaultSkill = (Define.TemplateIDs.SkillType)SkillBook.GetPlayerDefaultSkill(Define.InGameGrade.Normal).SkillData.TemplateID;

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
                    Define.PlayerController.INDICATOR, true);

            if (_fireSocket == null)
            {
                _fireSocket = Utils.FindChild<Transform>(this.gameObject,
                    Define.PlayerController.FIRE_SOCKET, true);

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
                    PAC.OnReady();
                }

                float degree = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;
                _indicator.eulerAngles = new Vector3(0, 0, degree);
                Turn(degree);
                // InGameLimitPos(transform.position);
                Managers.Stage.SetInLimitPos(this);
            }
            //RigidBody.velocity = Vector3.zero;
        }

        public bool IsFacingRight
                => (_animChildObject.transform.localScale.x != Define.PlayerController.CONSTANT_SCALE_X * -1f) ? true : false;
        private void Turn(float angle)
        {
            TurningAngle = Mathf.Sign(angle); // 각도 양수1, 음수-1
            if (TurningAngle < 0)
                LookAtDir = LookAtDirection.Right;
            else
                LookAtDir = LookAtDirection.Left;

            Vector3 turnChara = new Vector3(TurningAngle * Define.PlayerController.CONSTANT_SCALE_X * -1f,
                                        Define.PlayerController.CONSTANT_SCALE_Y, Define.PlayerController.CONSTANT_SCALE_Z);
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
            }
            else
            {
                CreatureState = Define.CreatureState.Run;
                // _indicator.gameObject.SetActive(true);
                // FireSocketSpriteRenderer.enabled = true;
                FireSocketSpriteRenderer.enabled = false; // 냐중에 어떻게 해야할지 고쳐야함
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
            while (PAC.AnimController.GetCurrentAnimatorStateInfo(0).IsName("IdleMelee") == false)
                yield return null;

            Managers.Sprite.UpgradePlayerAppearance(this, newSkill.SkillData.InGameGrade);
            PAC.OnReady();
            newSkill.ActivateSkill();
        }

        public override void OnDamaged(BaseController attacker, SkillBase skill)
                        => base.OnDamaged(attacker, skill);

        // bool bChange = false;
        public Define.PlayerEmotion emotion = Define.PlayerEmotion.Default;

        private void Update()
        {
            // Debug.Log(Mathf.Abs(transform.position.x - Managers.Stage.LeftBottom.x));
            // Debug.Log(Managers.Stage.IsInLimitPos(this.transform));

            if (Input.GetKeyDown(KeyCode.T))
            {
                
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
                // PAC.DeathFront();
                // Managers.Sprite.SetPlayerEmotion(Define.PlayerEmotion.Death);
                var findGems = Managers.Object.GridController.
                GatherObjects(transform.position, EnvCollectDist + 99f).ToList();
                foreach (var gem in findGems)
                {
                    GemController gc = gem.GetComponent<GemController>();
                    gc.GetGem();
                }
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
                SkillBook.UpgradeRepeatSkill((int)SkillBook.PlayerDefaultSkill);
            }
        }

        // private void TestVec()
        // {
        //     Vector2 myPos = transform.position;
        //     Vector2 upDir = Quaternion.Euler(0, 0, 45f * 0.5f) * ShootDir;
        //     Vector2 downDir = Quaternion.Euler(0, 0, -45f * 0.5f) * ShootDir;
        //     Debug.Log("UP SCALE : " + upDir.magnitude); // 1f
        //     Debug.Log("DOWN SCALE : " + downDir.magnitude); // 1f
        //     Debug.DrawRay(myPos, upDir * 10, Color.red, -1f);
        //     Debug.DrawRay(myPos, downDir * 10, Color.red, -1f);
        // }

        private void OnDestroy()
        {
            if (Managers.Game != null)
                Managers.Game.OnMoveDirChanged -= OnMoveDirChangedHandler;

            if (this.IsValid())
                AnimEvents.OnRepeatAttack -= SkillBook.GeneratePlayerAttack;
        }

        // "GuardSword1_Rare.sprite"
        // [ContextMenu("ChangeWeaponTest")]
        // public void ChangeWeaponTest()
        // {
        //     GetComponent<Character>().PrimaryMeleeWeapon = Managers.Resource.Load<Sprite>("GuardSword1_Rare.sprite");
        //     GetComponent<Character>().PrimaryMeleeWeaponRenderer.sprite = GetComponent<Character>().PrimaryMeleeWeapon;
        // }

        // private void CollectEnv2() // LEGACY
        // {
        //     float sqrCollectDist = EnvCollectDist * EnvCollectDist;

        //     // ToList로 사본을 먼저 만들고 순회
        //     List<GemController> gems = Managers.Object.Gems.ToList();
        //     foreach (GemController gem in gems)
        //     {
        //         Vector3 dir = gem.transform.position - transform.position;
        //         if (dir.sqrMagnitude <= sqrCollectDist)
        //         {
        //             Managers.Game.Gem += 1;
        //             Managers.Object.Despawn(gem);
        //         }
        //     }

        //     // (선택)0.5f : 오브젝트의 크기를 더하면 된다. 구슬의 중심점만 닿으면 되면 빼도 됨.
        //     var findGems = Managers.Object.GridController.GatherObjects(transform.position, EnvCollectDist + 0.5f);
        //     Debug.Log($"Search Gems : {findGems.Count} / Total Gems : {gems.Count}");
        // }

        // // TEMP : FireProjectile
        // private Coroutine _coFireProjectile;
        // private void StartProjectile()
        // {
        //     if (_coFireProjectile != null)
        //         StopCoroutine(_coFireProjectile);

        //     _coFireProjectile = StartCoroutine(CoStartProjectile());
        // }

        // private IEnumerator CoStartProjectile()
        // {
        //     // 몇 초 마다 한 번씩 쏜다 -> 데이터 시트에서 꺼내온다. 지금은 0.5초
        //     WaitForSeconds wait = new WaitForSeconds(0.5f);
        //     while (true)
        //     {
        //         // 나중에 총구모양 있으면 총구 모양 위치에다가
        //         ProjectileController pc = Managers.Object.
        //                         Spawn<ProjectileController>(_fireSocket.position, 
        //                         (int)Define.PlayerData.SkillTemplateIDs.FireBall);
        //         yield return new WaitUntil(() => (pc != null)); // 이 코루틴 하나로 다해결
        //         pc.SetInfo(this, (_fireSocket.position - _indicator.position).normalized);

        //         yield return wait;
        //     }
        // }
    }
}

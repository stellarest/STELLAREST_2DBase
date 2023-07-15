using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine.Rendering;
using System.Reflection;

namespace STELLAREST_2D
{
    public class PlayerController : CreatureController
    {
        public float EnvCollectDist { get; private set; } = 1f; // 이건 데이터 시트로 안빼도 됨
        private Transform _indicator;
        public Transform Indicator => _indicator;

        private Transform _fireSocket;
        public Vector3 FireSocket => _fireSocket.position;

        public Vector3 ShootDir => (_fireSocket.position - _indicator.position).normalized;
        //public PlayerAnimationController PAC { get; protected set; }

        [field: SerializeField]
        public float TurningAngle { get; private set; }

        private GameObject _animChildObject;
        public Vector3 AnimationLocalScale => _animChildObject.transform.localScale;
        public AnimationEvents AnimEvents { get; private set; }

        private bool _getReady = false;
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = Define.ObjectType.Player;
            Utils.InitLog(this.GetType());

            Managers.Game.OnMoveDirChanged += OnMoveDirChangedHandler;

            _animChildObject = Utils.FindChild(gameObject, "Animation");
            AnimEvents = _animChildObject.GetComponent<AnimationEvents>();
            
            PAC = gameObject.GetOrAddComponent<PlayerAnimationController>();
            PAC.Owner = this;

            // TODO : 스킬은 처음에 UI에서 고르던 다른 방식으로 하던 지금처럼 하던 마음대로 채택
            Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerBody);
            return true;
        }

        public override void SetInfo(int templateID)
        {
            base.SetInfo(templateID);
            GetIndicator();

            // TODO
            // Define.TemplateIDs.SkillType.PaladinSwing : UI에서 최초 캐릭터 셀렉트시 결정 
            AnimEvents.PlayerDefaultAttack = Define.TemplateIDs.SkillType.PaladinSwing;
            AnimEvents.OnRepeatAttack += SkillBook.PlayerDefaultAttack;
        }

        private void GetIndicator()
        {
            if (_indicator == null)
                _indicator = Utils.FindChild<Transform>(this.gameObject,
                    Define.PlayerController.INDICATOR, true);

            if (_fireSocket == null)
                _fireSocket = Utils.FindChild<Transform>(this.gameObject,
                    Define.PlayerController.FIRE_SOCKET, true);

            _indicator.gameObject.SetActive(false);
            GetComponent<CircleCollider2D>().enabled = true;
        }

        protected override void SetSortingGroup()
        {
            GetComponent<SortingGroup>().sortingOrder = (int)Define.SortingOrder.Player;
        }

        public Vector3 StartAttackPos { get; private set; }
        public Vector3 EndAttackPos { get; set; }
        public float MovementPower => (EndAttackPos - StartAttackPos).magnitude;
        public void Attack()
        {
            switch (TemplateID)
            {
                case (int)Define.TemplateIDs.Player.Gary_Paladin:
                    PAC.MeleeSlash(CreatureData.RepeatAttackAnimSpeed);
                    StartAttackPos = transform.position;
                    IsAttackStart = true;
                    break;
            }
        }


        public void MoveByJoystick()
        {
            Vector3 dir = MoveDir.normalized * MoveSpeed * Time.deltaTime;
            transform.position += dir;

            // Get Degrees = 180f / PI = Rad2Deg
            // if (_moveDir != Vector2.zero)
            //     _indicator.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * 180f / Mathf.PI);
            if (MoveDir != Vector2.zero)
            {
                if (_getReady == false)
                {
                    PAC.Ready();
                    _getReady = true;
                }

                float degree = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;
                _indicator.eulerAngles = new Vector3(0, 0, degree);
                Turn(degree);
                InGameLimitPos(transform.position);
            }

            //RigidBody.velocity = Vector3.zero;
        }

        public bool IsFacingRight => (_animChildObject.transform.localScale.x != Define.PlayerController.CONSTANT_SCALE_X * -1f) ? true : false;
        private void Turn(float angle)
        {
            TurningAngle = Mathf.Sign(angle); // 각도 양수1, 음수-1
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
            float sqrCollectDist = EnvCollectDist * EnvCollectDist;

            var allSpawnedGems = Managers.Object.Gems.ToList();
            var findGems = Managers.Object.GridController.
                            GatherObjects(transform.position, EnvCollectDist + 0.5f).ToList();

            // 맵안에 있는 잼들은 디폴트로 시간이 지나면 다 죽임.
            foreach (GemController allSpawnedGem in allSpawnedGems)
                allSpawnedGem.Alive = false;

            // 플레이어가 이동하다가 발견된 잼은 살림
            foreach (var findGem in findGems)
            {
                GemController gc = findGem.GetComponent<GemController>();
                gc.Alive = true;

                Vector3 dir = findGem.transform.position - transform.position;
                if (dir.sqrMagnitude <= sqrCollectDist)
                {
                    Managers.Game.Gem += 1;
                    Managers.Object.Despawn(gc);
                }
            }
            //Debug.Log($"Find Gem : {findGems.Count} / Total Gem : {allSpawnedGems.Count}");
        }

        private void OnMoveDirChangedHandler(Vector2 moveDir)
        {
            this.MoveDir = moveDir;
            if (moveDir == Vector2.zero)
            {
                PAC.Idle();
                //_indicator.gameObject.SetActive(false);
                _indicator.gameObject.SetActive(false); // 각도 확인용
            }
            else
            {
                PAC.Run();
                _indicator.gameObject.SetActive(true);
            }
        }

        public override void OnDamaged(BaseController attacker, SkillBase skill, float damage)
        {
            base.OnDamaged(attacker, skill, damage);
        }

        private void Update()
        {
            MoveByJoystick();
            CollectEnv();

            if (Input.GetKeyDown(KeyCode.Space))
                SkillBook.ActivateRepeatSkill((int)Define.TemplateIDs.SkillType.PaladinSwing);
                
            if (Input.GetKeyDown(KeyCode.T))
                SkillBook.UpgradeRepeatSkill((int)Define.TemplateIDs.SkillType.PaladinSwing);
        }

        private void OnDestroy()
        {
            if (Managers.Game != null)
                Managers.Game.OnMoveDirChanged -= OnMoveDirChangedHandler;

            if (this.IsValid())
                AnimEvents.OnRepeatAttack -= SkillBook.PlayerDefaultAttack;
        }

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

        // // TEMP : EgoSword
        // private EgoSwordController _egoSword;
        // private void StartEgoSword()
        // {
        //     if (_egoSword.IsValid())
        //         return;

        //     // Debug.Log("### Spawn Ego Sword ###");
        //     _egoSword = Managers.Object.Spawn<EgoSwordController>(_indicator.position, 
        //                                 (int)Define.PlayerData.SkillTemplateIDs.EgoSword);
        //     _egoSword.transform.SetParent(_indicator);
        //     _egoSword.ActivateSkill();
        // }
    }
}

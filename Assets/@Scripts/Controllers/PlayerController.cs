using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace STELLAREST_2D
{
    public class PlayerController : CreatureController
    {
        private Data.PlayerStatData _playerData;
        public Data.PlayerStatData PlayerData
        {
            get => _playerData;
            set
            {
                _playerData = value; // 플레이어의 고유 데이터까지 전부 다 담아낼거 하나
                // 아래는 크리처 공용 셋팅
                this.MaxHp = _playerData.maxHp;
                this.Hp = this.MaxHp;
                this.MoveSpeed = _playerData.moveSpeed;
            }
        }

        // private Vector2 _moveDir = Vector2.zero;
        // public Vector2 MoveDir
        // {
        //     get => _moveDir;
        //     set { MoveDir = value.normalized; }
        // }

        public float EnvCollectDist { get; private set; } = 1f;
        
        [SerializeField] private Transform _indicator;
        public Transform Indicator => _indicator;
        public Vector3 FireSocket => _fireSocket.position;
        public Vector3 ShootDir => (_fireSocket.position - _indicator.position).normalized;

        [SerializeField] private Transform _fireSocket;

        public override bool Init()
        {
            if (base.Init() == false)
                return true;
            Debug.Log("### PC INIT SUCCESS ###");
            Managers.Game.OnMoveDirChanged += OnMoveDirChangedHandler;

            GetIndicator();
            // StartProjectile();
            // StartEgoSword();
            
            // TODO
            // 원래는 처음에 UI에서 고르는것으로 해야되지만.. 일단 이렇게
            FireballSkill fs = Skills.AddSkill<FireballSkill>(transform.position);
            // 부모 자식으로 붙이던지 알아서..
            EgoSword es = Skills.AddSkill<EgoSword>(_indicator.position);

            return true;
        }

        private void GetIndicator()
        {
            if (_indicator == null)
                _indicator = Utils.FindChild<Transform>(this.gameObject, 
                    Define.PlayerData.INDICATOR, true);

            if (_fireSocket == null)
                _fireSocket = Utils.FindChild<Transform>(this.gameObject, 
                    Define.PlayerData.FIRE_SOCKET, true);
        }

        private void Update()
        {
            MovePlayerByController();
            CollectEnv();
        }

        private void OnDestroy()
        {
            if (Managers.Game != null)
                Managers.Game.OnMoveDirChanged -= OnMoveDirChangedHandler;
        }

        public void MovePlayerByController()
        {
            Vector3 dir = MoveDir.normalized * MoveSpeed * Time.deltaTime;
            transform.position += dir;

            // if (_moveDir != Vector2.zero)
            //     _indicator.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * 180f / Mathf.PI);
            if (MoveDir != Vector2.zero)
                _indicator.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg);

            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }

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
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            MonsterController target = other.gameObject.GetComponent<MonsterController>();
            if (target == null) // 이거 target이 pull에 들어간 순간 유의해야함. null이 아니기 때문
                return;
        }

        public override void OnDamaged(BaseController attacker, int damage)
        {
            base.OnDamaged(attacker, damage);

            // TEMP
            // CreatureController cc = attacker as CreatureController;
            // cc?.OnDamaged(this, 10000);
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

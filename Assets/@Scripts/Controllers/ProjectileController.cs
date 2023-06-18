using UnityEngine;

namespace STELLAREST_2D
{
    public class ProjectileController : SkillBase
    {
        private CreatureController _owner;
        private Vector3 _moveDir; // 방향 속도 등등 나중에 데이터 시트에서 불러와야함.
        private float _lifetime = 10.0f; // 데이터로 빼야함

        public override bool Init()
        {
            base.Init();
            StartDestroy(_lifetime);
            return true;
        }

        public void SetInfo(CreatureController owner, Vector3 moveDir)
        {
            this._owner = owner;
            this._moveDir = moveDir;
        }

        // public override void UpdateController()
        // {
        //     base.UpdateController();
        //     transform.position += _moveDir * ProjectileSpeed * Time.deltaTime;
        // }

        private void OnTriggerEnter2D(Collider2D other)
        {
            MonsterController mc = other.gameObject.GetComponent<MonsterController>();
            if (mc.IsValid() == false) // 몬스터가 pooling된 객체라는 것도 생각해야되서 IsValid로 체크하자
                return; // 이미 몬스터가 죽었다는 의미
            if (this.IsValid() == false) // 총알 자기 자신이 죽었는데 또 들어올수도 있음. 이중으로 체크
                return;

            // this(투사체)가 아니라 owner로. 나중에 어그로 이런거 고려하려면
            // *** SetInfo하기전에 먼저 충돌되면 문제가 발생할 수도 있다. ***
            mc.OnDamaged(_owner, damage: Damage);
            StopDestroy();

            Managers.Object.Despawn(this);
        }
    }
}

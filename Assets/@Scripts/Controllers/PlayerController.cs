using UnityEngine;

namespace STELLAREST_2D
{
    public class PlayerController : CreatureController
    {
        private Vector2 _moveDir = Vector2.zero;
        public Vector2 MoveDir
        {
            get => _moveDir;
            set { _moveDir = value.normalized; }
        }

        private void Start()
        {
            Managers.Game.OnMoveChanged += OnMoveChangedHandler;
            _speed = 5f;
        }

        private void Update()
        {
            MovePlayerByController();
        }

        private void OnDestroy()
        {
            if (Managers.Game != null)
                Managers.Game.OnMoveChanged -= OnMoveChangedHandler;
        }

        public void MovePlayerByController()
        {
            Vector3 dir = _moveDir.normalized * _speed * Time.deltaTime;
            transform.position += dir;
        }

        private void OnMoveChangedHandler(Vector2 moveDir)
        {
            this._moveDir = moveDir;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            MonsterController target = other.gameObject.GetComponent<MonsterController>();
            if (target == null)
                return;
        }

        public override void OnDamaged(BaseController attacker, int damage)
        {
            base.OnDamaged(attacker, damage);
            Debug.Log($"OnDamaged !! Current : {Hp}");

            // TEMP
            CreatureController cc = attacker as CreatureController;
            cc?.OnDamaged(this, 10000);
        }
    }
}

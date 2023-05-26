using System.Collections;
using UnityEngine;

namespace STELLAREST_2D
{
    public class MonsterController : CreatureController
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            ObjectType = Define.ObjectType.Monster;

            return true;            
        }

        private void FixedUpdate()
        {
            PlayerController pc = Managers.Game.Player;
            if (pc == null)
                return;

            Vector3 toPlayer = pc.transform.position - transform.position;
            Vector3 newPos = transform.position + (toPlayer.normalized * Time.deltaTime * _speed);
            //transform.position = newPos;
            GetComponent<Rigidbody2D>().MovePosition(newPos);
            GetComponent<SpriteRenderer>().flipX = toPlayer.x > 0;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            PlayerController target = other.gameObject.GetComponent<PlayerController>();
            if (target == null)
                return;

            if (_coDotDamage != null)
                StopCoroutine(_coDotDamage);

            _coDotDamage = StartCoroutine(CoStartDotDamage(target));
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            PlayerController target = other.gameObject.GetComponent<PlayerController>();
            if (target == null)
                return;
            // if (target.isActiveAndEnabled == false)
            //     return;

            if (_coDotDamage != null)
                StopCoroutine(_coDotDamage);
            _coDotDamage = null;
        }

        private Coroutine _coDotDamage;
        public IEnumerator CoStartDotDamage(PlayerController target)
        {
            while (true)
            {
                // *** 데미지는 무조건 피해자쪽에서 처리하는것이 좋다 ***
                target.OnDamaged(this, 2); // 도트 데미지 예시..
                yield return new WaitForSeconds(0.1f);
            }
        }

        protected override void OnDead()
        {
            base.OnDead();

            if (_coDotDamage != null)
                StopCoroutine(_coDotDamage);
            _coDotDamage = null;

            //Managers.Object.Despawn<MonsterController>(this);
            Managers.Object.Despawn(this);
        }
    }
}

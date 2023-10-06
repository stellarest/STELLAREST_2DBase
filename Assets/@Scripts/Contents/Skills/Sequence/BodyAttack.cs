using System;
using System.Collections;
using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    public class BodyAttack : SequenceSkill
    {
        #region Temp Constant Options
        private const float DESIRED_TIME_TO_REACH = 0.15f;
        private const float DESIRED_TIME_TO_RETURN = 0.25f;
        private const float DESIRED_TIME_TO_END_WAIT = 0.75f; // origin : 0.5f
        #endregion
        private Vector3 _startReachPoint = Vector3.zero;
        private Vector3 _endReachPoint = Vector3.zero;

        private Vector3 _startReturnPoint = Vector3.zero;
        private Vector3 _endReturnPoint = Vector3.zero;
        private float _delta = 0f;

        [SerializeField] private AnimationCurve _curveEaseOut = null;

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            InitBodycolliderInfo();
        }

        public override void DoSkillJob(Action callback = null) 
            => StartCoroutine(CoDoBodyAttack(callback));

        private IEnumerator CoDoBodyAttack(Action callback = null)
        {
            if (this.Owner.MainTarget == null)
                yield break;

            Ready();
            while (true)
            {
                // NEED TO CALL FACE TO TARGET
                yield return new WaitUntil(() => ReachToTarget());
                this.Owner.CreatureState = Define.CreatureState.Idle;
                yield return new WaitUntil(() => Return());
                yield return new WaitUntil(() => EndWait());
                this.Owner.CreatureState = Define.CreatureState.Run;
                yield break;
                //callback?.Invoke();
            }
        }

        private void Ready()
        {
            this.HitCollider.enabled = true;
            _startReachPoint = this.Owner.transform.position;
            _endReachPoint = this.Owner.MainTarget.Center.transform.position;
        }

        private bool ReachToTarget()
        {
            _delta += Time.deltaTime;
            float percent = _delta / DESIRED_TIME_TO_REACH;
            Utils.Log($"ReachToTarget Percent : {percent}");
            this.Owner.transform.position = Vector3.Lerp(_startReachPoint, _endReachPoint, percent);
            if (percent > 1f)
            {
                this.HitCollider.enabled = false;
                _delta = 0f;
                _startReturnPoint = this.Owner.Center.transform.position;
                _endReturnPoint = _startReachPoint;
                return true;
            }

            return false;
        }

        private bool Return()
        {
            _delta += Time.deltaTime;
            float percent = _delta / DESIRED_TIME_TO_RETURN;
            this.Owner.transform.position = Vector3.Lerp(_startReturnPoint, _endReturnPoint, _curveEaseOut.Evaluate(percent));
            if (percent > 1f)
            {
                _delta = 0f;
                return true;
            }

            return false;
        }

        private bool EndWait()
        {
            _delta += Time.deltaTime;
            float percent = _delta / DESIRED_TIME_TO_END_WAIT;
            if (percent > 1f)
            {
                _delta = 0f;
                return true;
            }

            return false;
        }

        private void InitBodycolliderInfo()
        {
            CircleCollider2D ownerHitBody = this.Owner.GetComponent<Collider2D>() as CircleCollider2D;
            HitCollider = GetComponent<Collider2D>();
            CircleCollider2D circle = HitCollider as CircleCollider2D;
            circle.offset = new Vector2(ownerHitBody.offset.x, ownerHitBody.offset.y);
            circle.radius = ownerHitBody.radius;
            HitCollider.enabled = false;

            if (this.Owner?.IsPlayer() == false)
                Managers.Collision.InitCollisionLayer(this.gameObject, Define.CollisionLayers.MonsterAttack);
            else
                Managers.Collision.InitCollisionLayer(this.gameObject, Define.CollisionLayers.PlayerAttack);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc?.IsValid() == false)
                return;

            cc?.OnDamaged(this.Owner, this);
        }
    }
}

// namespace STELLAREST_2D
// {
//     public class BodyAttack : SequenceSkill
//     {
//         private Coroutine _coroutine;
//         private Vector3 _startAttackPoint;
//         private CircleCollider2D _attackCol;

//         public override void SetSkillInfo(CreatureController owner, int templateID)
//         {
//             base.SetSkillInfo(owner, templateID);
//             transform.localPosition = Vector3.zero;
//             transform.localScale = Vector3.one; // ???

//             _attackCol = GetComponent<CircleCollider2D>();
//             _attackCol.enabled = false;

//             if (Owner?.IsMonster() == true)
//                 Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.MonsterAttack);
//         }

//         public override void DoSkill(System.Action callback = null)
//         {
//             if (_coroutine != null)
//                 StopCoroutine(_coroutine);

//             _coroutine = StartCoroutine(CoBodyAttack(callback));
//         }

//         private bool _returnBody = false;
//         private IEnumerator CoBodyAttack(System.Action callback)
//         {
//             _startAttackPoint = Owner.transform.position;
//             Vector3 target = Managers.Game.Player.transform.position;

//             float elapsedTime = 0f;
//             float desiredReachTime = 0.15f;
//             float percent = 0f;
//             //float endWait = 1f;
//             float endWait = 0.5f;

//             Managers.Sprite.SetMonsterFace(Owner as MonsterController, Define.MonsterFace.Angry);
//             mc.CreatureState = Define.CreatureState.Attack;
//             _attackCol.enabled = true;
//             while (percent < 1f)
//             {
//                 elapsedTime += Time.deltaTime;
//                 percent = elapsedTime / desiredReachTime;
//                 if (percent < 1f)
//                     Owner.transform.position = Vector3.Lerp(_startAttackPoint, target, percent);
//                 else
//                 {
//                     _attackCol.enabled = false;
//                     yield return CoBodyReturn(callback, _startAttackPoint, target);
//                     yield return new WaitUntil(() => _returnBody);
//                     mc.CreatureState = Define.CreatureState.Idle;
//                     yield return new WaitForSeconds(endWait);
//                     Managers.Sprite.SetMonsterFace(Owner as MonsterController, Define.MonsterFace.Normal);
//                     mc.CreatureState = Define.CreatureState.Run;
//                     yield break;
//                 }

//                 yield return null;
//             }
//         }

//         private IEnumerator CoBodyReturn(System.Action callback, Vector3 initPos, Vector3 target)
//         {
//             float elapsedTime = 0f;
//             float desiredReturnTime = 0.25f;
//             float percent = 0f;
//             while (percent < 1f)
//             {
//                 elapsedTime += Time.deltaTime;
//                 percent = elapsedTime / desiredReturnTime;
//                 Owner.transform.position = Vector3.Lerp(target, initPos, percent);
//                 yield return null;
//             }
//             _returnBody = true;
//             // Owner.BodyCol.enabled = true;
//         }

//         private void OnTriggerEnter2D(Collider2D other)
//         {
//             if (Owner.IsValid() == false || Managers.Game.Player.IsValid() == false)
//                 return;

//             if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.PlayerBody, other.gameObject.layer))
//             {
//                 Managers.Game.Player.OnDamaged(Owner, this);
//             }
//         }

//         public override void OnPreSpawned() => base.OnPreSpawned();
//     }
// }

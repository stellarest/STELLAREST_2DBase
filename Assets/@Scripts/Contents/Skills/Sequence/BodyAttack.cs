using System;
using System.Collections;
using STELLAREST_2D.Data;
using UnityEngine;

using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

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

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            InitBodyColliderInfo();
        }

        public override void DoSkillJob(Action callback = null)
        {
            StartCoroutine(CoDoBodyAttack(delegate()
            {
                //this.Owner.SkillBook.Deactivate(SkillTemplate.BodyAttack);
                this.Owner.SkillBook.RandomizeSequenceGroup(SkillTemplate.BodyAttack);
            }));
        }

        private IEnumerator CoDoBodyAttack(Action callback = null)
        {
            if (this.Owner.MainTarget == null)
                yield break;

            Ready();
            yield return new WaitUntil(() => ReachToTarget());
            yield return new WaitUntil(() => Return());
            this.Owner.CreatureState = Define.CreatureState.Idle;
            yield return new WaitForSeconds(DESIRED_TIME_TO_END_WAIT);
            this.Owner.CreatureState = Define.CreatureState.Run;
            callback?.Invoke();
        }

        private void Ready()
        {
            _delta = 0f;
            this.HitCollider.enabled = true;
            _startReachPoint = this.Owner.transform.position;
            _endReachPoint = this.Owner.MainTarget.Center.transform.position;
        }

        private bool ReachToTarget()
        {
            _delta += Time.deltaTime;
            float percent = (_delta / DESIRED_TIME_TO_REACH) * Owner.SpeedModifier;
            this.Owner.transform.position = Vector3.Lerp(_startReachPoint, _endReachPoint, percent);
            if (percent > 1f)
            {
                _delta = 0f;
                this.HitCollider.enabled = false;
                _startReturnPoint = this.Owner.transform.position;
                _endReturnPoint = _startReachPoint;
                return true;
            }

            return false;
        }

        private bool Return()
        {
            _delta += Time.deltaTime;
            float percent = (_delta / DESIRED_TIME_TO_RETURN)  * Owner.SpeedModifier;
            this.Owner.transform.position = Vector3.Lerp(_startReturnPoint, _endReturnPoint, percent);
            if (percent > 1f)
            {
                _delta = 0f;
                return true;
            }

            return false;
        }

        private void InitBodyColliderInfo()
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

using System;
using System.Collections;
using STELLAREST_2D.Data;
using UnityEngine;

using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;
using CrowdControl = STELLAREST_2D.Define.TemplateIDs.CrowdControl;

namespace STELLAREST_2D
{
    public class BodyAttack : SequenceSkill
    {
        #region Constant Options (Temp)
        private const float DESIRED_TIME_TO_REACH = 0.15f;
        private const float DESIRED_TIME_TO_RETURN = 0.25f;
        private const float DESIRED_TIME_TO_END_WAIT = 0.75f; // origin : 0.5f
        //private const float DESIRED_TIME_TO_END_WAIT = 5.75f; // origin : 0.5f // SKILL DATA COOL TIME으로 하면 될텐데,,,

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
                //this.Owner.SkillBook.ReserveNextSequence((SkillTemplate.BodyAttack);
                //this.Owner.SkillBook.ReserveNextSequence(currentEnd: SkillTemplate.BodyAttack);
                this.Owner.SetDefaultHead();
                this.Owner.SkillBook.RandomizeSequenceGroup(SkillTemplate.BodyAttack);
            }));
        }

        private IEnumerator CoDoBodyAttack(Action callback = null)
        {
            if (CanStartAction() == false)
            {
                if (this.Owner.CreatureState == Define.CreatureState.Skill)
                    this.Owner.CreatureState = Define.CreatureState.Run;

                Utils.Log("Deactivate Body Attack."); // 임시 개선 사항 (Deaictvate Sequence 추가해야함, 안해도 상관 없을것같긴하지만)
                this.Owner.SkillBook.Deactivate(SkillTemplate.BodyAttack);
                yield break;
            }

            Ready();
            yield return new WaitUntil(() => ReachToTarget());
            yield return new WaitUntil(() => Return());
            this.Owner.CreatureState = Define.CreatureState.Idle;
            yield return new WaitForSeconds(DESIRED_TIME_TO_END_WAIT);
            this.Owner.CreatureState = Define.CreatureState.Run;
            callback?.Invoke();
        }

        private bool CanStartAction()
        {
            if (this.Owner.MainTarget == null)
                return false;
            else if ((this.Owner.MainTarget.Center.position - this.Owner.Center.position).sqrMagnitude > this.Owner.Stat.CollectRange * this.Owner.Stat.CollectRange)
                return false;

            return true;
        }

        private void Ready()
        {            
            _delta = 0f;
            this.HitCollider.enabled = true;
            _startReachPoint = this.Owner.transform.position;
            _endReachPoint = this.Owner.MainTarget.Center.transform.position;
            this.Owner.SetBattleHead();
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

            HitPoint = other.ClosestPoint(this.transform.position);
            cc?.OnDamaged(this.Owner, this);
        }
    }
}

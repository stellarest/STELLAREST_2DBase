using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class ProjectileController : SkillBase
    {
        private Rigidbody2D _rigid;
        public SkillBase CurrentSkill { get; private set; }

        public override bool Init()
        {
            base.Init();
            ObjectType = Define.ObjectType.Projectile;

            _rigid = GetComponent<Rigidbody2D>();
            return true;
        }

        private Vector3 _shootDir;
        private float _speed;
        private float _initialTurningDir = 0f; // turn -> turn 하면 다시 파티클이 플레이어를 쫓아가는 것을 막아줌, 필요시 사용
        private bool _offParticle = false;
        private float _continuousSpeedRatio = 1f;

        private int _currentBounceCount = 0;
        private GameObject _target = null;

        public void SetProjectileInfo(CreatureController owner, SkillBase currentSkill, Vector3 shootDir, Vector3 spawnPos, Vector3 localScale, Vector3 indicatorAngle,
                    float turningSide = 0f, float continuousSpeedRatio = 1f, float continuousAngle = 0f, float continuousFlipX = 0f)
        {
            this.Owner = owner;
            if (owner?.IsPlayer() == true)
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
            else
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.MonsterAttack);

            this.CurrentSkill = currentSkill;
            this.SkillData = currentSkill.SkillData;
            this._shootDir = shootDir;
            this._continuousSpeedRatio = continuousSpeedRatio;

            _initialTurningDir = turningSide;
            _offParticle = false;

            transform.position = spawnPos;
            transform.localScale = localScale;

            StartDestroy(currentSkill.SkillData.Duration);
            switch (currentSkill.SkillData.OriginTemplateID)
            {
                case (int)Define.TemplateIDs.SkillType.PaladinSwing:
                    {
                        GetComponent<PaladinSwing>().SetSwingInfo(owner, currentSkill.SkillData.TemplateID, indicatorAngle, 
                                    turningSide, continuousAngle, continuousFlipX);
                        StartCoroutine(CoPaladinSwing());
                    }
                    break;

                case (int)Define.TemplateIDs.SkillType.ThrowingStar:
                    {
                        _target = null;
                        StartCoroutine(CoThrowingStart());
                    }
                    break;
            }
        }

        private IEnumerator CoThrowingStart()
        {
            float selfRot = 0f;
            while (true)
            {
                selfRot += CurrentSkill.SkillData.SelfRotationZSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Euler(0, 0, selfRot);
                float movementSpeed = Owner.CharaData.MoveSpeed + SkillData.Speed;

                transform.position += _shootDir * movementSpeed * Time.deltaTime;
                yield return null;
            }
        }

        //float desiredCompletedTime = skillData.Duration;
        private IEnumerator CoPaladinSwing()
        {
            float projectileSpeed = SkillData.Speed * _continuousSpeedRatio;
            while (true)
            {
                // ControlCollisionTime(SkillData.Duration - 0.1f); // 필요할 때 사용
                if (Managers.Game.Player.IsMoving && _offParticle == false) // Moving Melee Attack
                {
                    if (Managers.Game.Player.IsInLimitMaxPosX || Managers.Game.Player.IsInLimitMaxPosY)
                    {
                        // float minSpeed = Managers.Game.Player.MovementPower + SkillData.Speed;
                        // float maxSpeed = Owner.CreatureData.MoveSpeed + SkillData.Speed;
                        float minSpeed = Managers.Game.Player.MovementPower + projectileSpeed;
                        float maxSpeed = Owner.CharaData.MoveSpeed + projectileSpeed;

                        float movementPowerRatio = Managers.Game.Player.MovementPower / Owner.CharaData.MoveSpeed;
                        _speed = Mathf.Lerp(minSpeed, maxSpeed, movementPowerRatio);
                    }
                    else
                        _speed = Owner.CharaData.MoveSpeed + projectileSpeed;
                }
                else // Static Melee Attack
                    _speed = projectileSpeed;

                SetOffParticle();
                transform.position += _shootDir * _speed * Time.deltaTime;
                ControlCollisionTime(SkillData.Duration);

                yield return null;
            }
        }

        private float _sensitivity = 0.6f;
        private void SetOffParticle()
        {
            if (_initialTurningDir != Managers.Game.Player.TurningAngle)
                _offParticle = true;
            if (Managers.Game.Player.IsMoving == false)
                _offParticle = true;
            // if (Vector3.Distance(Managers.Game.Player.ShootDir, _shootDir) > _sensitivity)
            //     _offParticle = true;
            if ((Managers.Game.Player.ShootDir - _shootDir).sqrMagnitude > _sensitivity * _sensitivity)
                _offParticle = true;
        }

        float delta = 0f;
        private void ControlCollisionTime(float controlTime)
        {
            delta += Time.deltaTime;
            if (controlTime <= delta)
            {
                GetComponent<Collider2D>().enabled = false;
                delta = 0f;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // SkillData.PenetrationCount // -1 : 무제한 관통
            MonsterController mc = other.GetComponent<MonsterController>();
            if (mc.IsValid() == false)
                return;

            if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.MonsterBody, other.gameObject.layer))
            {
                switch (CurrentSkill.SkillData.OriginTemplateID)
                {
                    case (int)Define.TemplateIDs.SkillType.PaladinSwing:
                        {
                            mc.OnDamaged(Owner, CurrentSkill);
                        }
                        break;

                    case (int)Define.TemplateIDs.SkillType.ThrowingStar:
                        if (_currentBounceCount == CurrentSkill.SkillData.BounceCount)
                        {
                            _currentBounceCount = 0;
                            mc.OnDamaged(Owner, CurrentSkill);
                            Managers.Object.ResetBounceHits(Define.TemplateIDs.SkillType.ThrowingStar);
                            Managers.Object.Despawn(this.GetComponent<ProjectileController>());
                        }
                        else
                        {
                            _currentBounceCount++;
                            mc.OnDamaged(Owner, CurrentSkill);
                            mc.IsThrowingStarBounceHit = true;
                            _target = Managers.Object.GetClosestTarget(mc.transform, Define.TemplateIDs.SkillType.ThrowingStar);
                            if (_target != null)
                                _shootDir = (_target.transform.position - transform.position).normalized;
                            else
                            {
                                _currentBounceCount = 0;
                                Managers.Object.ResetBounceHits(Define.TemplateIDs.SkillType.ThrowingStar);
                                Managers.Object.Despawn(this.GetComponent<ProjectileController>());
                            }
                        }
                        break;
                }
            }
        }
    }
}

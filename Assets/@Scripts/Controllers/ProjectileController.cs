using System.Collections;
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

        private bool _isOnInterpolateScale = false;
        private Vector2 _interpolateStartScale = Vector2.zero;
        private Vector2 _interpolateTargetScale = Vector2.zero;

        public void SetProjectileInfo(CreatureController owner, SkillBase currentSkill, Vector3 shootDir, Vector3 spawnPos, Vector3 localScale, Vector3 indicatorAngle, float turningSide = 0f, 
                    float continuousSpeedRatio = 1f, float continuousAngle = 0f, float continuousFlipX = 0f, float continuousFlipY = 0f, 
                    float shootDirectionIntensity = 1f, float? interPolTargetX = null, float? interPolTargetY = null)
        {
            this.Owner = owner;
            this.CurrentSkill = currentSkill;

            this.SkillData = currentSkill.SkillData;
            this._shootDir = shootDir * shootDirectionIntensity;
            this._continuousSpeedRatio = continuousSpeedRatio;

            _initialTurningDir = turningSide;
            _offParticle = false;

            transform.position = spawnPos;

            // +++++ Interpolate Scales +++++
            transform.localScale = localScale;
            if (interPolTargetX.HasValue && interPolTargetY.HasValue)
            {
                _isOnInterpolateScale = true;
                _interpolateStartScale = localScale;
                _interpolateTargetScale = new Vector2(interPolTargetX.Value, interPolTargetY.Value);
            }
            else
                _isOnInterpolateScale = false;

            StartDestroy(currentSkill.SkillData.Duration);
            switch (currentSkill.SkillData.OriginTemplateID)
            {
                case (int)Define.TemplateIDs.SkillType.PaladinMeleeSwing:
                case (int)Define.TemplateIDs.SkillType.KnightMeleeSwing:
                case (int)Define.TemplateIDs.SkillType.PhantomKnightMeleeSwing:
                case (int)Define.TemplateIDs.SkillType.WarriorMeleeSwing:
                case (int)Define.TemplateIDs.SkillType.BerserkerMeleeSwing:
                    {
                        GetComponent<MeleeSwing>().SetSwingInfo(owner, currentSkill.SkillData.TemplateID, indicatorAngle,
                                    turningSide, continuousAngle, continuousFlipX, continuousFlipY);

                        if (currentSkill.SkillData.OriginTemplateID == (int)Define.TemplateIDs.SkillType.PhantomKnightMeleeSwing || 
                            currentSkill.SkillData.OriginTemplateID == (int)Define.TemplateIDs.SkillType.WarriorMeleeSwing || 
                            currentSkill.SkillData.OriginTemplateID == (int)Define.TemplateIDs.SkillType.BerserkerMeleeSwing)
                            _shootDir = Quaternion.Euler(0, 0, continuousAngle * -1) * _shootDir;

                        StartCoroutine(CoMeleeSwing());
                    }
                    break;

                case (int)Define.TemplateIDs.SkillType.ThrowingStar:
                    {
                        _target = null;
                        StartCoroutine(CoThrowingStar());
                    }
                    break;

                case (int)Define.TemplateIDs.SkillType.Boomerang:
                    {
                        if (SkillData.InGameGrade != Define.InGameGrade.Legendary)
                            StartCoroutine(CoBoomerang());
                        else
                            StartCoroutine(CoBoomerangLegendary());
                    }
                    break;
            }
        }

        //float desiredCompletedTime = skillData.Duration;
        private IEnumerator CoMeleeSwing()
        {
            float projectileSpeed = SkillData.Speed * _continuousSpeedRatio;
            controllColDelta = 0f;
            float t = 0f;

            while (true)
            {
                if (Managers.Game.Player.IsMoving && _offParticle == false && CurrentSkill.SkillData.Speed != 0f) // Moving Melee Attack
                {
                    if (Managers.Game.Player.IsInLimitMaxPosX || Managers.Game.Player.IsInLimitMaxPosY)
                    {
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

                if (_isOnInterpolateScale)
                {
                    t += Time.deltaTime;
                    float percent = t / SkillData.Duration;
                    transform.localScale = Vector2.Lerp(_interpolateStartScale, _interpolateTargetScale, percent);
                }

                ControlCollisionTime(SkillData.Duration * SkillData.CollisionKeepingRatio);

                yield return null;
            }
        }

        private IEnumerator CoThrowingStar()
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

        private IEnumerator CoBoomerang()
        {
            float selfRot = 0f;
            float currentSpeed = Owner.CharaData.MoveSpeed + SkillData.Speed;
            float deceleration = 50f; // 감속 속도를 조절하려면 필요에 따라 값을 변경

            while (true)
            {
                // 부메랑 회전
                selfRot += CurrentSkill.SkillData.SelfRotationZSpeed * Time.deltaTime * 1.1f;
                transform.rotation = Quaternion.Euler(0, 0, selfRot);

                currentSpeed -= deceleration * Time.deltaTime;
                deceleration += 0.05f;
                transform.position += _shootDir * currentSpeed * Time.deltaTime;

                yield return null;
            }
        }

        private IEnumerator CoBoomerangLegendary()
        {
            BoomerangChild child = transform.GetChild(0).GetComponent<BoomerangChild>();
            if (child.Trail != null)
                child.Trail.enabled = false;

            bool canStartChildRot = false;
            bool isToOwner = false;

            float waitdelta = 0f;
            float selfRot = 0f;
            float currentSpeed = Owner.CharaData.MoveSpeed + SkillData.Speed;
            float deceleration = 50f;

            while (true)
            {
                selfRot += CurrentSkill.SkillData.SelfRotationZSpeed * Time.deltaTime * 1.5f;
                selfRot %= 360f;
                transform.rotation = Quaternion.Euler(0, 0, selfRot);

                if (isToOwner == false)
                {
                    currentSpeed -= deceleration * Time.deltaTime;
                    deceleration += 0.05f;
                    transform.position += _shootDir * currentSpeed * Time.deltaTime;
                }
                else if(isToOwner && child.IsReadyToOwner)
                {
                    child.Trail.enabled = false;
                    Vector3 toOwnerDir = (Owner.transform.position - transform.position).normalized;
                    transform.position += toOwnerDir * (SkillData.Speed) * Time.deltaTime;

                    if ((transform.position - Owner.transform.position).sqrMagnitude < 1)
                    {
                        canStartChildRot = false;
                        isToOwner = false;
                        StopDestroy();
                        Managers.Resource.Destroy(gameObject);
                    }
                }

                if (canStartChildRot)
                    child.RotateAround(isToOwner);

                if (currentSpeed < 0 && canStartChildRot == false || Managers.Stage.IsOutOfPos(transform.position) && canStartChildRot == false)
                {
                    child.RotStartTime = Time.time;
                    child.Trail.enabled = true;
                    yield return new WaitUntil(() => StopAndChildRotation(child, ref selfRot, ref waitdelta, ref canStartChildRot, ref isToOwner));
                }

                yield return null;
            }
        }

        private bool StopAndChildRotation(BoomerangChild child, ref float selfRot, ref float waitDelta, ref bool canStartChildRot, ref bool isToOwner)
        {
            waitDelta += Time.deltaTime;

            selfRot += CurrentSkill.SkillData.SelfRotationZSpeed * Time.deltaTime * 1.5f;
            selfRot %= 360f;
            transform.rotation = Quaternion.Euler(0, 0, selfRot);

            child.RotateAround();
            if (waitDelta > child.DesiredSelfRotTime)
            {
                waitDelta = 0f;
                isToOwner = true;
                canStartChildRot = true;
                child.ToOwnerStartTime = Time.time;
                return true;
            }
            
            return false;
        }

        private readonly float _sensitivity = 0.6f;
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

        float controllColDelta = 0f;
        private void ControlCollisionTime(float controlTime)
        {
            controllColDelta += Time.deltaTime;
            if (controlTime <= controllColDelta)
            {
                GetComponent<Collider2D>().enabled = false;
                controllColDelta = 0f;
            }
        }

        private IEnumerator CoDotDamage<T>(T cc, float delay = 0.15f) where T : CreatureController
        {
            float t = 0f;
            while (t < delay)
            {
                t += Time.deltaTime;
                yield return null;
            }
            cc.OnDamaged(Owner, CurrentSkill);
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
                    case (int)Define.TemplateIDs.SkillType.PaladinMeleeSwing:
                    case (int)Define.TemplateIDs.SkillType.KnightMeleeSwing:
                    case (int)Define.TemplateIDs.SkillType.PhantomKnightMeleeSwing:
                        {
                            mc.OnDamaged(Owner, CurrentSkill);
                        }
                        break;

                    case (int)Define.TemplateIDs.SkillType.WarriorMeleeSwing:
                    case (int)Define.TemplateIDs.SkillType.BerserkerMeleeSwing:
                        {
                            mc.OnDamaged(Owner, CurrentSkill);
                            // 맞는듯
                            // Vector3 hitPoint = other.ClosestPoint(new Vector2(transform.position.x, transform.position.y));
                            // GameObject go = new GameObject() { name = "AA" };
                            // go.transform.position = hitPoint;
                            // Debug.Log("CHECK HIT POINT");
                            // Debug.Break();
                        }
                        break;

                    case (int)Define.TemplateIDs.SkillType.ThrowingStar:
                        if (_currentBounceCount == CurrentSkill.SkillData.BounceCount)
                        {
                            _currentBounceCount = 0;
                            mc.OnDamaged(Owner, CurrentSkill);
                            Managers.Object.ResetSkillHittedStatus(Define.TemplateIDs.SkillType.ThrowingStar);
                            Managers.Object.Despawn(this.GetComponent<ProjectileController>());
                        }
                        else
                        {
                            _currentBounceCount++;
                            mc.OnDamaged(Owner, CurrentSkill);
                            mc.IsThrowingStarHit = true;
                            _target = Managers.Object.GetNextTarget(mc.gameObject, Define.TemplateIDs.SkillType.ThrowingStar);
                            if (_target != null)
                                _shootDir = (_target.transform.position - transform.position).normalized;
                            else
                            {
                                _currentBounceCount = 0;
                                Managers.Object.ResetSkillHittedStatus(Define.TemplateIDs.SkillType.ThrowingStar);
                                Managers.Object.Despawn(this.GetComponent<ProjectileController>());
                            }
                        }
                        break;

                    case (int)Define.TemplateIDs.SkillType.Boomerang:
                        {
                            mc.OnDamaged(Owner, CurrentSkill);
                        }
                        break;
                }
            }
        }
    }
}

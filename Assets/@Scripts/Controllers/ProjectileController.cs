using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

using static STELLAREST_2D.Define;

namespace STELLAREST_2D
{
    public class ProjectileLaunchInfoEventArgs : System.EventArgs
    {
        public ProjectileLaunchInfoEventArgs(ProjectileLaunchInfoEventArgs arg)
        {
            this.InitialLookAtDir = arg.InitialLookAtDir;
            this.ShootDir = arg.ShootDir;
            this.IndicatorAngle = arg.IndicatorAngle;
            this.StartLocalScale = arg.StartLocalScale;
            this.Angle = arg.Angle;
            this.FlipX = arg.FlipX;
            this.FlipY = arg.FlipY;
            this.AddMovementSpeedRatio = arg.AddMovementSpeedRatio;
            this.AddRotationSpeedRatio = arg.AddRotationSpeedRatio;
            if (arg.TargetScale.x > Mathf.Abs(arg.StartLocalScale.x) || arg.TargetScale.y > Mathf.Abs(arg.StartLocalScale.y))
            {
                this.TargetScale = new Vector3(arg.TargetScale.x, arg.TargetScale.y, 1f);
                this.IsOnTargetScale = true;
            }

            this.Duration = arg.Duration;
            this.MovementSpeed = arg.MovementSpeed;
            this.RotationSpeed = arg.RotationSpeed;
            this.MaxBounceCount = arg.MaxBounceCount;
            this.MaxPenetrationCount = arg.MaxPenetrationCount;
        }

        public ProjectileLaunchInfoEventArgs(LookAtDirection lootAtDir, Vector3 shootDir, Vector3 indicatorAngle, Vector3 localScale,
                    float angle, float flipX, float flipY, float addMovementSpeedRatio, float addRotationSpeedRatio, float targetScaleX, 
                    float targetScaleY, float duration, float movementSpeed, float rotationSpeed, int maxBounceCount, int maxPenetrationCount)
        {
            this.InitialLookAtDir = lootAtDir;
            this.ShootDir = shootDir;
            this.IndicatorAngle = indicatorAngle;
            this.StartLocalScale = localScale;
            this.Angle = angle;
            this.FlipX = flipX;
            this.FlipY = flipY;
            this.AddMovementSpeedRatio = addMovementSpeedRatio;
            this.AddRotationSpeedRatio = addRotationSpeedRatio;
            if (targetScaleX > Mathf.Abs(localScale.x) && targetScaleY > Mathf.Abs(localScale.y))
            {
                TargetScale = new Vector3(targetScaleX, targetScaleY, 1);
                this.IsOnTargetScale = true;
            }
            else if (targetScaleX > Mathf.Abs(localScale.x))
            {
                TargetScale = new Vector3(targetScaleX, localScale.y, 1f);
                this.IsOnTargetScale = true;
            }
            else if (targetScaleY > Mathf.Abs(localScale.y))
            {
                TargetScale = new Vector3(localScale.x, targetScaleY, 1f);
                this.IsOnTargetScale = true;
            }
            else
                TargetScale = new Vector3(localScale.x, localScale.y, 1f);

            this.Duration = duration;
            this.MovementSpeed = movementSpeed;
            this.RotationSpeed = rotationSpeed;

            this.MaxBounceCount = maxBounceCount;
            this.MaxPenetrationCount = maxPenetrationCount;
        }

        public Define.LookAtDirection InitialLookAtDir { get; private set; } = Define.LookAtDirection.Right;
        public Vector3 ShootDir { get; set; } = Vector3.zero;
        public Vector3 IndicatorAngle { get; set; } = Vector3.zero;
        public Vector3 StartLocalScale { get; private set; } = Vector3.zero;

        public float Angle { get; set; } = 0f;
        public float FlipX { get; set; } = 0f;
        public float FlipY { get; set; } = 0f;
        public float AddMovementSpeedRatio { get; set; } = 0f;
        public float AddRotationSpeedRatio { get; set; } = 0f;
        public float InterpolateTargetScaleX { get; private set; } = 0f;
        public float InterpolateTargetScaleY { get; private set; } = 0f;
        public Vector3 TargetScale { get; private set; } = Vector3.zero;
        public bool IsOnTargetScale { get; private set; } = false;

        public float Duration { get; set; } = 0f;
        public float MovementSpeed { get; set; } = 0f;
        public float RotationSpeed { get; set; } = 0f;

        public int MaxBounceCount { get; private set; } = 0;
        public int MaxPenetrationCount { get; private set; } = 0;
    }

    public class ProjectileController : SkillBase
    {
        public ProjectileLaunchInfoEventArgs Value { get; private set; } = null;
        public int BounceCount { get; private set; } = 0;
        public int PenetrationCount { get; private set; } = 0;
        private bool _isOffFromOwner = false;

        public bool CanStillBounce => (this.BounceCount++ < Value.MaxBounceCount)
                ?   Managers.Object.Monsters.Count != this.BounceCount
                :   false;

        private Coroutine _coProjectile = null;
        public void OnProjectileLaunchInfoHandler(object sender, ProjectileLaunchInfoEventArgs e)
        {
            this.Value = new ProjectileLaunchInfoEventArgs(e);
            this._isOffFromOwner = false;
        }

        // TEMP
        public void SetOptionsManually(Vector3 shootDir, float movementSpeed, float duration,
                                float addMovementSpeedRatio, float angle)
        {
            this.Value.ShootDir = shootDir;
            this.Value.MovementSpeed = movementSpeed;
            this.Value.Duration = duration;
            this.Value.AddMovementSpeedRatio = addMovementSpeedRatio;
            this.Value.Angle = angle;
        }

        public void Launch(FixedValue.TemplateID.Skill skillType)
        {
            transform.localScale = new Vector3(this.Value.StartLocalScale.x, this.Value.StartLocalScale.y, 1f);
            StartCoroutine(CoColliderDuration(this.Value.Duration * 0.5f));
            switch (skillType)
            {
                // +++++ GARY +++++
                case FixedValue.TemplateID.Skill.Paladin_Unique_Mastery:
                case FixedValue.TemplateID.Skill.PhantomKnightMastery:
                    StartDestroy(this.Value.Duration);
                    OnSetParticleInfo?.Invoke(this.Value.IndicatorAngle, this.Value.InitialLookAtDir, this.Value.Angle, this.Value.FlipX, this.Value.FlipY);
                    _coProjectile = StartCoroutine(CoMeleeSwing());
                    break;
                case FixedValue.TemplateID.Skill.KnightMastery:
                    StartDestroy(this.Value.Duration);
                    OnSetParticleInfo?.Invoke(this.Value.IndicatorAngle, this.Value.InitialLookAtDir, this.Value.Angle, this.Value.FlipX, this.Value.FlipY);
                    this.Value.ShootDir = Quaternion.Euler(0, 0, this.Value.Angle * -1) * this.Value.ShootDir;
                    _coProjectile = StartCoroutine(CoMeleeSwing());
                    break;

                // +++++ REINA +++++
                case FixedValue.TemplateID.Skill.ArrowMasterMastery:
                case FixedValue.TemplateID.Skill.ForestGuardianMastery:
                    StartDestroy(this.Value.Duration);
                    _coProjectile = StartCoroutine(CoRangedShot());
                    break;
                case FixedValue.TemplateID.Skill.ElementalArcherMastery:
                    StartDestroy(this.Value.Duration);
                    if (this.Data.Grade < Define.InGameGrade.Ultimate)
                        _coProjectile = StartCoroutine(CoRangedShot());
                    else
                        _coProjectile = StartCoroutine(CoRangedGuidedShot());
                    break;

                // +++++ KENNETH +++++
                case FixedValue.TemplateID.Skill.AssassinMastery:
                case FixedValue.TemplateID.Skill.Assassin_Unique_Elite_C1:
                case FixedValue.TemplateID.Skill.Ninja_Unique_Elite_C1:
                    StartDestroy(this.Value.Duration);
                    OnSetParticleInfo?.Invoke(this.Value.IndicatorAngle, this.Value.InitialLookAtDir, this.Value.Angle, this.Value.FlipX, this.Value.FlipY);
                    _coProjectile = StartCoroutine(CoMeleeSwing());
                    break;
                case FixedValue.TemplateID.Skill.NinjaMastery:
                    StartDestroy(this.Value.Duration);
                    _coProjectile = StartCoroutine(CoRangedShot());
                    break;

                // +++++ PUBLIC SKILLS +++++
                case FixedValue.TemplateID.Skill.ThrowingStar:
                    StartDestroy(this.Value.Duration);
                    _coProjectile = StartCoroutine(CoThrowingStar());
                    break;

                case FixedValue.TemplateID.Skill.Boomerang:
                    if (Data.Grade < Data.MaxGrade)
                    {
                        StartDestroy(this.Value.Duration);
                        _coProjectile = StartCoroutine(CoBoomerang());
                    }
                    else
                        _coProjectile = StartCoroutine(CoBoomerangUltimate());
                    break;

                case FixedValue.TemplateID.Skill.PhantomKnight_Unique_Elite_C1:
                    StartDestroy(this.Value.Duration);
                    _coProjectile = StartCoroutine(CoPhantomSoulChild());
                    break;
            }

            if (this.Value.IsOnTargetScale)
                StartCoroutine(CoScaleInterpolation());
        }

        private IEnumerator CoMeleeSwing()
        {
            float movementSpeed = this.Value.MovementSpeed * this.Value.AddMovementSpeedRatio;
            while (true)
            {
                if (this.Owner.IsMoving && this._isOffFromOwner == false && this.Value.MovementSpeed != 0)
                {
                    if (this.Owner.IsInLimitMaxPosX && this.Owner.IsInLimitMaxPosY)
                    {
                        float minSpeed = this.Owner.GetMovementPower + movementSpeed;
                        float maxSpeed = this.Owner.Stat.MovementSpeed + movementSpeed;
                        float movementPowerRatio = this.Owner.GetMovementPower / this.Owner.Stat.MovementSpeed;
                        this.Value.MovementSpeed = Mathf.Lerp(minSpeed, maxSpeed, movementPowerRatio);
                    }
                    else
                        this.Value.MovementSpeed = this.Owner.Stat.MovementSpeed + movementSpeed;
                }
                else
                    this.Value.MovementSpeed = movementSpeed;

                SetSwingOffFromOwner();
                transform.position += this.Value.ShootDir * this.Value.MovementSpeed * Time.deltaTime;

                yield return null;
            }
        }

        private IEnumerator CoRangedShot()
        {
            //_shootDir = Quaternion.Euler(0, 0, _continuousAngle) * this.Owner.ShootDir;
            this.Value.ShootDir = Quaternion.Euler(0, 0, this.Value.Angle) * this.Value.ShootDir;

            float degrees = Mathf.Atan2(this.Value.ShootDir.y, this.Value.ShootDir.x) * Mathf.Rad2Deg;
            this.transform.rotation = Quaternion.Euler(0, 0, degrees);
            this.transform.localScale = Vector3.one;
            float movementSpeed = this.Value.MovementSpeed * this.Value.AddMovementSpeedRatio;

            while (true)
            {
                this.transform.position += this.Value.ShootDir * movementSpeed * Time.deltaTime;
                yield return null;
            }
        }

        private const float RANGED_GUIDED_SHOT_ROT_SPEED = 80f;
        private IEnumerator CoRangedGuidedShot()
        {
            float degrees = Mathf.Atan2(this.Value.ShootDir.y, this.Value.ShootDir.x) * Mathf.Rad2Deg;
            this.transform.rotation = Quaternion.Euler(0, 0, degrees);
            this.transform.localScale = Vector3.one;
            while (true)
            {
                CreatureController target = Utils.GetClosestTarget<MonsterController>(this.transform.position);
                if (target.IsValid() && target.IsDeadState == false)
                {
                    this.Value.ShootDir = (target.transform.position - this.transform.position).normalized;
                    degrees = Mathf.Atan2(this.Value.ShootDir.y, this.Value.ShootDir.x) * Mathf.Rad2Deg;
                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(0, 0, degrees), Time.deltaTime * RANGED_GUIDED_SHOT_ROT_SPEED);

                    if (target.IsDeadState)
                        Utils.LogBreak("BREAK.");
                }
                else
                    this.transform.rotation =  Quaternion.Euler(0, 0, Mathf.Atan2(this.Value.ShootDir.y, this.Value.ShootDir.x) * Mathf.Rad2Deg);
  
                this.transform.position += this.Value.ShootDir * this.Value.MovementSpeed * Time.deltaTime;
                if (HitCollider.enabled == false)
                    HitCollider.enabled = true;

                yield return null;
            }
        }

        private readonly float Sensitivity = 0.6f;
        private void SetSwingOffFromOwner()
        {
            if (this.Owner.LookAtDir != this.Value.InitialLookAtDir)
                this._isOffFromOwner = true;

            if (this.Owner.IsMoving == false)
                this._isOffFromOwner = true;

            if ((this.Owner.ShootDir - this.Value.ShootDir).sqrMagnitude > Sensitivity * Sensitivity)
                this._isOffFromOwner = true;
        }

        private IEnumerator CoThrowingStar()
        {
            float rotAngle = 0f;
            while (true)
            {
                rotAngle += this.Value.RotationSpeed * Time.deltaTime;
                rotAngle %= 360f;
                transform.rotation = Quaternion.Euler(0, 0, rotAngle);
                float movementSpeed = Owner.Stat.MovementSpeed + this.Value.MovementSpeed;
                transform.position += this.Value.ShootDir * movementSpeed * Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator CoBoomerang()
        {
            float rotAngle = 0f;
            float movementSpeed = Owner.Stat.MovementSpeed + this.Value.MovementSpeed;
            float decelerationIntensity = 50f; // 감속 속도를 조절하려면 필요에 따라 값을 변경
            while (true)
            {
                rotAngle += this.Value.RotationSpeed * Time.deltaTime;
                rotAngle %= 360f;
                transform.rotation = Quaternion.Euler(0, 0, rotAngle);

                movementSpeed -= decelerationIntensity * Time.deltaTime;
                decelerationIntensity += 0.05f;
                transform.position += this.Value.ShootDir * movementSpeed * Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator CoBoomerangUltimate()
        {
            Transform child = transform.GetChild(0);
            SpriteTrail.SpriteTrail trail = GetComponent<Boomerang>().Trail;
            trail.enabled = false;
            AnimationCurve curve = GetComponent<Boomerang>().Curve;

            bool isToOwner = false;
            bool isGoingToShootDir = true;
            bool rotAround_StartEnlargeDistance = true;

            float rotAngle = 0f;
            float movementSpeed = Owner.Stat.MovementSpeed + this.Value.MovementSpeed;
            float decelerationIntensity = 50f;

            // +++ ROT AROUND BASE +++
            float rotAround_Delta = 0f;
            float rotAround_MinDistance = 1f;
            float rotAround_MaxDistance = 8f;
            // +++ START ROT ENLARGE OR SHRINK && KEEP OPTIONS +++
            float rotAround_AdjustDistanceDesiredDuration = 1.15f;

            while (true)
            {
                if (isGoingToShootDir)
                {
                    // +++ GO TO SHOOTDIR +++
                    if (isToOwner == false)
                    {
                        rotAngle += this.Value.RotationSpeed * Time.deltaTime;
                        rotAngle %= 360f;
                        transform.rotation = Quaternion.Euler(0, 0, rotAngle);

                        movementSpeed -= decelerationIntensity * Time.deltaTime;
                        decelerationIntensity += 0.5f;
                        transform.position += this.Value.ShootDir * movementSpeed * Time.deltaTime;
                        if (movementSpeed < 0f || Managers.Stage.IsOutOfPos(transform.position))
                            isGoingToShootDir = false;
                    }
                    // +++ RETURN TO OWNER +++
                    else
                    {
                        rotAngle += this.Value.RotationSpeed * Time.deltaTime;
                        rotAngle %= 360f;
                        transform.rotation = Quaternion.Euler(0, 0, rotAngle);

                        Vector3 toOwner = (this.Owner.transform.position - child.transform.position).normalized;
                        transform.position += toOwner * this.Value.MovementSpeed * Time.deltaTime;
                        if ((this.Owner.transform.position - child.transform.position).sqrMagnitude < 1f)
                            break;
                    }
                }
                else
                {
                    // STOP AND START ROTATE AROUND
                    trail.enabled = true;
                    yield return new WaitUntil(() => BoomerangUltimate_DoRotateAround(child: child, curve: curve, rotAround_StartEnlargeDistance: ref rotAround_StartEnlargeDistance,
                        rotAngle: ref rotAngle, rotAround_Delta: ref rotAround_Delta, rotAround_AdjustDistanceSameDesiredDuration: rotAround_AdjustDistanceDesiredDuration,
                        rotAround_MinDistance: rotAround_MinDistance, rotAround_MaxDistance: rotAround_MaxDistance));
                    trail.enabled = false;
                    isToOwner = true;
                    isGoingToShootDir = true;
                }

                yield return null;
            }

            this.SR.enabled = false;
            this.RigidBody.simulated = false;
            this.HitCollider.enabled = false;

            // +++
            GetComponent<Boomerang>().DoSkillJobManually(this, 1f);
            //Managers.Object.Despawn<SkillBase>(this);
        }

        private bool BoomerangUltimate_DoRotateAround(Transform child, AnimationCurve curve, ref bool rotAround_StartEnlargeDistance,
            ref float rotAngle, ref float rotAround_Delta, float rotAround_AdjustDistanceSameDesiredDuration, float rotAround_MinDistance, float rotAround_MaxDistance)
        {
            rotAngle += this.Value.RotationSpeed * Time.deltaTime;
            rotAngle %= 360f;
            transform.rotation = Quaternion.Euler(0, 0, rotAngle);

            rotAround_Delta += Time.deltaTime;
            // +++ TO ENLARGE DISTANCE +++
            if (rotAround_StartEnlargeDistance)
            {
                // percent에 별다른 속도를 점점 Lerp로 가중시켜서 힘좀 줘도 될듯
                float percent = rotAround_Delta / rotAround_AdjustDistanceSameDesiredDuration;
                if (percent < 1f)
                    child.localPosition = Vector3.right * Mathf.Lerp(rotAround_MinDistance, rotAround_MaxDistance, curve.Evaluate(percent));
                else
                {
                    rotAround_StartEnlargeDistance = false;
                    rotAround_Delta = 0f;
                }
            }
            // +++ TO SHRINK DISTANCE +++
            else
            {
                rotAround_Delta += Time.deltaTime;
                float percent = rotAround_Delta / rotAround_AdjustDistanceSameDesiredDuration;
                if (percent < 1f)
                    child.localPosition = Vector3.right * Mathf.Lerp(rotAround_MaxDistance, rotAround_MinDistance, curve.Evaluate(percent));
                else
                    return true;
            }

            return false;
        }

        private IEnumerator CoPhantomSoulChild()
        {
            while (true)
            {
                this.transform.position += (this.Owner.FireSocketPosition - this.Owner.Center.position).normalized * this.Value.MovementSpeed * Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator CoScaleInterpolation()
        {
            float delta = 0f;
            float percent = 0f;

            // TEMP REINA MASTERY INTERPOLATION
            // if (_interpolateStartScale.x < 0)
            //     transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z - 180f);
            while (percent < 1f)
            {
                delta += Time.deltaTime;
                percent = delta / this.Data.Duration;
                transform.localScale = Vector3.Lerp(this.Value.StartLocalScale, this.Value.TargetScale, percent);
                yield return null;
            }
        }

        private IEnumerator CoColliderDuration(float duration)
        {
            this.HitCollider.enabled = true;
            yield return new WaitForSeconds(duration);
            this.HitCollider.enabled = false;
        }

        // 데미지가 아닌, 프로젝타일 로직과 관련된 것만 적는다.
        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            switch (this.Data.TemplateOrigin)
            {
                case FixedValue.TemplateID.Skill.ThrowingStar:
                    this.Value.ShootDir = NextBounceTarget(cc, Define.HitFromType.ThrowingStar);
                    break;

                case FixedValue.TemplateID.Skill.ArrowMasterMastery:
                    if (this.Value.MaxPenetrationCount == -1)
                        return;
                    else
                        Managers.Object.Despawn(this);
                    break;

                case FixedValue.TemplateID.Skill.ElementalArcherMastery:
                    {
                        if (this.Data.Grade == Define.InGameGrade.Default)
                            Managers.Object.Despawn(this);
                        else
                            StopCoroutine(_coProjectile); // Projectile Stop 만,,,
                    }
                    break;

                case FixedValue.TemplateID.Skill.ForestGuardianMastery:
                    {
                        if (this.Data.Grade > Define.InGameGrade.Elite)
                            StopCoroutine(_coProjectile);
                        else
                            Managers.Object.Despawn(this);
                    }
                    break;

                case FixedValue.TemplateID.Skill.NinjaMastery:
                    {
                        if (this.Data.Grade < this.Data.MaxGrade)
                            Managers.Object.Despawn(this);
                        else
                        {
                            Managers.Object.Despawn(this);

                            // RangedShot kunaiUltimate = this.GetComponent<RangedShot>();
                            // if (kunaiUltimate.IsLaunchedFromOwner && kunaiUltimate.IsAlreadyGeneratedKunais == false)
                            // {
                            //     StopCoroutine(_coProjectile);
                            //     this.HitCollider.enabled = false;
                            //     this.SR.enabled = false;
                            // }
                            // // else if (kunaiUltimate.IsLaunchedFromOwner && kunaiUltimate.IsAlreadyGeneratedKunais)
                            // // {
                            // //     Managers.Object.Despawn(this);
                            // // }
                            // else
                            //     Managers.Object.Despawn(this);
                        }
                    }
                    break;

                case FixedValue.TemplateID.Skill.PhantomKnight_Unique_Elite_C1:
                    Managers.Object.Despawn(this);
                    break;
            }
        }

        // private void OnTriggerExit2D(Collider2D other)
        // {
        //     if (this.Data.TemplateID ==(int)SkillTemplate.NinjaMastery + (int)this.Data.MaxGrade - 1)
        //     {
        //         if (this.GetComponent<RangedShot>().IsLaunchedFromOwner)
        //         {
        //             Utils.Log("OUT,,,");
        //             this.HitCollider.enabled = false;
        //             Managers.Object.Despawn(this);
        //         }
        //     }
        // }

        private Vector3 NextBounceTarget(CreatureController cc, Define.HitFromType hitFromType = Define.HitFromType.None)
        {
            if (CanStillBounce == false)
            {
                if (this.IsValid())
                    Managers.Object.Despawn(this);
                    
                return this.Value.ShootDir;
            }

            Vector3 shootDir = Vector3.zero;
            if (Managers.Collision.IsCorrectTarget(Define.CollisionLayers.MonsterBody, cc.gameObject.layer))
                shootDir = Utils.GetClosestFromTargetDirection<MonsterController>(cc, hitFromType);
            else if (Managers.Collision.IsCorrectTarget(Define.CollisionLayers.PlayerBody, cc.gameObject.layer))
            {
                if (this.IsValid())
                    Managers.Object.Despawn(this);
            }

            if (shootDir == Vector3.zero)
                shootDir = this.Value.ShootDir;

            return shootDir;
        }

        private void OnDestroy()
        {
            this.OnProjectileLaunchInfo -= OnProjectileLaunchInfoHandler;
        }
    }
}

// -------------------------------------------------------------------------------------
// -------------------------------------------------------------------------------------
// public void SetProjectileInfo(Vector3 shootDir, Define.LookAtDirection lootAtDir, Vector3 localScale, Vector3 indicatorAngle,
//                             float continuousAngle, float continuousSpeedRatio, float continuousFlipX, float continuousFlipY,
//                             float interPolateTargetScaleX, float interpolateTargetScaleY, bool isOnlyVisible)
// {
//     _shootDir = shootDir;
//     _initialLookAtDir = lootAtDir;
//     transform.localScale = localScale;

//     if (interPolateTargetScaleX > localScale.x || interpolateTargetScaleY > localScale.y)
//     {
//         _interpolateStartScale = localScale;
//         _interpolateTargetScale = new Vector3(interPolateTargetScaleX, interpolateTargetScaleY, 1f);
//         _isOnInterpolateScale = true;
//     }
//     else
//         _isOnInterpolateScale = false;

//     //StartDestroy(RepeatSkill.Data.Duration);

//     _isOnlyVisible = isOnlyVisible;
//     EnableCollider(isOnlyVisible ? false : true);

//     // switch (RepeatSkill.Data.RepeatSkillType)
//     // {
//     //     case Define.TemplateIDs.CreatureStatus.RepeatSkill.PaladinMeleeSwing:
//     //         {
//     //             _isOffParticle = false;
//     //             _targetSkill.SetParticleInfo(indicatorAngle, lootAtDir, continuousAngle, continuousFlipX, continuousFlipY);
//     //             StartCoroutine(CoMeleeSwing(continuousSpeedRatio));
//     //         }
//     //         break;
//     // }
// }

// private SkillBase _targetSkill = null;
// public void SetInitialCloneInfo(SkillBase skillOrigin)
// {
//     // if (IsFirstPooling == false)
//     //     return;
//     IsFirstPooling = false;

//     string className = string.Empty;
//     //ObjectType = objectType;
//     // if (this.ObjectType == Define.ObjectType.RepeatProjectile)
//     // {
//     //     RepeatSkill = skillOrigin.RepeatSkill; // +++ ORIGIN SKILL DATA +++
//     //     className = Define.NAME_SPACE_MAIN + "." + RepeatSkill.Data.RepeatSkillType.ToString();
//     // }
//     // else if(this.ObjectType == Define.ObjectType.SequenceProjectile) { /* DO SOMETHING */ }


//     Owner = skillOrigin.Owner;
//     if (skillOrigin.Owner?.IsPlayer() == true)
//     {
//         Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
//     }
//     else if (skillOrigin.Owner?.IsMonster() == true) { /* DO SOMETHING */ }

//     // if (_rigid == null)
//     //     _rigid = GetComponent<Rigidbody2D>();

//     if (_collider == null)
//         _collider = GetComponent<Collider2D>();

//     if (className.Contains("MeleeSwing"))
//     {
//         _targetSkill = GetComponent<MeleeSwing>();
//         //_targetSkill.GetComponent<MeleeSwing>().InitRepeatSkill(skillOrigin.RepeatSkill);
//     }

//     //Utils.LogStrong("Success::SetInitialCloneInfo");
// }

// private Rigidbody2D _rigid = null;
//public SkillBase CurrentSkill { get; private set; }
//private Collider2D _collider = null;
//private void EnableCollider(bool enable) => _collider.enabled = enable;

// private readonly float Sensitivity = 0.6f;
// private void CheckOffParticle()
// {
//     if (_initialLookAtDir != Managers.Game.Player.LookAtDir)
//         _isOffParticle = true;
//     if (Managers.Game.Player.IsMoving == false)
//         _isOffParticle = true;
//     if ((Managers.Game.Player.ShootDir - _shootDir).sqrMagnitude > Sensitivity * Sensitivity)
//         _isOffParticle = true;
//     // if (Vector3.Distance(Managers.Game.Player.ShootDir, _shootDir) > _sensitivity)
//     //     _offParticle = true;
// }

// private float _deltaForColliderPreDisable;
// private void PreDisableCollider(float lifeTime)
// {
//     if (_isOnlyVisible)
//         return;

//     _deltaForColliderPreDisable += Time.deltaTime;
//     if (_deltaForColliderPreDisable > lifeTime)
//     {
//         EnableCollider(false);
//         _deltaForColliderPreDisable = 0f;
//     }
// }

// ================================================================================================================
// private IEnumerator CoMeleeSwing(float continuousSpeedRatio)
// {
//     float t = 0f;
//     float projectileSpeed = RepeatSkill.RepeatSkillData.Speed * continuousSpeedRatio;
//     float duration = RepeatSkill.RepeatSkillData.Duration;
//     float colliderKeepingRatio = RepeatSkill.RepeatSkillData.CollisionKeepingRatio;

//     while (true)
//     {
//         // // +++ MOVING MELEE ATTACK +++
//         if (Managers.Game.Player.IsMoving && _isOffParticle == false && RepeatSkill.RepeatSkillData.Speed != 0f)
//         {
//             if (Managers.Game.Player.IsInLimitMaxPosX || Managers.Game.Player.IsInLimitMaxPosY)
//             {
//                 float minSpeed = Managers.Game.Player.MovementPower + projectileSpeed;
//                 float maxSpeed = Owner.CreatureStat.MoveSpeed + projectileSpeed;

//                 float movementPowerRatio = Managers.Game.Player.MovementPower / Owner.CreatureStat.MoveSpeed;
//                 _speed = Mathf.Lerp(minSpeed, maxSpeed, movementPowerRatio);
//             }
//             else
//                 _speed = Owner.CreatureStat.MoveSpeed + projectileSpeed;
//         }
//         // +++ STATIC MELEE ATTACK +++
//         else
//             _speed = projectileSpeed;

//         CheckOffParticle();
//         transform.position += _shootDir * _speed * Time.deltaTime;

//         if (_isOnInterpolateScale)
//         {
//             t += Time.deltaTime;
//             float percent = t / RepeatSkill.Data.Duration;
//             transform.localScale = Vector2.Lerp(_interpolateStartScale, _interpolateTargetScale, percent);
//         }

//         PreDisableCollider(duration * colliderKeepingRatio);
//         yield return null;
//     }
// }

// public void SetProjectileInfo(CreatureController owner, SkillBase currentSkill, Vector3 shootDir, Vector3 spawnPos, Vector3 localScale, Vector3 indicatorAngle, 
//             Define.LookAtDirection lootAtDir = Define.LookAtDirection.Right, float continuousSpeedRatio = 1f, float continuousAngle = 0f, float continuousFlipX = 0f, float continuousFlipY = 0f, 
//             float continuousPowers = 1, bool? isOnHit = null, float? interPolTargetX = null, float? interPolTargetY = null)
// {
//     this.Owner = owner;
//     this.CurrentSkill = currentSkill;
//     this._shootDir = shootDir * continuousPowers;
//     this._continuousSpeedRatio = continuousSpeedRatio;

//     _initOwnerLootAtDir = lootAtDir;
//     _offParticle = false;
//     transform.position = spawnPos;

//     // +++++ Interpolate Local Scales +++++
//     transform.localScale = localScale;
//     if (interPolTargetX.HasValue && interPolTargetY.HasValue)
//     {
//         _isOnInterpolateScale = true;
//         _interpolateStartScale = localScale;
//         _interpolateTargetScale = new Vector2(interPolTargetX.Value, interPolTargetY.Value);
//     }
//     else
//         _isOnInterpolateScale = false;

//     StartDestroy(currentSkill.SkillData.Duration);
//     switch (currentSkill.SkillData.OriginTemplateID)
//     {
//         case (int)Define.TemplateIDs.SkillType.PaladinMeleeSwing:
//         case (int)Define.TemplateIDs.SkillType.KnightMeleeSwing:
//         case (int)Define.TemplateIDs.SkillType.PhantomKnightMeleeSwing:
//         case (int)Define.TemplateIDs.SkillType.AssassinMeleeSwing:
//         case (int)Define.TemplateIDs.SkillType.ThiefMeleeSwing:
//         case (int)Define.TemplateIDs.SkillType.WarriorMeleeSwing:
//         case (int)Define.TemplateIDs.SkillType.BerserkerMeleeSwing:
//         case (int)Define.TemplateIDs.SkillType.SkeletonKingMeleeSwing:
//         case (int)Define.TemplateIDs.SkillType.PirateMeleeSwing:
//         case (int)Define.TemplateIDs.SkillType.QueenMeleeSwing:
//             {
//                 GetComponent<MeleeSwing>().SetSwingInfo(owner, currentSkill.SkillData.TemplateID, indicatorAngle,
//                             lootAtDir, continuousAngle, continuousFlipX, continuousFlipY);

//                 if (isOnHit == true || isOnHit == null)
//                     EnableColliders(true);
//                 else if (isOnHit == false)
//                     EnableColliders(false);

//                 if (currentSkill.SkillData.OriginTemplateID == Define.TemplateIDs.SkillType.PhantomKnightMeleeSwing ||
//                     currentSkill.SkillData.OriginTemplateID == Define.TemplateIDs.SkillType.AssassinMeleeSwing ||
//                     currentSkill.SkillData.OriginTemplateID == Define.TemplateIDs.SkillType.ThiefMeleeSwing ||
//                     currentSkill.SkillData.OriginTemplateID == Define.TemplateIDs.SkillType.WarriorMeleeSwing ||
//                     currentSkill.SkillData.OriginTemplateID == Define.TemplateIDs.SkillType.BerserkerMeleeSwing ||
//                     currentSkill.SkillData.OriginTemplateID == Define.TemplateIDs.SkillType.PirateMeleeSwing || 
//                     currentSkill.SkillData.OriginTemplateID == Define.TemplateIDs.SkillType.QueenMeleeSwing)
//                 {
//                     _shootDir = Quaternion.Euler(0, 0, continuousAngle * -1) * _shootDir;
//                 }

//                 StartCoroutine(CoMeleeSwing());
//             }
//             break;

//         case (int)Define.TemplateIDs.SkillType.ArrowMasterRangedShot:
//             {
//                 _currentPenetrationCount = 0;
//                 GetComponent<RangedShot>().SetSkillInfo(Owner, currentSkill.SkillData.TemplateID);
//                 StartCoroutine(CoArrowShot());
//             }
//             break;

//         case (int)Define.TemplateIDs.SkillType.ElementalArcherRangedShot:
//             {
//                 GetComponent<RangedShot>().SetSkillInfo(Owner, currentSkill.SkillData.TemplateID);
//                 if (currentSkill.SkillData.InGameGrade == Define.InGameGrade.Epic)
//                 {
//                     //Managers.Effect.ShowWindTrailEffect(this);
//                     //Managers.Effect.ShowFireTrailEffect(this);
//                     //Managers.Effect.ShowIceTrailEffect(this);
//                     //Managers.Effect.ShowBubbleTrailEffect(this);
//                     Managers.Effect.ShowLightTrailEffect(this);
//                 }

//                 StartCoroutine(CoArrowShot());
//             }
//             break;

//         case (int)Define.TemplateIDs.SkillType.ForestWardenRangedShot:
//             {
//                 GetComponent<RangedShot>().SetSkillInfo(Owner, currentSkill.SkillData.TemplateID);
//                 StartCoroutine(CoArrowShot());
//             }
//             break;

//         case (int)Define.TemplateIDs.SkillType.ThrowingStar:
//             {
//                 _target = null;
//                 StartCoroutine(CoThrowingStar());
//             }
//             break;

//         case (int)Define.TemplateIDs.SkillType.Boomerang:
//             {
//                 if (currentSkill.SkillData.InGameGrade != Define.InGameGrade.Legendary)
//                     StartCoroutine(CoBoomerang());
//                 else
//                     StartCoroutine(CoBoomerangLegendary());
//             }
//             break;
//     }
// }

// private IEnumerator CoMeleeSwing_TEMP()
// {
//     float projectileSpeed = CurrentSkill.SkillData.Speed * _continuousSpeedRatio;
//     controllColDelta = 0f;
//     float t = 0f;

//     while (true)
//     {
//         if (Managers.Game.Player.IsMoving && _offParticle == false && CurrentSkill.SkillData.Speed != 0f) // Moving Melee Attack
//         {
//             if (Managers.Game.Player.IsInLimitMaxPosX || Managers.Game.Player.IsInLimitMaxPosY)
//             {
//                 float minSpeed = Managers.Game.Player.MovementPower + projectileSpeed;
//                 float maxSpeed = Owner.CreatureData.MoveSpeed + projectileSpeed;

//                 float movementPowerRatio = Managers.Game.Player.MovementPower / Owner.CreatureData.MoveSpeed;
//                 _speed = Mathf.Lerp(minSpeed, maxSpeed, movementPowerRatio);
//             }
//             else
//                 _speed = Owner.CreatureData.MoveSpeed + projectileSpeed;
//         }
//         else // Static Melee Attack
//             _speed = projectileSpeed;

//         SetOffParticle();
//         transform.position += _shootDir * _speed * Time.deltaTime;

//         if (_isOnInterpolateScale)
//         {
//             t += Time.deltaTime;
//             float percent = t / CurrentSkill.SkillData.Duration;
//             transform.localScale = Vector2.Lerp(_interpolateStartScale, _interpolateTargetScale, percent);
//         }

//         ControlCollisionTime(CurrentSkill.SkillData.Duration * CurrentSkill.SkillData.CollisionKeepingRatio);
//         yield return null;
//     }
// }

// private IEnumerator CoThrowingStar()
// {
//     float selfRot = 0f;
//     while (true)
//     {
//         selfRot += CurrentSkill.SkillData.SelfRotationZSpeed * Time.deltaTime;
//         transform.rotation = Quaternion.Euler(0, 0, selfRot);
//         float movementSpeed = Owner.CreatureData.MoveSpeed + CurrentSkill.SkillData.Speed;

//         transform.position += _shootDir * movementSpeed * Time.deltaTime;
//         yield return null;
//     }
// }

// private IEnumerator CoArrowShot()
// {
//     float projectileSpeed = CurrentSkill.SkillData.Speed * _continuousSpeedRatio;
//     while (true)
//     {
//         float movementSpeed = Owner.CreatureData.MoveSpeed + projectileSpeed;
//         transform.position += _shootDir * movementSpeed * Time.deltaTime;
//         yield return null;
//     }
// }

// private IEnumerator CoBoomerang()
// {
//     float selfRot = 0f;
//     float currentSpeed = Owner.CreatureData.MoveSpeed + CurrentSkill.SkillData.Speed;
//     float deceleration = 50f; // 감속 속도를 조절하려면 필요에 따라 값을 변경

//     while (true)
//     {
//         // 부메랑 회전
//         selfRot += CurrentSkill.SkillData.SelfRotationZSpeed * Time.deltaTime * 1.1f;
//         transform.rotation = Quaternion.Euler(0, 0, selfRot);

//         currentSpeed -= deceleration * Time.deltaTime;
//         deceleration += 0.05f;
//         transform.position += _shootDir * currentSpeed * Time.deltaTime;

//         yield return null;
//     }
// }

// private IEnumerator CoBoomerangLegendary()
// {
//     BoomerangChild child = transform.GetChild(0).GetComponent<BoomerangChild>();
//     if (child.Trail != null)
//         child.Trail.enabled = false;

//     bool canStartChildRot = false;
//     bool isToOwner = false;

//     float waitdelta = 0f;
//     float selfRot = 0f;
//     float currentSpeed = Owner.CreatureData.MoveSpeed + CurrentSkill.SkillData.Speed;
//     float deceleration = 50f;

//     while (true)
//     {
//         selfRot += CurrentSkill.SkillData.SelfRotationZSpeed * Time.deltaTime * 1.5f;
//         selfRot %= 360f;
//         transform.rotation = Quaternion.Euler(0, 0, selfRot);

//         if (isToOwner == false)
//         {
//             currentSpeed -= deceleration * Time.deltaTime;
//             deceleration += 0.05f;
//             transform.position += _shootDir * currentSpeed * Time.deltaTime;
//         }
//         else if(isToOwner && child.IsReadyToOwner)
//         {
//             child.Trail.enabled = false;
//             Vector3 toOwnerDir = (Owner.transform.position - transform.position).normalized;
//             transform.position += toOwnerDir * (CurrentSkill.SkillData.Speed) * Time.deltaTime;

//             if ((transform.position - Owner.transform.position).sqrMagnitude < 1)
//             {
//                 canStartChildRot = false;
//                 isToOwner = false;
//                 StopDestroy();
//                 Managers.Resource.Destroy(gameObject);
//             }
//         }

//         if (canStartChildRot)
//             child.RotateAround(isToOwner);

//         if (currentSpeed < 0 && canStartChildRot == false || Managers.Stage.IsOutOfPos(transform.position) && canStartChildRot == false)
//         {
//             child.RotStartTime = Time.time;
//             child.Trail.enabled = true;
//             yield return new WaitUntil(() => StopAndChildRotation(child, ref selfRot, ref waitdelta, ref canStartChildRot, ref isToOwner));
//         }

//         yield return null;
//     }
// }

// private bool StopAndChildRotation(BoomerangChild child, ref float selfRot, ref float waitDelta, ref bool canStartChildRot, ref bool isToOwner)
// {
//     waitDelta += Time.deltaTime;

//     selfRot += CurrentSkill.SkillData.SelfRotationZSpeed * Time.deltaTime * 1.5f;
//     selfRot %= 360f;
//     transform.rotation = Quaternion.Euler(0, 0, selfRot);

//     child.RotateAround();
//     if (waitDelta > child.DesiredSelfRotTime)
//     {
//         waitDelta = 0f;
//         isToOwner = true;
//         canStartChildRot = true;
//         child.ToOwnerStartTime = Time.time;
//         return true;
//     }

//     return false;
// }

// private readonly float _sensitivity = 0.6f;
// private void SetOffParticle()
// {
//     if (_initOwnerLootAtDir != Managers.Game.Player.LookAtDir)
//         _offParticle = true;
//     if (Managers.Game.Player.IsMoving == false)
//         _offParticle = true;
//     // if (Vector3.Distance(Managers.Game.Player.ShootDir, _shootDir) > _sensitivity)
//     //     _offParticle = true;
//     if ((Managers.Game.Player.ShootDir - _shootDir).sqrMagnitude > _sensitivity * _sensitivity)
//         _offParticle = true;
// }

// float controllColDelta = 0f;
// private void ControlCollisionTime(float controlTime)
// {
//     controllColDelta += Time.deltaTime;
//     if (controlTime <= controllColDelta)
//     {
//         EnableColliders(false);
//         controllColDelta = 0f;
//     }
// }

// private IEnumerator CoDotDamage<T>(T cc, SkillBase attacker, int dotCount, float delay = 0.1f) where T : CreatureController
// {
//     int currentCount = 0;
//     float t = 0f;
//     while (currentCount < dotCount)
//     {
//         t += Time.deltaTime;
//         if (t > delay)
//         {
//             cc.OnDamaged(Owner, CurrentSkill);
//             t = 0f;
//             ++currentCount;
//         }

//         yield return null;
//     }

//     // if (attacker.SkillData.OriginTemplateID == (int)Define.TemplateIDs.SkillType.ForestWardenRangedShot)
//     // {
//     //     GameObject go = Managers.Effect.ShowImpactHitEffect(Define.ImpactHits.Leaves, this.transform.position);
//     //     go.GetComponent<ImpactHit>().SetInfo(Define.ImpactHits.Leaves, Owner, CurrentSkill, this);
//     //     Managers.Object.Despawn(this.GetComponent<ProjectileController>());
//     // }
// }

// private void OnTriggerEnter2D(Collider2D other)
// {
//     // SkillData.PenetrationCount // -1 : 무제한 관통
//     MonsterController mc = other.GetComponent<MonsterController>();
//     if (mc.IsValid() == false)
//         return;

//     if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.MonsterBody, other.gameObject.layer))
//     {
//         switch (CurrentSkill.SkillData.OriginTemplateID)
//         {
//             case (int)Define.TemplateIDs.SkillType.PaladinMeleeSwing:
//             case (int)Define.TemplateIDs.SkillType.KnightMeleeSwing:
//             case (int)Define.TemplateIDs.SkillType.PhantomKnightMeleeSwing:
//             case (int)Define.TemplateIDs.SkillType.AssassinMeleeSwing:
//             case (int)Define.TemplateIDs.SkillType.ThiefMeleeSwing:
//                 {
//                     mc.OnDamaged(Owner, CurrentSkill);
//                 }
//                 break;

//             case (int)Define.TemplateIDs.SkillType.WarriorMeleeSwing:
//             case (int)Define.TemplateIDs.SkillType.BerserkerMeleeSwing:
//                 {
//                     mc.OnDamaged(Owner, CurrentSkill);
//                     // 맞는듯
//                     // Vector3 hitPoint = other.ClosestPoint(new Vector2(transform.position.x, transform.position.y));
//                     // GameObject go = new GameObject() { name = "AA" };
//                     // go.transform.position = hitPoint;
//                     // Debug.Log("CHECK HIT POINT");
//                     // Debug.Break();
//                 }
//                 break;

//             case (int)Define.TemplateIDs.SkillType.ArrowMasterRangedShot:
//                 {
//                     if (CurrentSkill.SkillData.InGameGrade < Define.InGameGrade.Epic)
//                     {
//                         mc.OnDamaged(Owner, CurrentSkill);
//                     }
//                     else // EPIC && LEGENDARY
//                     {
//                         // Hmm...
//                         // GameObject go = Managers.Effect.ShowImpactHitEffect(Define.ImpactHits.ArrowBigHit, this.transform.position);
//                         // go.GetComponent<ImpactHit>().SetInfo(Define.ImpactHits.ArrowBigHit, Owner, CurrentSkill, this);
//                         // mc.OnDamaged(Owner, CurrentSkill);

//                         // if (Owner.Buff != null && Owner.Buff.IsBuffOn)
//                         //     Managers.Effect.ShowImpactHitEffect(Define.ImpactHits.ArrowBigHit, this.transform.position);
//                         mc.OnDamaged(Owner, CurrentSkill);
//                     }
//                 }
//                 break;

//             case (int)Define.TemplateIDs.SkillType.ElementalArcherRangedShot:
//                 {
//                     mc.OnDamaged(Owner, CurrentSkill);
//                     Managers.Object.Despawn(this.GetComponent<ProjectileController>());

//                     if (CurrentSkill.SkillData.InGameGrade == Define.InGameGrade.Epic)
//                     {
//                         //Managers.Effect.ShowImpactWindEffect(mc.Body.transform.position, CurrentSkill.SkillData.InGameGrade);
//                         //Managers.Effect.ShowImpactFireEffect(mc.Body.transform.position, CurrentSkill.SkillData.InGameGrade);
//                         //Managers.Effect.ShowImpactIceEffect(mc.Body.transform.position, CurrentSkill.SkillData.InGameGrade);
//                         //Managers.Effect.ShowImpactBubbleEffect(mc.Body.transform.position);
//                         //Managers.Effect.ShowImpactLightEffect(mc.Body.transform.position);
//                     }
//                 }
//                 break;

//             case (int)Define.TemplateIDs.SkillType.ForestWardenRangedShot:
//                 {
//                     mc.OnDamaged(Owner, CurrentSkill);
//                     if (CurrentSkill.SkillData.HasCC)
//                     {
//                         if (Random.Range(0f, 1f) <= CurrentSkill.SkillData.CCRate)
//                         {
//                             //Vector3 hitPoint = other.ClosestPoint(new Vector2(transform.position.x, transform.position.y));
//                             Managers.CC.ApplyCC<MonsterController>(mc, CurrentSkill.SkillData, this);
//                             if (CurrentSkill.SkillData.InGameGrade < Define.InGameGrade.Legendary)
//                             {
//                                 if (CurrentSkill.SkillData.InGameGrade == Define.InGameGrade.Epic)
//                                 {
//                                     // GameObject go = Managers.Effect.ShowImpactHitEffect(Define.ImpactHits.Leaves, this.transform.position);
//                                     // go.GetComponent<ImpactHit>().SetInfo(Define.ImpactHits.Leaves, Owner, CurrentSkill, this);
//                                 }

//                                 Managers.Object.Despawn(this.GetComponent<ProjectileController>());
//                             }
//                             else
//                             {
//                                 StartCoroutine(CoDotDamage<MonsterController>(mc, CurrentSkill, 4, 0.05f));
//                             }
//                         }
//                     }
//                 }
//                 break;

//             case (int)Define.TemplateIDs.SkillType.SkeletonKingMeleeSwing:
//                 {
//                     mc.OnDamaged(Owner, CurrentSkill);
//                 }
//                 break;;

//             case (int)Define.TemplateIDs.SkillType.PirateMeleeSwing:
//                 {
//                     mc.OnDamaged(Owner, CurrentSkill);
//                     // +++ CC State 만들때 작업하기 +++
//                     // if (CurrentSkill.SkillData.InGameGrade == Define.InGameGrade.Legendary)
//                     //     Managers.Effect.ShowCursedText(mc);
//                 }
//                 break;

//             case (int)Define.TemplateIDs.SkillType.ThrowingStar:
//                 if (_currentBounceCount == CurrentSkill.SkillData.BounceCount)
//                 {
//                     _currentBounceCount = 0;
//                     mc.OnDamaged(Owner, CurrentSkill);
//                     Managers.Object.ResetSkillHittedStatus(Define.TemplateIDs.SkillType.ThrowingStar);
//                     Managers.Object.Despawn(this.GetComponent<ProjectileController>());
//                 }
//                 else
//                 {
//                     _currentBounceCount++;
//                     mc.OnDamaged(Owner, CurrentSkill);
//                     mc.IsThrowingStarHit = true;
//                     _target = Managers.Object.GetNextTarget(mc.gameObject, Define.TemplateIDs.SkillType.ThrowingStar);
//                     if (_target != null)
//                         _shootDir = (_target.transform.position - transform.position).normalized;
//                     else
//                     {
//                         _currentBounceCount = 0;
//                         Managers.Object.ResetSkillHittedStatus(Define.TemplateIDs.SkillType.ThrowingStar);
//                         Managers.Object.Despawn(this.GetComponent<ProjectileController>());
//                     }
//                 }
//                 break;

//             case (int)Define.TemplateIDs.SkillType.Boomerang:
//                 {
//                     mc.OnDamaged(Owner, CurrentSkill);
//                 }
//                 break;
//         }
//     }
// }
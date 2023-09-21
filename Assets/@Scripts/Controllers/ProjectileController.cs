using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace STELLAREST_2D
{
    public class ProjectileController : SkillBase
    {
        private SkillBase _targetSkill = null;

        public void SetInitialCloneInfo(SkillBase skillOrigin)
        {
            // if (IsFirstPooling == false)
            //     return;
            IsFirstPooling = false;

            string className = string.Empty;
            //ObjectType = objectType;
            // if (this.ObjectType == Define.ObjectType.RepeatProjectile)
            // {
            //     RepeatSkill = skillOrigin.RepeatSkill; // +++ ORIGIN SKILL DATA +++
            //     className = Define.NAME_SPACE_MAIN + "." + RepeatSkill.Data.RepeatSkillType.ToString();
            // }
            // else if(this.ObjectType == Define.ObjectType.SequenceProjectile) { /* DO SOMETHING */ }


            Owner = skillOrigin.Owner;
            if (skillOrigin.Owner?.IsPlayer() == true)
            {
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
            }
            else if (skillOrigin.Owner?.IsMonster() == true) { /* DO SOMETHING */ }

            if (_rigid == null)
                _rigid = GetComponent<Rigidbody2D>();

            if (_collider == null)
                _collider = GetComponent<Collider2D>();

            if (className.Contains("MeleeSwing"))
            {
                _targetSkill = GetComponent<MeleeSwing>();
                //_targetSkill.GetComponent<MeleeSwing>().InitRepeatSkill(skillOrigin.RepeatSkill);
            }

            //Utils.LogStrong("Success::SetInitialCloneInfo");
        }

        private Rigidbody2D _rigid = null;
        //public SkillBase CurrentSkill { get; private set; }
        private Collider2D _collider = null;
        private void EnableCollider(bool enable) => _collider.enabled = enable;

        private Vector3 _shootDir = Vector3.zero;
        private float _speed = 0f;
        private Define.LookAtDirection _initialLookAtDir = Define.LookAtDirection.Right;
        private bool _isOffParticle = false;
        private bool _isOnlyVisible = false;
        private float _colliderPreDisableLifeDelta = 0f;
        private bool _isOnInterpolateScale = false;
        private Vector3 _interpolateStartScale = Vector3.zero;
        private Vector3 _interpolateTargetScale = Vector3.zero;

        public void SetProjectileInfo(Vector3 shootDir, Define.LookAtDirection lootAtDir, Vector3 localScale, Vector3 indicatorAngle,
                                    float continuousAngle, float continuousSpeedRatio, float continuousFlipX, float continuousFlipY,
                                    float interPolateTargetScaleX, float interpolateTargetScaleY, bool isOnlyVisible)
        {
            _shootDir = shootDir;
            _initialLookAtDir = lootAtDir;
            transform.localScale = localScale;

            if (interPolateTargetScaleX > localScale.x || interpolateTargetScaleY > localScale.y)
            {
                _interpolateStartScale = localScale;
                _interpolateTargetScale = new Vector3(interPolateTargetScaleX, interpolateTargetScaleY, 1f);
                _isOnInterpolateScale = true;
            }
            else
                _isOnInterpolateScale = false;


            //StartDestroy(RepeatSkill.Data.Duration);

            _isOnlyVisible = isOnlyVisible;
            EnableCollider(isOnlyVisible ? false : true);

            // switch (RepeatSkill.Data.RepeatSkillType)
            // {
            //     case Define.TemplateIDs.CreatureStatus.RepeatSkill.PaladinMeleeSwing:
            //         {
            //             _isOffParticle = false;
            //             _targetSkill.SetParticleInfo(indicatorAngle, lootAtDir, continuousAngle, continuousFlipX, continuousFlipY);
            //             StartCoroutine(CoMeleeSwing(continuousSpeedRatio));
            //         }
            //         break;
            // }
        }

        private readonly float Sensitivity = 0.6f;
        private void CheckOffParticle()
        {
            if (_initialLookAtDir != Managers.Game.Player.LookAtDir)
                _isOffParticle = true;
            if (Managers.Game.Player.IsMoving == false)
                _isOffParticle = true;
            if ((Managers.Game.Player.ShootDir - _shootDir).sqrMagnitude > Sensitivity * Sensitivity)
                _isOffParticle = true;
            // if (Vector3.Distance(Managers.Game.Player.ShootDir, _shootDir) > _sensitivity)
            //     _offParticle = true;
        }

        private void PreDisableCollider(float lifeTime)
        {
            if (_isOnlyVisible)
                return;

            _colliderPreDisableLifeDelta += Time.deltaTime;
            if (_colliderPreDisableLifeDelta > lifeTime)
            {
                EnableCollider(false);
                _colliderPreDisableLifeDelta = 0f;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            MonsterController mc = other.GetComponent<MonsterController>();
            if (mc.IsValid() == false)
                return;

            if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.MonsterBody, other.gameObject.layer))
                mc.OnDamaged(Owner, _targetSkill);
        }
    }
}
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
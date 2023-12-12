using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public abstract class UniqueSkill : SkillBase
    {
        public override void Activate()
        {
            base.Activate();

            if (_coSkillActivate != null)
                StopCoroutine(_coSkillActivate);

            IsStopped = false;
            _coSkillActivate = StartCoroutine((CoStartSkill()));
        }

        protected abstract IEnumerator CoStartSkill();
        protected abstract void DoSkillJob(System.Action callback = null);
        protected virtual IEnumerator CoGenerateProjectile()
        {
            int continuousCount = this.Data.ContinuousCount;
            Define.LookAtDirection lootAtDir = Owner.LookAtDir;
            Vector3 shootDir = Owner.ShootDir;
            Vector3 localScale = transform.localScale;
            if (this.Data.UsePresetLocalScale == false)
            {
                localScale = Owner.LocalScale;
                localScale *= 0.8f;
            }
            // Vector3 localScale = Owner.LocalScale;
            // localScale *= 0.8f;

            Vector3 indicatorAngle = Owner.Indicator.eulerAngles;
            float movementSpeed = this.Data.MovementSpeed;
            float rotationSpeed = this.Data.RotationSpeed;
            float lifeTime = this.Data.Duration;
            bool isColliderHalfRatio = this.Data.UseColliderHalfLifeTime;

            float[] continuousAngles = new float[continuousCount];
            float[] continuousSpeedRatios = new float[continuousCount];
            float[] continuousFlipXs = new float[continuousCount];
            float[] continuousFlipYs = new float[continuousCount];

            float[] addtionalLocalPositionXs = new float[continuousCount];
            float[] addtionalLocalPositionYs = new float[continuousCount];

            float[] interpolateTargetScaleXs = new float[continuousCount];
            float[] interpolateTargetScaleYs = new float[continuousCount];
            for (int i = 0; i < continuousCount; ++i)
            {
                continuousSpeedRatios[i] = this.Data.ContinuousSpeedRatios[i];
                continuousFlipXs[i] = this.Data.ContinuousFlipXs[i];
                continuousFlipYs[i] = this.Data.ContinuousFlipYs[i];
                if (this.Owner.IsFacingRight == false)
                {
                    continuousAngles[i] = this.Data.ContinuousAngles[i] * -1;
                    interpolateTargetScaleXs[i] = this.Data.TargetScaleInterpolations[i].x * -1;
                    interpolateTargetScaleYs[i] = this.Data.TargetScaleInterpolations[i].y;
                }
                else
                {
                    continuousAngles[i] = this.Data.ContinuousAngles[i];
                    interpolateTargetScaleXs[i] = this.Data.TargetScaleInterpolations[i].x;
                    interpolateTargetScaleYs[i] = this.Data.TargetScaleInterpolations[i].y;
                }
            }

            int maxBounceCount = this.Data.MaxBounceCount;
            int maxPenetrationCount = this.Data.MaxPenetrationCount;

            Vector3 additionalSpawnPos = Vector3.zero;
            if (this.Data.AdditionalCustomValues.Length != 0)
                additionalSpawnPos = this.Data.AdditionalCustomValues[0].Point3D;

            Vector3 spawnPosOnFirstPoint = (this.Data.IsLaunchedFromFireSocket) ? this.Owner.FireSocketPosition : this.Owner.transform.position;
            for (int i = 0; i < continuousCount; ++i)
            {
                //Vector3 spawnPos = (this.Data.IsOnFireSocket) ? this.Owner.FireSocketPosition : (this.Owner.transform.position + (Vector3.up * this.Data.AddtionalSpawnHeightRatio));
                //Vector3 spawnPos = (this.Data.IsLaunchedFromFireSocket) ? this.Owner.FireSocketPosition : this.Owner.transform.position + this.Data.AdditionalSpawnPosFromOwner;
                Vector3 spawnPos = (this.Data.IsLaunchedFromFireSocket) ? this.Owner.FireSocketPosition : this.Owner.transform.position + additionalSpawnPos;
                if (Utils.IsThief(this.Owner))
                    spawnPos = spawnPosOnFirstPoint;

                SkillBase clone = Managers.Object.Spawn<SkillBase>(spawnPos: spawnPos, templateID: this.Data.TemplateID,
                       spawnObjectType: Define.ObjectType.Skill, isPooling: true);
                clone.InitClone(this.Owner, this.Data);
                if (clone.PC != null)
                {
                    clone.OnProjectileLaunchInfo?.Invoke(this, new ProjectileLaunchInfoEventArgs(
                        lootAtDir: lootAtDir,
                        shootDir: shootDir,
                        localScale: localScale,
                        indicatorAngle: indicatorAngle,
                        movementSpeed: movementSpeed,
                        rotationSpeed: rotationSpeed,
                        lifeTime: lifeTime,
                        continuousAngle: continuousAngles[i],
                        continuousSpeedRatio: continuousSpeedRatios[i],
                        continuousFlipX: continuousFlipXs[i],
                        continuousFlipY: continuousFlipYs[i],
                        interpolateTargetScaleX: interpolateTargetScaleXs[i],
                        interpolateTargetScaleY: interpolateTargetScaleYs[i],
                        isColliderHalfRatio: isColliderHalfRatio,
                        maxBounceCount: maxBounceCount,
                        maxPenetrationCount: maxPenetrationCount
                    ));

                    clone.PC.Launch();
                }

                yield return new WaitForSeconds(Data.ContinuousSpacing);
            }

            Owner.AttackEndPoint = transform.position;
        }

        public virtual void OnActiveMasteryActionHandler() { }
        public virtual void OnActiveEliteActionHandler() { }
        public virtual void OnActiveUltimateActionHandler() { }

        public void EnableParticles(ParticleSystem[] particles, bool isOnEnable)
        {
            for (int i = 0; i < particles.Length; ++i)
            {
                if (isOnEnable)
                {
                    particles[i].gameObject.SetActive(isOnEnable);
                    particles[i].Play();
                }
                else
                    particles[i].gameObject.SetActive(isOnEnable);
            }
        }

        public bool WaitUntilEndOfPlayingParticles(ParticleSystem[] _particles)
        {
            bool isAnyPlaying = false;
            for (int i = 0; i < _particles.Length; ++i)
            {
                if (_particles[i].isPlaying)
                {
                    isAnyPlaying = true;
                    break;
                }
            }

            return (isAnyPlaying == false) ? true : false;
        }

        public void TakeOnParticlesFromParent(ParticleSystem[] particles, Transform parentTarget)
        {
            particles[0].transform.SetParent(parentTarget);
            for (int i = 0; i < particles.Length; ++i)
                particles[i].transform.localPosition = Vector3.zero;
        }

        public void TakeOffParticlesFromParent(ParticleSystem[] particles) => particles[0].transform.SetParent(null);

        public override void Deactivate()
        {
            if (_coSkillActivate != null)
            {
                StopCoroutine(_coSkillActivate);
                _coSkillActivate = null;
            }

            base.Deactivate();
        }
    }
}

// private List<SequenceSkill> SequenceSkills = new List<SequenceSkill>();
// private int _sequenceIdx = 0;
// public void StartNextSequenceSkill()
// {
//     if (this.IsStopped)
//         return;

//     if (this.SequenceSkills.Count == 0)
//         return;

//     //SequenceSkills[_sequenceIdx].DoSkillJob(OnFinishedSequenceSkill);
//     SequenceSkills[_sequenceIdx].DoSkillJob(delegate()
//     {
//         _sequenceIdx = (_sequenceIdx + 1) % SequenceSkills.Count;
//         StartNextSequenceSkill();
//     });
// }

// 하나의 스킬을 쓸때, 다른걸 못쓰니까 Action으로 콜백을 받는 방식으로하면 굉장히 편해질수 있음
// private void OnFinishedSequenceSkill()
// {
//     _sequenceIdx = (_sequenceIdx + 1) % SequenceSkills.Count;
//     StartNextSequenceSkill();
// }
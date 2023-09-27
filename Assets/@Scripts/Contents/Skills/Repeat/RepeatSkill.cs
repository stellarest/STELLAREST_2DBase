using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    // RepeatSkill 자체로는 스킬 제작 불가능
    // 추상 클래스라 RepeatSkill 상속받아서 제작해야함.
    // RepeatSkill은 동시에 2개 이상의 스킬이 발동된다는 특징이 있음
    // Fireball, EgoSword가 동시에 발동됨
    public abstract class RepeatSkill : SkillBase
    {
        private Coroutine _coSkillActivate = null;

        public override void Activate()
        {
            base.Activate();

            if (_coSkillActivate != null)
                StopCoroutine(_coSkillActivate);

            IsStopped = false;
            _coSkillActivate = StartCoroutine(CoStartSkill());
        }

        protected virtual IEnumerator CoStartSkill()
        {
            WaitForSeconds wait = new WaitForSeconds(Data.CoolTime);
            while (true)
            {
                DoSkillJob();
                yield return wait;
            }
        }

        protected virtual void DoSkillJob()
        {
            Owner.AttackStartPoint = transform.position; // -->> CHECK THIS AFTER
            StartCoroutine(CoCloneSkill());
        }

        public virtual void DoSkillJobManually(SkillBase caller, float delay) => StartCoroutine(Delay(caller, delay));
        
        protected virtual IEnumerator Delay(SkillBase caller, float delay)
        {
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
                this.DoSkillJob();
                Managers.Object.Despawn<SkillBase>(caller);
            }
            else
            {
                this.DoSkillJob();
                Managers.Object.Despawn<SkillBase>(caller);
            }
        }

        public void OnCloneExclusiveRepeatSkillHandler()
        {
            if (IsStopped)
                return;

            Owner.AttackStartPoint = transform.position;
            StartCoroutine(this.CoCloneSkill());
        }

        protected virtual IEnumerator CoCloneSkill() // FROM DoSkillJob
        {
            int continuousCount = this.Data.ContinuousCount;
            Define.LookAtDirection lootAtDir = Owner.LookAtDir;
            Vector3 shootDir = Owner.ShootDir;
            Vector3 localScale = Owner.LocalScale;
            localScale *= 0.8f;

            Vector3 indicatorAngle = Owner.Indicator.eulerAngles;
            float movementSpeed = this.Data.MovementSpeed;
            float rotationSpeed = this.Data.RotationSpeed;
            float lifeTime = this.Data.Duration;
            float colliderPreDisableLifeRatio = this.Data.ColliderPreDisableLifeRatio;

            float[] continuousAngles = new float[continuousCount];
            float[] continuousSpeedRatios = new float[continuousCount];
            float[] continuousFlipXs = new float[continuousCount];
            float[] continuousFlipYs = new float[continuousCount];
            float[] interpolateTargetScaleXs = new float[continuousCount];
            float[] interpolateTargetScaleYs = new float[continuousCount];
            bool[] isOnlyVisibles = new bool[continuousCount];
            for (int i = 0; i < continuousCount; ++i)
            {
                continuousSpeedRatios[i] = this.Data.ContinuousSpeedRatios[i];
                continuousFlipXs[i] = this.Data.ContinuousFlipXs[i];
                continuousFlipYs[i] = this.Data.ContinuousFlipYs[i];
                isOnlyVisibles[i] = this.Data.IsOnlyVisibles[i];
                if (this.Owner.IsFacingRight == false)
                {
                    continuousAngles[i] = this.Data.ContinuousAngles[i] * -1;
                    interpolateTargetScaleXs[i] = this.Data.ScaleInterpolations[i].x * -1;
                    interpolateTargetScaleYs[i] = this.Data.ScaleInterpolations[i].y;
                }
                else
                {
                    continuousAngles[i] = this.Data.ContinuousAngles[i];
                    interpolateTargetScaleXs[i] = this.Data.ScaleInterpolations[i].x;
                    interpolateTargetScaleYs[i] = this.Data.ScaleInterpolations[i].y;
                }
            }

            for (int i = 0; i < continuousCount; ++i)
            {
                Vector3 spawnPos = (this.Data.IsOnFireSocket) ? this.Owner.FireSocketPosition : this.Owner.transform.position;
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
                        colliderPreDisableLifeRatio: colliderPreDisableLifeRatio,
                        continuousAngle: continuousAngles[i],
                        continuousSpeedRatio: continuousSpeedRatios[i],
                        continuousFlipX: continuousFlipXs[i],
                        continuousFlipY: continuousFlipYs[i],
                        interpolateTargetScaleX: interpolateTargetScaleXs[i],
                        interpolateTargetScaleY: interpolateTargetScaleYs[i],
                        isOnlyVisible: isOnlyVisibles[i]
                    ));

                    clone.PC.Launch();
                }

                yield return new WaitForSeconds(Data.ContinuousSpacing);
            }
        }

        public override void Deactivate(bool isPoolingClear = false)
        {
            if (IsStopped)
                return;

            if (_coSkillActivate != null)
            {
                StopCoroutine(_coSkillActivate);
                _coSkillActivate = null;
            }

            base.Deactivate(isPoolingClear);
            //IsStopped = true;
        }
    }
}

using System.Collections;
using UnityEngine;

namespace STELLAREST_2D
{
    public abstract class PublicSkill : SkillBase
    {
        public System.Action OnDeactivateRepeatSkill = null; // USE THIS WHEN YOU NEED,, (OPTIONAL)
        public virtual void OnDeactivateRepeatSkillHandler() { }

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
            WaitForSeconds wait = new WaitForSeconds(Data.Cooldown);
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

        public virtual void DoSkillJobManually(SkillBase caller, float delay) 
        {
            if (this.IsStopped && _coSkillActivate != null)
            {
                StopCoroutine(_coSkillActivate);
                _coSkillActivate = null;
                StartDestroy(1f);
                return;
            }

            _coSkillActivate = StartCoroutine(CoDoSkillJobManually(caller, delay));
        }
        
        protected virtual IEnumerator CoDoSkillJobManually(SkillBase caller, float delay)
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

        // public void OnActiveRepeatSkillHandler()
        // {
        //     if (IsStopped)
        //         return;

        //     Owner.AttackStartPoint = transform.position;
        //     this.Owner.ShowMuzzle();
        //     StartCoroutine(this.CoCloneSkill());
        // }

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
            //float colliderPreDisableLifeRatio = this.Data.ColliderPreDisableLifeRatio;
            bool isColliderHalfRatio = this.Data.UseColliderHalfLifeTime;

            float[] continuousAngles = new float[continuousCount];
            float[] continuousSpeedRatios = new float[continuousCount];
            float[] continuousFlipXs = new float[continuousCount];
            float[] continuousFlipYs = new float[continuousCount];

            float [] addtionalLocalPositionXs = new float[continuousCount];
            float [] addtionalLocalPositionYs = new float[continuousCount];

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

            Vector3 spawnPosOnFirstPoint = (this.Data.IsLaunchedFromFireSocket) ? this.Owner.FireSocketPosition : this.Owner.transform.position;
            for (int i = 0; i < continuousCount; ++i)
            {
                Vector3 spawnPos = (this.Data.IsLaunchedFromFireSocket) ? this.Owner.FireSocketPosition : this.Owner.transform.position;
                if (Utils.IsThief(this.Owner))
                    spawnPos = spawnPosOnFirstPoint;
                // if (Utils.IsMeleeSwing(this.Data.OriginalTemplate))
                // {
                //     //Utils.Log("IS MELEE SWING !!");
                //     spawnPos = spawnPosOnFirstPoint;
                // }
                // else
                //     Utils.Log("IS NOT MELEE SWING,,,");
                
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
                        //colliderPreDisableLifeRatio: colliderPreDisableLifeRatio,
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

        // +++ FIXED BOOKMARKS +++
        public override void Deactivate()
        {
            if (IsStopped)
                return;

            if (_coSkillActivate != null)
            {
                OnDeactivateRepeatSkill?.Invoke();
                StopCoroutine(_coSkillActivate);
                _coSkillActivate = null;
            }

            base.Deactivate();
        }
    }
}

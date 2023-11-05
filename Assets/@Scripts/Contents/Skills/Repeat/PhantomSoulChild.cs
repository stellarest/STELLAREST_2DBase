using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STELLAREST_2D.Data;

namespace STELLAREST_2D
{
    public class PhantomSoulChild : RepeatSkill
    {
        private PhantomSoul _parent = null;
        public void SetParent(PhantomSoul parent) => this._parent = parent;

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            GetComponent<Collider2D>().enabled = false;
            foreach (var particle in GetComponentsInChildren<ParticleSystem>())
            {
                var emission = particle.emission;
                emission.enabled = false;
            }
        }

        public override void InitClone(CreatureController ownerFromOrigin, SkillData dataFromOrigin)
        {
            if (this.IsFirstPooling)
            {
                HitCollider = GetComponent<Collider2D>();
                base.InitClone(ownerFromOrigin, dataFromOrigin);

                foreach (var psRenderer in GetComponentsInChildren<ParticleSystemRenderer>())
                {
                    if (psRenderer.gameObject.name.Contains("Trail"))
                        psRenderer.sortingOrder = (int)Define.SortingOrder.Player - 1;
                    else
                        psRenderer.sortingOrder = (int)Define.SortingOrder.Skill;
                }

                this.IsFirstPooling = false;
            }
        }

        protected override IEnumerator CoCloneSkill()
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
            bool isColliderHalfRatio = this.Data.IsColliderHalfRatio;

            float[] continuousAngles = new float[continuousCount];
            float[] continuousSpeedRatios = new float[continuousCount];
            float[] continuousFlipXs = new float[continuousCount];
            float[] continuousFlipYs = new float[continuousCount];

            float [] addtionalLocalPositionXs = new float[continuousCount];
            float [] addtionalLocalPositionYs = new float[continuousCount];

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

            int maxBounceCount = this.Data.MaxBounceCount;
            int maxPenetrationCount = this.Data.MaxPenetrationCount;

            Vector3 spawnPosOnFirstPoint = (this.Data.IsOnFireSocket) ? this.Owner.FireSocketPosition : this.Owner.transform.position;
            for (int i = 0; i < continuousCount; ++i)
            {
                Vector3 spawnPos = (this.Data.IsOnFireSocket) ? this.Owner.FireSocketPosition : this.Owner.transform.position;
                if (Utils.IsThief(this.Owner))
                    spawnPos = spawnPosOnFirstPoint;
                
                this._parent.PlayBursts();
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
                        isOnlyVisible: isOnlyVisibles[i],
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid())
                return;

            cc.OnDamaged(this.Owner, this);
        }

    }
}

using System.Collections;
using Assets.HeroEditor.Common.Scripts.Common;
using STELLAREST_2D.Data;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

using VFXTrail = STELLAREST_2D.Define.TemplateIDs.VFX.Trail;

namespace STELLAREST_2D
{
    public class RangedShot : RepeatSkill
    {
        public SpriteTrail.SpriteTrail SpriteTrail { get; private set; } = null;
        private RangedShotChild _rangedShotChild = null;
        private GameObject _goChild = null;
        private GameObject _trail = null;

        private const int CLONED_KUNAI_MAX_COUNT = 3;
        private const float CLONED_KUNAI_CONTINUOUS_ANGLE = 12f;
        [field: SerializeField] public bool IsLaunchedFromOwner { get; private set; } = false;
        [field: SerializeField] public bool IsAlreadyGeneratedKunais { get; private set; } = false;

        private Vector3 _kunaiUltimateShootDir = Vector3.zero;
        
        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            GetComponent<Rigidbody2D>().simulated = false;
            GetComponent<Collider2D>().enabled = false;
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            if (GetComponentInChildren<SpriteTrail.SpriteTrail>() != null)
                GetComponentInChildren<SpriteTrail.SpriteTrail>().enabled = false;
        }

        public override void InitClone(CreatureController ownerFromOrigin, SkillData dataFromOrigin)
        {
            if (this.IsFirstPooling)
            {
                SR = GetComponentInChildren<SpriteRenderer>();
                RigidBody = GetComponent<Rigidbody2D>();
                HitCollider = GetComponent<Collider2D>();
                base.InitClone(ownerFromOrigin, dataFromOrigin);
                if (Utils.IsArrowMaster(ownerFromOrigin))
                    InitArrowMasterMastery(ownerFromOrigin, dataFromOrigin);
                else if (Utils.IsElementalArcher(ownerFromOrigin))
                    InitElementalArcherMastery(ownerFromOrigin, dataFromOrigin);
                else if (Utils.IsForestGuardian(ownerFromOrigin))
                    InitForestGuardianMastery(ownerFromOrigin, dataFromOrigin);

                this.IsFirstPooling = false;
            }

            // Spawn Trail
            if (Utils.IsElementalArcher(ownerFromOrigin))
                LaunchElementalArcherMastery();
            else if (Utils.IsForestGuardian(ownerFromOrigin))
                LaunchForestGuardianMastery();
            else if (Utils.IsNinja(ownerFromOrigin) && dataFromOrigin.Grade == dataFromOrigin.MaxGrade)
                LaunchNinjaMasteryUltimate();   
        }

        private void InitArrowMasterMastery(CreatureController ownerFromOrigin, SkillData dataFromOrigin)
        {
            if (this.Data.Grade == this.Data.MaxGrade)
                SpriteTrail = GetComponentInChildren<SpriteTrail.SpriteTrail>();
        }

        private void InitElementalArcherMastery(CreatureController ownerFromOrigin, SkillData dataFromOrigin)
        {
            if (this.Data.Grade > Define.InGameGrade.Default)
            {
                _rangedShotChild = Utils.FindChild<RangedShotChild>(gameObject);
                _rangedShotChild.Init(ownerFromOrigin, dataFromOrigin, this);
            }

            if (this.Data.Grade == this.Data.MaxGrade)
                TrailSocket = Utils.FindChild(gameObject, "TrailSocket").transform;
        }

        private void InitForestGuardianMastery(CreatureController ownerFromOrigin, SkillData dataFromOrigin)
        {
            if (this.Data.Grade == this.Data.MaxGrade)
            {
                TrailSocket = Utils.FindChild(gameObject, "TrailSocket").transform;
                _goChild = this.transform.GetChild(2).gameObject;
                _goChild.transform.localScale += (Vector3.one * 0.5f);
            }
        }

        private void LaunchElementalArcherMastery()
        {
            if (_rangedShotChild != null)
            {
                _rangedShotChild.ChildHitCollider.enabled = true;
                SR.enabled = true;
                HitCollider.enabled = true;
            }

            if (TrailSocket != null)
                Managers.VFX.Trail(VFXTrail.Wind, this, this.Owner);
        }

        private void LaunchForestGuardianMastery()
        {
            SR.enabled = true;
            if (_goChild != null)
                _goChild.SetActive(false);

            if (TrailSocket != null)
                _trail = Managers.VFX.Trail(VFXTrail.Light, this, this.Owner);
        }

        private void LaunchNinjaMasteryUltimate()
        {
            this.SR.enabled = true;
            this.IsLaunchedFromOwner = true;
            this.HitCollider.enabled = true;
            _kunaiUltimateShootDir = this.Owner.ShootDir;
        }

        protected override void DoSkillJob()
                => Owner.CreatureState = Define.CreatureState.Skill;

        protected override void SetSortingOrder()
        {
            SR.sortingOrder = (int)Define.SortingOrder.Skill;
            if (SpriteTrail != null)
                SpriteTrail.m_OrderInSortingLayer = (int)Define.SortingOrder.Skill;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            if (Utils.IsArrowMaster(this.Owner))
                cc.OnDamaged(this.Owner, this);
            else if (Utils.IsElementalArcher(this.Owner))
                OnCollisionElementalArcherMastery(cc);
            else if (Utils.IsForestGuardian(this.Owner))
            {
                HitPoint = other.ClosestPoint(this.transform.position);
                OnCollisionForestGuardianMastery(cc, HitPoint);
            }
            else // NINJA TEMP
            {
                cc.OnDamaged(this.Owner, this);
            }
        }
        /*
                // if (this.Data.Grade < this.Data.MaxGrade)
                //     cc.OnDamaged(this.Owner, this);
                // else if (this.Data.Grade == this.Data.MaxGrade && this.IsLaunchedFromOwner)
                // {
                //     cc.OnDamaged(this.Owner, this);
                //     if (this.IsLaunchedFromOwner)
                //     {
                //         //HitPoint = other.ClosestPoint(this.transform.position);
                //         float[] continuousAngles = new float[CLONED_KUNAI_MAX_COUNT];
                //         for (int i = 0; i < CLONED_KUNAI_MAX_COUNT; ++i)
                //         {
                //             if (i == 0)
                //                 continuousAngles[i] = 0f;
                //             else if (i == 1)
                //                 continuousAngles[i] = i * CLONED_KUNAI_CONTINUOUS_ANGLE;
                //             else
                //                 continuousAngles[i] = continuousAngles[i - 1] * -1;
                //         }

                //         Vector3 firstHitPos = this.transform.position;
                //         ProjectileController[] clonesPC = new ProjectileController[CLONED_KUNAI_MAX_COUNT];
                //         for (int i = 0; i < CLONED_KUNAI_MAX_COUNT; ++i)
                //         {
                //             SkillBase clone = Managers.Object.Spawn<SkillBase>(spawnPos: firstHitPos, templateID: this.Data.TemplateID,
                //                 spawnObjectType: Define.ObjectType.Skill, isPooling: true);
                //             clone.InitCloneManually(this.Owner, this.Data);
                //             clone.SR.enabled = true;

                //             clone.PC.SetOptionsManually(this.Owner.ShootDir, this.Data.MovementSpeed, this.Data.Duration, 1f, continuousAngles[i], false, false);
                //             clonesPC[i] = clone.PC;
                //             //clone.PC.Launch();
                //             //_coCheckIsStillHitting = StartCoroutine(this.CoCheckIsStillHitting(clone.GetComponent<RangedShot>()));
                //         }

                //         for (int i = 0; i < clonesPC.Length; ++i)
                //         {
                //             if (clonesPC[i] != null)
                //                 StartCoroutine(CoLaunch(clonesPC[i]));
                //         }
                //     }
                // }
                // else if (CanClonedDamage)
                // {
                //     cc.OnDamaged(this.Owner, this);
                //     this.IsDamagedFromCloned = true;
                // }
        */

        private void OnTriggerExit2D(Collider2D other)
        {
            if (Utils.IsNinja(this.Owner) == false || this.Data.Grade != this.Data.MaxGrade)
                return;
            if (this.IsLaunchedFromOwner && IsAlreadyGeneratedKunais == false)
            {
                //Utils.LogBreak("OnTriggerExit2D BREAK");
                float[] continuousAngles = new float[CLONED_KUNAI_MAX_COUNT];
                for (int i = 0; i < CLONED_KUNAI_MAX_COUNT; ++i)
                {
                    if (i == 0)
                        continuousAngles[i] = 0f;
                    else if (i == 1)
                        continuousAngles[i] = i * CLONED_KUNAI_CONTINUOUS_ANGLE;
                    else
                        continuousAngles[i] = continuousAngles[i - 1] * -1;
                }

                Vector3 outOfPos = this.transform.position;
                //ProjectileController[] clonesPC = new ProjectileController[CLONED_KUNAI_MAX_COUNT];
                for (int i = 0; i < CLONED_KUNAI_MAX_COUNT; ++i)
                {
                    SkillBase clone = Managers.Object.Spawn<SkillBase>(spawnPos: outOfPos, templateID: this.Data.TemplateID,
                        spawnObjectType: Define.ObjectType.Skill, isPooling: true);
                    clone.InitCloneManually(this.Owner, this.Data);
                    clone.SR.enabled = true;

                    clone.PC.SetOptionsManually(_kunaiUltimateShootDir, this.Data.MovementSpeed, this.Data.Duration, 1f, continuousAngles[i], false, false);
                    clone.PC.Launch();
                    //clonesPC[i] = clone.PC;
                    //clone.PC.Launch();
                    //_coCheckIsStillHitting = StartCoroutine(this.CoCheckIsStillHitting(clone.GetComponent<RangedShot>()));
                }

                IsAlreadyGeneratedKunais = true;
            }
        }

        private void OnCollisionElementalArcherMastery(CreatureController cc)
        {
            if (this.Data.Grade == Define.InGameGrade.Default)
                cc.OnDamaged(this.Owner, this);
            else if (_rangedShotChild != null)
            {
                _rangedShotChild.gameObject.SetActive(true);
                SR.enabled = false;
                //HitCollider.enabled = false;
            }
        }

        private void OnCollisionForestGuardianMastery(CreatureController cc, Vector3 hitPoint)
        {
            cc.OnDamaged(this.Owner, this);
            if (_goChild != null)
            {
                _goChild.SetActive(true);
                SR.enabled = false;
            }
    
            if (_trail != null)
                _trail.SetActive(false);
        }

        private void OnDisable()
        {
            if (Utils.IsElementalArcher(this.Owner))
            {
                if (_rangedShotChild != null)
                    _rangedShotChild.SetActive(false);
            }
            else if (Utils.IsForestGuardian(this.Owner))
            {
                if (_goChild != null)
                    _goChild.SetActive(false);
                    
                if (_trail != null)
                    _trail.SetActive(false);
            }
            else if (Utils.IsNinja(this.Owner) && this.Data.Grade == this.Data.MaxGrade)
            {
                if (this.HitCollider != null)
                    this.HitCollider.enabled = false;

                this.IsLaunchedFromOwner = false;
                this.IsAlreadyGeneratedKunais = false;
            }
        }
    }
}

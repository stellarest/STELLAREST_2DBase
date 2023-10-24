using Assets.HeroEditor.Common.Scripts.Common;
using STELLAREST_2D.Data;
using Unity.VisualScripting;
using UnityEngine;

using VFXTrail = STELLAREST_2D.Define.TemplateIDs.VFX.Trail;

namespace STELLAREST_2D
{
    public class RangedShot : RepeatSkill
    {
        public SpriteTrail.SpriteTrail SpriteTrail { get; private set; } = null;
        private RangedShotChild _rangedShotChild01 = null;
        private GameObject _child01 = null;
        private GameObject _child02 = null;

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
                _rangedShotChild01 = Utils.FindChild<RangedShotChild>(gameObject);
                _rangedShotChild01.Init(ownerFromOrigin, dataFromOrigin, this);
            }

            if (this.Data.Grade == this.Data.MaxGrade)
                TrailSocket = Utils.FindChild(gameObject, "TrailSocket").transform;
        }

        private void InitForestGuardianMastery(CreatureController ownerFromOrigin, SkillData dataFromOrigin)
        {
            if (this.Data.Grade == Define.InGameGrade.Elite)
                _child01 = this.transform.GetChild(1).gameObject;
            else if (this.Data.Grade == this.Data.MaxGrade)
            {
                _child01 = this.transform.GetChild(1).gameObject;
                _child02 = this.transform.GetChild(2).gameObject;
            }
        }

        private void LaunchElementalArcherMastery()
        {
            if (_rangedShotChild01 != null)
            {
                _rangedShotChild01.ChildHitCollider.enabled = true;
                SR.enabled = true;
                HitCollider.enabled = true;
            }

            if (this.Data.Grade == this.Data.MaxGrade)
                Managers.VFX.Trail(VFXTrail.Wind, this, this.Owner);
        }

        private void LaunchForestGuardianMastery()
        {
            if (this.Data.Grade > Define.InGameGrade.Default)
            {
                SR.enabled = true;
                if (_child01 != null)
                    _child01.SetActive(false);

                if (_child02 != null)
                    _child02.SetActive(false);
            }
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
        }

        private void OnCollisionElementalArcherMastery(CreatureController cc)
        {
            if (this.Data.Grade == Define.InGameGrade.Default)
                cc.OnDamaged(this.Owner, this);
            else if (_rangedShotChild01 != null)
            {
                _rangedShotChild01.gameObject.SetActive(true);
                SR.enabled = false;
                //HitCollider.enabled = false;
            }
        }

        private void OnCollisionForestGuardianMastery(CreatureController cc, Vector3 hitPoint)
        {
            cc.OnDamaged(this.Owner, this);
            if (this.Data.Grade > Define.InGameGrade.Default)
            {
                SR.enabled = false;

                if (_child01 != null)
                    _child01.SetActive(true);

                if (_child02 != null)
                    _child02.SetActive(true);
            }
        }

        private void OnDisable()
        {
            if (Utils.IsElementalArcher(this.Owner))
            {
                if (_rangedShotChild01 != null)
                    _rangedShotChild01.SetActive(false);
            }
            else if (Utils.IsForestGuardian(this.Owner))
            {
                if (_child01 == null || _child02 == null)
                    return;

                if (_child01 != null)
                    _child01.SetActive(false);

                if (_child02 != null)
                    _child02.SetActive(false);
            }
        }
    }
}

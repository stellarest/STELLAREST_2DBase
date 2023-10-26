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
        }
    }
}

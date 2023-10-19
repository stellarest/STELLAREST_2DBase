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
        private RangedShotChild _childExplosion = null;

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

                this.IsFirstPooling = false;
            }

            // Spawn Trail
            if (Utils.IsElementalArcher(ownerFromOrigin))
                LaunchElementalArcherMastery();
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
                _childExplosion = Utils.FindChild<RangedShotChild>(gameObject);
                _childExplosion.Init(ownerFromOrigin, dataFromOrigin, this);
            }

            if (this.Data.Grade == this.Data.MaxGrade)
                TrailSocket = Utils.FindChild(gameObject, "TrailSocket").transform;
        }

        private void LaunchElementalArcherMastery()
        {
            if (_childExplosion != null)
            {
                _childExplosion.ChildHitCollider.enabled = true;
                SR.enabled = true;
                HitCollider.enabled = true;
            }

            if (this.Data.Grade == this.Data.MaxGrade)
                Managers.VFX.Trail(VFXTrail.Wind, this, this.Owner);
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
        }

        private void OnCollisionElementalArcherMastery(CreatureController cc)
        {
            if (this.Data.Grade == Define.InGameGrade.Default)
                cc.OnDamaged(this.Owner, this);
            else if (_childExplosion != null)
            {
                _childExplosion.gameObject.SetActive(true);
                SR.enabled = false;
                HitCollider.enabled = false;
            }
        }

        private void OnDisable()
        {
            if (Utils.IsElementalArcher(this.Owner))
            {
                if (_childExplosion != null)
                    _childExplosion.SetActive(false);
            }
        }
    }
}

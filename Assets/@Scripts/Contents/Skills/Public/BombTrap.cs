// using System.Collections;
// using System.Collections.Generic;
using System.Collections;
using CartoonFX;
using STELLAREST_2D.Data;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace STELLAREST_2D
{
    /*
        +++++++++++++++++++++++++++++++++++++++++++
            Continuous Count : Max Count
            Duration : Start Explosion Time
            CoolTime : Generate Time
        +++++++++++++++++++++++++++++++++++++++++++
    */
    public class BombTrap : PublicSkill
    {
        public BombTrap Commander { get; private set; } = null;

        private BombTrapChild _childExplosion = null;
        private GameObject _childSmoke = null;
        private GameObject _childFuse = null;

        private bool _isRolling = false;

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            GetComponent<Rigidbody2D>().simulated = false;
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
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
                //Utils.Log("BombTrab Init Clone.");
                Commander = ownerFromOrigin.SkillBook.GetComponentInChildren<BombTrap>();
                _childExplosion = transform.GetChild(0).GetComponent<BombTrapChild>();
                _childExplosion.Init(ownerFromOrigin, dataFromOrigin);

                SR = GetComponent<SpriteRenderer>();
                RigidBody = GetComponent<Rigidbody2D>();
                HitCollider = GetComponent<Collider2D>();
                SetSortingOrder();

                _childSmoke = transform.GetChild(1).gameObject;
                _childFuse = transform.GetChild(2).gameObject;

                base.InitClone(ownerFromOrigin, dataFromOrigin);
                Commander.OnDeactivateRepeatSkill += this.OnDeactivateRepeatSkillHandler;
                this.IsFirstPooling = false;
            }

            StartBombTrap();
        }

        public int _currentCount = 0;
        private float _coolTimeDelta = 0f;
        protected override IEnumerator CoStartSkill()
        {
            while (true)
            {
                //Utils.Log($"{this._currentCount} / {this.Data.ContinuousCount}");
                // this._currentCount < this.Data.ContinuousCount
                if (this._currentCount < this.Data.ContinuousCount)
                {
                    _coolTimeDelta += Time.deltaTime;
                    if (_coolTimeDelta > this.Data.Cooldown)
                    {
                        DoSkillJob();
                        _coolTimeDelta = 0f;
                    }
                }

                yield return null;
            }
        }

        protected override void DoSkillJob()
        {
            StartCoroutine(CoGenerateBombTrap());
        }

        private readonly float MIN_RANDOM_DISTANCE = 5f;
        private readonly float MAX_RANDOM_DISTNACE = 8f;
        private IEnumerator CoGenerateBombTrap()
        {
            SkillBase clone = Managers.Object.Spawn<SkillBase>(spawnPos: Vector3.zero, templateID: this.Data.TemplateID,
                    spawnObjectType: Define.ObjectType.Skill, isPooling: true);
            BombTrap bombTrap = clone as BombTrap;
            bombTrap.InitClone(this.Owner, this.Data);

            Vector3 startPos = this.Owner.Center.position;
            Vector3 targetPos = Utils.GetRandomPosition(startPos, MIN_RANDOM_DISTANCE, MAX_RANDOM_DISTNACE);

            float percent = 0f;
            Quaternion startRot = transform.rotation;
            Quaternion targetRot = Quaternion.Euler(0 ,0, startRot.eulerAngles.z + 359f);

            bombTrap._isRolling = true;
            while (percent < 1f)
            {
                percent += (Time.deltaTime * bombTrap.Data.MovementSpeed);
                bombTrap.transform.position = Vector3.Lerp(startPos, targetPos, percent);

                float angleZ = Mathf.Lerp(startRot.eulerAngles.z, targetRot.eulerAngles.z, percent);
                bombTrap.transform.rotation = Quaternion.Euler(0, 0, angleZ);
                yield return null;
            }

            bombTrap.RigidBody.simulated = true;
            bombTrap.HitCollider.enabled = true;

            bombTrap._childSmoke.SetActive(true);
            bombTrap._childFuse.SetActive(true);
            ++this._currentCount;
            bombTrap._isRolling = false;
        }

        private void StartBombTrap()
        {
            SR.enabled = true;
            RigidBody.simulated = false;
            HitCollider.enabled = false;

            _childExplosion.RigidBody.simulated = false;
            _childExplosion.HitCollider.enabled = false;
            _childExplosion.gameObject.SetActive(false);

            _childSmoke.SetActive(false);
            _childFuse.SetActive(false);
        }

        private void StartExplosion()
        {
            this.RigidBody.simulated = false;
            this.HitCollider.enabled = false;
            StartCoroutine(Managers.VFX.CoMatStrongTint(Define.MaterialColor.White, this, SR, this.Data.Duration, delegate
            {
                _childFuse.SetActive(false); // FIX

                _childExplosion.RigidBody.simulated = true;
                _childExplosion.HitCollider.enabled = true;
                _childExplosion.gameObject.SetActive(true);
                SR.enabled = false;
                this.Commander._currentCount--;
            }));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            StartExplosion();
        }

        protected override void SetSortingOrder() 
            => GetComponent<SortingGroup>().sortingOrder = (int)Define.SortingOrder.Skill;

        public override void OnDeactivateRepeatSkillHandler()
        {
            if (_isRolling)
            {
                // Utils.LogBreak("BREAK.");
                // this.RigidBody.simulated = true;
                // this.HitCollider.enabled = true;

                // this._childSmoke.SetActive(true);
                // this._childFuse.SetActive(true);
                // ++Commander._currentCount;

                // Managers.Object.SkillCounts를 맞추려면 제거하는게 맞는 것 같음
                Managers.Object.Despawn(this.GetComponent<SkillBase>());
                _isRolling = false;
            }
        }
    }
}
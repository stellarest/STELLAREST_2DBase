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
    public class BombTrap : RepeatSkill
    {
        public BombTrap Commander { get; private set; } = null;

        private BombTrapChild _childExplosion = null;
        private GameObject _childSmoke = null;
        private GameObject _childFuse = null;

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
                Utils.Log("BombTrab Init Clone.");

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
                this.IsFirstPooling = false;
            }

            StartBombTrap();
        }

        public int _currentCount = 0;
        private readonly int MAX_COUNT = 5;
        private float _coolTimeDelta = 0f;
        protected override IEnumerator CoStartSkill()
        {
            while (true)
            {
                Utils.Log($"{this._currentCount} / {this.MAX_COUNT}");

                if (this._currentCount < this.MAX_COUNT)
                {
                    _coolTimeDelta += Time.deltaTime;
                    if (_coolTimeDelta > this.Data.CoolTime)
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
            // if (this._currentCount >= this.MAX_COUNT)
            //     yield break;

            SkillBase clone = Managers.Object.Spawn<SkillBase>(spawnPos: Vector3.zero, templateID: this.Data.TemplateID,
                    spawnObjectType: Define.ObjectType.Skill, isPooling: true);
            BombTrap bombTrap = clone as BombTrap;
            bombTrap.InitClone(this.Owner, this.Data);

            Vector3 startPos = this.Owner.Center.position;
            Vector3 targetPos = Utils.GetRandomPosition(startPos, MIN_RANDOM_DISTANCE, MAX_RANDOM_DISTNACE);

            float percent = 0f;
            Quaternion startRot = transform.rotation;
            Quaternion targetRot = Quaternion.Euler(0 ,0, startRot.eulerAngles.z + 359f);

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

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            this.RigidBody.simulated = false;
            this.HitCollider.enabled = false;

            _childExplosion.RigidBody.simulated = true;
            _childExplosion.HitCollider.enabled = true;
            _childExplosion.gameObject.SetActive(true);
            SR.enabled = false;

            this.Commander._currentCount--;
        }

        protected override void SetSortingOrder() 
            => GetComponent<SortingGroup>().sortingOrder = (int)Define.SortingOrder.Skill;
    }
}

// namespace STELLAREST_2D
// {
//     public class BombTrap : RepeatSkill
//     {
//         public enum Child { Sprite, Fuse, Smoke, Explosion, Max }

//         public BombTrap Generator { get; private set; } = null; 
//         private const int MAX_COUNT = 2;
//         public int Count { get; set; } = 0;
//         private Collider2D _bodyCol = null;
//         private GameObject[] _childs = null;
//         public bool IsOnStepped { get; set; } = false;

//         protected override void DoSkillJob()
//         {
//             if (Count < MAX_COUNT)
//             {
//                 Debug.Log("GENERATE BOMB TRAP !!");
//                 GenerateBombTrab();
//                 ++Count;
//             }
//         }

//         // 이것도 풀링이 되기 전까진 계속 똑같이 세팅하니까 바꿔야함. Init은 한번만 되도록.
//         private void GenerateBombTrab()
//         {
//             GameObject go = Managers.Resource.Instantiate(SkillData.PrimaryLabel, pooling: true);
//             BombTrap bombTrap = go.GetComponent<BombTrap>();
//             bombTrap.SetSkillInfo(Owner, SkillData.TemplateID);
//             bombTrap.transform.position = Owner.transform.position;

//             if (Owner?.IsPlayer() == true)
//                 Managers.Collision.InitCollisionLayer(bombTrap.gameObject, Define.CollisionLayers.PlayerAttack);

//             InitBombTrap(bombTrap);
//             StartCoroutine(CoRunning(bombTrap));
//         }

//         private void InitBombTrap(BombTrap bombTrap)
//         {
//             bombTrap._childs = new GameObject[(int)Child.Max];
//             for (int i = 0; i < (int)Child.Max; ++i)
//             {
//                 bombTrap._childs[i] = bombTrap.transform.GetChild(i).gameObject;
//                 if (i != (int)Child.Sprite)
//                     bombTrap._childs[i].SetActive(false);
//                 else
//                     bombTrap._childs[i].SetActive(true);
//             }

//             if (SkillData.InGameGrade == Define.InGameGrade.Legendary)
//                 bombTrap.transform.GetChild((int)Child.Explosion + 1).gameObject.SetActive(false);

//             bombTrap._bodyCol = bombTrap.GetComponent<Collider2D>();
//             bombTrap._bodyCol.enabled = false;
//             bombTrap.Generator = this;
//         }

//         private Vector2 GetBombTrapPosition(BombTrap bombTrap) => bombTrap.transform.position;

//         private void EnableChild(BombTrap bombTrap, Child child, bool enable) 
//                 => bombTrap._childs[(int)child].SetActive(enable);

//         private void EnableChild(GameObject go, Child child, bool enable)
//         {
//             BombTrap bombTrap = go.GetComponent<BombTrap>();
//             bombTrap.EnableChild(bombTrap, child, enable);
//         }
        
//         private IEnumerator CoRunning(BombTrap bombTrap)
//         {
//             Vector2 startPos = GetBombTrapPosition(bombTrap);
//             Vector2 targetPos = Utils.GetRandomPosition(Owner.transform.position, 5f, 8f);

//             float percent = 0f;
//             bombTrap.transform.rotation = Quaternion.identity;
//             Quaternion startRot = bombTrap.transform.rotation;
//             Quaternion targetRot = Quaternion.Euler(0, 0, startRot.eulerAngles.z + 359f);
//             while (percent < 1f)
//             {
//                 percent += Time.deltaTime * SkillData.Speed;
//                 bombTrap.transform.position = Vector2.Lerp(startPos, targetPos, percent);

//                 float angleZ = Mathf.Lerp(startRot.eulerAngles.z, targetRot.eulerAngles.z, percent);
//                 bombTrap.transform.rotation = Quaternion.Euler(0, 0, angleZ);
//                 yield return null;
//             }

//             EnableChild(bombTrap, Child.Fuse, true);
//             EnableChild(bombTrap, Child.Smoke, true);
//             bombTrap._bodyCol.enabled = true;
//         }

//         public override void OnPreSpawned()
//         {
//             base.OnPreSpawned();
//             foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
//                 sr.enabled = false;

//             foreach (var col in GetComponentsInChildren<Collider2D>())
//                 col.enabled = false;

//             GetComponent<Collider2D>().enabled = false;
//             foreach (var particle in GetComponentsInChildren<ParticleSystem>())
//             {
//                 var emission = particle.emission;
//                 emission.enabled = false;
//             }
//         }

//         private void StartExplosion()
//         {
//             _childs[(int)Child.Sprite].SetActive(false);
//             Managers.Effect.ResetEnvSimpleMaterial(_childs[(int)Child.Sprite]);
//             _childs[(int)Child.Explosion].SetActive(true);
//         }

//         private void OnTriggerEnter2D(Collider2D other)
//         {
//             MonsterController mc = other.GetComponent<MonsterController>();
//             if (mc.IsValid() == false)
//                 return;

//             if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.MonsterBody, other.gameObject.layer))
//             {
//                 if (IsOnStepped == false)
//                 {
//                     IsOnStepped = true;
//                     for (Child child = Child.Sprite; child < Child.Max; ++child)
//                         EnableChild(gameObject, child, child == Child.Sprite); // 일단 sprite빼고 모두 끈다.

//                     Managers.Effect.AddEnvSimpleMaterial(_childs[(int)Child.Sprite]);
//                     StartCoroutine(Managers.Effect.EnvSimpleMaterial_StrongTintWhite(_childs[(int)Child.Sprite], SkillData.Duration,
//                         () => StartExplosion()));
//                 }
//             }
//         }
//     }
// }
// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class BombTrap : RepeatSkill
    {
        // public override void SetParticleInfo(Vector3 startAngle, Define.LookAtDirection lookAtDir, float continuousAngle, float continuousFlipX, float continuousFlipY)
        // {
        // }

        protected override void DoSkillJob()
        {
        }
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
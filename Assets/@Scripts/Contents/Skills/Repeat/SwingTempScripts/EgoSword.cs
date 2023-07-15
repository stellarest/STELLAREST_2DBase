// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace STELLAREST_2D
// {
//     public class EgoSword : RepeatSkill
//     {
//         private ParticleSystem[] _swingParticles;

//         public override bool Init()
//         {
//             if (base.Init() == false)
//                 return true;
            
//             Debug.Log("### EgoSword INIT ###");

//             _swingParticles = new ParticleSystem[4];
//             // for (int i = 0; i < _swingParticles.Length; ++i)
//             // {
//             //     //string childObjectName = "EgoSword_Melee_";
//             //     string childObjectName = Define.PlayerController.EGO_SWORD_CHILD;
//             //     childObjectName += (i + 1).ToString("D2");
//             //     _swingParticles[i] = Utils.FindChild(gameObject, childObjectName).GetComponent<ParticleSystem>();
//             // }

//             return true;
//         }

//         protected override IEnumerator CoStartSkill()
//         {
//             // WaitForSeconds wait = new WaitForSeconds(CoolTime);
//             WaitForSeconds wait = new WaitForSeconds(1f);

//             while (true)
//             {
//                 Owner.PAC.MeleeSlash();
//                 SetParticles(SwingType.First);
//                 _swingParticles[(int)SwingType.First].gameObject.SetActive(true);
//                 yield return new WaitForSeconds(_swingParticles[(int)SwingType.First].main.duration);

//                 Owner.PAC.MeleeSlash();
//                 SetParticles(SwingType.Second);
//                 _swingParticles[(int)SwingType.Second].gameObject.SetActive(true);
//                 yield return new WaitForSeconds(_swingParticles[(int)SwingType.Second].main.duration);

//                 Owner.PAC.MeleeSlash();
//                 SetParticles(SwingType.Third);
//                 _swingParticles[(int)SwingType.Third].gameObject.SetActive(true);
//                 yield return new WaitForSeconds(_swingParticles[(int)SwingType.Third].main.duration);

//                 Owner.PAC.MeleeJab();
//                 SetParticles(SwingType.Fourth);
//                 _swingParticles[(int)SwingType.Fourth].gameObject.SetActive(true);
//                 yield return new WaitForSeconds(_swingParticles[(int)SwingType.Fourth].main.duration);

//                 yield return wait;
//             }
//         }

//         private void SetParticles(SwingType swingType)
//         {
//             if (Managers.Game.Player == null)
//                 return;

//             Vector3 tempAngle = Managers.Game.Player.Indicator.eulerAngles;
//             transform.localEulerAngles = tempAngle;
//             transform.position = Managers.Game.Player.transform.position;

//             float radian = Mathf.Deg2Rad * tempAngle.z * -1f;
//             var main = _swingParticles[(int)swingType].main;
//             main.startRotation = radian;
//         }

//         private void OnTriggerEnter2D(Collider2D other)
//         {
//             MonsterController mc = other.transform.GetComponent<MonsterController>();
//             if (mc.IsValid() == false)
//                 return;
            
//             // TEMP
//             // ObjManager에서 skillInfo 못받아오는중
//             // Damage = 1000;
//             // mc.OnDamaged(Owner, null, Damage);
//         }

//         protected override void DoSkillJob()
//         {
//         }
//     }
// }


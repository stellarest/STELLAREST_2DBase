// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace STELLAREST_2D
// {
//     public class GaryPaladinSwing : RepeatSkill
//     {
//         private GaryPaladinSwingChild[] _garyPaladinSwingChild;

//         public override void SetInitialSkillInfo(CreatureController owner, int templateID)
//         {
//             Utils.InitLog(this.GetType());
//             base.SetInitialSkillInfo(owner, templateID);

//             _garyPaladinSwingChild = new GaryPaladinSwingChild[(int)RepeatType.RepeatMax];
//             for (int i = 0; i < _garyPaladinSwingChild.Length; ++i)
//             {
//                 string child = SkillData.Name + "_v" + (i + 1).ToString("D2");
//                 _garyPaladinSwingChild[i] = Utils.FindChild(gameObject, child).GetComponent<GaryPaladinSwingChild>();
//                 ParticleSystemRenderer particleRenderer = _garyPaladinSwingChild[i].GetComponent<ParticleSystemRenderer>();
//                 particleRenderer.sortingOrder = (int)Define.SortingOrder.ParticleEffect;
//                 _garyPaladinSwingChild[i].Init(Owner, SkillData);
//             }
 
//             Managers.Game.Player.AnimEvents.OnRepeatAttack += EnableSwingParticle;
//         }

//         private void EnableSwingParticle()
//         {
//             _garyPaladinSwingChild[(int)SkillData.InGameGrade].gameObject.SetActive(true);
//         }

//         protected override IEnumerator CoStartSkill()
//         {
//             //WaitForSeconds wait = new WaitForSeconds(SkillData.CoolTime);
//             WaitForSeconds wait = new WaitForSeconds(0.25f);
//             while (true)
//             {
//                 DoSkillJob();
//                 yield return wait;
//             }
//         }

//         protected override void DoSkillJob() 
//         {
//             Managers.Game.Player.Attack();
//         }

//         private void OnDestroy()
//         {
//             if (Managers.Game.Player != null)
//                 Managers.Game.Player.AnimEvents.OnRepeatAttack -= EnableSwingParticle;
//         }
//     }
// }

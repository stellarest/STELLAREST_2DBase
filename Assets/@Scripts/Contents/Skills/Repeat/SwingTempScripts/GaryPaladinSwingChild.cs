// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace STELLAREST_2D
// {
//     public class GaryPaladinSwingChild : MonoBehaviour
//     {
//         private const float TO_SCALE_ONE = 0.8f;
        
//         private ParticleSystem _particle;
//         private CreatureController _owner;
//         private Data.SkillData _skillData;

//         public void Init(CreatureController owner, Data.SkillData skillData)
//         {
//             _particle = GetComponent<ParticleSystem>();
//             ParticleSystemRenderer particleRenderer = GetComponent<ParticleSystemRenderer>();
//             particleRenderer.sortingOrder = (int)Define.SortingOrder.ParticleEffect;

//             this._owner = owner;
//             this._skillData = skillData;
//         }

//         private void Update()
//         {
//             if (_owner.IsMoving)
//                 transform.position += _shootDir * _owner.CreatureData.MoveSpeed * _speed * Time.deltaTime;
//         }

//         private Vector3 _shootDir = Vector3.zero;
//         private float _speed = 1f;
//         private void OnEnable()
//         {
//             if (Managers.Game.Player == null)
//                 return;

//             Vector3 tempAngle = Managers.Game.Player.Indicator.eulerAngles;
//             transform.localEulerAngles = tempAngle;

//             _shootDir = Managers.Game.Player.ShootDir;
//             _speed = _skillData.ProjectileSpeed;
            
//             var main = _particle.main;
//             main.startRotation = Mathf.Deg2Rad * tempAngle.z * -1f;
//             main.flipRotation = Managers.Game.Player.TurningAngle;
//             transform.position = Managers.Game.Player.transform.position + (_shootDir * 4f); // 위치 가져와야함..

//             transform.localScale = Managers.Game.Player.AnimationLocalScale * TO_SCALE_ONE;
//         }
//     }
// }

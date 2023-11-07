// using System.Buffers;
// using System.Collections;
// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEngine;

using System.Collections;

namespace STELLAREST_2D
{
    public class GuardiansShield : BuffBase
    {
        protected override IEnumerator CoBuff()
        {
            throw new System.NotImplementedException();
        }
    }
}

// namespace STELLAREST_2D
// {
//     public class GuardiansShield : BuffBase
//     {
//         private ParticleSystem[] _shildOn = null;
//         private ParticleSystem[] _shildOff = null;
//         private ParticleSystem _hit = null; // Glow_Burst

//         protected override IEnumerator CoBuff() { yield break; }

//         protected override void InitBuff()
//         {
//             GameObject shiledOn = gameObject.transform.GetChild(0).gameObject;
//             _shildOn = shiledOn.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
//             for (int i = 0; i < _shildOn.Length; ++i)
//             {
//                 if (_shildOn[i].gameObject.name.Contains("Hit_Burst"))
//                 {
//                     _hit = _shildOn[i];
//                     break;
//                 }
//             }

//             GameObject shiledOff = gameObject.transform.GetChild(1).gameObject;
//             _shildOff = shiledOff.GetComponentsInChildren<ParticleSystem>(includeInactive: true);

//             if (_target.CreatureData.TemplateID == (int)Define.TemplateIDs.Creatures.Player.Gary_Paladin)
//                 transform.localPosition = new Vector3(0f, 0.7f, 0f);

//             Play();
//         }

//         public void Hit() => _hit.Play();

//         public override void Play()
//         {
//             _target.CreatureData.ShieldHp = _buffData.BuffType.ShieldHp;

//             for (int i = 0; i < _shildOff.Length; ++i)
//                 _shildOff[i].gameObject.SetActive(false);

//             for (int i = 0; i < _shildOn.Length; ++i)
//             {
//                 _shildOn[i].gameObject.SetActive(true);
//                 _shildOn[i].Play();
//             }

//             IsBuffOn = true;
//         }

//         public override void Stop()
//         {
//             for (int i = 0; i < _shildOn.Length; ++i)
//                 _shildOn[i].gameObject.SetActive(false);

//             for (int i = 0; i < _shildOff.Length; ++i)
//             {
//                 _shildOff[i].gameObject.SetActive(true);
//                 _shildOff[i].Play();
//             }

//             IsBuffOn = false;
//         }
//     }
// }

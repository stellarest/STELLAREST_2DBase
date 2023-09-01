using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace STELLAREST_2D
{
    public class GuardiansShield : BuffBase
    {
        private ParticleSystem[] _shildOn = null;
        private ParticleSystem[] _shildOff = null;

        protected override IEnumerator CoBuff() { yield break; }

        protected override void Init()
        {
            GameObject shiledOn = gameObject.transform.GetChild(0).gameObject;
            _shildOn = shiledOn.GetComponentsInChildren<ParticleSystem>(includeInactive: true);

            GameObject shiledOff = gameObject.transform.GetChild(1).gameObject;
            _shildOff = shiledOff.GetComponentsInChildren<ParticleSystem>(includeInactive: true);

            if (_target.CharaData.TemplateID == (int)Define.TemplateIDs.Player.Gary_Paladin)
                transform.localPosition = new Vector3(0f, 0.7f, 0f);

            Play();
        }

        public override void Play()
        {
            _target.CharaData.ShieldHp = _buffData.BuffType.ShieldHp;

            for (int i = 0; i < _shildOff.Length; ++i)
                _shildOff[i].gameObject.SetActive(false);

            for (int i = 0; i < _shildOn.Length; ++i)
            {
                _shildOn[i].gameObject.SetActive(true);
                _shildOn[i].Play();
            }

            IsBuffOn = true;
        }

        public override void Stop()
        {
            for (int i = 0; i < _shildOn.Length; ++i)
                _shildOn[i].gameObject.SetActive(false);

            for (int i = 0; i < _shildOff.Length; ++i)
            {
                _shildOff[i].gameObject.SetActive(true);
                _shildOff[i].Play();
            }

            IsBuffOn = false;
        }
    }
}

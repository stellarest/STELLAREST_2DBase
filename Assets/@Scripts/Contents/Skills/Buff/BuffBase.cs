using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Authentication.ExtendedProtection;
using Unity.VisualScripting;
using UnityEngine;

namespace STELLAREST_2D
{
    public abstract class BuffBase : MonoBehaviour
    {
        [System.Serializable]
        public class BuffType
        {
            public float ShieldHp;
            public float AttackSpeedUpRatio;
            public float DamageUpRatio;
        }

        protected CreatureController _target;
        protected SkillBase _skill;
        protected Data.BonusBuffData _buffData = null;
        public Data.BonusBuffData BuffData => _buffData;

        protected ParticleSystem[] _particles = null;
        public bool IsBuffOn { get; protected set; } = false;
        protected Coroutine _coBuff = null;

        public void StartBuff(CreatureController target, SkillBase skill, Data.BonusBuffData buffData)
        {
            _target = target;
            _skill = skill;
            _buffData = buffData;
            if (buffData.IsOnParent)
            {
                gameObject.transform.SetParent(target.transform);
                gameObject.transform.localPosition = Vector3.zero;
            }
            gameObject.transform.position = target.transform.position;

            Init();

            if (_coBuff != null)
            {
                _coBuff = null;
                StopCoroutine(_coBuff);
            }

            IsBuffOn = true;
            _coBuff = StartCoroutine(CoBuff());
        }

        protected virtual void Init() 
        { 
            _particles = GetComponentsInChildren<ParticleSystem>();
        }

        public void DestroyBuff()
        {
            if (_buffData.IsLoopType)
                return;

            Managers.Resource.Destroy(gameObject);
        }

        public virtual void Play()
        {
            if (_particles.Length > 0)
            {
                for (int i = 0; i < _particles.Length; ++i)
                    _particles[i].Play();
            }
        }

        public virtual void Stop()
        {
            if (_particles.Length > 0)
            {
                for (int i = 0; i < _particles.Length; ++i)
                    _particles[i].Stop();
            }
        }

        protected abstract IEnumerator CoBuff();
    }
}


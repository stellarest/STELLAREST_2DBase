using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using STELLAREST_2D.Data;
using Unity.VisualScripting;
using UnityEngine;

using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

namespace STELLAREST_2D
{
    public class PhantomSoul : SequenceSkill
    {
        private ParticleSystem[] _particles = null;
        private const float INITIAL_LOCAL_POS_X = 3.5f;
        private const float INITIAL_LOCAL_POS_Y = 1f;

        private ParticleSystem[] _bursts = null;
        private const int BURST_MAX_COUNT = 2;
        private const string FIND_ELECTRIC_BURST = "ElectricBurst";
        private const string FIND_ENERGY_BURST = "EnergyBurst";
        
        private PhantomSoulChild _child = null;

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            _particles = GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            for (int i = 0; i < _particles.Length; ++i)
            {
                if (_particles[i].gameObject.name.Contains("Trail"))
                    _particles[i].GetComponent<ParticleSystemRenderer>().sortingOrder = (int)Define.SortingOrder.Player - 1;
                else
                    _particles[i].GetComponent<ParticleSystemRenderer>().sortingOrder = (int)Define.SortingOrder.Skill;

                _particles[i].gameObject.SetActive(false);
            }

            _bursts = new ParticleSystem[BURST_MAX_COUNT];
            for (int i = 0; i < _bursts.Length;)
            {
                for (int j = 0; j < _particles.Length; ++j)
                {
                    if (_particles[j].gameObject.name.Contains(FIND_ELECTRIC_BURST) || 
                        _particles[j].gameObject.name.Contains(FIND_ENERGY_BURST))
                    {
                        _bursts[i++] = _particles[j];
                        continue;
                    }
                }
            }

            // HitCollider = GetComponent<CircleCollider2D>();
            // HitCollider.enabled = false;
            _child = this.Owner.SkillBook.ForceGetSkillMember(SkillTemplate.PhantomSoul_Elite_Child, 0).GetComponent<PhantomSoulChild>();
            _child.SetParent(this);
            this.Owner.SkillBook.LevelUp(SkillTemplate.PhantomSoul_Elite_Child);

            this.Owner.OnLookAtDirChanged += this.OnLookAtDirChangedHandler;
            Utils.Log("ADD EVENT : this.OnLookAtDirChangedHandler");
        }

        public override void DoSkillJob(Action callback = null)
        {
            this.Owner.SkillBook.Deactivate(SkillTemplate.PhantomKnightMastery);
            this.transform.localPosition = new Vector3(INITIAL_LOCAL_POS_X * (int)this.Owner.LookAtDir, INITIAL_LOCAL_POS_Y, 0f);

            this.Owner.ReserveSkillAnimationType(this.Data.AnimationType);
            Owner.CreatureState = Define.CreatureState.Skill;
        }

        public override void OnActiveSequenceSkillHandler()
        {
            for (int i = 0; i < _particles.Length; ++i)
                _particles[i].gameObject.SetActive(true);

            this.transform.DOScale(endValue: this.transform.localScale * 1.5f, 1f).SetEase(Ease.InOutSine).
                SetLoops(-1, LoopType.Yoyo);

            StartCoroutine(CoTickPhantomSoul());
            StartCoroutine(CoActivatePhantomSoulChild_Temp()); // TEMP
        }

        private IEnumerator CoActivatePhantomSoulChild_Temp()
        {
            yield return new WaitForSeconds(3f);
            this.Owner.SkillBook.Activate(SkillTemplate.PhantomSoul_Elite_Child);
            yield return null;
        }

        public void PlayBursts() 
        {
            for (int i = 0; i < _bursts.Length; ++i) 
                _bursts[i].Play();
        }

        private IEnumerator CoTickPhantomSoul()
        {
            while (true)
            {
                this.Owner.FireSocket.position = this.transform.position;
                yield return null;
            }
        }
        
        public bool IsJumping { get; private set; } = false;

        public void OnLookAtDirChangedHandler(Define.LookAtDirection lookAtDir)
        {
            IsJumping = true;
            float jumpPower = 3.15f;
            //float jumpPower = UnityEngine.Random.Range(0f, 1f) >= 0.5f ? -3.5f : 3.15f;
            if (lookAtDir == Define.LookAtDirection.Right)
                jumpPower = -3.5f;

            transform.DOLocalJump(endValue: new Vector3(INITIAL_LOCAL_POS_X * (int)lookAtDir, INITIAL_LOCAL_POS_Y, 0f),
                       jumpPower,
                       numJumps: 1,
                       duration: 0.45f).SetEase(Ease.InOutSine).OnComplete(() => IsJumping = false);

            // transform.DOLocalJump(endValue: new Vector3(INITIAL_LOCAL_POS_X * (int)lookAtDir, INITIAL_LOCAL_POS_Y, 0f),
            //     jumpPower,
            //     numJumps: 1,
            //     duration: 0.45f).SetEase(Ease.InOutSine).OnComplete(() => IsJumping = false).OnComplete(() => _electricBurst.Play());
            // 점프 이후, DOLocalMove 또는 DORotate하는 것도 괜찮을듯
        }

        // this.Owner.SkillBook.LevelUp(SkillTemplate.Phantom_Elite_Child);
        // this.Owner.SkillBook.Activate(SkillTemplate.Phantom_Elite_Child); // 아마 플레이어로부터 발사될것임...
        // 실제로 파이어 소켓의 위치를 Phantom으로 하면 편할것같음
        // 발사할 때 파티클 펑 터지는거 있어야하고, 그거 펑 터질때 Phantom Elite Child 발사하고
        // 베지어 곡선 세팅해야되는데 이건 나중에
        // 파이어소켓으 위치를 실시간으로 이녀석으로 잡고, 파이어소켓으로부터 발사하게하면 될듯
        private void OnDestroy()
        {
            if (this.Owner.OnLookAtDirChanged != null)
            {
                this.Owner.OnLookAtDirChanged -= this.OnLookAtDirChangedHandler;
                Utils.Log("RELEASE EVENT : this.Owner.OnLookAtDirChanged");
            }
        }
    }
}

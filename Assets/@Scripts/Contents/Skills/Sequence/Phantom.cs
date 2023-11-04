using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using STELLAREST_2D.Data;
using UnityEngine;

using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

namespace STELLAREST_2D
{
    public class Phantom : SequenceSkill
    {
        private ParticleSystem[] _particles = null;
        private const float INITIAL_LOCAL_POS_X = 3.5f;
        private const float INITIAL_LOCAL_POS_Y = 1f;

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

            this.Owner.OnLookAtDirChanged += this.OnLookAtDirChangedHandler;
            Utils.Log("ADD EVENT : this.OnLookAtDirChangedHandler");
        }

        public override void DoSkillJob(Action callback = null)
        {
            this.Owner.SkillBook.Deactivate(SkillTemplate.PhantomKnightMastery);
            this.Owner.ReserveSkillAnimationType(this.Data.AnimationType);
            Owner.CreatureState = Define.CreatureState.Skill;

            this.transform.localPosition = new Vector3(INITIAL_LOCAL_POS_X * (int)this.Owner.LookAtDir, INITIAL_LOCAL_POS_Y, 0f);
            // this.transform.DOLocalMoveY(FIXED_MOVE_Y_TARGET, DESIRED_MOVE_Y_DURATION).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            this.transform.DOScale(endValue: this.transform.localScale * 1.5f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }

        public override void OnActiveSequenceSkillHandler()
        {
            for (int i = 0; i < _particles.Length; ++i)
                _particles[i].gameObject.SetActive(true);
        }
        
        public bool IsJumping { get; private set; } = false;
        public void OnLookAtDirChangedHandler(Define.LookAtDirection lootAtDir)
        {
            IsJumping = true;
            float jumpPower = UnityEngine.Random.Range(0f, 1f) >= 0.5f ? -3.5f : 3.15f;
            transform.DOLocalJump(endValue: new Vector3(INITIAL_LOCAL_POS_X * (int)lootAtDir, INITIAL_LOCAL_POS_Y, 0f),
                jumpPower,
                numJumps: 1,
                duration: 0.45f).SetEase(Ease.InOutSine).OnComplete(() => IsJumping = false);
        }

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

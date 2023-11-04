using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

namespace STELLAREST_2D
{
    public class SoulController : BaseController
    {
        private Transform _target = null;
        private bool _isToTarget = false;
        
        public void Init()
        {
            if (this.IsFirstPooling)
            {
                GetComponent<ParticleSystemRenderer>().sortingOrder = (int)Define.SortingOrder.EnvEffect;
                if (Managers.Game.Player.IsValid())
                    _target = Managers.Game.Player.Center;

                this.IsFirstPooling = false;
            }
        }

        public void Run(int runDir)
        {
            _isToTarget = false;
            this.transform.DOJump(
                endValue: _target.position,
                jumpPower: 5 * runDir,
                numJumps: 1,
                duration: 0.5f
            ).SetEase(Ease.InOutSine).OnComplete(() => 
            {
                this.transform.DOMove(_target.position, 0.01f).SetEase(Ease.Linear).OnComplete(() => 
                    Managers.Resource.Destroy(this.gameObject));
            });

            // StartCoroutine(CoTick());
        }

        public void Run()
        {
            var tween = this.transform.DOJump(
                endValue: _target.position,
                jumpPower: 5,
                numJumps: 1,
                duration: 0.25f
            ).SetEase(Ease.Linear);

            //var t2 = this.transform.DOMoveY(1f, 1f, true);
            //t2.OnUpdate(() => t2.ChangeEndValue())
        }

        // private IEnumerator CoTick()
        // {
        //     while (true)
        //     {
        //         if (_isToTarget)
        //         {
        //             this.transform.position = Vector3.Lerp(this.transform.position, _target.position, Time.deltaTime * 20f);
        //             // Vector3 toTargetDir = (_target.position - this.transform.position).normalized;
        //             // this.transform.position += toTargetDir * 30f * Time.deltaTime;
        //         }

        //         float distance = (_target.position - this.transform.position).sqrMagnitude;
        //         if (distance < 0.25f * 0.25f)
        //         {
        //             Managers.Resource.Destroy(this.gameObject);
        //         }

        //         yield return null;
        //     }
        // }
    }
}

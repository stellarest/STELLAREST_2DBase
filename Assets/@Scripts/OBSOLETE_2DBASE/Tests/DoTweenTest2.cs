using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace STELLAREST_2D
{
    public class DoTweenTest2 : MonoBehaviour
    {
        public GameObject Go;
        public GameObject GO_Inner;
        public float duration = 3f;

        public GameObject[] GOs;

        public GameObject GoJumper;
        public GameObject GoShaker;
        public GameObject GoPuncher;
        public GameObject GoTarget;
        public GameObject GoColorChanger;

        private void Start()
        {
            // var sequence = DOTween.Sequence();
            // for (int i = 0; i < GOs.Length; ++i)
            //     sequence.Append(GOs[i].transform.DOMoveX(5, Random.Range(1f, 2f)));

            // sequence.OnComplete(delegate ()
            // {
            //     for (int i = 0; i < GOs.Length; ++i)
            //         GOs[i].transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBounce);
            // });

            // transform.DOMove(new Vector3(5, 0, 0), duration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            // GO_Inner.transform.DORotate(new Vector3(0, 360, 0), duration * 0.5f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
            // GO_Inner.transform.DOLocalMove(new Vector3(0, -3, 0), duration * 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

            // GOs[0].transform.DOMoveX(5, Random.Range(1f, 2f)).OnComplete(() => 
            // {
            //     GOs[1].transform.DOMoveX(5, Random.Range(1f, 2f)).OnComplete(() => 
            //     {
            //         GOs[2].transform.DOMoveX(5, Random.Range(1f, 2f)).OnComplete(() =>
            //         {
            //             for (int i = 0; i < GOs.Length; ++i)
            //             {
            //                 GOs[i].transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBounce);
            //             }
            //         });
            //     });
            // });

            // SetSpeedBased(true) : 같은 시간이 아닌 속력으로 움직이게 함
            // 오브젝트 2개가 있으면 같은 시간이 아닌 같은 속력으로 움직이게 됨
            // Go.transform.DOMoveX(endValue: 3f, duration: 2f).SetSpeedBased(true);

            // AsyncBaby();
            // Tasks();

            // float, vector lerping
            DOVirtual.Float(from: 0f, to: 10f, duration: 3, v => {
                Utils.Log(v);
            }).SetEase(Ease.InBounce).SetLoops(-1, LoopType.Yoyo);
        }

        private float shakeDuration = 0.5f;
        private float shakeStrength = 0.5f;
        private float puncherDuration = 0.5f;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //GoJumper.transform.DOJump(new Vector3(5f, 0, 0), 3f, 1, 0.5f).SetEase(Ease.InOutSine);
                GoJumper.transform.DOJump(
                    endValue: new Vector3(3f, 0, 0),
                    jumpPower: 3f,
                    numJumps: 1,
                    duration: 0.5f
                ).SetEase(Ease.InOutSine);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                var tween = GoShaker.transform.DOShakePosition(shakeDuration, shakeStrength);
                // 여러번 Shake되면 Pos, Rot, Scale등이 전부 바뀌어서 이와 같이 예외처리 해도 됨
                // if (tween.IsPlaying())
                //     return;
                GoShaker.transform.DOShakeRotation(shakeDuration, shakeStrength);
                GoShaker.transform.DOShakeScale(shakeDuration, shakeStrength);

                // tween이 재생되고 있는 오브젝트를 제거하고 싶을 때 트윈부터 Kill한다.
                // DOTWeen은 안전해서 에러가나도 예외를 던지지는 않겠지만,
                // 그래도 DOKill을 하지 않으면 실제로 DOTWeen은 transform에 계속 접근하려고 할 것임.
                // 그래서 무조건 DOTween을 사용했던 오브젝트를 제거하려면 DOKill을 해줘야함.
                // transform.DOKill();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                GoPuncher.transform.DOPunchPosition(
                    punch: Vector3.right * 3, // this is local space
                    duration: puncherDuration,
                    vibrato: 0,
                    elasticity: 0);

                GoTarget.transform.DOShakePosition(
                    duration: 0.5f,
                    strength: 0.5f,
                    vibrato: 10).SetDelay(puncherDuration * 0.5f);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                GoColorChanger.GetComponent<SpriteRenderer>().material.DOColor(Random.ColorHSV(), 0.3f).OnComplete(ChangeColor);
            }
        }

        // Async
        // private async void AsyncBaby()
        // {
        //     for (int i = 0; i < GOs.Length; ++i)
        //     {
        //         await GOs[i].transform.DOMoveX(5f, Random.Range(1f, 2f)).SetEase(Ease.InOutQuad).AsyncWaitForCompletion();
        //     }
        // }

        // private async void Tasks()
        // {
        //     var tasks = new List<Task>();
        //     for (int i = 0; i < GOs.Length; ++i)
        //     {
        //         tasks.Add(GOs[i].transform.DOMoveX(5f, Random.Range(1f, 2f)).SetEase(Ease.InOutQuad).AsyncWaitForCompletion());
        //     }

        //     await Task.WhenAll(tasks);

        //     for (int i = 0; i < GOs.Length; ++i)
        //     {
        //         GOs[i].transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBounce);
        //     }
        // }

        private void ChangeColor()
        {
            GoColorChanger.GetComponent<SpriteRenderer>().material.DOColor(Random.ColorHSV(), 0.3f).OnComplete(this.ChangeColor);
        }
    }
}

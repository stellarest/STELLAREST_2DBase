using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class GemController : BaseController
    {
        public bool Alive = false;
        private float _countTime = 0f;
        private float _selfDespawnTime = 10f;
        private Coroutine _coGemDestroy;

        public override bool Init()
        {
            base.Init();
            Debug.Log("GC INIT");
            return true;
        }

        private void OnEnable()
        {
            _coGemDestroy = StartCoroutine(CoDestroy());
        }

        private IEnumerator CoDestroy()
        {
            while (true)
            {
                if (Alive) // 이 사이에 다시 Alive가 될 수도 있다.
                {
                    _countTime = 0f;
                    // Debug.Log("<color=white> Still Alive.. </color>");
                }

                yield return new WaitUntil(() => (Alive == false));

                _countTime += Time.deltaTime;
                if (_countTime >= _selfDespawnTime)
                {
                    // Debug.Log("<color=red> Despawn Gem </color>");
                    if (this.IsValid()) // 2중으로 체크
                    {
                        Managers.Object.Despawn(this);
                        yield break;
                    }
                }

                // Debug.LogWarning("Gem will be dead");
                yield return null;
            }
        }
    }
}

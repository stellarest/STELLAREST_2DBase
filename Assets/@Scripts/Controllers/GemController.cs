using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace STELLAREST_2D
{
    public enum GemSize { Normal = 1, Large = 2 }

    public class GemController : BaseController
    {
        public bool Alive = false;
        private float _countTime = 0f;
        private float _selfDespawnTime = 10f;
        private Coroutine _coGemDestroy;

        private GemSize _gemSize;
        public GemSize GemSize
        {
            get => _gemSize;
            set
            {
                _gemSize = value;

                SpriteRenderer spr = GetComponent<SpriteRenderer>();
                spr.sortingOrder = (int)Define.SortingOrder.Item;

                if (_gemSize == GemSize.Normal)
                {
                    transform.localScale = Vector2.one * 0.85f;
                    spr.sprite = Managers.Resource.Load<Sprite>(Define.Labels.Sprites.GEM_NORMAL);
                }
                else
                {
                    // transform.localScale = new Vector2(1.25f, 1.25f);
                    transform.localScale = Vector2.one;
                    spr.sprite = Managers.Resource.Load<Sprite>(Define.Labels.Sprites.GEM_LARGE);
                }
            }
        }

        private Coroutine _coMoveToPlayer = null;
        public void GetGem()
        {
            //Managers.Effect.ShowGemGather(this);
            Managers.Object.GridController.Remove(gameObject);
            // IsVaild is isActiveAndEnabled
            if (this.IsValid() && _coMoveToPlayer == null)
            {
                Sequence seq = DOTween.Sequence();
                Vector3 dir = (transform.position - Managers.Game.Player.transform.position).normalized;
                Vector3 target = gameObject.transform.position + dir * 1.5f;
                seq.Append(transform.DOMove(target, 0.15f).SetEase(Ease.Linear)).OnComplete(() =>
                {
                    _coMoveToPlayer = StartCoroutine(CoMoveToPlayer());
                });
            }
        }

        private void OnDisable()
        {
            if (_coMoveToPlayer != null)
            {
                StopCoroutine(_coMoveToPlayer);
                _coMoveToPlayer = null;
            }
        }

        float _minSpeed = 30f;
        float _maxSpeed = 50f;
        private IEnumerator CoMoveToPlayer()
        {
            while (true)
            {
                Vector3 dir = transform.position - Managers.Game.Player.transform.position;
                float distance = dir.magnitude;

                float t = Mathf.Clamp01(Time.time / 0.5f);
                float speed = Mathf.Lerp(_minSpeed, _maxSpeed, t);

                transform.position = Vector3.MoveTowards(transform.position, Managers.Game.Player.transform.position,
                    Time.deltaTime * speed);

                if (distance < 0.3f)
                {
                    Managers.Game.Gem = (int)this.GemSize;
                    //Managers.Effect.ShowGemExplosion(this);
                    Managers.Object.Despawn(this);
                    yield break;
                }

                yield return null;
            }
        }

        private void OnEnable()
        {
            // _coGemDestroy = StartCoroutine(CoDestroy());
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

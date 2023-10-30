using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using SpriteLabels = STELLAREST_2D.Define.Labels.Sprites;
using VFXEnv = STELLAREST_2D.Define.TemplateIDs.VFX.Environment;

namespace STELLAREST_2D
{
    public class GemController : BaseController
    {
        private const float GEM_NORMAL_SCALE_RATIO = 0.85f;
        private const float GEM_LARGE_SCALE_RATIO = 1.1f;
        private SpriteRenderer _sr = null;

        public void Init()
        {
            if (this.IsFirstPooling)
            {
                _sr = GetComponent<SpriteRenderer>();
                _sr.sortingOrder = (int)Define.SortingOrder.Item;
                this.IsFirstPooling = false;
            }

            if (Managers.Game.Player != null)
            {
                float luck = Managers.Game.Player.Stat.Luck;
                if (UnityEngine.Random.Range(0f, 1f) <= luck)
                    GemSize = Define.GemSize.Large;
                else
                    GemSize = Define.GemSize.Normal;
            }
            else
                GemSize = Define.GemSize.Normal;
        }

        private Define.GemSize _gemSize;
        public Define.GemSize GemSize
        {
            get => _gemSize;
            set
            {
                _gemSize = value;
                if (_gemSize == Define.GemSize.Normal)
                {
                    transform.localScale = Vector3.one * GEM_NORMAL_SCALE_RATIO;
                    _sr.sprite = Managers.Resource.Load<Sprite>(SpriteLabels.GEM_NORMAL);
                }
                else
                {
                    transform.localScale = Vector3.one * GEM_LARGE_SCALE_RATIO;
                    _sr.sprite = Managers.Resource.Load<Sprite>(SpriteLabels.GEM_LARGE);
                }
            }
        }

        private Coroutine _coMoveToPlayer = null;
        public void GetGem()
        {
            //Managers.Effect.ShowGemGather(this);
            Managers.VFX.Environment(VFXEnv.GemGather, this.transform.position);
            Managers.Object.GridController.Remove(gameObject);
            if (this.IsValid() && _coMoveToPlayer == null)
            {
                Sequence seq = DOTween.Sequence();
                Vector3 dir = (transform.position - Managers.Game.Player.Center.position).normalized;
                Vector3 target = gameObject.transform.position + (dir * 1.5f);
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
                //Vector3 dir = transform.position - Managers.Game.Player.transform.position;
                Vector3 dir = transform.position - Managers.Game.Player.Center.position;
                float distance = dir.magnitude;

                float t = Mathf.Clamp01(Time.time / 0.5f);
                float speed = Mathf.Lerp(_minSpeed, _maxSpeed, t);

                transform.position = Vector3.MoveTowards(transform.position, Managers.Game.Player.Center.position,
                    Time.deltaTime * speed);

                if (distance < 0.3f)
                {
                    //Managers.Game.Gem = (int)this.GemSize;
                    //Managers.Effect.ShowGemExplosion(this);
                    //Managers.VFX.Environment(VFXEnv.GemExplosion, this.transform.position);
                    Managers.Game.Gem += (int)this.GemSize;
                    Managers.VFX.Environment(VFXEnv.GemExplosion, Managers.Game.Player.Center.position);
                    Managers.Object.Despawn(this);
                    yield break;
                }

                yield return null;
            }
        }
    }
}

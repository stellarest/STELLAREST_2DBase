using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class FadeEffect : MonoBehaviour
    {
        private float fadeTime = 0.5f; // 페이드 효과 완료 시간
        private float endAlpha = 0f; // 페이드 효과 재생 완료 후 알파값
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            // UI마다 현재 설정되어있는 알파값이 다르기 때문에 FadeIn이 종료되는 알파값을 따로 설정
            endAlpha = _spriteRenderer.color.a;
        }

        public void FadeIn()
        {
            // UI 알파값 0 to 1
            StartCoroutine(Fade(0, endAlpha));
        }

        private IEnumerator Fade(float start, float end)
        {
            float current = 0f;
            float percent = 0f;

            while (percent < 1f)
            {
                current += Time.deltaTime;
                percent = current / fadeTime;

                Color color = _spriteRenderer.color;
                color.a = Mathf.Lerp(start, end, percent);
                _spriteRenderer.color = color;

                yield return null;
            }
        }
    }
}

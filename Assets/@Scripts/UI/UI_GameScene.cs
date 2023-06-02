using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace STELLAREST_2D
{
    public class UI_GameScene : UI_Base
    {
        [SerializeField] private TextMeshProUGUI _killCountText;
        [SerializeField] private Slider _gemSlider;

        // int _gemCount; 이런 데이터 코드는 UI코드에 넣지 마셈
        // int _inventoryItemCount;
        // 항상 Data와 UI는 분리
        // 하나의 UI 프리팹 + 무조건 이에 대응하는 스크립트 부착

        public void SetGemCountRatio(float ratio)
        {
            _gemSlider.value = ratio;
        }

        public void SetKillCount(int killCount)
        {
            _killCountText.text = $"{killCount.ToString()}";
        }

        // 규모가 커지면 아래와 같은 방식으로 바꿔야함
        // public void SetInfo()
        // {
        // }

        // public void RefreshUI()
        // {
        // }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace STELLAREST_2D.UI
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
            //_killCountText.text = $"{killCount.ToString()}";
            _killCountText.text = $"{killCount}";
        }

        // 규모가 커지면 아래와 같은 방식으로 바꿔야함
        // UI의 대표적인 정보를 들고있으면,
        // public void SetInfo()
        // {
            // 여기서 정보가 바뀌면 RefreshUI
        // }

        // 하나의 정보가 바뀌면 다른 정보도 체크해서 그 정보들도 Refresh 시킨다.
        // public void RefreshUI()
        // {
        // }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace STELLAREST_2D.UI
{
    public class UI_GameResultPopup : UI_Base // 원래는 UI_POP Base를 따로 파서 해주긴 했었음
    {
        // 규칙
        // 먼저, 스크립트 이름을 최상단 UI 오브젝트 이름과 똑같이 파준다
        // UI 오브젝트에 달려있는 이름을 기억한다. 이것을 Enum으로 쭈욱 파준다.
        private enum GameObjects
        {
            ContentObject, // 반드시 게임 오브젝트의 이름이랑 같아야 함
            ResultRewardScrollContentObject,
            ResultGoldObject,
            ResultKillObject,
        }

        private enum Texts
        {
            GameResultPopupTitleText, // 반드시 게임 오브젝트의 이름이랑 같아야 함
            ResultStageValueText,
            ResultSurvivalTimeText,
            ResultSurvivalTimeValueText,
            ResultGoldValueText,
            ResultKillValueText,
            ConfirmButtonText,
        }

        private enum Buttons
        {
            StatisticsButton, // 반드시 게임 오브젝트의 이름이랑 같아야 함
            ConfirmButton,
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            // 위에서 Enum으로 정의해준것들부터 바인딩
            // ***** GetButton같은거 할 때, 어떻게 제대로 인덱스로 가져와지는지 파악할 것 *****
            BindGameObject(typeof(GameObjects));
            BindText(typeof(Texts));
            BindButton(typeof(Buttons));

            // Button btn = GetButton((int)Buttons.StatisticsButton);
            // UI_Base.BindEvent(btn.gameObject, () => Debug.Log("OnClickStatisticsButton")); // BindEvent는 static 함수니까 extension으로 빼자
            // 근데 지금 버튼 BindEvent는 없는듯??? 클릭했을때 되긴 하겠다.
            GetButton((int)Buttons.StatisticsButton).gameObject.BindEvent(() => Debug.Log("OnClickStatisticsButton")); // 일단 람다로 TEMP 처리
            GetButton((int)Buttons.ConfirmButton).gameObject.BindEvent(() => Debug.Log("OnClickConfirmButton"));

            RefreshUI();

            return true;
        }

        public void SetInfo() // 나중에 이런저런 인자를 넘겨주면 리프레쉬
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            if (_init == false)
                return;

            // 한번에 정보 취합
            GetText((int)Texts.GameResultPopupTitleText).text = "Game Result";
            GetText((int)Texts.ResultStageValueText).text = "4 STAGE";
            GetText((int)Texts.ResultSurvivalTimeText).text = "Survival Time";
            GetText((int)Texts.ResultSurvivalTimeValueText).text = "14:23";
            GetText((int)Texts.ResultGoldValueText).text = "200";
            GetText((int)Texts.ResultKillValueText).text = "100";
            GetText((int)Texts.ConfirmButtonText).text = "OK";
        }
    }
}

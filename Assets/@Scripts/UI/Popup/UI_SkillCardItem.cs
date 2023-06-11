using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D.UI
{
    public class UI_SkillCardItem : UI_Base
    {
        // 어떤 스킬, 몇레벨, 데이터 시트 아이디 등등
        private int _templateID;
        private Data.SkillData _skillData;

        public void SetInfo(int templateID)
        {
            _templateID = templateID;
            Managers.Data.SkillDict.TryGetValue(templateID, out _skillData);
        }

        public void OnClickItem()
        {
            // TODO : 스킬 레벨 업그레이드
            Debug.Log("OnClickItem");
            Managers.UI.ClosePopup(); // 마지막으로 띄운 UI를 꺼줌
        }
    }
}

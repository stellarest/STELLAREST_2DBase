using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class UI_SkillSelectPopup : UI_Base
    {
        [SerializeField] private Transform _grid;

        // 
        private List<UI_SkillCardItem> _skillCardItems = new List<UI_SkillCardItem>();

        private void Start()
        {
            PopulateGrid();
        }

        private void PopulateGrid() // 그냥 뭐 단순하게 Grid 채워주기
        {
            // 먼저 가지고 있는건 날려주고
            foreach (Transform tr in _grid)
                Managers.Resource.Destroy(tr.gameObject);

            // 데이터시트에 따라 어떻게 만들지
            // 만약 스킬 카드 아이템이 100개면 100개를 싹 생성부터하고
            // 그다음에 유저가 안가지고 있는 녀석은 꺼놓기만하고 얻으면 켜주는 방식으로 해도 되고 맘대로
            // 왜냐면 매번 생성하는 것은 부하가 심하다고 함
            for (int i = 0; i < 3; ++i)
            {
                var go = Managers.Resource.Instantiate("UI_SkillCardItem.prefab", pooling: false);
                UI_SkillCardItem skillItem = go.GetOrAddComponent<UI_SkillCardItem>();
                skillItem.transform.SetParent(_grid.transform);
                _skillCardItems.Add(skillItem);
            }
        }
    }
}

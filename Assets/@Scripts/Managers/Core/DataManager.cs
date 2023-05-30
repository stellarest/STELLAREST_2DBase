using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace STELLAREST_2D
{
    // PlayFab으로 빼와야할 것 같은데. XML은 패스
    // 데이터는 원격으로 받아오는 녀석이라 어드레서블로 넣어야함
    public interface ILoader<Key, Value>
    {
        Dictionary<Key, Value> MakeDict();
    }

    public class DataManager
    {
        public Dictionary<int, Data.PlayerData> PlayerDict { get; private set; } = new Dictionary<int, Data.PlayerData>();
        public Dictionary<int, Data.SkillData> SkillDict { get; private set; } = new Dictionary<int, Data.SkillData>();
        public Dictionary<int, Data.MonsterData> MonsterDict { get; private set; } = new Dictionary<int, Data.MonsterData>();

        #region JObject Datas
        private JObject jsonPlayerData = new JObject();
        private JObject jsonSkillData = new JObject();
        private JObject jsonMonsterData = new JObject();
        #endregion

        public void Init()
        {
            PlayerDict = LoadJson<Data.PlayerDataLoader, int, Data.PlayerData>("PlayerData.json").MakeDict();
            SkillDict = LoadJson<Data.SkillDataLoader, int, Data.SkillData>("SkillData.json").MakeDict();
            MonsterDict = LoadJson<Data.MonsterDataLoader, int, Data.MonsterData>("MonsterData.json").MakeDict();
        }

        private T LoadJson<T, Key, Value>(string path) where T : ILoader<Key, Value>
        {
            TextAsset textAsset = Managers.Resource.Load<TextAsset>($"{path}");
            SetJObjects<T>(textAsset.text);

            // FromJson<PlayerDataLoader>으로 DeSerialize되면서 객체화됨
            // 그리고 자동으로, PlayerDataLoader.stats에 json Data가 차곡 차곡 들어감.
            return UnityEngine.JsonUtility.FromJson<T>(textAsset.text);
        }

        #region Data_Debug
        #if UNITY_EDITOR
        public void PrintPlayerJsonData() => Debug.Log("<color=green>" + jsonPlayerData.ToString() + "</color>");
        public void PrintSkillData()
        {
            Debug.Log("==============");
            Debug.Log($"{SkillDict[1].templateID}");
            Debug.Log($"{SkillDict[1].name}");
            Debug.Log($"{SkillDict[1].type}");
            Debug.Log($"{SkillDict[1].prefab}");
            Debug.Log($"{SkillDict[1].damage}");
            Debug.Log($"{SkillDict[1].speed}");
            Debug.Log("==============");
        }

        private void SetJObjects<T>(string text)
        {
            System.Type type = typeof(T);
            if (type == typeof(Data.PlayerDataLoader))
                jsonPlayerData = JObject.Parse(text);
            else if (type == typeof(Data.SkillDataLoader))
                jsonSkillData = JObject.Parse(text);
            else if (type == typeof(Data.MonsterDataLoader))
                jsonMonsterData = JObject.Parse(text);
        }
        #endif
        #endregion

        /*
        	public void Init()
            {
                //PlayerDic = LoadJson<Data.PlayerDataLoader, int, Data.PlayerData>("PlayerData.json").MakeDict();
                PlayerDic = LoadXml<Data.PlayerDataLoader, int, Data.PlayerData>("PlayerData.xml").MakeDict();
            }

            Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
            {
                TextAsset textAsset = Managers.Resource.Load<TextAsset>($"{path}");
                return JsonUtility.FromJson<Loader>(textAsset.text);
            }


        public void Init()
        {
            // LoadJson은 결국 Loader 타입을 반환할 것이므로 MakeDict로 Dic Return
            StatDict = LoadJson<StatData, int, StatInfo>("StatData").MakeDict();            
        }

        private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
        {
            //TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Rookiss/Base/Data/StatData");
            //Debug.Log(textAsset.text);
            TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Rookiss/Base/Data/{path}");

            // --> JsonUtility.FromJson<T> : DeSerialize : 파일(텍스트 등 전송 가능한 형태의 파일)을 실제 메모리로 긁어옴
            return JsonUtility.FromJson<Loader>(textAsset.text);
            
            // List to Dic : data.stats.ToDictionary() --> 옛날에 IOS쪽에서 Linq 버그가 굉장히 많았다고 함. 그래서 Linq 일단 사용 중지
        }
        */
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace STELLAREST_2D
{
    public interface ILoader<Key, Value>
    {
        Dictionary<Key, Value> MakeDict();
    }

    public class DataManager
    {
        public Dictionary<int, Data.CreatureData> CreatureDict { get; private set; } = new Dictionary<int, Data.CreatureData>();
        public Dictionary<int, Data.SkillData> SkillDict { get; private set; } = new Dictionary<int, Data.SkillData>();

        public void Init()
        {
            CreatureDict = LoadJson<Data.CreatureDataLoader, int, Data.CreatureData>
                            (Define.LoadDatas.CREATURES).MakeDict();

            SkillDict = LoadJson<Data.SkillDataLoader, int, Data.SkillData>
                            (Define.LoadDatas.SKILLS).MakeDict();
        }

        private T LoadJson<T, Key, Value>(string path) where T : ILoader<Key, Value>
        {
            TextAsset textAsset = Managers.Resource.Load<TextAsset>($"{path}");
            
            // SetJObjects<T>(textAsset.text); 당장 필요한건 아님
            // FromJson<PlayerDataLoader>으로 DeSerialize되면서 객체화됨
            // 그리고 자동으로, PlayerDataLoader.stats에 json Data가 차곡 차곡 들어감.
            return UnityEngine.JsonUtility.FromJson<T>(textAsset.text);
        }

        public HashSet<string> PrefabKeys = new HashSet<string>();
        public string GetPrefabKey(string key) => PrefabKeys.FirstOrDefault(s => s.Contains(key));
        public void PrintAllPrefabKeys()
        {
            foreach(string key in PrefabKeys) 
                Debug.Log("KEY : " + key);
        }
    }
}

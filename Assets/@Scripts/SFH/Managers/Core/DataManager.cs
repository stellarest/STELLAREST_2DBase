using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public interface ILoader<Key, Value>
    {
        Dictionary<Key, Value> MakeDict();
    }

    public class DataManager
    {
        //public Dictionary<int, Data.TestData> TestData_Temp { get; private set; } = new Dictionary<int, Data.TestData>();
        public Dictionary<int, Data.CreatureData> CreatureDic { get; private set; } = new Dictionary<int, Data.CreatureData>();

        public void Init()
        {
            //TestData_Temp = LoadJson<Data.TestDataLoader, int, Data.TestData>(FixedValue.String.TEST_DATA).MakeDict();
            CreatureDic = LoadJson<Data.CreatureDataLoader, int, Data.CreatureData>(FixedValue.String.CREATURE_DATA).MakeDict();
        }

        private T LoadJson<T, Key, Value>(string path) where T : ILoader<Key, Value>
        {
            TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
            return JsonConvert.DeserializeObject<T>(textAsset.text);
        }
    }
}

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
        public Dictionary<int, Data.PassiveSkillData> PassiveSkillDict { get; private set; } = new Dictionary<int, Data.PassiveSkillData>();

        public void Init()
        {
            CreatureDict = LoadJson<Data.CreatureDataLoader, int, Data.CreatureData>
                            (Define.LoadJson.CREATURE).MakeDict();

            SkillDict = LoadJson<Data.SkillDataLoader, int, Data.SkillData>
                            (Define.LoadJson.SKILL).MakeDict();

            PassiveSkillDict = LoadJson<Data.PassiveSkillDataLoader, int, Data.PassiveSkillData>
                            (Define.LoadJson.PASSIVE_SKILL).MakeDict();
        }

        private T LoadJson<T, Key, Value>(string path) where T : ILoader<Key, Value>
        {
            TextAsset textAsset = Managers.Resource.Load<TextAsset>($"{path}");

            return UnityEngine.JsonUtility.FromJson<T>(textAsset.text);
        }
    }
}

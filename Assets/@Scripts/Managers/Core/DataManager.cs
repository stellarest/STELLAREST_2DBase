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

        public Dictionary<int, Data.BuffData> BuffDict { get; private set; } = new Dictionary<int, Data.BuffData>();


        public void Init()
        {
            CreatureDict = LoadJson<Data.CreatureDataLoader, int, Data.CreatureData>
                            (Define.Labels.Data.CREATURE).MakeDict();

            SkillDict = LoadJson<Data.SkillDataLoader, int, Data.SkillData>
                            (Define.Labels.Data.SKILL).MakeDict();

            PassiveSkillDict = LoadJson<Data.PassiveSkillDataLoader, int, Data.PassiveSkillData>
                            (Define.Labels.Data.PASSIVE_SKILL).MakeDict();

            BuffDict = LoadJson<Data.BuffDataLoader, int, Data.BuffData>
                            (Define.Labels.Data.BUFF).MakeDict();
        }

        private T LoadJson<T, Key, Value>(string path) where T : ILoader<Key, Value>
        {
            TextAsset textAsset = Managers.Resource.Load<TextAsset>($"{path}");

            return UnityEngine.JsonUtility.FromJson<T>(textAsset.text);
        }
    }
}

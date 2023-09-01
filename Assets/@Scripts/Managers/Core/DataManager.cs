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
        public Dictionary<int, Data.BonusStatData> BonusStatDict { get; private set; } = new Dictionary<int, Data.BonusStatData>();

        public Dictionary<int, Data.BonusBuffData> BuffDict { get; private set; } = new Dictionary<int, Data.BonusBuffData>();

        public void Init()
        {
            CreatureDict = LoadJson<Data.CreatureDataLoader, int, Data.CreatureData>
                            (Define.Labels.Data.CREATURE).MakeDict();

            SkillDict = LoadJson<Data.SkillDataLoader, int, Data.SkillData>
                            (Define.Labels.Data.SKILL).MakeDict();

            BonusStatDict = LoadJson<Data.BonusStatDataLoader, int, Data.BonusStatData>
                            (Define.Labels.Data.BONUS_STAT).MakeDict();

            BuffDict = LoadJson<Data.BonusBuffDataLoader, int, Data.BonusBuffData>
                            (Define.Labels.Data.BONUS_BUFF).MakeDict();
        }

        private T LoadJson<T, Key, Value>(string path) where T : ILoader<Key, Value>
        {
            TextAsset textAsset = Managers.Resource.Load<TextAsset>($"{path}");

            return UnityEngine.JsonUtility.FromJson<T>(textAsset.text);
        }
    }
}

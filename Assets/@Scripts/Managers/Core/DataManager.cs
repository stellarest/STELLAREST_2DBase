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
        public Dictionary<int, Data.CreatureData> CreaturesDict { get; private set; } = new Dictionary<int, Data.CreatureData>();
        public Dictionary<int, Data.CreatureStatData> StatsDict { get; private set; } = new Dictionary<int, Data.CreatureStatData>();
        public Dictionary<int, Data.SkillData> SkillsDict { get; private set; } = new Dictionary<int, Data.SkillData>();
        //public Dictionary<int, Data.SequenceSkillData> SequenceSkillsDict { get; private set; } = new Dictionary<int, Data.SequenceSkillData>();
        //public Dictionary<int, Data.BuffSkillData> BuffSkillsDict { get; private set; } = new Dictionary<int, Data.BuffSkillData>();

        public void Init()
        {
            CreaturesDict = LoadJson<Data.CreatureDataLoader, int, Data.CreatureData>
                            (Define.Labels.Data.CREATURE).MakeDict();

            StatsDict = LoadJson<Data.CreatureStatDataLoader, int, Data.CreatureStatData>
                            (Define.Labels.Data.CREATURE_STAT).MakeDict();

            SkillsDict = LoadJson<Data.SkillDataLoader, int, Data.SkillData>
                            (Define.Labels.Data.SKILL).MakeDict();

            // SequenceSkillsDict = LoadJson<Data.SequenceSkillDataLoader, int, Data.SequenceSkillData>
            //                 (Define.Labels.Data.SEQUENCE_SKILL).MakeDict();

            // BuffSkillsDict = LoadJson<Data.BuffSkillDataLoader, int, Data.BuffSkillData>
            //                 (Define.Labels.Data.BUFF_SKILL).MakeDict();
        }

        private T LoadJson<T, Key, Value>(string path) where T : ILoader<Key, Value>
        {
            TextAsset textAsset = Managers.Resource.Load<TextAsset>($"{path}");

            return UnityEngine.JsonUtility.FromJson<T>(textAsset.text);
        }
    }
}

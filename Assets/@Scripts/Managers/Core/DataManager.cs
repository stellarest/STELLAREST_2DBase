using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static STELLAREST_2D.Define;
using STELLAREST_2D.Data;

namespace STELLAREST_2D
{
    public interface ILoader<Key, Value>
    {
        Dictionary<Key, Value> MakeDict();
    }

    public class DataManager
    {
        public Dictionary<int, CreatureData> CreaturesDict { get; private set; } = new Dictionary<int, CreatureData>();
        public Dictionary<int, SkillData> SkillsDict { get; private set; } = new Dictionary<int, SkillData>();

        public void Init()
        {
            CreaturesDict = LoadJson<CreatureDataLoader, int, CreatureData>(FixedValue.Load.CREATURE_DATA).MakeDict();
            SkillsDict = LoadJson<SkillDataLoader, int, SkillData>(FixedValue.Load.SKILL_DATA).MakeDict();
        }

        private T LoadJson<T, Key, Value>(string path) where T : ILoader<Key, Value>
            => UnityEngine.JsonUtility.FromJson<T>(Managers.Resource.Load<TextAsset>($"{path}").text);
    }
}

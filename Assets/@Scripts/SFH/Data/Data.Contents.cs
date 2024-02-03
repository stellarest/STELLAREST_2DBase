using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_SFH.Data
{
    [Serializable]
    public class CreatureData
    {
        public int DataId;
        public string DescriptionTextID;
        public string PrefabLabel;
    }

    [Serializable]
    public class CreatureDataLoader : ILoader<int, CreatureData>
    {
        public List<CreatureData> Creatures { get; } = new List<CreatureData>();

        public Dictionary<int, CreatureData> MakeDict()
        {
            Dictionary<int, CreatureData> dict = new Dictionary<int, CreatureData>();
            
            foreach (var creature in Creatures)
                dict.Add(creature.DataId, creature);

            return dict;
        }
    }
}
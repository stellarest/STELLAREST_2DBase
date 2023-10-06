using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    // 200200 - 200201, 200202
    // 200203 - 200204, 200205
    // 200206 - 200207, 200208

    // Key : TemplateOrigin 
    // -> GetLastSkill (Origin)

    // Elem1 (List1)
    // - Paladin Mastery
    // - Paladin Mastery (Elite)
    // - Paladin Mastery (Ultiate) --> Last SKill

    // Key : TemplateOrigin
    // Elem2 (List2)
    // - Throwing Star
    // - Throwing Star (Elite)

    // Key : TemplateOrigin
    // Elem3 (List3)
    // - Lazer Bolt

    // Key : TemplateOrigin
    // Elem4 (List4)
    // - Spear
    // - Spear (Elite)
    // - Spear (Ultimate)

    public interface IGroupDictionary
    {
        public void InitLeader(object groupLeader);
        public void AddMember(object groupLeader, object groupMember);
    }

    [System.Serializable]
    public class SerializableGroupDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        public const int DictionaryGroupMaxCount = (int)Define.InGameGrade.Ultimate;

        [SerializeField] private List<TKey> _keys = new List<TKey>();
        [SerializeField] private List<TValue> _values = new List<TValue>();

        private Dictionary<int, List<int>> _numberGroups = new Dictionary<int, List<int>>();
        public List<int> GetGroupNumbers(int keyOrigin)
            => _numberGroups.TryGetValue(keyOrigin, out List<int> value) ? value : null;

        public void AddGroup(int key, TValue value)
        {
            if (IsInheritFromIGroupNumberDictionary(value) == false)
                return;

            foreach (KeyValuePair<int, List<int>> pair in _numberGroups)
            {
                if (pair.Value.Contains(key))
                {
                    key = pair.Key;
                    break;
                }
            }

            // Add Group Leader
            if (_numberGroups.ContainsKey(key) == false)
            {
                List<int> groups = new List<int>();
                for (int i = key; i < key + DictionaryGroupMaxCount; ++i)
                    groups.Add(i);

                _numberGroups.Add(groups[0], groups);
                (value as IGroupDictionary).InitLeader(value);
                this.Add((TKey)(object)key, value);
            }
            else
            {
                // Value of Group Leader
                TValue leaderValue = this[(TKey)(object)key];
                (leaderValue as IGroupDictionary).AddMember(leaderValue, value);
            }
        }

        private bool IsInheritFromIGroupNumberDictionary(TValue value)
        {
            if (value is IGroupDictionary)
                return true;
            else
            {
                Utils.LogCritical(nameof(SerializableGroupDictionary<TKey, TValue>), nameof(IsInheritFromIGroupNumberDictionary),
                "You have to inherit from IGroupNumberDictionary for using group members of dictionary");
                return false;
            }
        }

        // -------------------------------------------------------------------------------------
        // -------------------------------------------------------------------------------------
        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                _keys.Add(pair.Key);
                _values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            // save dict to list
            this.Clear();
            if (_keys.Count != _values.Count)
                throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Both types of key and value have to be serializable."));

            for (int i = 0; i < _keys.Count; ++i)
                this.Add(_keys[i], _values[i]);
        }
    }
}
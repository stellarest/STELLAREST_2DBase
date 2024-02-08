using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_SFH.Data
{
    // TestData
    [Serializable]
    public class TestData
    {
		public int Level;
		public int Exp;
		public List<int> Skills;
		public float Speed;
		public string Name;
    }

    [Serializable]
    public class TestDataLoader : ILoader<int, TestData>
    {
        // Excel To Json -> public 접근 지정자만 가능 (프로퍼티 X)
        public List<TestData> MySuperTests = new List<TestData>();

        public Dictionary<int, TestData> MakeDict()
        {
            Dictionary<int, TestData> dict = new Dictionary<int, TestData>();

            foreach (var test in MySuperTests)
                dict.Add(test.Level, test);

            return dict;
        }
    }
}
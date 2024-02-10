using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_SFH.Data
{
    [Serializable]
	public class CreatureData
	{
		public int DataID;
		public string DescriptionTextID;
		public string PrefabLabel;
		public float ColliderOffsetX;
		public float ColliderOffsetY;
		public float ColliderRadius;
		public float Mass;
		public float MaxHp;
		public float MaxHpBonus;
		public float Atk;
		public float AtkRange;
		public float AtkBonus;
		public float Def;
		public float MoveSpeed;
		public float TotalExp;
		public float HpRate;
		public float AtkRate;
		public float DefRate;
		public float MoveSpeedRate;
		public string AnimatorDataID;
		public string AnimatorName;
		public List<int> SkillIdList = new List<int>();
		public int DropItemId;
	}

	[Serializable]
	public class CreatureDataLoader : ILoader<int, CreatureData>
	{
		public List<CreatureData> creatures = new List<CreatureData>();

		public Dictionary<int, CreatureData> MakeDict()
		{
			Dictionary<int, CreatureData> dict = new Dictionary<int, CreatureData>();
			foreach (CreatureData creature in creatures)
				dict.Add(creature.DataID, creature);
			return dict;
		}
	}

    #if UNITY_EDITOR
    #region TEST_DATA
    // TestData
    [Serializable]
    public class TestData
    {
		public int Level;
		public int WhatThe;
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
    #endregion
    #endif
}
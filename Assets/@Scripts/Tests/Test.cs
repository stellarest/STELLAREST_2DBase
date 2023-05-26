using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class Test : MonoBehaviour
    {
        public Dictionary<string, int> tDict = new Dictionary<string, int>();

        [ContextMenu("TEST_DICT")]
        private void TEST_DICT()
        {
            tDict.Add("aaa", 111);
            tDict.Add("bbb", 222);
            tDict.Add("ccc", 333);

            if (tDict.TryGetValue("bbb", out int result))
                Debug.Log("RESULT : " + result);

            int a = result + 888;
            Debug.Log(a);

            if (tDict.TryGetValue("ddd", out int result2))
            {
                Debug.Log("RESULT2 SUCCESS");
                Debug.Log("RESULT2 : " + result2);
            }
            else
            {
                Debug.Log("RESULT2 FAILED");
                Debug.Log("RESULT2 : " + result2);
            }
        }

        [ContextMenu("TEST_HASH")]
        private void TEST_HASH()
        {
            HashSet<int> hash = new HashSet<int>();
            hash.Add(1);
            hash.Add(2);
            hash.Add(3);
            hash.Add(1);
            hash.Add(3);
            hash.Add(3);

            Debug.Log("COUNT : " + hash.Count);
            foreach (var elem in hash)
                Debug.Log("elem : " + elem);

            hash.Remove(2);
            Debug.Log("REMOVE 2");

            bool containsOne = hash.Contains(1);
            bool containsTwo = hash.Contains(2);

            Debug.Log("ContainsOne : " + containsOne);
            Debug.Log("ContainsTwo : " + containsTwo);
        }

        private void SomeMethod(int yourNumber, string yourName)
        {
        }

        private void CallSomeMethod()
        {
            SomeMethod(yourNumber: 999, yourName: "Shown");
        }

        // public GameObject go;

        // private void Start()
        // {
        //     if (go != null)
        //     {
        //         for (int i = 0; i < go.transform.childCount; ++i)
        //             Debug.Log(go.transform.GetChild(i).gameObject.name);
        //         Debug.Log("============================");
        //         foreach (var elem in go.GetComponentsInChildren<Transform>())
        //             Debug.Log(elem.gameObject.name);
        //     }
        // }

        // [ContextMenu("SubsTest")]
        // private void SubsTest()
        // {
        //     string monsterName = "Monster1 (Clone)";
        //     int indexOf = monsterName.IndexOf("(Clone)");
        //     monsterName = monsterName.Substring(0, indexOf);
        //     Debug.Log(monsterName);
        // }

        // private string myName = string.Empty;

        // private void Start()
        // {
        //     string monsterName = "Monster1 (Clone)";
        //     int index = monsterName.IndexOf("(Clone)");
        //     monsterName = monsterName.Substring(0, index);
        //     Debug.Log($"NAME : {monsterName}");
        // }
    }
}

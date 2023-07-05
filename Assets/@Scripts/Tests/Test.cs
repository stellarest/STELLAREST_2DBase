using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D.Test
{
    public class Test : MonoBehaviour
    {
        public AnimationCurve ViewCurveShape;
        private AnimationCurve _curve;

        public enum MyEnumTest
        {
            HongGilDong = 10001,
            KimDDoKkang = 10002,
            SiRaSoNi = 10003
        }

        [ContextMenu("T_E_S_T")]
        public void T_E_S_T()
        {
            List<int> lst = new List<int>();
            lst.Add(10001);
            lst.Add(10002);

            // 헐.. 이렇게하면 int가 나오고
            // foreach(int t in lst)
            // {
            //     string name = t.ToString();
            //     Debug.Log(name);
            // }

            // 이렇게하면 Enum 이름으로 나옴
            // foreach (MyEnumTest t in lst)
            // {
            //     string name = t.ToString();
            //     Debug.Log(name);
            // }
        }

        private void Start()
        {
            // _curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 3));
            // _curve.preWrapMode = WrapMode.PingPong;
            // _curve.postWrapMode = WrapMode.PingPong;

            // Managers.Resource.LoadAllAsync<Object>("PreLoad", (delegate (string key, int count, int totalCount)
            // {
            //     // 굳이 count / totalCount를 적어놓은 이유는 처음에 로딩화면, 로딩바에서 1,2,3,4,5... 이를 이용해 퍼센테이지로 표현 가능
            //     // 이런 프로세스바를 나중에 구현하기 위함
            //     Debug.Log($"Key : {key}, Count : {count} / TotalCount : {totalCount}");
            //     // if (count == totalCount)
            //     //     StartLoaded();
            // }));
        }

        // float elapsedTime = 0f;
        // float desiredTime = 3f;

        // private void Update()
        // {
        //     elapsedTime += Time.deltaTime;
        //     float percent = elapsedTime / desiredTime;
        //     // 0 ~ 3초까진 Ping으로 올라가고 (to 100%)
        //     // 3 ~ 6초까진 Pong으로 내려옴. 총 6초 (to 200%)
        //     if (percent < 2f)
        //     {
        //         Debug.Log("Time : " + elapsedTime);
        //         Debug.Log("Percent : " + percent);
        //         transform.position = new Vector3(transform.position.x, _curve.Evaluate(percent), transform.position.z);
        //     }
        // }

        // public Transform From;
        // public Transform To;
        // private void Update()
        // {
        //     float angle = Vector2.SignedAngle(From.position, To.position);
        //     Debug.Log(angle);
        // }

        // public Dictionary<string, int> tDict = new Dictionary<string, int>();

        // [ContextMenu("TestListAddRange")]
        // private void TestListAddRange()
        // {
        //     List<int> lst1 = new List<int>() { 1, 2, 3 };
        //     HashSet<int> hash = new HashSet<int>() { 4, 5, 6, 7} ;
        //     lst1.AddRange(hash);

        //     Debug.Log("lst1.AddRange(hashSet)");
        //     for (int i = 0; i < lst1.Count; ++i)
        //         Debug.Log(lst1[i]);
        // }

        // [ContextMenu("TestStringReplace")]
        // private void TestStringReplace()
        // {
        //     string key = "EXPGem_01.sprite";
        //     string loadKey = key.Replace(".sprite", "");
        //     loadKey = loadKey.Replace("EXP", "Gold"); // GoldGem_01
        //     Debug.Log(loadKey);
        // }

        // [ContextMenu("TEST_DICT")]
        // private void TEST_DICT()
        // {
        //     tDict.Add("aaa", 111);
        //     tDict.Add("bbb", 222);
        //     tDict.Add("ccc", 333);

        //     if (tDict.TryGetValue("bbb", out int result))
        //         Debug.Log("RESULT : " + result);

        //     int a = result + 888;
        //     Debug.Log(a);

        //     if (tDict.TryGetValue("ddd", out int result2))
        //     {
        //         Debug.Log("RESULT2 SUCCESS");
        //         Debug.Log("RESULT2 : " + result2);
        //     }
        //     else
        //     {
        //         Debug.Log("RESULT2 FAILED");
        //         Debug.Log("RESULT2 : " + result2);
        //     }
        // }

        // [ContextMenu("TEST_HASH")]
        // private void TEST_HASH()
        // {
        //     HashSet<int> hash = new HashSet<int>();
        //     hash.Add(1);
        //     hash.Add(2);
        //     hash.Add(3);
        //     hash.Add(1);
        //     hash.Add(3);
        //     hash.Add(3);

        //     Debug.Log("COUNT : " + hash.Count);
        //     foreach (var elem in hash)
        //         Debug.Log("elem : " + elem);

        //     hash.Remove(2);
        //     Debug.Log("REMOVE 2");

        //     bool containsOne = hash.Contains(1);
        //     bool containsTwo = hash.Contains(2);

        //     Debug.Log("ContainsOne : " + containsOne);
        //     Debug.Log("ContainsTwo : " + containsTwo);
        // }

        // private void SomeMethod(int yourNumber, string yourName)
        // {
        // }

        // private void CallSomeMethod()
        // {
        //     SomeMethod(yourNumber: 999, yourName: "Shown");
        // }

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

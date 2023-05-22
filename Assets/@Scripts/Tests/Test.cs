using UnityEngine;

namespace STELLAREST_2D
{
    public class Test : MonoBehaviour
    {
        [ContextMenu("SubsTest")]
        private void SubsTest()
        {
            string monsterName = "Monster1 (Clone)";
            int indexOf = monsterName.IndexOf("(Clone)");
            monsterName = monsterName.Substring(0, indexOf);
            Debug.Log(monsterName);
        }

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

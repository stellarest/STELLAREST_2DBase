using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class DrawCenterTest : MonoBehaviour
    {
        public CreatureController Target = null;
        CreatureController prevTarget = null;
        bool checkLog = false;
        private void Update()
        {
            if (Target == null)
                return;

            // CHECK CREATURE'S CENTER POS
            Vector3 toTargetDir = (Target.Center.position - this.transform.position);
            float toTargetDistance = toTargetDir.magnitude;
            if (!checkLog)
            {
                Utils.Log($"TARGET : {Target.gameObject.name}");
                Utils.Log($"TARGET'S CENTER : {Target.Center.gameObject.name}");
                checkLog = !checkLog;
                prevTarget = Target;
            }

            if (prevTarget != Target)
                checkLog = false;

            Debug.DrawRay(this.transform.position, toTargetDir.normalized * toTargetDistance, Color.magenta, -1f);
        }

    }
}

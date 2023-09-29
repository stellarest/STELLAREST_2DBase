using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class DebugDrawLines : MonoBehaviour
    {
        public float MIN_DISTANCE = 0f;
        public float MAX_DISTANCE = 0f;
        public GameObject DebugTarget = null;
        public bool IsOn = false;

        private void Update()
        {
            if (IsOn == false || DebugTarget == null)
                return;

            // LEFT
            Debug.DrawRay(DebugTarget.transform.position, DebugTarget.transform.right * -1 * MAX_DISTANCE, Color.red, -1f);
            Debug.DrawRay(DebugTarget.transform.position, DebugTarget.transform.right * -1 * MIN_DISTANCE, Color.blue, -1f);

            // RIGHT
            Debug.DrawRay(DebugTarget.transform.position, DebugTarget.transform.right * MAX_DISTANCE, Color.red, -1f);
            Debug.DrawRay(DebugTarget.transform.position, DebugTarget.transform.right * MIN_DISTANCE, Color.blue, -1f);

            // UP
            Debug.DrawRay(DebugTarget.transform.position, DebugTarget.transform.up * MAX_DISTANCE, Color.red, -1f);
            Debug.DrawRay(DebugTarget.transform.position, DebugTarget.transform.up * MIN_DISTANCE, Color.blue, -1f);

            // DOWN
            Debug.DrawRay(DebugTarget.transform.position, DebugTarget.transform.up * -1 * MAX_DISTANCE, Color.red, -1f);
            Debug.DrawRay(DebugTarget.transform.position, DebugTarget.transform.up * -1 * MIN_DISTANCE, Color.blue, -1f);
        }
    }
}

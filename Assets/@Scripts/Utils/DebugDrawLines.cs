using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class DebugDrawLines : MonoBehaviour
    {
        public enum DrawType { None, Line, Circle }
        public DrawType Type = DrawType.None;

        [Header("Draw Lines")]
        public float MIN_DISTANCE = 0f;
        public float MAX_DISTANCE = 0f;
        public GameObject DebugTarget = null;
        public bool IsOn = false;

        [Header("Draw Circle")]
        public int CircleSegments = 50;
        public float Radius = 5f;

        private void Update()
        {
            if (IsOn == false || DebugTarget == null || Type == DrawType.None || Type == DrawType.Circle)
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

        private void OnDrawGizmos()
        {
            if (IsOn == false || Type == DrawType.None || Type == DrawType.Line)
                return;

            float angle = 360f / CircleSegments;
            Vector3 startPoint = this.transform.position + (Vector3.right * Radius);
            
            for (int i = 0; i < CircleSegments; ++i)
            {
                float x = Mathf.Cos(Mathf.Deg2Rad * (i * angle)) * Radius;
                float y = Mathf.Sin(Mathf.Deg2Rad * (i * angle)) * Radius;

                Vector3 endPoint = this.transform.position + new Vector3(x, y, 0f);
                Gizmos.DrawLine(startPoint, endPoint);
                startPoint = endPoint;
            }
        }
    }
}

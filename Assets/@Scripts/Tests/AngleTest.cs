using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace STELLAREST_2D
{
    public class AngleTest : MonoBehaviour
    {
        public float MyAngle = 0;
        public float AngleOne = 30f;
        public float AngleTwo = -30f;

        Vector2 dir = Vector2.zero;

        public GameObject Target1;
        public GameObject Target2;

        private void Start()
        {
            dir = transform.up;
        }

        private void LateUpdate()
        {
            // transform.rotation = Quaternion.Euler(0, 0, MyAngle);
            // dir = transform.up;

            // Vector2 testVec1 = Quaternion.Euler(0, 0, AngleOne) * dir;
            // Target1.transform.position = testVec1 * 5f;

            // Vector2 testVec2 = Quaternion.Euler(0, 0, AngleTwo) * dir;
            // Target2.transform.position = testVec2 * 5f;

            Debug.DrawRay(transform.position, transform.up * 5f, Color.red, -1f);
            Debug.Log("LOCAL : " + transform.up);

            Debug.Log("LOCAL TO WORLD : " + transform.TransformDirection(transform.up));
            Debug.DrawRay(transform.position, transform.TransformDirection(transform.up) * 3f, Color.magenta, -1f);
        }

        [ContextMenu("UP_DIR")]
        private void UP_DIR()
        {
            dir = transform.up;
        }

        [ContextMenu("RIGHT_DIR")]
        private void RIGHT_DIR()
        {
            dir = transform.right;
        }
    }
}

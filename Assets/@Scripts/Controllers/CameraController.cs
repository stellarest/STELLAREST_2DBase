using UnityEngine;

namespace STELLAREST_2D
{
    public class CameraController : MonoBehaviour
    {
        public GameObject Target { get; set; }

        private void LateUpdate()
        {
            if (Target == null)
                return;

            Vector3 targetPos = Target.transform.position;
            transform.position = new Vector3(targetPos.x, targetPos.y, -10f);
        }
    }
}
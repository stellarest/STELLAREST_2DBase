using UnityEngine;
using Cinemachine;

namespace STELLAREST_2D
{
    public class CameraController : MonoBehaviour
    {
        public GameObject Target { get; set; }

        public void SetTarget(GameObject target)
        {
            GetComponent<CinemachineVirtualCamera>().Follow = target.transform;
            this.Target = target;
        }

        // private void LateUpdate()
        // {
        //     if (Target == null)
        //         return;

        //     Vector3 targetPos = Target.transform.position;
        //     transform.position = new Vector3(targetPos.x, targetPos.y, -10f);
        // }
    }
}
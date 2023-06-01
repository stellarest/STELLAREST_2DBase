using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class MapTileController : MonoBehaviour // BaseController 상속받지 않아도 됨
    {
        // 무한맵 교체는 스프라이트만 불러와서 교체해도 됨
        private void OnTriggerExit2D(Collider2D other)
        {
            Camera camera = other.gameObject.GetComponent<Camera>();
            if (camera == null)
                return;

            Vector3 dir = camera.transform.position - transform.position;
            float dirX = dir.x < 0 ? -1 : 1; // -1 or 1
            float dirY = dir.y < 0 ? -1 : 1; // -1 or 1

            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                transform.Translate(Vector3.right * dirX * 200f);
            else
                transform.Translate(Vector3.up * dirY * 200f);
        }
    }
}

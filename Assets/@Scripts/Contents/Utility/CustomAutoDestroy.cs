using System.Collections;
using UnityEngine;

namespace STELLAREST_2D
{
    public class CustomAutoDestroy : MonoBehaviour
    {
        public void StartDestroy(float duration)
                => StartCoroutine(CoStartDestory(duration));

        private IEnumerator CoStartDestory(float duration)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                yield return null;
            }
            
            Managers.Resource.Destroy(gameObject);
        }
    }
}


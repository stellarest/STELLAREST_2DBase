using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    public GameObject BulletTest;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            UnityEngine.Object.Instantiate(BulletTest, transform.position, Quaternion.identity);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTest : MonoBehaviour
{
    public GameObject child; //  bullet image object
    public float speed = 5f;
    public Vector3 shootDir = Vector2.right; // bullet dir

    float angle = 0f;
    public float distance = 5f;
    public float rotationSpeed = 3000f;

    void Update()
    {
        angle += Time.deltaTime * rotationSpeed;
        if (angle > 360f)
            angle -= 360f;

        float radian = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(radian) * distance;
        float y = Mathf.Sin(radian) * distance;
        Vector2 bulletPosition = new Vector2(x, y);

        transform.position += speed * shootDir * Time.deltaTime;
        child.transform.localPosition = bulletPosition;
    }
}

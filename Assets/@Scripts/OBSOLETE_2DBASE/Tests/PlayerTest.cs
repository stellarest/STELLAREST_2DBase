using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;

using STELLAREST_2D;

public class PlayerTest : MonoBehaviour
{
    public int SearchCount = 0;
    //public float coneAngle = 45f;

    public float Dist = 10f;
    //public float DesiredSearchAngle = 45f;

    public float DesiredSearchAngle = 45f;

    public List<GameObject> Targets;
    public int Count = 0;

    // CHECK DENSED POINT
    public float detectionRadius = 5f;
    public LayerMask mask;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Vector3 mostDensePosition = Vector3.zero;
        int maxMonsterCount = 0;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, mask);
        foreach (var col in colliders)
        {
            int monsterCount = 0;
            foreach (var otherCol in colliders)
            {
                if (col == otherCol)
                    continue;
                
                float dist = Vector2.Distance(col.transform.position, otherCol.transform.position);
                if (dist <= detectionRadius)
                {
                    ++monsterCount;
                }
            }

            if (monsterCount > maxMonsterCount)
            {
                maxMonsterCount = monsterCount;
                mostDensePosition = col.transform.position;
            }
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(mostDensePosition, 1f);
    }

    // // Just Check Dot Product
    // public Transform DotTarget = null;
    // private void OnDrawGizmos()
    // {
    //     if (DotTarget != null)
    //     {
    //         Gizmos.color = Color.yellow;
    //         Gizmos.DrawLine(transform.position, transform.right * 10f);

    //         Vector2 toTargetDir = (DotTarget.position - transform.position).normalized;
    //         Gizmos.color = Color.white;
    //         Gizmos.DrawRay(transform.position, toTargetDir * 10f);

    //         float dot = Vector2.Dot(transform.right, toTargetDir);
    //         Debug.Log("DOT : " + dot);
    //         float angle = Mathf.Acos(dot) * 180f / Mathf.PI;
    //         Debug.Log("ANGLE : " + angle);
    //     }
    // }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // private void OnDrawGizmos()
    // {
    //     // Gizmos.color = Color.yellow;
    //     // Gizmos.DrawWireSphere(transform.position, 1f); // This is Player

    //     // Vector2 upVec = Quaternion.Euler(0, 0, 45f * 0.5f) * transform.right;
    //     // Vector2 downVec = Quaternion.Euler(0, 0, -45f * 0.5f) * transform.right;
    //     Vector2 upVec = Quaternion.Euler(0, 0, DesiredSearchAngle * 0.5f) * transform.right;
    //     Vector2 downVec = Quaternion.Euler(0, 0, -DesiredSearchAngle * 0.5f) * transform.right;

    //     Vector2 myPos = transform.position;
    //     Gizmos.DrawLine(myPos, myPos + (upVec * 10f));
    //     Gizmos.DrawLine(myPos, myPos + (downVec * 10f));
    //     Gizmos.color = Color.yellow;

    //     // upVec, downVec의 각도를 구한 것이고.
    //     float dotScale = Vector2.Dot(upVec, downVec);
    //     float rangeAngle = Mathf.Acos(dotScale) * Mathf.Rad2Deg; // 45f

    //     // Debug.Log("RANGE ANGLE : " + rangeAngle);
    //     // Debug.Log("Vec2Angle : " + Vector2.Angle(upVec, downVec)); // 45f 동일
    //     if (Targets != null && Targets.Count > 0)
    //     {
    //         foreach (var target in Targets)
    //         {
    //             Vector2 targetPos = target.transform.position;
    //             Vector2 toTargetDir = (targetPos - myPos).normalized;
    //             float dotProduct = Vector2.Dot(toTargetDir, transform.right); // ***
    //             float toTargetAngle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

    //             if ((targetPos - myPos).sqrMagnitude <= Dist * Dist)
    //             {
    //                 if (toTargetAngle * 2f <= rangeAngle)
    //                 {
    //                     Debug.Log("DOT : " + dotProduct);
    //                     Debug.Log("Found Obj : " + target.gameObject.name);
    //                     Gizmos.DrawRay(myPos, toTargetDir * Vector2.Distance(myPos, target.transform.position));
    //                 }
    //             }
    //         }
    //     }

        // if (Target != null)
        // {
        //     Vector2 targetPos = Target.transform.position;
        //     Vector2 toTargetDir = (targetPos - myPos).normalized;
        //     float toTargetDot = Vector2.Dot(toTargetDir, downVec);
        //     float toTargetAngle = Mathf.Acos(toTargetDot) * Mathf.Rad2Deg;
        //     Debug.Log("ORIGIN ANGLE : " + toTargetAngle);
        //     // Debug.Log("TARGET ANGLE : " + toTargetAngle * 2f);
        //     // if (toTargetAngle * 2f <= rangeAngle)
        //     // {
        //     //     Gizmos.color = Color.red;
        //     //     Gizmos.DrawRay(transform.position, toTargetDir * 10f);
        //     // }
        //     // else
        //     // {
        //     //     Gizmos.color = Color.white;
        //     //     Gizmos.DrawRay(transform.position, toTargetDir * 10f);
        //     // }
        // }
    // }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawWireSphere(transform.position, 1f); // This is Player

    //     if (target != null)
    //     {
    //         Vector2 toTargetDir = (target.transform.position - transform.position).normalized;
    //         Gizmos.color = Color.blue;
    //         Gizmos.DrawRay(transform.position, toTargetDir * 5f);

    //         Vector2 lineDown = Quaternion.Euler(0, 0, coneAngle / 2f) * transform.right;
    //         Vector2 lineUp = Quaternion.Euler(0, 0, coneAngle / 2f) * transform.right;
    //         Vector2 myPos = transform.position;

    //         Gizmos.color = Color.yellow;

    //         float angleToTarget = Vector2.Angle(transform.right, toTargetDir);
    //         if (angleToTarget <= coneAngle / 2f)
    //         {
    //             Gizmos.color = Color.magenta;
    //             Gizmos.DrawLine(myPos, myPos + lineDown * 5f);
    //             Gizmos.DrawLine(myPos, myPos + lineUp * 5f);
    //         }
    //         else
    //         {
    //             Gizmos.color = Color.yellow;
    //             Gizmos.DrawLine(myPos, myPos + lineDown * 5f);
    //             Gizmos.DrawLine(myPos, myPos + lineUp * 5f);
    //         }
    //     }
    // }
}

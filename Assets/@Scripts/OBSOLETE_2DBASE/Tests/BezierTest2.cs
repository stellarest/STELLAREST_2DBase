using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class BezierTest2 : MonoBehaviour
    {
        public LineRenderer line;
        
        [Min(2)]
        public int LineCount = 40;

        public Vector3 P1Pos = new Vector3(0, 3, 0);
        public Vector3 P2Pos = new Vector3(0, 3, 0);

        public GameObject GO_P0_Player;
        public GameObject GO_P1;
        public GameObject GO_P2;
        public GameObject GO_P3_Monster;

        private void Update()
        {
            DrawLine();
        }

        // 3차 베지에 곡선
        public Vector3 SetBezier(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3, float t)
        {
            Vector3 M0 = Vector3.Lerp(P0, P1, t);
            Vector3 M1 = Vector3.Lerp(P1, P2, t);
            Vector3 M2 = Vector3.Lerp(P2, P3, t);

            Vector3 B0 = Vector3.Lerp(M0, M1, t);
            Vector3 B1 = Vector3.Lerp(M1, M2, t);

            return Vector3.Lerp(B0, B1, t);
        }


        public void DrawLine()
        {
            line.positionCount = LineCount;

            GO_P1.transform.position = GO_P0_Player.transform.position + P1Pos;
            GO_P2.transform.position = GO_P3_Monster.transform.position + P2Pos;

            for (int i = 0; i < LineCount; ++i)
            {
                float t = 0f;
                if (i == 0)
                    t = 0;
                else
                    t = (float)i / (LineCount - 1);

                Vector3 bezier = SetBezier(GO_P0_Player.transform.position,
                    GO_P1.transform.position, GO_P2.transform.position, GO_P3_Monster.transform.position, t);

                line.SetPosition(i, bezier);
            }
        }
    }
}

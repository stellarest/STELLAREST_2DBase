using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace STELLAREST_2D
{
    public class Bezier : MonoBehaviour
    {
        public LineRenderer line;
        [Min(2)]
        public int LineCount = 20;

        public Vector3 P1Pos = new Vector3(0, 5, 0);
        public Vector3 P2Pos = new Vector3(0, 5, 0);

        public GameObject GO_P0;
        public GameObject GO_P1;
        public GameObject GO_P2;
        public GameObject GO_P3;
        public GameObject Player;

        private void Start()
        {
            Player.transform.position = GO_P0.transform.position;
            StartCoroutine(CoMovingCurve());
        }

        private IEnumerator CoMovingCurve()
        {
            float delta = 0f;
            float percent = 0f;
            bool movingForward = true;

            while (true)
            {
                delta += Time.deltaTime;
                if (movingForward)
                    percent = delta / 2f;
                else
                    percent = 1f - (delta / 2f);

                if (percent <= 1f && percent >= 0f)
                {
                    Player.transform.position = SetBezier(GO_P0.transform.position,
                        GO_P1.transform.position, GO_P2.transform.position, GO_P3.transform.position, percent);
                }
                else
                {
                    movingForward = !movingForward;
                    delta = 0f;
                }

                yield return null;
            }
        }

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

            GO_P1.transform.position = GO_P0.transform.position + P1Pos;
            GO_P2.transform.position = GO_P3.transform.position + P2Pos;

            for (int i = 0; i < LineCount; ++i)
            {
                float t = 0f;
                if (i == 0)
                    t= 0;
                else
                    t = (float)i / (LineCount - 1);

                Vector3 bezier = SetBezier(GO_P0.transform.position,
                    GO_P1.transform.position, GO_P2.transform.position, GO_P3.transform.position, t);

                line.SetPosition(i, bezier);
            }
        }
    }
}

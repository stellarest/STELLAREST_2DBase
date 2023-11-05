using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class StageManager
    {
        public Vector3 LeftBottom { get; set; }
        public Vector3 RightTop { get; set; }

        public void Init()
        {
            GameObject go = UnityEngine.GameObject.Find("Map_01_Forest");
            LeftBottom = Utils.FindChild(go, "LeftBottom").transform.position;
            RightTop = Utils.FindChild(go, "RightTop").transform.position;

            // TEMP
            go.SetActive(false);
        }

        public Vector2Int MinimumPosition
                => new Vector2Int(Mathf.CeilToInt(LeftBottom.x), Mathf.CeilToInt(LeftBottom.y));

        public Vector2Int MaximumPosition
                => new Vector2Int(Mathf.FloorToInt(RightTop.x), Mathf.FloorToInt(RightTop.y));

        public void SetInLimitPos(CreatureController cc) => SetInLimitPos(cc.transform);
        public void SetInLimitPos(Transform transform)
        {
            // Min
            if (transform.position.x <= LeftBottom.x)
                transform.position = new Vector2(LeftBottom.x, transform.position.y);
            if (transform.position.y <= LeftBottom.y)
                transform.position = new Vector2(transform.position.x, LeftBottom.y);

            // Max
            if (transform.position.x >= RightTop.x)
                transform.position = new Vector2(RightTop.x, transform.position.y);
            if (transform.position.y >= RightTop.y)
                transform.position = new Vector2(transform.position.x, RightTop.y);
        }

        public bool IsOutOfPos(Vector3 position)
        {
            if (position.x < LeftBottom.x)
                return true;
            if (position.y < LeftBottom.y)
                return true;

            if (position.x > RightTop.x)
                return true;
            if (position.y > RightTop.y)
                return true;

            return false;
        }

        public bool IsInLimitPos(Transform transform)
            => IsInLimitPosX(transform) || IsInLimitPosY(transform);

        private bool IsInLimitPosX(Transform transform)
                => Mathf.Abs(transform.position.x - LeftBottom.x) < Mathf.Epsilon || Mathf.Abs(transform.position.x - RightTop.x) < Mathf.Epsilon;

        private bool IsInLimitPosY(Transform transform)
                => Mathf.Abs(transform.position.y - LeftBottom.y) < Mathf.Epsilon || Mathf.Abs(transform.position.y - RightTop.y) < Mathf.Epsilon;
    }
}

using UnityEngine;

namespace STELLAREST_2D
{
    public class BaseController : MonoBehaviour
    {
        public Define.ObjectType ObjectType { get; protected set; }

        private void Awake()
        {
            Init();
        }

        // 자식 Start에서 작성하면, 부모 Start가 씹힘
        // 그래서 부모에서 가상함수로 따로 파주고 자식에서 오버라이드해서 각자 Init 할 수 있게 해주면 좋음
        private bool _init = false;
        public virtual bool Init()
        {
            if (_init)
                return false;

            _init = true;
            return true;
        }
    }
}

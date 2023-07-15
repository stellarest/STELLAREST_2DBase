using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class Chicken : MonsterController
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Utils.InitLog(this.GetType());
            MonsterState = Define.MonsterState.Idle;

            return true;
        }
    }
}

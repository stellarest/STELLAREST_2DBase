using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_SFH
{
    public class UI_Popup : UI_Base
    {
        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Managers.UI.SetCanvas(gameObject, true);
            return true;
        }

        public virtual void ClosePopupUI() 
            => Managers.UI.ClosePopupUI(this);
    }
}


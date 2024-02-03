using System.Collections;
using System.Collections.Generic;
using STELLAREST_2D.Data;
using UnityEngine;

using FaceType = STELLAREST_2D.Define.FaceType;

namespace STELLAREST_2D
{
    /*
        +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        +++ Check Eye Object in Hierarchy (HEAD -> EYES) IN ADVANCE +++
        +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    */
    public class MonsterRendererController : CreatureRendererController
    {
        [SerializeField] private Sprite _defaultEyes = null;
        [SerializeField] private Sprite _combatEyes = null;
        [SerializeField] private Sprite _deadEyes = null;

        public override void InitRendererController(BaseController owner, CreatureData initialCreatureData)
        {
            if (this.Owner != null)
                return;

            base.InitRendererController(owner, initialCreatureData);
            InitMonsterFace();
        }

        private void InitMonsterFace()
        {
            if (this._defaultEyes == null || this._combatEyes == null || this._deadEyes == null)
            {
                Utils.LogCritical(nameof(MonsterRendererController), nameof(InitMonsterFace), "Monster have to Set Eyes per Face Types in advance.");
                return;
            }

            FaceContainer faceContainer = new FaceContainer();
            faceContainer.FaceType = FaceType.Default;
            faceContainer.Eyes = _defaultEyes;
            faceContainer.EyesColor = Color.white;
            FaceContainerDict.Add(FaceType.Default, faceContainer);

            faceContainer = new FaceContainer();
            faceContainer.FaceType = FaceType.Combat;
            faceContainer.Eyes = _combatEyes;
            faceContainer.EyesColor = Color.white;
            FaceContainerDict.Add(FaceType.Combat, faceContainer);

            faceContainer = new FaceContainer();
            faceContainer.FaceType = FaceType.Dead;
            faceContainer.Eyes = _deadEyes;
            faceContainer.EyesColor = Color.white;
            FaceContainerDict.Add(FaceType.Dead, faceContainer);
        }
    }
}

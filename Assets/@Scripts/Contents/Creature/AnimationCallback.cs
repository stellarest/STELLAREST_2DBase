using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

namespace STELLAREST_2D
{
    public class AnimationCallback : MonoBehaviour
    {
        private CreatureController _owner = null;
        public System.Action OnCloneRepeatSkill = null;

        public void Init(CreatureController owner) => this._owner = owner;
        public void OnCloneRepeatSkillHandler() => OnCloneRepeatSkill?.Invoke();
        public void OnFaceBattleHandler() => this._owner.RendererController.OnFaceBattleHandler();
        public void OnFaceDefaultHandler() => this._owner.RendererController.OnFaceDefaultHandler();
        public void OnFaceDeadHandler() => this._owner.RendererController.OnFaceDeadHandler();
        public void OnDustVFXHandler() => this._owner.RendererController.OnDustVFXHandler();
    }
}

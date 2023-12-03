using UnityEngine;

namespace STELLAREST_2D
{
    public class AnimationCallback : MonoBehaviour
    {
        private CreatureController _owner = null;
        public event System.Action OnActiveMasteryAction = null;
        public event System.Action OnActiveEliteAction = null;

        public void Init(CreatureController owner) => this._owner = owner;
        public void OnActiveMasteryActionHandler() => OnActiveMasteryAction?.Invoke();
        public void OnActiveEliteActionHandler() => OnActiveEliteAction?.Invoke();
        public void OnActiveUltimateActionHandler() { }
        public void OnFaceDefaultHandler() => this._owner.RendererController.OnFaceDefaultHandler();
        public void OnFaceCombatHandler() => this._owner.RendererController.OnFaceCombatHandler();
        public void OnFaceDeadHandler() => this._owner.RendererController.OnFaceDeadHandler();
        public void OnDustVFXHandler() => this._owner.RendererController.OnDustVFXHandler();
    }
}

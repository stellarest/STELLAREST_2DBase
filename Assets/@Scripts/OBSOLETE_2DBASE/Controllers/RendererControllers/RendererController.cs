using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SpriteTrail;
using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    public class BaseRenderer
    {
        public BaseRenderer(string tag, Material matOrigin, Color colorOrigin)
        {
            this.Tag = tag;
            this.MatOrigin = matOrigin;
            this.ColorOrigin = colorOrigin;
        }

        public string Tag { get; } = null;
        public Material MatOrigin { get; } = null;
        public Color ColorOrigin { get; } = Color.white;
    }

    public class RendererContainer
    {
        private SpriteRenderer[] _spriteRenderers = null;
        public SpriteRenderer[] SpriteRenderers
        {
            get => _spriteRenderers;
            set
            {
                if (_spriteRenderers != null && _spriteRenderers.Length > 0)
                    return;

                _spriteRenderers = new SpriteRenderer[value.Length];
                for (int i = 0; i < value.Length; ++i)
                    _spriteRenderers[i] = value[i];
            }
        }

        private BaseRenderer[] _baseRenderers = null;
        public BaseRenderer[] BaseContainers
        {
            get => _baseRenderers;
            set
            {
                if (_baseRenderers != null && _baseRenderers.Length > 0)
                    return;

                _baseRenderers = value;
            }
        }
    }

    public class RendererModerator
    {
        public Dictionary<Define.InGameGrade, RendererContainer> RendererContainerDict { get; }
                    = new Dictionary<Define.InGameGrade, RendererContainer>();

        public void AddRendererContainers(Define.InGameGrade grade, BaseRenderer[] baseRenderers, SpriteRenderer[] SPRs)
        {
            RendererContainer rendererContainer = new RendererContainer();
            rendererContainer.SpriteRenderers = SPRs;
            rendererContainer.BaseContainers = baseRenderers;
            RendererContainerDict.Add(grade, rendererContainer);
        }

        public SpriteRenderer[] GetSpriteRenderers(Define.InGameGrade grade)
            => RendererContainerDict.TryGetValue(grade, out RendererContainer value) ? value.SpriteRenderers : null;
        public BaseRenderer[] GetBaseRenderers(Define.InGameGrade grade)
            => RendererContainerDict.TryGetValue(grade, out RendererContainer value) ? value.BaseContainers : null;
    }
    
    public class RendererController : BaseController
    {
        public System.Action<Define.InGameGrade> OnRefreshRenderer = null;

        public BaseController Owner { get; protected set; } = null;
        public CreatureController OwnerAsCreature { get; protected set; } = null;
        [field: SerializeField] public Define.InGameGrade KeyGrade { get; protected set; } = Define.InGameGrade.Default;

        public Dictionary<BaseController, RendererModerator> RendererModeratorDict { get; } = new Dictionary<BaseController, RendererModerator>();
        public SpriteRenderer[] OwnerSPRs { get; protected set; } = null;
        public bool IsChangingMaterial { get; protected set; } = false;

        public SpriteTrail.SpriteTrail[] Trails { get; protected set; } = null;

        public void EnableSpriteTrails(bool isOnEnable)
        {
            if (Trails.Length == 0)
                return;

            for (int i = 0; i < Trails.Length; ++i)
                Trails[i].enabled = isOnEnable;
        }

        private TrailPreset currentTrail = null;
        public void ChangeSpriteTrails(TrailPreset trailPreset)
        {
            if (Trails.Length == 0)
                return;

            if (currentTrail == trailPreset)
                return;

            for (int i = 0; i < Trails.Length; ++i)
                Trails[i].SetTrailPreset(trailPreset);
            
            currentTrail = trailPreset;
        }

        public void InitRendererController(BaseController owner) => this.Owner = owner;
        public virtual void InitRendererController(BaseController owner, CreatureData initialCreatureData)
        {
            this.InitRendererController(owner);
            this.OwnerAsCreature = owner.GetComponent<CreatureController>();
            this.OwnerSPRs = OwnerAsCreature.AnimTransform.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);

            for (int i = 0; i < this.OwnerSPRs.Length; ++i)
            {
                SpriteTrail.SpriteTrail SPT = this.OwnerSPRs[i].gameObject.GetOrAddComponent<SpriteTrail.SpriteTrail>();
                SPT.SetTrailPreset(Managers.VFX.SO_SPT_BLOB);
                SPT.m_OrderInSortingLayer = this.OwnerSPRs[i].sortingOrder;
            }

            currentTrail = Managers.VFX.SO_SPT_BLOB;
            Trails = this.Owner.GetComponentsInChildren<SpriteTrail.SpriteTrail>(includeInactive: true);
            EnableSpriteTrails(false);
        }

        public SpriteRenderer[] SpriteRenderers(Define.InGameGrade grade)
            => RendererModeratorDict.TryGetValue(this.Owner, out RendererModerator value) ? value.GetSpriteRenderers(grade) : null;
        public BaseRenderer[] BaseRenderers(Define.InGameGrade grade)
            => RendererModeratorDict.TryGetValue(this.Owner, out RendererModerator value) ? value.GetBaseRenderers(grade) : null;

        public virtual void EnterInGame() => ResetMaterial();
        protected virtual void OnRefreshRendererHandler(Define.InGameGrade keyGrade) { }
        
        public virtual void OnFaceDefaultHandler() { }
        public virtual void OnFaceCombatHandler() { }
        public virtual void OnFaceDeadHandler() { }
        public virtual void OnDustVFXHandler() { }

        public void SetMaterial(Material mat)
        {
            IsChangingMaterial = true;
            for (int i = 0; i < OwnerSPRs.Length; ++i)
            {
                if (OwnerSPRs[i].sprite != null)
                    OwnerSPRs[i].material = mat;
            }
        }

        public void ResetMaterial()
        {
            BaseRenderer[] BCs = this.BaseRenderers(KeyGrade);
            int legnth = Mathf.Min(BCs.Length, OwnerSPRs.Length);
            for (int i = 0; i < legnth; ++i)
                OwnerSPRs[i].material = BCs[i].MatOrigin;

            IsChangingMaterial = false;
        }

        private void OnDestroy()
        {
            if (OnRefreshRenderer != null)
            {
                OnRefreshRenderer -= OnRefreshRendererHandler;
                Utils.Log("ReleaseEvent : OnUpdatePlayerFaceExpressionContainerHandler");
            }

            ResetMaterial();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using STELLAREST_2D.Data;
using UnityEngine;

using VFXEnv = STELLAREST_2D.Define.TemplateIDs.VFX.Environment;
using CrowdControl = STELLAREST_2D.Define.TemplateIDs.CrowdControl;
using MaterialType = STELLAREST_2D.Define.MaterialType;
using MaterialColor = STELLAREST_2D.Define.MaterialColor;
using UnityEditor;
using UnityEditor.MemoryProfiler;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;

namespace STELLAREST_2D
{
    public class BaseContainer
    {
        public BaseContainer(string tag, Material matOrigin, Color colorOrigin)
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

        private BaseContainer[] _baseContainers = null;
        public BaseContainer[] BaseContainers
        {
            get => _baseContainers;
            set
            {
                if (_baseContainers != null && _baseContainers.Length > 0)
                    return;

                _baseContainers = value;
            }
        }
    }

    public class RendererModerator
    {
        public Dictionary<Define.InGameGrade, RendererContainer> RendererContainerDict { get; }
                    = new Dictionary<Define.InGameGrade, RendererContainer>();

        public void AddRendererContainers(Define.InGameGrade grade, BaseContainer[] baseContainers, SpriteRenderer[] SPRs)
        {
            RendererContainer rendererContainer = new RendererContainer();
            rendererContainer.SpriteRenderers = SPRs;
            rendererContainer.BaseContainers = baseContainers;
            RendererContainerDict.Add(grade, rendererContainer);
        }

        public SpriteRenderer[] GetSpriteRenderers(Define.InGameGrade grade)
            => RendererContainerDict.TryGetValue(grade, out RendererContainer value) ? value.SpriteRenderers : null;
        public BaseContainer[] GetBaseContainers(Define.InGameGrade grade)
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

        public void InitRendererController(BaseController owner)
        {
            this.Owner = owner;
        }

        public virtual void InitRendererController(BaseController owner, InitialCreatureData initialCreatureData)
        {
            this.InitRendererController(owner);
            this.OwnerAsCreature = owner.GetComponent<CreatureController>();
            this.OwnerSPRs = OwnerAsCreature.AnimTransform.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
        }

        public SpriteRenderer[] SpriteRenderers(Define.InGameGrade grade)
            => RendererModeratorDict.TryGetValue(this.Owner, out RendererModerator value) ? value.GetSpriteRenderers(grade) : null;
        public BaseContainer[] BaseContainers(Define.InGameGrade grade)
            => RendererModeratorDict.TryGetValue(this.Owner, out RendererModerator value) ? value.GetBaseContainers(grade) : null;

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
            BaseContainer[] BCs = this.BaseContainers(KeyGrade);
            int legnth = Mathf.Min(BCs.Length, OwnerSPRs.Length);
            for (int i = 0; i < legnth; ++i)
                OwnerSPRs[i].material = BCs[i].MatOrigin;

            //ResetEyes();
            IsChangingMaterial = false;
        }

        // private SpriteRenderer _playerEyesSPR = null;
        // public SpriteRenderer PlayerEyesSPR
        // {
        //     get
        //     {
        //         if (this.IsPlayer == false)
        //         {
        //             Utils.LogStrong(nameof(RendererController), nameof(PlayerEyesSprite), $"Monster tries to get acccess \"PlayerEyesSprite\"");
        //             return null;
        //         }

        //         return _playerEyesSPR;
        //     }

        //     private set => _playerEyesSPR = value;
        // }

        // +++ LOADER PLAYER FACE EXPRESSIONS +++
        // public Dictionary<Define.InGameGrade, PlayerFaceExpression[]> PlayerFaceExpressionsDict { get; private set; }
        //                                         = new Dictionary<Define.InGameGrade, PlayerFaceExpression[]>();

        // +++++ CACHE PLAYER FACE EXPRESSINOS +++++
        // public Dictionary<Define.FaceExpressionType, PlayerFaceExpressionContainer> PlayerFaceExpressionContainerDict { get; private set; }
        //                                         = new Dictionary<Define.FaceExpressionType, PlayerFaceExpressionContainer>();

        // public virtual void InitRendererController(CreatureController owner, InitialCreatureData initialCreatureData) 
        // {
        //     if (DictModerator != null)
        //     {
        //         Utils.LogStrong(nameof(RendererController), nameof(InitRendererController), $"Already Completed Init : {DictModerator != null}");
        //         return;
        //     }

        //     DictModerator = new Dictionary<BaseController, Moderator>();
        // }

        // public virtual void InitRendererController(CreatureController owner, InitialCreatureData initialCreatureData)
        // {
        //     // if (this.Owner != null || this._moderatorDict != null)
        //     //     return;

        //     // // INIT CURRENT
        //     // this.Owner = owner;
        //     // //OwnerSPRs = owner.SPRs;
        //     // // 이건 오너 그 자체
        //     // OwnerSPRs = owner.AnimTransform.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);

        //     // if (this.IsPlayer)
        //     // {
        //     //     InitPlayerEyes();
        //     //     InitPlayerFace();
        //     //     InitPlayerFaceExpression(initialCreatureData);

        //     //     OnUpdatePlayerFaceExpressionContainerHandler(_currentKeyGrade); // FOR CACHING METHOD
        //     //     OnUpgrade += OnUpdatePlayerFaceExpressionContainerHandler;
        //     // }
        //     // else
        //     //     InitMonsterHead();

        //     // _moderatorDict = new Dictionary<CreatureController, Moderator>();
        //     // Moderator moderator = new Moderator();

        //     // GameObject go = Managers.Resource.Load<GameObject>(initialCreatureData.PrimaryLabel);
        //     // SpriteRenderer[] SPRs = go.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
        //     // BaseContainer[] BCs = new BaseContainer[SPRs.Length];
        //     // for (int i = 0; i < SPRs.Length; ++i)
        //     // {
        //     //     Material matOrigin = new Material(SPRs[i].sharedMaterial);
        //     //     Color colorOrigin = SPRs[i].color;
        //     //     BCs[i] = new BaseContainer(SPRs[i].name, matOrigin, colorOrigin);
        //     // }
        //     // moderator.AddRendererContainers(Define.InGameGrade.Default, BCs, SPRs);

        //     // // INIT RESTS OF THE NEXT
        //     // List<SkillBase> skillList = new List<SkillBase>();
        //     // foreach (KeyValuePair<int, SkillGroup> pair in owner.SkillBook.SkillGroupsDict)
        //     // {
        //     //     for (int i = 0; i < pair.Value.MemberCount; ++i)
        //     //         skillList.Add(pair.Value.Members[i].SkillOrigin);
        //     // }

        //     // SkillBase[] modelingLabelSkills = skillList.Where(s => s.Data.ModelingLabel.Length > 0).ToArray();
        //     // if (modelingLabelSkills.Length > 0)
        //     // {
        //     //     for (int i = 0; i < modelingLabelSkills.Length; ++i)
        //     //     {
        //     //         Define.InGameGrade keyGrade = modelingLabelSkills[i].Data.Grade;
        //     //         string modelingLabel = modelingLabelSkills[i].Data.ModelingLabel;
        //     //         go = Managers.Resource.Load<GameObject>(modelingLabel);
        //     //         if (go == null)
        //     //             Utils.LogCritical(nameof(RendererContainer), nameof(InitRendererController), $"Check Modeling Label : {modelingLabel}");

        //     //         SPRs = go.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
        //     //         BCs = new BaseContainer[SPRs.Length];
        //     //         for (int j = 0; j < SPRs.Length; ++j)
        //     //         {
        //     //             Material matOrigin = new Material(SPRs[j].sharedMaterial);
        //     //             Color colorOrigin = SPRs[j].color;
        //     //             BCs[j] = new BaseContainer(SPRs[j].name, matOrigin, colorOrigin);
        //     //         }

        //     //         moderator.AddRendererContainers(keyGrade, BCs, SPRs);
        //     //     }
        //     // }

        //     // _moderatorDict.Add(owner, moderator);
        // }

        //public void OnDustVFXHandler() => Managers.VFX.Environment(VFXEnv.Dust, this.Owner);

        //public PlayerFace PlayerFace { get; private set; } = null;
        // public SpriteRenderer MonsterHead
        // {
        //     get
        //     {
        //         if (this.IsPlayer)
        //         {
        //             Utils.LogStrong(nameof(RendererController), nameof(MonsterHead), $"Player tries to get acccess \"MonsterHead\"");
        //             return null;
        //         }

        //         return _monsterHead;
        //     }

        //     private set => _monsterHead = value;
        // }

        // private void InitPlayerEyes()
        // {
        //     for (int i = 0; i < OwnerSPRs.Length; ++i)
        //     {
        //         if (OwnerSPRs[i].name.Contains("Eyes"))
        //         {
        //             this.PlayerEyesSPR = OwnerSPRs[i];
        //             this.PlayerEyesSprite = OwnerSPRs[i].sprite;
        //         }
        //     }
        // }

        // private void Hologram(Material hologramMat, float resetDelay)
        // {
        //     if (this.IsChangingMaterial == false)
        //     {
        //         this.IsChangingMaterial = true;
        //         ChangeMaterial(hologramMat);
        //         StartCoroutine(CoHologram(resetDelay));
        //     }
        // }

        private IEnumerator CoHologram(float resetDelay)
        {
            float percent = 0f;
            while (percent < 1f)
            {
                Managers.VFX.Mat_Hologram.SetFloat(Managers.VFX.SHADER_HOLOGRAM, percent);
                percent += Time.deltaTime * 20f;
                yield return null;
            }

            //StartCoroutine(CoResetMaterial(resetDelay));

            percent = 1f;
            float elapsedTime = 0f;
            while (percent > 0f)
            {
                Managers.VFX.Mat_Hologram.SetFloat(Managers.VFX.SHADER_HOLOGRAM, percent);
                elapsedTime += Time.deltaTime;
                percent = 1f - (elapsedTime * 20f);
                yield return null;
            }
            // StartCoroutine(CoReset(resetDelay));
        }

        // private void FadeOut(Material fadeMat, float resetDelay)
        // {
        //     ChangeMaterial(fadeMat); // *** PLAYER EYES SPRITE의 경우, FADE 전용 EYES로 교체 필요.
        //     StartCoroutine(CoFade(resetDelay));
        // }

        private IEnumerator CoFade(float resetDelay)
        {
            Managers.VFX.Mat_Fade.SetFloat(Managers.VFX.SHADER_FADE, 1f);
            float delta = 0f;
            float percent = 1f;
            while (percent > 0f)
            {
                delta += Time.deltaTime;
                percent = 1f - (delta / Managers.VFX.DESIRED_TIME_FADE_OUT);
                Managers.VFX.Mat_Fade.SetFloat(Managers.VFX.SHADER_FADE, percent);
                yield return null;
            }
        }

        public void Upgrade(Define.InGameGrade keyGrade)
        {
            for (int i = 0; i < OwnerSPRs.Length; ++i)
                OwnerSPRs[i].sprite = null;

            // _currentKeyGrade = keyGrade;
            // SpriteRenderer[] nextSPRs = SpriteRenderers(keyGrade);
            // int length = Mathf.Max(OwnerSPRs.Length, nextSPRs.Length);
            // for (int i = 0; i < length; ++i)
            // {
            //     if ((i < OwnerSPRs.Length) && (i < nextSPRs.Length))
            //     {
            //         if (nextSPRs[i].name.Contains(OwnerSPRs[i].name))
            //         {
            //             // nextSPRs[i].sprite가 갖고 있는것만 집어 넣어 넣으면 해결
            //             if (nextSPRs[i].sprite != null)
            //             {
            //                 OwnerSPRs[i].gameObject.SetActive(nextSPRs[i].gameObject.activeSelf);
            //                 OwnerSPRs[i].gameObject.GetComponent<SpriteRenderer>().enabled = nextSPRs[i].gameObject.activeSelf; // 추가
            //                 OwnerSPRs[i].sprite = nextSPRs[i].sprite;
            //                 OwnerSPRs[i].color = nextSPRs[i].color;
            //             }
            //         }
            //     }
            // }

            OnRefreshRenderer?.Invoke(keyGrade);
        }

        private void ChangeMaterial(Material mat)
        {
            // +++++ 개선 필요 +++++
            // if (this.IsPlayer && CurrentFaceState != Define.FaceExpressionType.Dead)
            //     PlayerEyesSPR.sprite = null;

            // for (int i = 0; i < OwnerSPRs.Length; ++i)
            // {
            //     if (OwnerSPRs[i].sprite != null)
            //         OwnerSPRs[i].material = mat;
            // }
        }

#if UNITY_EDITOR
        // public bool IsPlayerDeadEyes()
        // {
        //     PlayerFaceExpressionContainer container = PlayerFaceExpressionContainerDict[Define.FaceExpressionType.Dead];
        //     return container.Eyes.name.Contains("Eyes_Dead");
        // }
#endif

        private void OnDestroy()
        {
            // TODO : RESET
            if (OnRefreshRenderer != null)
            {
                OnRefreshRenderer -= OnRefreshRendererHandler;
                Utils.Log("ReleaseEvent : OnUpdatePlayerFaceExpressionContainerHandler");
            }
        }
    }
}

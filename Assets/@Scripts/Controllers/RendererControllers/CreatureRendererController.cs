using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DamageNumbersPro;
using STELLAREST_2D.Data;
using UnityEngine;
using UnityEngine.TextCore.Text;
using CrowdControl = STELLAREST_2D.Define.TemplateIDs.CrowdControl;
using FaceType = STELLAREST_2D.Define.FaceType;
using VFXEnv = STELLAREST_2D.Define.TemplateIDs.VFX.Environment;

namespace STELLAREST_2D
{
    [System.Serializable]
    public class FaceContainerLoader
    {
        public Define.InGameGrade MasteryGrade;
        public FaceContainerKeyLoader[] FaceContainerKeyLoaders;
    }

    [System.Serializable]
    public class FaceContainerKeyLoader
    {
        public string FaceTag;
        public FaceType FaceType;

        public string EyebrowsKey;
        public string EyebrowsColor;

        public string EyesKey;
        public string EyesColor;

        public string MouthKey;
        public string MouthColor;
    }

    public class FaceContainer
    {
        public FaceType FaceType;

        public Sprite Eyebrows { get; set; } = null;
        public Color EyebrowsColor { get; set; } = Color.white;

        public Sprite Eyes { get; set; } = null;
        public Color EyesColor { get; set; } = Color.white;

        public Sprite Mouth { get; set; } = null;
        public Color MouthColor { get; set; } = Color.white;
    }

    public class FaceReference
    {
        public Define.ObjectType CreatureType { get; set; } = Define.ObjectType.None;
        public SpriteRenderer EyebrowsSPR { get; set; } = null;
        public SpriteRenderer EyesSPR { get; set; } = null;
        public SpriteRenderer MouthSPR { get; set; } = null;
    }

    public class CreatureRendererController : RendererController
    {
        public Dictionary<Define.InGameGrade, FaceContainer[]> InitialLoadedFaceContainersDict { get; } = new Dictionary<Define.InGameGrade, FaceContainer[]>();
        public Dictionary<FaceType, FaceContainer> FaceContainerDict { get; } = new Dictionary<FaceType, FaceContainer>();
        public FaceReference FaceRef { get; } = new FaceReference();

        [SerializeField] private FaceType _faceType = FaceType.Default;
        public FaceType FaceType
        {
            get => _faceType;
            private set
            {
                if (_faceType == value)
                    return;

                switch (value)
                {
                    case FaceType.Default:
                        {
                            if (this.OwnerAsCreature[CrowdControl.Stun])
                                return;

                            if (this.OwnerAsCreature.IsDeadState)
                                return;

                            if (FaceContainerDict.TryGetValue(FaceType.Default, out FaceContainer faceContainer) == false)
                            {
                                Utils.LogCritical(nameof(CreatureRendererController), nameof(FaceType),
                                    $"Faield to load faceContainer in \"FaceType(Property)\": {FaceType.Default}");
                                return;
                            }

                            SetFace(faceContainer);
                            _faceType = value;
                        }
                        break;

                    case FaceType.Combat:
                        {
                            if (this.OwnerAsCreature.IsDeadState)
                                return;

                            if (FaceContainerDict.TryGetValue(FaceType.Combat, out FaceContainer faceContainer) == false)
                            {
                                Utils.LogCritical(nameof(CreatureRendererController), nameof(FaceType),
                                    $"Faield to load faceContainer in \"FaceType(Property)\": {FaceType.Combat}");
                                return;
                            }

                            SetFace(faceContainer);
                            _faceType = value;
                        }
                        break;

                    case FaceType.Dead:
                        {
                            if (FaceContainerDict.TryGetValue(FaceType.Dead, out FaceContainer faceContainer) == false)
                            {
                                Utils.LogCritical(nameof(CreatureRendererController), nameof(FaceType),
                                    $"Faield to load faceContainer in \"FaceType(Property)\": {FaceType.Dead}");
                                return;
                            }

                            SetFace(faceContainer);
                            _faceType = value;
                        }
                        break;
                }
            }
        }

        private void SetFace(FaceContainer faceContainer)
        {
            if (faceContainer.Eyebrows != null)
            {
                FaceRef.EyebrowsSPR.sprite = faceContainer.Eyebrows;
                FaceRef.EyebrowsSPR.color = faceContainer.EyebrowsColor;
            }

            if (faceContainer.Eyes != null)
            {
                FaceRef.EyesSPR.sprite = faceContainer.Eyes;
                FaceRef.EyesSPR.color = faceContainer.EyesColor;
            }

            if (faceContainer.Mouth != null)
            {
                FaceRef.MouthSPR.sprite = faceContainer.Mouth;
                FaceRef.MouthSPR.color = faceContainer.MouthColor;
            }
        }

        public override void InitRendererController(BaseController owner, InitialCreatureData initialCreatureData)
        {
            if (this.Owner != null)
                return;

            base.InitRendererController(owner, initialCreatureData);
            InitModels(initialCreatureData);
            InitFace(initialCreatureData);
        }

        private void InitModels(InitialCreatureData initialCreatureData)
        {
            RendererModerator rendererModerator = new RendererModerator();

            // +++ NOT INSTANTIATED OBJECT +++
            GameObject go = Managers.Resource.Load<GameObject>(initialCreatureData.PrimaryLabel);
            SpriteRenderer[] SPRs = go.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
            BaseContainer[] BCs = new BaseContainer[SPRs.Length];
            for (int i = 0; i < SPRs.Length; ++i)
            {
                Material matCloned = new Material(SPRs[i].sharedMaterial);
                //Material matCloned = new Material(SPRs[i].material);

                Color colorOrigin = new Color(SPRs[i].color.r, SPRs[i].color.g, SPRs[i].color.g, SPRs[i].color.a);
                BCs[i] = new BaseContainer(SPRs[i].name, matCloned, colorOrigin);
            }
            rendererModerator.AddRendererContainers(Define.InGameGrade.Default, BCs, SPRs);

            // INIT REST OF THE NEXT GRADE MODELS
            List<SkillBase> skills = new List<SkillBase>();
            foreach (KeyValuePair<int, SkillGroup> pair in this.OwnerAsCreature.SkillBook.SkillGroupsDict)
            {
                for (int i = 0; i < pair.Value.MemberCount; ++i)
                    skills.Add(pair.Value.Members[i].SkillOrigin);
            }

            SkillBase[] modelingLabelSkills = skills.Where(s => s.Data.ModelingLabel.Length > 0).ToArray();
            if (modelingLabelSkills.Length > 0)
            {
                for (int i = 0; i < modelingLabelSkills.Length; ++i)
                {
                    Define.InGameGrade keyGrade = modelingLabelSkills[i].Data.Grade;
                    string modelingLabel = modelingLabelSkills[i].Data.ModelingLabel;
                    go = Managers.Resource.Load<GameObject>(modelingLabel);
                    if (go == null)
                    {
                        Utils.LogCritical(nameof(CreatureRendererController), nameof(InitModels), $"Faield to load model : {modelingLabel}");
                        return;
                    }

                    SPRs = go.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
                    BCs = new BaseContainer[SPRs.Length];
                    for (int j = 0; j < SPRs.Length; ++j)
                    {
                        Material matOrigin = new Material(SPRs[j].sharedMaterial);
                        Color colorOrigin = new Color(SPRs[j].color.r, SPRs[j].color.g, SPRs[j].color.g, SPRs[j].color.a);
                        BCs[j] = new BaseContainer(SPRs[j].name, matOrigin, colorOrigin);
                    }

                    rendererModerator.AddRendererContainers(keyGrade, BCs, SPRs);
                }
            }

            RendererModeratorDict.Add(this.OwnerAsCreature, rendererModerator);
        }

        private void InitFace(InitialCreatureData initialCreatureData)
        {
            for (int i = 0; i < initialCreatureData.FaceContainerLoaders.Length; ++i)
            {
                Define.InGameGrade masteryGrade = initialCreatureData.FaceContainerLoaders[i].MasteryGrade;
                int length = initialCreatureData.FaceContainerLoaders[i].FaceContainerKeyLoaders.Length;
                FaceContainer[] faceContainers = new FaceContainer[length];
                for (int j = 0; j < length; ++j)
                {
                    FaceContainerKeyLoader keyLoader = initialCreatureData.FaceContainerLoaders[i].FaceContainerKeyLoaders[j];
                    faceContainers[j] = new FaceContainer();
                    faceContainers[j].FaceType = keyLoader.FaceType;
                    faceContainers[j].Eyebrows = Managers.Resource.Load<Sprite>(keyLoader.EyebrowsKey);
                    if (ColorUtility.TryParseHtmlString(keyLoader.EyebrowsColor, out Color eyebrowsColor))
                        faceContainers[j].EyebrowsColor = eyebrowsColor;
                    else
                    {
                        Utils.LogStrong(nameof(CreatureRendererController), nameof(InitFace),
                            $"Failed to load Eyebrows Color Code : {keyLoader.EyebrowsColor}");

                        faceContainers[j].EyebrowsColor = Color.white;
                    }

                    faceContainers[j].Eyes = Managers.Resource.Load<Sprite>(keyLoader.EyesKey);
                    if (ColorUtility.TryParseHtmlString(keyLoader.EyesColor, out Color eyesColor))
                        faceContainers[j].EyesColor = eyesColor;
                    else
                    {
                        Utils.LogStrong(nameof(CreatureRendererController), nameof(InitFace),
                            $"Failed to load Eyes Color Code : {keyLoader.EyesColor}");

                        faceContainers[j].EyesColor = Color.white;
                    }

                    faceContainers[j].Mouth = Managers.Resource.Load<Sprite>(keyLoader.MouthKey);
                    if (ColorUtility.TryParseHtmlString(keyLoader.MouthColor, out Color mouthColor))
                        faceContainers[j].MouthColor = mouthColor;
                    else
                    {
                        Utils.LogStrong(nameof(CreatureRendererController), nameof(InitFace),
                            $"Failed to load Eyes Color Code : {keyLoader.EyesColor}");

                        faceContainers[j].EyesColor = Color.white;
                    }
                }

                InitialLoadedFaceContainersDict.Add(masteryGrade, faceContainers);
            }

            InitFaceRef();
            if (this.OwnerAsCreature.ObjectType == Define.ObjectType.Player)
            {
                OnRefreshRendererHandler(KeyGrade);
                OnRefreshRenderer += OnRefreshRendererHandler;
            }
        }

        private void InitFaceRef()
        {
            if (this.OwnerAsCreature.ObjectType != Define.ObjectType.Player && this.OwnerAsCreature.ObjectType != Define.ObjectType.Monster)
            {
                Utils.LogStrong(nameof(CreatureController), nameof(InitFaceRef),
                    $"Only Player and Monster can access to \"InitCreatureFace\", Please Check in the Object Type : {this.OwnerAsCreature.ObjectType}");
                return;
            }

            FaceRef.CreatureType = this.OwnerAsCreature.ObjectType;
            for (int i = 0; i < this.OwnerSPRs.Length; ++i)
            {
                if (this.OwnerSPRs[i].gameObject.name.Contains("Eyebrows"))
                    FaceRef.EyebrowsSPR = this.OwnerSPRs[i];
                else if (this.OwnerSPRs[i].gameObject.name.Contains("Eyes"))
                    FaceRef.EyesSPR = this.OwnerSPRs[i];
                else if (this.OwnerSPRs[i].gameObject.name.Contains("Mouth"))
                    FaceRef.MouthSPR = this.OwnerSPRs[i];
            }
        }

        public override void EnterInGame()
        {
            this.FaceType = FaceType.Default;
            ResetMaterial();
        }

        protected override void OnRefreshRendererHandler(Define.InGameGrade keyGrade)
        {
            if (InitialLoadedFaceContainersDict.TryGetValue(keyGrade, out FaceContainer[] initialLoadedFaceContainers) == false)
            {
                Utils.LogCritical(nameof(CreatureRendererController), nameof(OnRefreshRendererHandler),
                                    $"Failed to called \"OnRefreshFaceContainerDictHandler\" : {keyGrade}");
                return;
            }
            else if (this.KeyGrade == Define.InGameGrade.Ultimate)
                return;

            if (FaceContainerDict.Count > 0)
                FaceContainerDict.Clear();

            this.KeyGrade = keyGrade;
            for (int i = 0; i < initialLoadedFaceContainers.Length; ++i)
            {
                FaceContainer faceContainer = new FaceContainer();
                faceContainer.FaceType = initialLoadedFaceContainers[i].FaceType;

                faceContainer.Eyebrows = initialLoadedFaceContainers[i].Eyebrows;
                faceContainer.EyebrowsColor = initialLoadedFaceContainers[i].EyebrowsColor;

                faceContainer.Eyes = initialLoadedFaceContainers[i].Eyes;
                faceContainer.EyesColor = initialLoadedFaceContainers[i].EyesColor;

                faceContainer.Mouth = initialLoadedFaceContainers[i].Mouth;
                faceContainer.MouthColor = initialLoadedFaceContainers[i].MouthColor;

                FaceContainerDict.Add(initialLoadedFaceContainers[i].FaceType, faceContainer);
            }

            RefreshModel(keyGrade);
        }

        private void RefreshModel(Define.InGameGrade keyGrade)
        {
            for (int i = 0; i < OwnerSPRs.Length; ++i)
                OwnerSPRs[i].sprite = null;

            SpriteRenderer[] nextSPRs = SpriteRenderers(keyGrade);
            int length = Mathf.Max(OwnerSPRs.Length, nextSPRs.Length);
            for (int i = 0; i < length; ++i)
            {
                if ((i < OwnerSPRs.Length) && (i < nextSPRs.Length))
                {
                    if (nextSPRs[i].name.Contains(OwnerSPRs[i].name))
                    {
                        // nextSPRs[i].sprite가 갖고 있는것만 집어 넣어 넣으면 해결
                        if (nextSPRs[i].sprite != null)
                        {
                            OwnerSPRs[i].gameObject.SetActive(nextSPRs[i].gameObject.activeSelf);
                            OwnerSPRs[i].gameObject.GetComponent<SpriteRenderer>().enabled = nextSPRs[i].gameObject.activeSelf; // 추가
                            OwnerSPRs[i].sprite = nextSPRs[i].sprite;
                            OwnerSPRs[i].color = nextSPRs[i].color;
                        }
                    }
                }
            }

            Utils.Log($"Success OnRefreshFaceContainerHandler : {keyGrade}");
        }

        public override void OnFaceDefaultHandler() => this.FaceType = FaceType.Default;
        public override void OnFaceCombatHandler() => this.FaceType = FaceType.Combat;
        public override void OnFaceDeadHandler() => this.FaceType = FaceType.Dead;
        public override void OnDustVFXHandler() => Managers.VFX.Environment(VFXEnv.Dust, this.OwnerAsCreature);

        public void HideFace(bool isOnHide)
        {
            switch (FaceRef.CreatureType)
            {
                case Define.ObjectType.Player:
                    {
                        if (isOnHide)
                        {
                            if (FaceRef.EyebrowsSPR.sprite != null)
                                FaceRef.EyebrowsSPR.sprite = null;

                            if (FaceRef.EyesSPR.sprite != null)
                                FaceRef.EyesSPR.sprite = null;

                            if (FaceRef.MouthSPR.sprite != null)
                                FaceRef.MouthSPR.sprite = null;
                        }
                        else
                            ShowFace(Define.ObjectType.Player);
                    }
                    break;

                case Define.ObjectType.Monster:
                    {
                        if (isOnHide)
                        {
                            if (FaceRef.EyesSPR.sprite != null)
                                FaceRef.EyesSPR.sprite = null;
                        }
                        else
                            ShowFace(Define.ObjectType.Monster);
                    }
                    break;
            }
        }

        private void ShowFace(Define.ObjectType creatureType)
        {
            if (creatureType == Define.ObjectType.Player)
            {
                switch (this.FaceType)
                {
                    case FaceType.Default:
                        {
                            if (FaceContainerDict.TryGetValue(FaceType.Default, out FaceContainer container))
                            {
                                FaceRef.EyebrowsSPR.sprite = container.Eyebrows;
                                FaceRef.EyebrowsSPR.color = container.EyebrowsColor;

                                FaceRef.EyesSPR.sprite = container.Eyes;
                                FaceRef.EyesSPR.color = container.EyesColor;

                                FaceRef.MouthSPR.sprite = container.Mouth;
                                FaceRef.MouthSPR.color = container.MouthColor;
                            }
                        }
                        break;

                    case FaceType.Combat:
                        {
                            if (FaceContainerDict.TryGetValue(FaceType.Combat, out FaceContainer container))
                            {
                                FaceRef.EyebrowsSPR.sprite = container.Eyebrows;
                                FaceRef.EyebrowsSPR.color = container.EyebrowsColor;

                                FaceRef.EyesSPR.sprite = container.Eyes;
                                FaceRef.EyesSPR.color = container.EyesColor;

                                FaceRef.MouthSPR.sprite = container.Mouth;
                                FaceRef.MouthSPR.color = container.MouthColor;
                            }
                        }
                        break;

                    case FaceType.Dead:
                        {
                            if (FaceContainerDict.TryGetValue(FaceType.Dead, out FaceContainer container))
                            {
                                FaceRef.EyebrowsSPR.sprite = container.Eyebrows;
                                FaceRef.EyebrowsSPR.color = container.EyebrowsColor;

                                FaceRef.EyesSPR.sprite = container.Eyes;
                                FaceRef.EyesSPR.color = container.EyesColor;

                                FaceRef.MouthSPR.sprite = container.Mouth;
                                FaceRef.MouthSPR.color = container.MouthColor;
                            }
                        }
                        break;
                }
            }
            else if (creatureType == Define.ObjectType.Monster)
            {
                switch (this.FaceType)
                {
                    case FaceType.Default:
                        {
                            if (FaceContainerDict.TryGetValue(FaceType.Default, out FaceContainer container))
                            {
                                FaceRef.EyesSPR.sprite = container.Eyes;
                                FaceRef.EyesSPR.color = container.EyesColor;
                            }
                        }
                        break;

                    case FaceType.Combat:
                        {
                            if (FaceContainerDict.TryGetValue(FaceType.Combat, out FaceContainer container))
                            {
                                FaceRef.EyesSPR.sprite = container.Eyes;
                                FaceRef.EyesSPR.color = container.EyesColor;
                            }
                        }
                        break;

                    case FaceType.Dead:
                        {
                            if (FaceContainerDict.TryGetValue(FaceType.Dead, out FaceContainer container))
                            {
                                FaceRef.EyesSPR.sprite = container.Eyes;
                                FaceRef.EyesSPR.color = container.EyesColor;
                            }
                        }
                        break;
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.HeroEditor.Common.Scripts.CharacterScripts.Firearms;
using STELLAREST_2D.Data;
using UnityEngine;

using CrowdControl = STELLAREST_2D.Define.TemplateIDs.CrowdControl;
using FaceType = STELLAREST_2D.Define.FaceType;

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

    public class CreatureFace
    {
        public Define.ObjectType CreatureType { get; set; } = Define.ObjectType.None;
        public SpriteRenderer EyebrowsSPR { get; set; } = null;
        public SpriteRenderer EyesSPR { get; set; } = null;
        public SpriteRenderer MouthSPR { get; set; } = null;
    }

    public class CreatureRendererController : RendererController
    {
        public Dictionary<Define.InGameGrade, FaceContainer[]> InitialLoadedFaceContainersDict { get; } = new Dictionary<Define.InGameGrade, FaceContainer[]>();
        public Dictionary<FaceType, FaceContainer> CurrentGradeFaceContainerDict { get; } = new Dictionary<FaceType, FaceContainer>();
        public CreatureFace CreatureFace { get; } = new CreatureFace();
        
        [field: SerializeField] public Define.InGameGrade CurrentKeyGrade { get; private set; } = Define.InGameGrade.Default;
        [field: SerializeField] public FaceType CurrentFaceType { get; private set; } = FaceType.Default;

        public override void InitRendererController(BaseController owner, InitialCreatureData initialCreatureData)
        {
            if (this.Owner != null)
                return;

            base.InitRendererController(owner, initialCreatureData);
            InitCreatureFace();
            InitFace(initialCreatureData);
            InitModels(initialCreatureData);
        }

        private void InitCreatureFace()
        {
            if (this.OwnerAsCreature.ObjectType != Define.ObjectType.Player ||
                this.OwnerAsCreature.ObjectType != Define.ObjectType.Monster)
            {
                Utils.LogStrong(nameof(CreatureController), nameof(InitCreatureFace),
                    $"Another object tries to access InitCreatureFace : {this.OwnerAsCreature.ObjectType}");
                return;
            }

            CreatureFace.CreatureType = this.OwnerAsCreature.ObjectType;
            if (this.OwnerAsCreature.ObjectType == Define.ObjectType.Player)
            {
                for (int i = 0; i < this.OwnerSPRs.Length; ++i)
                {
                    if (this.OwnerSPRs[i].gameObject.name.Contains("Eyebrows"))
                        CreatureFace.EyebrowsSPR = this.OwnerSPRs[i];
                    else if (this.OwnerSPRs[i].gameObject.name.Contains("Eyes"))
                        CreatureFace.EyesSPR = this.OwnerSPRs[i];
                    else if (this.OwnerSPRs[i].gameObject.name.Contains("Mouth"))
                        CreatureFace.MouthSPR = this.OwnerSPRs[i];
                }
            }
        }

        private void InitFace(InitialCreatureData initialCreatureData)
        {
            for (int i = 0; i < initialCreatureData.FaceContainerLoaders.Length; ++i)
            {
                Define.InGameGrade masteryGrade = initialCreatureData.FaceContainerLoaders[i].MasteryGrade;
                
                int length = initialCreatureData.FaceContainerLoaders[i].FaceContainerKeyLoaders.Length;
                FaceContainer[] faceValues = new FaceContainer[length];
                for (int j = 0; j < length; ++j)
                {
                    FaceContainerKeyLoader keyLoader = initialCreatureData.FaceContainerLoaders[i].FaceContainerKeyLoaders[i];
                    faceValues[i] = new FaceContainer();
                    
                    // Init Face Type 1 (Default)
                    faceValues[i].FaceType = keyLoader.FaceType;

                    faceValues[i].Eyebrows = Managers.Resource.Load<Sprite>(keyLoader.EyebrowsKey);
                    if (ColorUtility.TryParseHtmlString(keyLoader.EyebrowsColor, out Color eyebrowsColor))
                        faceValues[i].EyebrowsColor = eyebrowsColor;
                    else
                    {
                        Utils.LogStrong(nameof(CreatureRendererController), nameof(InitFace), 
                            $"Failed to load Eyebrows Color Code : {keyLoader.EyebrowsColor}");

                        faceValues[i].EyebrowsColor = Color.white;
                    }

                    faceValues[i].Eyes = Managers.Resource.Load<Sprite>(keyLoader.EyesKey);
                    if (ColorUtility.TryParseHtmlString(keyLoader.EyesColor, out Color eyesColor))
                        faceValues[i].EyesColor = eyesColor;
                    else
                    {
                        Utils.LogStrong(nameof(CreatureRendererController), nameof(InitFace),
                            $"Failed to load Eyes Color Code : {keyLoader.EyesColor}");

                        faceValues[i].EyesColor = Color.white;
                    }

                    faceValues[i].Mouth = Managers.Resource.Load<Sprite>(keyLoader.MouthKey);
                    if (ColorUtility.TryParseHtmlString(keyLoader.MouthColor, out Color mouthColor))
                        faceValues[i].MouthColor = mouthColor;
                    else
                    {
                        Utils.LogStrong(nameof(CreatureRendererController), nameof(InitFace),
                            $"Failed to load Eyes Color Code : {keyLoader.EyesColor}");

                        faceValues[i].EyesColor = Color.white;
                    }
                }

                InitialLoadedFaceContainersDict.Add(masteryGrade, faceValues);
            }
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
                Material matOrigin = new Material(SPRs[i].sharedMaterial);
                Color colorOrigin = new Color(SPRs[i].color.r, SPRs[i].color.g, SPRs[i].color.g, SPRs[i].color.a);
                BCs[i] = new BaseContainer(SPRs[i].name, matOrigin, colorOrigin);
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
                        Material matOrigin = new Material(SPRs[i].sharedMaterial);
                        Color colorOrigin = new Color(SPRs[i].color.r, SPRs[i].color.g, SPRs[i].color.g, SPRs[i].color.a);
                        BCs[i] = new BaseContainer(SPRs[i].name, matOrigin, colorOrigin);
                    }

                    rendererModerator.AddRendererContainers(keyGrade, BCs, SPRs);
                }
            }

            RendererModeratorDict.Add(this.OwnerAsCreature, rendererModerator);
        }

        private void OnRefreshCurrentGradeFaceContainerDictHandler(Define.InGameGrade currentKeyGrade)
        {
            if (InitialLoadedFaceContainersDict.TryGetValue(currentKeyGrade, out FaceContainer[] initialLoadedFaceContainers) == false)
            {
                Utils.LogCritical(nameof(CreatureRendererController), nameof(OnRefreshCurrentGradeFaceContainerDictHandler), 
                                    $"Failed to called \"OnRefreshFaceContainerDictHandler\" : {currentKeyGrade}");
                return;
            }

            if (CurrentGradeFaceContainerDict.Count > 0)
                CurrentGradeFaceContainerDict.Clear();

            this.CurrentKeyGrade = currentKeyGrade;
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

                CurrentGradeFaceContainerDict.Add(initialLoadedFaceContainers[i].FaceType, faceContainer);
            }

            Utils.Log($"Success OnRefreshFaceContainerHandler : {currentKeyGrade}");
        }
        
        public override void OnFaceDefaultHandler()
        {
            if (CurrentGradeFaceContainerDict.TryGetValue(FaceType.Default, out FaceContainer faceContainer) == false)
            {
                Utils.LogCritical(nameof(CreatureRendererController), nameof(OnFaceDefaultHandler),
                    $"Faield to called \"OnFaceDefaultHandler\": {FaceType.Default}");
                return;
            }

            if (this.OwnerAsCreature[CrowdControl.Stun])
                return;

            this.CurrentFaceType = FaceType.Default;
            if (CreatureFace.CreatureType == Define.ObjectType.Player)
            {
                if (faceContainer.Eyebrows != null)
                {
                    CreatureFace.EyebrowsSPR.sprite = faceContainer.Eyebrows;
                    CreatureFace.EyebrowsSPR.color = faceContainer.EyebrowsColor;
                }

                if (faceContainer.Eyes != null)
                {
                    CreatureFace.EyesSPR.sprite = faceContainer.Eyes;
                    CreatureFace.EyesSPR.color = faceContainer.EyesColor;
                }

                if (faceContainer.Mouth != null)
                {
                    CreatureFace.MouthSPR.sprite = faceContainer.Mouth;
                    CreatureFace.MouthSPR.color = faceContainer.MouthColor;
                }
            }
            else if (this.CreatureFace.CreatureType == Define.ObjectType.Monster)
            {
            }
        }

        public override void OnFaceCombatHandler()
        {
            if (CurrentGradeFaceContainerDict.TryGetValue(FaceType.Combat, out FaceContainer faceContainer) == false)
            {
                Utils.LogCritical(nameof(CreatureRendererController), nameof(OnFaceDefaultHandler), 
                    $"Faield to called \"OnFaceCombatHandler\": {FaceType.Combat}");
                return;
            }

            this.CurrentFaceType = FaceType.Combat;
            if (CreatureFace.CreatureType == Define.ObjectType.Player)
            {
                if (faceContainer.Eyebrows != null)
                {
                    CreatureFace.EyebrowsSPR.sprite = faceContainer.Eyebrows;
                    CreatureFace.EyebrowsSPR.color = faceContainer.EyebrowsColor;
                }

                if (faceContainer.Eyes != null)
                {
                    CreatureFace.EyesSPR.sprite = faceContainer.Eyes;
                    CreatureFace.EyesSPR.color = faceContainer.EyesColor;
                }

                if (faceContainer.Mouth != null)
                {
                    CreatureFace.MouthSPR.sprite = faceContainer.Mouth;
                    CreatureFace.MouthSPR.color = faceContainer.MouthColor;
                }
            }
            else if (this.CreatureFace.CreatureType == Define.ObjectType.Monster)
            {
            }
        }

        public override void OnFaceDeadHandler()
        {
            if (CurrentGradeFaceContainerDict.TryGetValue(FaceType.Dead, out FaceContainer faceContainer) == false)
            {
                Utils.LogCritical(nameof(CreatureRendererController), nameof(OnFaceDefaultHandler),
                    $"Faield to called \"OnFaceDeadHandler\": {FaceType.Dead}");
                return;
            }

            CurrentFaceType = FaceType.Dead;
            if (CreatureFace.CreatureType == Define.ObjectType.Player)
            {
                if (faceContainer.Eyebrows != null)
                {
                    CreatureFace.EyebrowsSPR.sprite = faceContainer.Eyebrows;
                    CreatureFace.EyebrowsSPR.color = faceContainer.EyebrowsColor;
                }

                if (faceContainer.Eyes != null)
                {
                    CreatureFace.EyesSPR.sprite = faceContainer.Eyes;
                    CreatureFace.EyesSPR.color = faceContainer.EyesColor;
                }

                if (faceContainer.Mouth != null)
                {
                    CreatureFace.MouthSPR.sprite = faceContainer.Mouth;
                    CreatureFace.MouthSPR.color = faceContainer.MouthColor;
                }
            }
            else if (this.CreatureFace.CreatureType == Define.ObjectType.Monster)
            {
            }
        }

    // ============================================================================================================
    // ============================================================================================================
    // ============================================================================================================
    // ============================================================================================================
    // ============================================================================================================
    // ============================================================================================================
        // private void InitPlayerEyes()
        // {
        //     for (int i = 0; i < this.OwnerSPRs.Length; ++i)
        //     {
        //         if (this.OwnerSPRs[i].name.Contains("Eyes"))
        //         {
        //             PlayerEyes = this.OwnerSPRs[i].sprite;
        //             PlayerEyesSPR = this.OwnerSPRs[i];
        //         }
        //     }
        // }

        // private void InitPlayerFace()
        // {
        //     if (this.PlayerFace != null)
        //         return;

        //     this.PlayerFace = new PlayerFace();
        //     for (int i = 0; i < OwnerSPRs.Length; ++i)
        //     {
        //         if (this.OwnerSPRs[i].name.Contains("Eyebrows"))
        //             this.PlayerFace.eyebrowsSPR = OwnerSPRs[i];

        //         if (this.OwnerSPRs[i].name.Contains("Eyes"))
        //             this.PlayerFace.eyesSPR = OwnerSPRs[i];

        //         if (this.OwnerSPRs[i].name.Contains("Mouth"))
        //             this.PlayerFace.mouthSPR = OwnerSPRs[i];
        //     }
        // }

        // private void LoadPlayerExpressionsData(InitialCreatureData initialCreatureData)
        // {
        //     GameObject go = Managers.Resource.Load<GameObject>(initialCreatureData.PrimaryLabel);
        //     SpriteRenderer[] SPRs = go.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
        //     BaseContainer[] BCs = new BaseContainer[SPRs.Length];
        //     for (int i = 0; i < SPRs.Length; ++i)
        //     {
        //         Material matOrigin = new Material(SPRs[i].sharedMaterial);
        //         Color colorOrigin = SPRs[i].color;
        //         BCs[i] = new BaseContainer(SPRs[i].name, matOrigin, colorOrigin);
        //     }

        //     // Moderator moderator = new Moderator();
        //     // moderator.AddRendererContainers(Define.InGameGrade.Default, BCs, SPRs);
        // }
    }
}

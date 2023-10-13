using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using STELLAREST_2D.Data;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

namespace STELLAREST_2D
{
    // +++++ PLAYER FACE EXPRESSION +++++ PREV TEMP
    // [System.Serializable]
    // public class PlayerFaceExpressionLoader
    // {
    //     public Define.InGameGrade MasteryGrade;
    //     public Define.FaceExpressionType FaceExpressionType;
    //     public string EyebrowsPrimaryKey;
    //     public string EyebrowsColorCode;

    //     public string EyesPrimaryKey;
    //     public string EyesColorCode;

    //     public string MouthPrimaryKey;
    //     public string MouthColorCode;
    // }

    // public class PlayerFaceExpression
    // {
    //     public Sprite Eyebrows;
    //     public Color EyebrowsColor;

    //     public Sprite Eyes;
    //     public Color EyesColor;

    //     public Sprite Mouth;
    //     public Color MouthColor;
    // }
    // ============================================================================================================
    
    [System.Serializable]
    public class PlayerFaceExpressionLoader
    {
        public Define.InGameGrade MasteryGrade;
        public PlayerFaceExpressionKeyLoader[] PlayerFaceExpressionsKeyLoader;

    }

    [System.Serializable]
    public class PlayerFaceExpressionKeyLoader
    {
        public Define.FaceExpressionType FaceExpressionType;
        public string EyebrowsPrimaryKey;
        public string EyebrowsColorCode;

        public string EyesPrimaryKey;
        public string EyesColorCode;

        public string MouthPrimaryKey;
        public string MouthColorCode;
    }

    public class PlayerFaceExpression
    {
        public Define.FaceExpressionType ExpressionType;
        public Sprite Eyebrows;
        public Color EyebrowsColor;

        public Sprite Eyes;
        public Color EyesColor;

        public Sprite Mouth;
        public Color MouthColor;
    }

    // ============================================================================================================
    public class PlayerFace
    {
        public SpriteRenderer eyebrowsSPR;
        public SpriteRenderer eyesSPR;
        public SpriteRenderer mouthSPR;
    }

    // +++++ Base Container +++++
    public class BaseContainer
    {
        public BaseContainer(string tag, Material matOrigin, Color colorOrigin)
        {
            this.Tag = tag; // 나중에 부위 찾을때 활용할지도. (string 대신 PlayerBody 이걸로 대신해서), 그러면 무기 바꾸거나 장착할 때 겁내 편해짐
            this.MatOrigin = matOrigin;
            this.ColorOrigin = colorOrigin;
        }

        // public SpriteRenderer[] cachedSPRs { get; } = null; // READ ONLY PROPERTY
        // public SpriteRenderer[] cachedSPRs2 { get; private set; } = null; // READ ONLY FIELD
        public string Tag { get; } = null;
        public Material MatOrigin { get; } = null;
        public Color ColorOrigin { get; } = Color.white;
    }

    public class RendererContainer
    {
        private SpriteRenderer[] _spriteRenderers = null;
        public SpriteRenderer[] SpriteRenderers // Included null sprite also
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

    public class Moderator
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

    public class RendererController : MonoBehaviour
    {
        public CreatureController Owner { get; private set; } = null;
        private MonsterController _ownerAsMonsterController = null;
        public MonsterController OwnerAsMonsterController
        {
            get
            {
                if (this.IsPlayer)
                {
                    Utils.LogStrong(nameof(RendererController), nameof(OwnerAsMonsterController), $"Player tries to get acccess \"OwnerAsMonsterController\"");
                    return null;
                }

                return _ownerAsMonsterController;
            }

            private set => _ownerAsMonsterController = value;
        }

        public SpriteRenderer[] OwnerSPRs { get; private set; } = null;
        private Dictionary<CreatureController, Moderator> _moderatorDict = null;
        public bool IsChangingMaterial { get; private set; } = false;
        private Define.InGameGrade _currentKeyGrade = Define.InGameGrade.Default;
        [field: SerializeField] public Define.FaceExpressionType CurrentFaceState { get; private set; } = Define.FaceExpressionType.Default;

        private Sprite _playerEyesSprite = null;
        public Sprite PlayerEyesSprite 
        { 
            get
            {
                if (this.IsPlayer == false)
                {
                    Utils.LogStrong(nameof(RendererController), nameof(PlayerEyesSprite), $"Monster tries to get acccess \"PlayerEyesSprite\"");
                    return null;
                }

                return _playerEyesSprite;
            }

            private set => _playerEyesSprite = value;
        }

        private SpriteRenderer _playerEyesSPR = null;
        public SpriteRenderer PlayerEyesSPR
        {
            get
            {
                if (this.IsPlayer == false)
                {
                    Utils.LogStrong(nameof(RendererController), nameof(PlayerEyesSprite), $"Monster tries to get acccess \"PlayerEyesSprite\"");
                    return null;
                }

                return _playerEyesSPR;
            }

            private set => _playerEyesSPR = value;
        }

        // public Dictionary<Define.FaceExpressionType, PlayerFaceExpression> PlayerFaceExpressionDict { get; private set; } 
        //                                             = new Dictionary<Define.FaceExpressionType, PlayerFaceExpression>();

#region Player Expressions Block
        public Dictionary<Define.InGameGrade, PlayerFaceExpression[]> PlayerFaceExpressionsDict { get; private set; } 
                                                        = new Dictionary<Define.InGameGrade, PlayerFaceExpression[]>();

        private void InitPlayerFaceExpression(InitialCreatureData initialCreatureData)
        {
            //PlayerFaceExpressionLoader[] loaders = new PlayerFaceExpressionLoader[initialCreatureData.PlayerFaceExpressionsLoader.Length];
            for (int i = 0; i < initialCreatureData.PlayerFaceExpressionsLoader.Length; ++i)
            {
                PlayerFaceExpressionLoader loader = initialCreatureData.PlayerFaceExpressionsLoader[i];
                Define.InGameGrade keyMasteryGrade = loader.MasteryGrade;
                //PlayerFaceExpressionKeyLoader[] keyLoaders = new PlayerFaceExpressionKeyLoader[initialCreatureData.PlayerFaceExpressionsLoader[i].PlayerFaceExpressionsKeyLoader.Length];
                PlayerFaceExpression[] valuePlayerFaceExpressions = new PlayerFaceExpression[initialCreatureData.PlayerFaceExpressionsLoader[i].PlayerFaceExpressionsKeyLoader.Length];
                for (int j = 0; j < initialCreatureData.PlayerFaceExpressionsLoader[i].PlayerFaceExpressionsKeyLoader.Length; ++j)
                {
                    PlayerFaceExpressionKeyLoader keyLoader = initialCreatureData.PlayerFaceExpressionsLoader[i].PlayerFaceExpressionsKeyLoader[j];
                    valuePlayerFaceExpressions[j] = new PlayerFaceExpression();
                    valuePlayerFaceExpressions[j].ExpressionType = keyLoader.FaceExpressionType;
                    
                    // LOAD && SET EYEBROWS
                    valuePlayerFaceExpressions[j].Eyebrows = Managers.Resource.Load<Sprite>(keyLoader.EyebrowsPrimaryKey);
                    if (ColorUtility.TryParseHtmlString(keyLoader.EyebrowsColorCode, out Color eyebrowsColor))
                        valuePlayerFaceExpressions[j].EyebrowsColor = eyebrowsColor;
                    else
                    {
                        Utils.LogStrong(nameof(RendererContainer), nameof(InitPlayerFaceExpression), "Failed to load EyebrowsColor.");
                        valuePlayerFaceExpressions[j].EyebrowsColor = Color.white;
                    }

                    // LOAD && SET EYES
                    valuePlayerFaceExpressions[j].Eyes = Managers.Resource.Load<Sprite>(keyLoader.EyesPrimaryKey);
                    if (ColorUtility.TryParseHtmlString(keyLoader.EyesColorCode, out Color eyesColor))
                        valuePlayerFaceExpressions[j].EyesColor = eyesColor;
                    else
                    {
                        Utils.LogStrong(nameof(RendererContainer), nameof(InitPlayerFaceExpression), "Failed to load EyesColor.");
                        valuePlayerFaceExpressions[j].EyesColor = Color.white;
                    }
                    
                    // LOAD && SET MOUTH
                    valuePlayerFaceExpressions[j].Mouth = Managers.Resource.Load<Sprite>(keyLoader.MouthPrimaryKey);
                    if (ColorUtility.TryParseHtmlString(keyLoader.MouthColorCode, out Color mouthColor))
                        valuePlayerFaceExpressions[j].MouthColor = mouthColor;
                    else
                    {
                        Utils.LogStrong(nameof(RendererContainer), nameof(InitPlayerFaceExpression), "Failed to load EyesColor.");
                        valuePlayerFaceExpressions[j].MouthColor = Color.white;
                    }
                }

                PlayerFaceExpressionsDict.Add(keyMasteryGrade, valuePlayerFaceExpressions);
            }
        }

/*
            public Dictionary<Define.InGameGrade, PlayerFaceExpression[]> PlayerFaceExpressionsDict { get; private set; } 
            =  new Dictionary<Define.InGameGrade, PlayerFaceExpression[]>();
*/

        // IF NO CACHING
        public void OnFaceBattleHandler()
        {
            if (this.IsPlayer)
            {
                if (this.Owner.IsDeadState)
                    return;

                Utils.Log("Called OnFaceBattleHandler");
                // GRADE : Default - DEFAULT, BATTLE, DEAD 
                PlayerFaceExpression[] expressions = PlayerFaceExpressionsDict[_currentKeyGrade];
                for (int i = 0; i < expressions.Length; ++i)
                {
                    if (expressions[i].ExpressionType == Define.FaceExpressionType.Battle)
                    {
                        PlayerFace.eyebrowsSPR.sprite = expressions[i].Eyebrows;
                        PlayerFace.eyebrowsSPR.color = expressions[i].EyebrowsColor;

                        PlayerFace.eyesSPR.sprite = expressions[i].Eyes;
                        PlayerFace.eyesSPR.color = expressions[i].EyesColor;

                        PlayerFace.mouthSPR.sprite = expressions[i].Mouth;
                        PlayerFace.mouthSPR.color = expressions[i].MouthColor;
                    }
                }

                CurrentFaceState = Define.FaceExpressionType.Battle;
            }
            else
                MonsterHead.sprite = OwnerAsMonsterController.AngryHead;
        }

        // IF NO CACHING
        public void OnFaceDefaultHandler()
        {
            if (this.IsPlayer)
            {
                if (this.Owner.IsDeadState)
                    return;

                Utils.Log("Called OnFaceDefaultHandler");
                // GRADE : Default - DEFAULT, BATTLE, DEAD 
                PlayerFaceExpression[] expressions = PlayerFaceExpressionsDict[_currentKeyGrade];
                for (int i = 0; i < expressions.Length; ++i)
                {
                    if (expressions[i].ExpressionType == Define.FaceExpressionType.Default)
                    {
                        PlayerFace.eyebrowsSPR.sprite = expressions[i].Eyebrows;
                        PlayerFace.eyebrowsSPR.color = expressions[i].EyebrowsColor;

                        PlayerFace.eyesSPR.sprite = expressions[i].Eyes;
                        PlayerFace.eyesSPR.color = expressions[i].EyesColor;

                        PlayerFace.mouthSPR.sprite = expressions[i].Mouth;
                        PlayerFace.mouthSPR.color = expressions[i].MouthColor;
                    }
                }

                CurrentFaceState = Define.FaceExpressionType.Default;
            }
            else
                MonsterHead.sprite = OwnerAsMonsterController.AngryHead;
        }

        // IF NO CACHING
        // Remove Animation Event. Called from OnDead
        public void OnFaceDeadHandler()
        {
            if (this.IsPlayer)
            {
                Utils.Log("Called OnFaceDeadHandler");
                // GRADE : Default - DEFAULT, BATTLE, DEAD 
                PlayerFaceExpression[] expressions = PlayerFaceExpressionsDict[_currentKeyGrade];
                for (int i = 0; i < expressions.Length; ++i)
                {
                    if (expressions[i].ExpressionType == Define.FaceExpressionType.Dead)
                    {
                        PlayerFace.eyebrowsSPR.sprite = expressions[i].Eyebrows;
                        PlayerFace.eyebrowsSPR.color = expressions[i].EyebrowsColor;

                        PlayerFace.eyesSPR.sprite = expressions[i].Eyes;
                        PlayerFace.eyesSPR.color = expressions[i].EyesColor;

                        PlayerFace.mouthSPR.sprite = expressions[i].Mouth;
                        PlayerFace.mouthSPR.color = expressions[i].MouthColor;
                    }
                }

                CurrentFaceState = Define.FaceExpressionType.Dead;
            }
            else
                MonsterHead.sprite = OwnerAsMonsterController.AngryHead;
        }

#endregion
        public PlayerFace PlayerFace { get; private set; } = null;

        private SpriteRenderer _monsterHead = null;
        public SpriteRenderer MonsterHead 
        {
            get
            {
                if (this.IsPlayer)
                {
                    Utils.LogStrong(nameof(RendererController), nameof(MonsterHead), $"Player tries to get acccess \"MonsterHead\"");
                    return null;
                }

                return _monsterHead;
            }

            private set => _monsterHead = value;
        }
        
        public void InitRendererController(CreatureController owner, InitialCreatureData initialCreatureData)
        {
            if (this.Owner != null || this._moderatorDict != null)
                return;

            // INIT CURRENT
            this.Owner = owner;
            OwnerSPRs = owner.SPRs;

            if (this.IsPlayer)
            {
                InitPlayerEyes();
                InitPlayerFace();
                InitPlayerFaceExpression(initialCreatureData);
            }
            else
                InitMonsterHead();

            _moderatorDict = new Dictionary<CreatureController, Moderator>();
            Moderator moderator = new Moderator();

            GameObject go = Managers.Resource.Load<GameObject>(initialCreatureData.PrimaryLabel);
            SpriteRenderer[] SPRs = go.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
            BaseContainer[] BCs = new BaseContainer[SPRs.Length];
            for (int i = 0; i < SPRs.Length; ++i)
            {
                Material matOrigin = new Material(SPRs[i].sharedMaterial);
                Color colorOrigin = SPRs[i].color;
                BCs[i] = new BaseContainer(SPRs[i].name, matOrigin, colorOrigin);
            }
            moderator.AddRendererContainers(Define.InGameGrade.Default, BCs, SPRs);

            // INIT RESTS OF THE NEXT
            List<SkillBase> skillList = new List<SkillBase>();
            foreach (KeyValuePair<int, SkillGroup> pair in owner.SkillBook.SkillGroupsDict)
            {
                for (int i = 0; i < pair.Value.MemberCount; ++i)
                    skillList.Add(pair.Value.Members[i].SkillOrigin);
            }

            SkillBase[] modelingLabelSkills = skillList.Where(s => s.Data.ModelingLabel.Length > 0).ToArray();
            if (modelingLabelSkills.Length > 0)
            {
                for (int i = 0; i < modelingLabelSkills.Length; ++i)
                {
                    Define.InGameGrade keyGrade = modelingLabelSkills[i].Data.Grade;
                    string modelingLabel = modelingLabelSkills[i].Data.ModelingLabel;
                    go = Managers.Resource.Load<GameObject>(modelingLabel);
                    if (go == null)
                        Utils.LogCritical(nameof(RendererContainer), nameof(InitRendererController), $"Check Modeling Label : {modelingLabel}");

                    SPRs = go.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
                    BCs = new BaseContainer[SPRs.Length];
                    for (int j = 0; j < SPRs.Length; ++j)
                    {
                        Material matOrigin = new Material(SPRs[j].sharedMaterial);
                        Color colorOrigin = SPRs[j].color;
                        BCs[j] = new BaseContainer(SPRs[j].name, matOrigin, colorOrigin);
                    }

                    moderator.AddRendererContainers(keyGrade, BCs, SPRs);
                }
            }

            _moderatorDict.Add(owner, moderator);
        }

        private void InitPlayerEyes()
        {
            for (int i = 0; i < OwnerSPRs.Length; ++i)
            {
                if (OwnerSPRs[i].name.Contains("Eyes"))
                {
                    this.PlayerEyesSPR = OwnerSPRs[i];
                    this.PlayerEyesSprite = OwnerSPRs[i].sprite;
                }
            }
        }

        private void InitPlayerFace()
        {
            if (this.PlayerFace != null)
                return;
            
            this.PlayerFace = new PlayerFace();
            for (int i = 0; i < OwnerSPRs.Length; ++i)
            {
                if (OwnerSPRs[i].name.Contains("Eyebrows"))
                    this.PlayerFace.eyebrowsSPR = OwnerSPRs[i];

                if (OwnerSPRs[i].name.Contains("Eyes"))
                    this.PlayerFace.eyesSPR = OwnerSPRs[i];

                if (OwnerSPRs[i].name.Contains("Mouth"))
                    this.PlayerFace.mouthSPR = OwnerSPRs[i];
            }
        }   

        // private void InitPlayerFaceExpression(InitialCreatureData initialCreatureData)
        // {
        //     PlayerFaceExpressionLoader[] loaders = new PlayerFaceExpressionLoader[initialCreatureData.PlayerFaceExpressionsLoader.Length];
        //     for (int i = 0; i < initialCreatureData.PlayerFaceExpressionsLoader.Length; ++i)
        //     {
        //         PlayerFaceExpression faceExpression = new PlayerFaceExpression();

        //         // Eyebrows
        //         faceExpression.Eyebrows = Managers.Resource.Load<Sprite>(initialCreatureData.PlayerFaceExpressionsLoader[i].EyebrowsPrimaryKey);
        //         if (ColorUtility.TryParseHtmlString(initialCreatureData.PlayerFaceExpressionsLoader[i].EyebrowsColorCode, out Color eyeBrowsColor))
        //             faceExpression.EyebrowsColor = eyeBrowsColor;
        //         else
        //             Utils.LogStrong(nameof(RendererController), nameof(InitRendererController), $"Failed to set eyebrowsColor.");


        //         // Eyes
        //         faceExpression.Eyes = Managers.Resource.Load<Sprite>(initialCreatureData.PlayerFaceExpressionsLoader[i].EyesPrimaryKey);
        //         if (ColorUtility.TryParseHtmlString(initialCreatureData.PlayerFaceExpressionsLoader[i].EyesColorCode, out Color eyesColor))
        //             faceExpression.EyesColor = eyesColor;
        //         else
        //             Utils.LogStrong(nameof(RendererController), nameof(InitRendererController), $"Failed to set eyesColor.");


        //         // Mouth
        //         faceExpression.Mouth = Managers.Resource.Load<Sprite>(initialCreatureData.PlayerFaceExpressionsLoader[i].MouthPrimaryKey);
        //         if (ColorUtility.TryParseHtmlString(initialCreatureData.PlayerFaceExpressionsLoader[i].MouthColorCode, out Color mouthColor))
        //             faceExpression.MouthColor = mouthColor;
        //         else
        //             Utils.LogStrong(nameof(RendererController), nameof(InitRendererController), $"Failed to set mouthColor.");

        //         PlayerFaceExpressionDict.Add(initialCreatureData.PlayerFaceExpressionsLoader[i].FaceExpressionType, faceExpression);
        //     }
        // }

        private void InitMonsterHead()
        {
            if (this.MonsterHead != null || this.OwnerAsMonsterController != null)
                return;

            for (int i = 0; i < OwnerSPRs.Length; ++i)
            {
                if (OwnerSPRs[i].gameObject.name.Contains("Head"))
                    MonsterHead = OwnerSPRs[i];
            }

            OwnerAsMonsterController = this.Owner as MonsterController;
        }

        public void StartGame()
        {
            ResetMaterial();
            if (this.IsPlayer == false)
                this.MonsterHead.sprite = OwnerAsMonsterController.DefaultHead;
        }

        // public void OnFaceBattleHandler_TEMP()
        // {
        //     if (this.IsPlayer)
        //     {
        //         if (this.Owner.IsDeadState)
        //             return;

        //         Utils.Log($"Current KeyGrade : {_currentKeyGrade}");
                
        //         PlayerFaceExpression battleFace = this.PlayerFaceExpressionDict[Define.FaceExpressionType.Battle];
        //         PlayerFace.eyebrowsSPR.sprite = battleFace.Eyebrows;
        //         PlayerFace.eyebrowsSPR.color = battleFace.EyebrowsColor;

        //         PlayerFace.eyesSPR.sprite = battleFace.Eyes;
        //         PlayerFace.eyesSPR.color = battleFace.EyesColor;

        //         PlayerFace.mouthSPR.sprite = battleFace.Mouth;
        //         PlayerFace.mouthSPR.color = battleFace.MouthColor;
        //     }
        //     else
        //         MonsterHead.sprite = OwnerAsMonsterController.AngryHead;
        // }

        // public void OnFaceDefaultHandler_TEMP()
        // {
        //     if (this.IsPlayer)
        //     {
        //         if (this.Owner.IsDeadState)
        //             return;

        //         PlayerFaceExpression defaultFace = this.PlayerFaceExpressionDict[Define.FaceExpressionType.Default];
        //         PlayerFace.eyebrowsSPR.sprite = defaultFace.Eyebrows;
        //         PlayerFace.eyebrowsSPR.color = defaultFace.EyebrowsColor;

        //         PlayerFace.eyesSPR.sprite = defaultFace.Eyes;
        //         PlayerFace.eyesSPR.color = defaultFace.EyesColor;

        //         PlayerFace.mouthSPR.sprite = defaultFace.Mouth;
        //         PlayerFace.mouthSPR.color = defaultFace.MouthColor;
        //     }
        //     else
        //         MonsterHead.sprite = OwnerAsMonsterController.DefaultHead;
        // }

        // public void OnFaceDeadHandler_TEMP()
        // {
        //     if (this.IsPlayer)
        //     {
        //         PlayerFaceExpression deadFace = this.PlayerFaceExpressionDict[Define.FaceExpressionType.Dead];
        //         PlayerFace.eyebrowsSPR.sprite = deadFace.Eyebrows;
        //         PlayerFace.eyebrowsSPR.color = deadFace.EyebrowsColor;

        //         PlayerFace.eyesSPR.sprite = deadFace.Eyes;
        //         PlayerFace.eyesSPR.color = deadFace.EyesColor;

        //         PlayerFace.mouthSPR.sprite = deadFace.Mouth;
        //         PlayerFace.mouthSPR.color = deadFace.MouthColor;
        //     }
        // }

        public void ChangeMaterial(Define.MaterialType changeMatType, Material mat, float resetDelay)
        {
            switch (changeMatType)
            {
                case Define.MaterialType.None:
                    IsChangingMaterial = false;
                    break;

                case Define.MaterialType.Hit:
                    Hit(mat, resetDelay);
                    break;

                case Define.MaterialType.Hologram:
                    Hologram(mat, resetDelay);
                    break;

                case Define.MaterialType.FadeOut:
                    FadeOut(mat, resetDelay);
                    break;
            }
        }

        public void Hit(Material hitMat, float resetDelay)
        {
            if (this.IsChangingMaterial == false)
            {
                IsChangingMaterial = true;
                ChangeMaterial(hitMat);
                StartCoroutine(CoReset(resetDelay));
            }
        }

        private void Hologram(Material hologramMat, float resetDelay)
        {
            if (this.IsChangingMaterial == false)
            {
                this.IsChangingMaterial = true;
                ChangeMaterial(hologramMat);
                StartCoroutine(CoHologram(resetDelay));
            }
        }

        private IEnumerator CoHologram(float resetDelay)
        {
            float percent = 0f;
            while (percent < 1f)
            {
                Managers.VFX.Mat_Hologram.SetFloat(Managers.VFX.SHADER_HOLOGRAM, percent);
                percent += Time.deltaTime * 20f;
                yield return null;
            }

            StartCoroutine(CoReset(resetDelay));

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

        private void FadeOut(Material fadeMat, float resetDelay)
        {
            ChangeMaterial(fadeMat); // *** PLAYER EYES SPRITE의 경우, FADE 전용 EYES로 교체 필요.
            StartCoroutine(CoFade(resetDelay));
        }

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

            _currentKeyGrade = keyGrade;
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
                            OwnerSPRs[i].sprite = nextSPRs[i].sprite;
                            OwnerSPRs[i].color = nextSPRs[i].color;
                        }
                    }
                }
            }
        }

        private bool IsPlayer => this.Owner?.IsPlayer() == true;
        private SpriteRenderer[] SpriteRenderers(Define.InGameGrade grade)
            => _moderatorDict.TryGetValue(this.Owner, out Moderator value) ? value.GetSpriteRenderers(grade) : null;
        private BaseContainer[] BaseContainers(Define.InGameGrade grade)
            => _moderatorDict.TryGetValue(this.Owner, out Moderator value) ? value.GetBaseContainers(grade) : null;

        private IEnumerator CoReset(float resetDelay)
        {
            yield return new WaitForSeconds(resetDelay);
            ResetMaterial();
        }

        public void ResetMaterial()
        {
            // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // BCs.Length > OwnerSPRs.Length or BCs.Length == OwnerSPRs.Length (No Error)
            // BCs.Length < OwnerSPRs.Length (Error)
            // 정확한 원인은 아직 파악중
            // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            BaseContainer[] BCs = this.BaseContainers(_currentKeyGrade);
            int legnth = Mathf.Min(BCs.Length, OwnerSPRs.Length);
            for (int i = 0; i < legnth; ++i)
                OwnerSPRs[i].material = BCs[i].MatOrigin;
            if (this.IsPlayer)
            {
                PlayerEyesSPR.sprite = PlayerEyesSprite;
            }

            // for (int i = 0; i < OwnerSPRs.Length; ++i)
            //     OwnerSPRs[i].material = BCs[i].MatOrigin;
            // if (this.IsPlayer)
            //     PlayerEyesSPR.sprite = PlayerEyesSprite;

            IsChangingMaterial = false;
        }

        private void ChangeMaterial(Material mat)
        {
            if (this.IsPlayer)
                PlayerEyesSPR.sprite = null;

            for (int i = 0; i < OwnerSPRs.Length; ++i)
            {
                if (OwnerSPRs[i].sprite != null)
                    OwnerSPRs[i].material = mat;
            }
        }

        private void OnDestroy()
        {
            // TODO : RESET
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using STELLAREST_2D.Data;
using UnityEditor.Rendering;
using UnityEngine;

namespace STELLAREST_2D
{
    [System.Serializable]
    public class FaceExpressionLoader
    {
        public Define.FaceExpressionType FaceExpressionType;
        public string EyebrowsPrimaryKey;
        public string EyebrowsColorCode;

        public string EyesPrimaryKey;
        public string EyesColorCode;

        public string MouthPrimaryKey;
        public string MouthColorCode;
    }

    public class FaceExpression
    {
        public Sprite Eyebrows;
        public Color EyebrowsColor;

        public Sprite Eyes;
        public Color EyesColor;

        public Sprite Mouth;
        public Color MouthColor;
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
        public SpriteRenderer[] OwnerSPRs { get; private set; } = null;
        private Dictionary<CreatureController, Moderator> _moderatorDict = null;
        public bool IsChangingMaterial { get; private set; } = false;
        private Define.InGameGrade _currentKeyGrade = Define.InGameGrade.Default;

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

        public Dictionary<Define.FaceExpressionType, FaceExpression> FaceExpressionDict { get; private set; } 
                                                    = new Dictionary<Define.FaceExpressionType, FaceExpression>();
        
        public void InitRendererController(CreatureController owner, InitialCreatureData initialCreatureData)
        {
            if (this.Owner != null || this._moderatorDict != null)
                return;

            // INIT CURRENT
            this.Owner = owner;
            OwnerSPRs = owner.SPRs;

            if (this.IsPlayer)
                InitOwnerEyes();

            /*
                    public string EyesPrimaryKey;
                    public string EyesColorCode;

                    public string MouthPrimaryKey;
                    public string MouthColorCode;
            */

            // Color color = ColorUtility.HexToColor("#FFFFFF");

            FaceExpressionLoader[] loaders = new FaceExpressionLoader[initialCreatureData.FaceExpressionsLoader.Length];
            for (int i = 0; i < initialCreatureData.FaceExpressionsLoader.Length; ++i)
            {
                FaceExpression faceExpression = new FaceExpression();
                
                // Eyebrows
                faceExpression.Eyebrows = Managers.Resource.Load<Sprite>(initialCreatureData.FaceExpressionsLoader[i].EyebrowsPrimaryKey);
                if (ColorUtility.TryParseHtmlString(initialCreatureData.FaceExpressionsLoader[i].EyebrowsColorCode, out Color eyeBrowsColor))
                    faceExpression.EyebrowsColor = eyeBrowsColor;
                else
                    Utils.LogStrong(nameof(RendererController), nameof(InitRendererController), $"Failed to set eyebrowsColor.");


                // Eyes
                faceExpression.Eyes = Managers.Resource.Load<Sprite>(initialCreatureData.FaceExpressionsLoader[i].EyesPrimaryKey);
                if (ColorUtility.TryParseHtmlString(initialCreatureData.FaceExpressionsLoader[i].EyesColorCode, out Color eyesColor))
                    faceExpression.EyesColor = eyesColor;
                else
                    Utils.LogStrong(nameof(RendererController), nameof(InitRendererController), $"Failed to set eyesColor.");


                // Mouth
                faceExpression.Mouth = Managers.Resource.Load<Sprite>(initialCreatureData.FaceExpressionsLoader[i].MouthPrimaryKey);
                if (ColorUtility.TryParseHtmlString(initialCreatureData.FaceExpressionsLoader[i].MouthColorCode, out Color mouthColor))
                    faceExpression.MouthColor = mouthColor;
                else
                    Utils.LogStrong(nameof(RendererController), nameof(InitRendererController), $"Failed to set mouthColor.");

                FaceExpressionDict.Add(initialCreatureData.FaceExpressionsLoader[i].FaceExpressionType, faceExpression);
            }


            // CHECK 
            foreach (KeyValuePair<Define.FaceExpressionType, FaceExpression> pair in FaceExpressionDict)
            {
                Utils.Log($"Key : {pair.Key}");
                
                Utils.Log($"Eyebrows : {pair.Value.Eyebrows}");
                Utils.Log($"EyebrowsColor : {pair.Value.EyebrowsColor}");

                Utils.Log($"Eyes : {pair.Value.Eyes}");
                Utils.Log($"EyesColor : {pair.Value.EyesColor}");

                Utils.Log($"Mouth : {pair.Value.Mouth}");
                Utils.Log($"MouthColor : {pair.Value.MouthColor}");
            }

            Utils.LogBreak("BREAK");

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

        private void InitOwnerEyes()
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
                    DoHologram(mat, resetDelay);
                    break;
            }
        }

        public void Hit(Material mat, float resetDelay)
        {
            if (this.IsChangingMaterial == false)
            {
                IsChangingMaterial = true;

                if (this.IsPlayer)
                    PlayerEyesSPR.sprite = null;

                for (int i = 0; i < OwnerSPRs.Length; ++i)
                {
                    if (OwnerSPRs[i].sprite != null)
                        OwnerSPRs[i].material = mat;
                }

                StartCoroutine(ResetDelay(resetDelay));
            }
        }

        private IEnumerator ResetDelay(float resetDelay)
        {
            yield return new WaitForSeconds(resetDelay);
            ResetMaterial();
            IsChangingMaterial = false;
        }

        private void ResetMaterial()
        {
            BaseContainer[] BCs = this.BaseContainers(_currentKeyGrade);
            for (int i = 0; i < OwnerSPRs.Length; ++i)
                OwnerSPRs[i].material = BCs[i].MatOrigin;
            if (this.IsPlayer)
                PlayerEyesSPR.sprite = PlayerEyesSprite;
        }

        private void DoHologram(Material mat, float resetDelay)
        {
            if (this.IsChangingMaterial == false)
            {
                this.IsChangingMaterial = true;

                if (this.IsPlayer)
                    PlayerEyesSPR.sprite = null;

                for (int i = 0; i < OwnerSPRs.Length; ++i)
                {
                    if (OwnerSPRs[i].sprite != null)
                        OwnerSPRs[i].material = mat;
                }

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

            percent = 1f;
            float elapsedTime = 0f;
            while (percent > 0f)
            {
                Managers.VFX.Mat_Hologram.SetFloat(Managers.VFX.SHADER_HOLOGRAM, percent);
                elapsedTime += Time.deltaTime;
                percent = 1f - (elapsedTime * 20f);
                yield return null;
            }

            StartCoroutine(ResetDelay(resetDelay));
            //ResetDelay(resetDelay);
            // BaseContainer[] BCs = this.BaseContainers(_currentKeyGrade);
            // for (int i = 0; i < OwnerSPRs.Length; ++i)
            //     OwnerSPRs[i].material = BCs[i].MatOrigin;
            // if (this.IsPlayer)
            //     PlayerEyesSPR.sprite = PlayerEyesSprite;
            // IsChangingMaterial = false;
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

        private void OnDestroy()
        {
            // TODO : RESET
        }
    }
}

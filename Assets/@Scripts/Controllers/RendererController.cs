using System.Collections;
using System.Collections.Generic;
using System.Linq;
using STELLAREST_2D.Data;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.Rendering;
using UnityEngine;

namespace STELLAREST_2D
{
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

        // TODO : OwnerSPRs[i].gameObject.name.Contains("Eyes") : 이 부분 그냥 처음부터 초기화할 때 캐싱해둬야할 것 같음.
        // Player Eyes Sprite Renderer 미리 캐싱해야함
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

        
        public void InitRendererController(CreatureController owner, InitialCreatureData initialCreatureData)
        {
            if (this.Owner != null || this._moderatorDict != null)
                return;

            // INIT CURRENT
            this.Owner = owner;
            OwnerSPRs = owner.SPRs;

            _moderatorDict = new Dictionary<CreatureController, Moderator>();
            Moderator moderator = new Moderator();

            GameObject go = Managers.Resource.Load<GameObject>(initialCreatureData.PrimaryLabel);
            SpriteRenderer[] SPRs = go.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
            
            BaseContainer[] BCs = new BaseContainer[SPRs.Length];
            for (int i = 0; i < SPRs.Length; ++i)
            {
                if (this.IsPlayerEyes(SPRs[i]))
                    this._playerEyesSprite = SPRs[i].sprite;

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

        // TEMP
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

        private void DoHologram(Material mat, float resetDelay)
        {
            if (this.IsChangingMaterial == false)
            {
                this.IsChangingMaterial = true;
                for (int i = 0; i < OwnerSPRs.Length; ++i)
                {
                    if (this.IsPlayerEyes(OwnerSPRs[i]))
                        OwnerSPRs[i].sprite = null;

                    if (OwnerSPRs[i].sprite != null)
                        OwnerSPRs[i].material = mat;
                }

                StartCoroutine(CoHologram(resetDelay));
                IsChangingMaterial = false;
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

            BaseContainer[] BCs = this.BaseContainers(_currentKeyGrade);
            for (int i = 0; i < OwnerSPRs.Length; ++i)
            {
                if (this.IsPlayerEyes(OwnerSPRs[i]))
                    OwnerSPRs[i].sprite = PlayerEyesSprite;

                OwnerSPRs[i].material = BCs[i].MatOrigin;
            }
        }

        public void Hit(Material mat, float resetDelay)
        {
            if (this.IsChangingMaterial == false)
            {
                IsChangingMaterial = true;
                for (int i = 0; i < OwnerSPRs.Length; ++i)
                {
                    if (this.IsPlayerEyes(OwnerSPRs[i]))
                        OwnerSPRs[i].sprite = null;

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

        public void ResetMaterial()
        {
            BaseContainer[] BCs = this.BaseContainers(_currentKeyGrade);
            for (int i = 0; i < OwnerSPRs.Length; ++i)
            {
                if (this.IsPlayerEyes(OwnerSPRs[i]))
                    OwnerSPRs[i].sprite = PlayerEyesSprite;

                OwnerSPRs[i].material = BCs[i].MatOrigin;
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

        // public IEnumerator CoHit(Material matHit, float duration)
        // {
        //     float delta = 0f;
        //     // IsChangingMaterial = true;
        //     // SpriteRenderer[] SPRs = BaseRendererContainer.SpriteRenderers;
        //     // for (int i = 0; i < SPRs.Length; ++i)
        //     // {
        //     //     if (SPRs[i].sprite != null)
        //     //         SPRs[i].material = matHit;
        //     // }

        //     // float percent = 0f;
        //     // while (percent < 1f)
        //     // {
        //     //     delta += Time.deltaTime;
        //     //     percent = delta / duration;
        //     //     yield return null;
        //     // }

        //     yield return null;
        // }

        private bool IsPlayer => this.Owner?.IsPlayer() == true;
        private bool IsPlayerEyes(SpriteRenderer spr) => IsPlayer && spr.gameObject.name.Contains("Eyes");

        private SpriteRenderer[] SpriteRenderers(Define.InGameGrade grade)
            => _moderatorDict.TryGetValue(this.Owner, out Moderator value) ? value.GetSpriteRenderers(grade) : null;

        private BaseContainer[] BaseContainers(Define.InGameGrade grade)
            => _moderatorDict.TryGetValue(this.Owner, out Moderator value) ? value.GetBaseContainers(grade) : null;

        private void OnDestroy()
        {
        }
    }
}

// -------------------------------------------------------------------------------------
// -------------------------------------------------------------------------------------
// public void Reset_TEMP_ON_DESTROY()
// {
//     // if (this.Owner != null)
//     // {
//     //     SpriteRenderer[] SPRs = OwnerSPRs;
//     //     BaseContainer[] BCs = _baseRendererContainer.BaseContainers;
//     //     for (int i = 0; i < SPRs.Length; ++i)
//     //     {
//     //         SPRs[i].material = BCs[i].MatOrigin;
//     //         SPRs[i].color = BCs[i].ColorOrigin;
//     //     }
//     // }
// }

// private SpriteRenderer IsValid(CreatureController owner, SpriteRenderer spriteRenderer)
// {
//     if (owner?.IsPlayer() == true)
//     {
//         if (spriteRenderer.sprite == null)
//             return null;

//         if (spriteRenderer.gameObject.name.Contains(Define.FIRE_SOCKET))
//             return null;

//         return spriteRenderer;
//     }
//     else
//     {
//         /* DO SOMETHING : WHEN IS MONSTER or ENV */
//         return spriteRenderer;
//     }
// }
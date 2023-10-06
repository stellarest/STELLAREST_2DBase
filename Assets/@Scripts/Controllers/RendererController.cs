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

        public void ChangeMaterial(Material mat, float resetDelay)
        {
            Sprite eyesOrigin = null;
            if (this.IsChangingMaterial == false)
            {
                IsChangingMaterial = true;
                for (int i = 0; i < OwnerSPRs.Length; ++i)
                {
                    // TEMP
                    if (OwnerSPRs[i].gameObject.name.Contains("Eyes"))
                    {
                        eyesOrigin = OwnerSPRs[i].sprite;
                        OwnerSPRs[i].sprite = null;
                    }

                    if (OwnerSPRs[i].sprite != null)
                        OwnerSPRs[i].material = mat;
                }

                StartCoroutine(ResetDelay(resetDelay, eyesOrigin));
            }
        }

        private IEnumerator ResetDelay(float resetDelay, Sprite eyesSprite)
        {
            yield return new WaitForSeconds(resetDelay);
            ResetMaterial(eyesSprite);
            IsChangingMaterial = false;
        }

        public void ResetMaterial(Sprite eyesOrigin)
        {
            BaseContainer[] BCs = this.BaseContainers(_currentKeyGrade);

            for (int i = 0; i < OwnerSPRs.Length; ++i)
            {
                if (OwnerSPRs[i].gameObject.name.Contains("Eyes"))
                    OwnerSPRs[i].sprite = eyesOrigin;

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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace STELLAREST_2D
{
    /*
        _ownerRendererContainer : Current Sprite Renderers
        GetSpriteRenderers(Moderator) : Next Sprite Renderers
    */

    public class RendererController : MonoBehaviour
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

        // +++++ Renderer Container +++++
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

        // +++++ Moderator +++++
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

        public CreatureController Owner { get; private set; } = null;
        private RendererContainer _ownerRendererContainer = null;
        private SpriteRenderer[] OwnerPreCachedSpriteRenderers => _ownerRendererContainer.SpriteRenderers;
        private void InitOwnerContainer()
        {
            SpriteRenderer[] currentSPRs = Owner.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
            BaseContainer[] containers = new BaseContainer[currentSPRs.Length];
            for (int i = 0; i < currentSPRs.Length; ++i)
            {
                Material matOrigin = new Material(currentSPRs[i].sharedMaterial);
                Material clonedMatOrigin = new Material(matOrigin);
                Color colorOrigin = currentSPRs[i].color;
                containers[i] = new BaseContainer(currentSPRs[i].gameObject.name, clonedMatOrigin, colorOrigin);
            }

            _ownerRendererContainer = new RendererContainer();
            _ownerRendererContainer.SpriteRenderers = currentSPRs;
            _ownerRendererContainer.BaseContainers = containers;
        }

        // 외계인
        private Dictionary<CreatureController, Moderator> _moderatorDict = null;
        public void InitRendererController(CreatureController owner)
        {
            if (Owner != null)
                return;

            if (_moderatorDict != null)
                return;

            if (_ownerRendererContainer != null)
                return;

            this.Owner = owner;
            _moderatorDict = new Dictionary<CreatureController, Moderator>();
            Moderator moderator = new Moderator();

            List<SkillBase> skillList = new List<SkillBase>();
            foreach (KeyValuePair<int, SkillGroup> pair in owner.SkillBook.SkillGroupsDict)
            {
                for (int i = 0; i < pair.Value.MemberCount; ++i)
                    skillList.Add(pair.Value.Members[i].SkillOrigin);
            }

            SkillBase[] skills = skillList.Where(s => s.Data.ModelingLabel.Length > 0).ToArray();
            for (int i = 0; i < skills.Length; ++i)
            {
                Define.InGameGrade grade = skills[i].Data.Grade;
                string modelingLabel = skills[i].Data.ModelingLabel;
                GameObject go = Managers.Resource.Load<GameObject>(modelingLabel);
                if (go == null)
                    Utils.LogCritical(nameof(RendererContainer), nameof(InitRendererController), $"Check Modeling Label : {modelingLabel}");

                SpriteRenderer[] SPRs = go.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
                BaseContainer[] containers = new BaseContainer[SPRs.Length];
                for (int j = 0; j < SPRs.Length; ++j)
                {
                    Material clonedMatOrigin = new Material(SPRs[j].sharedMaterial);
                    Color colorOrigin = SPRs[j].color;
                    containers[j] = new BaseContainer(SPRs[j].gameObject.name, clonedMatOrigin, colorOrigin);
                }

                moderator.AddRendererContainers(grade, containers, SPRs);
            }

            if (moderator.RendererContainerDict.Count > 0)
                _moderatorDict.Add(owner, moderator);

            InitOwnerContainer();
        }

        public void Upgrade(Define.InGameGrade next)
        {
            SpriteRenderer[] ownerSPRs = OwnerPreCachedSpriteRenderers;
            for (int i = 0; i < ownerSPRs.Length; ++i)
                ownerSPRs[i].sprite = null;

            SpriteRenderer[] nextSPRs = SpriteRenderers(next);
            int length = Mathf.Max(ownerSPRs.Length, nextSPRs.Length);
            for (int i = 0; i < length; ++i)
            {
                if ((i < ownerSPRs.Length) && (i < nextSPRs.Length))
                {
                    if (nextSPRs[i].gameObject.name.Contains(ownerSPRs[i].name))
                    {
                        // nextSPRs[i].sprite가 갖고 있는것만 집어 넣어 넣으면 해결
                        if (nextSPRs[i].sprite != null)
                        {
                            // CHECK : nextSPRs[i].gameObject.activeSelf : Sprite를 들고있는데 꺼져있을수도 있어서
                            ownerSPRs[i].gameObject.SetActive(nextSPRs[i].gameObject.activeSelf);
                            ownerSPRs[i].sprite = nextSPRs[i].sprite;
                            ownerSPRs[i].color = nextSPRs[i].color;
                        }
                    }
                }
            }
        }

        private SpriteRenderer[] SpriteRenderers(Define.InGameGrade grade)
            => _moderatorDict.TryGetValue(this.Owner, out Moderator value) ? value.GetSpriteRenderers(grade) : null;

        private BaseContainer[] BaseContainers(Define.InGameGrade grade)
            => _moderatorDict.TryGetValue(this.Owner, out Moderator value) ? value.GetBaseContainers(grade) : null;

        private SpriteRenderer[] IsValidSpriteRenderers()
        {
            List<SpriteRenderer> lst = new List<SpriteRenderer>();
            SpriteRenderer[] SPRs = _ownerRendererContainer.SpriteRenderers;
            for (int i = 0; i < SPRs.Length; ++i)
                lst.Add(IsValid(this.Owner, SPRs[i]));

            return lst.ToArray();
        }

        private SpriteRenderer IsValid(CreatureController owner, SpriteRenderer spriteRenderer)
        {
            if (owner?.IsPlayer() == true)
            {
                if (spriteRenderer.sprite == null)
                    return null;

                if (spriteRenderer.gameObject.name.Contains(Define.FIRE_SOCKET))
                    return null;

                return spriteRenderer;
            }
            else
            {
                /* DO SOMETHING : WHEN IS MONSTER or ENV */
                return spriteRenderer; 
            }
        }

        public void Reset()
        {
            if (this.Owner != null)
            {
                SpriteRenderer[] SPRs = _ownerRendererContainer.SpriteRenderers;
                BaseContainer[] BCs = _ownerRendererContainer.BaseContainers;
                for (int i = 0; i < SPRs.Length; ++i)
                {
                    SPRs[i].material = BCs[i].MatOrigin;
                    SPRs[i].color = BCs[i].ColorOrigin;
                }
            }
        }

        private void OnDestroy() => Reset();
    }
}
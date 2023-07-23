using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

namespace STELLAREST_2D
{
    public struct CreatureMaterial
    {
        public CreatureMaterial(SpriteRenderer spriteRenderer, Material matOrigin, Color colorOrigin)
        {
            this.spriteRender = spriteRenderer;
            this.matOrigin = matOrigin;
            this.colorOrigin = colorOrigin;
        }
        public SpriteRenderer spriteRender;
        public Material matOrigin;
        public Color colorOrigin; // color 수정이 필요할 경우
    }

    public class EffectManager
    {
        private Material _matHitWhite;
        private Material _matHitRed;
        private Material _matFade;
        private Material _matGlitch;

        private GameObject _upgradePlayerBuffEffect;
        private int SHADER_HIT_EFFECT = Shader.PropertyToID("_StrongTintFade");
        private int SHADER_FADE_EFFECT = Shader.PropertyToID("_CustomFadeAlpha");
        private int SHADER_GLITCH = Shader.PropertyToID("_GlitchFade");

        private Dictionary<int, CreatureMaterial[]> _creatureMats = new Dictionary<int, CreatureMaterial[]>();

        public void Init()
        {
            _matHitWhite = Managers.Resource.Load<Material>(Define.MaterialLabels.MAT_HIT_WHITE);
            _matHitRed = Managers.Resource.Load<Material>(Define.MaterialLabels.MAT_HIT_RED);
            _matFade = Managers.Resource.Load<Material>(Define.MaterialLabels.MAT_FADE);
            _matGlitch = Managers.Resource.Load<Material>(Define.MaterialLabels.MAT_GLITCH);

            if (_upgradePlayerBuffEffect == null)
                _upgradePlayerBuffEffect = Utils.FindChild(Managers.Game.Player.gameObject, Define.PlayerController.UPGRADE_PLAYER_BUFF);
        }

        public void ChangeCreatureMaterials(CreatureController cc)
        {
            _creatureMats.Remove(cc.CharaData.TemplateID);
            AddCreatureMaterials(cc);
        }

        public void AddCreatureMaterials(CreatureController cc)
        {
            int length = 0;
            if (cc?.IsMonster() == false)
            {
                SpriteRenderer[] sprArr = cc.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
                for (int i = 0; i < sprArr.Length; ++i)
                {
                    if (sprArr[i].gameObject.activeInHierarchy == false)
                        continue;

                    if (sprArr[i].sprite == null)
                        continue;

                    if (sprArr[i].gameObject.name.Contains(Define.PlayerController.FIRE_SOCKET))
                        continue;

                    ++length;
                }
                CreatureMaterial[] playerMats = new CreatureMaterial[length];

                int index = 0;
                for (int i = 0; i < sprArr.Length; ++i)
                {
                    if (sprArr[i].gameObject.activeInHierarchy == false)
                        continue;

                    if (sprArr[i].sprite == null)
                        continue;

                    if (sprArr[i].gameObject.name.Contains(Define.PlayerController.FIRE_SOCKET))
                        continue;

                    playerMats[index++] = new CreatureMaterial(sprArr[i], sprArr[i].material, sprArr[i].color);
                }

                _creatureMats.Add(cc.CharaData.TemplateID, playerMats);
            }
            else
            {
                SpriteRenderer[] sprArr = cc.GetComponentsInChildren<SpriteRenderer>();
                CreatureMaterial[] creatureMats = new CreatureMaterial[sprArr.Length];
                for (int i = 0; i < sprArr.Length; ++i)
                    creatureMats[i] = new CreatureMaterial(sprArr[i], sprArr[i].material, sprArr[i].color);

                _creatureMats.Add(cc.CharaData.TemplateID, creatureMats);
            }
        }

        public IEnumerator CoEffectFade(CreatureController cc, bool isFadingOut = false)
        {
            CreatureMaterial[] mats = _creatureMats[cc.CharaData.CreatureData.TemplateID];

            for (int i = 0; i < mats.Length; ++i)
                mats[i].spriteRender.material = _matFade;

            float elapsedTime = 0f;
            float desiredTime = 2f;
            float percent = 0f;
            if (isFadingOut == false)
            {
                while (percent < 1f)
                {
                    _matFade.SetFloat(SHADER_FADE_EFFECT, percent);
                    elapsedTime += Time.deltaTime;
                    percent = elapsedTime / desiredTime;
                    yield return null;
                }
            }
            else
            {
                percent = 1f;
                while (percent > 0f)
                {
                    _matFade.SetFloat(SHADER_FADE_EFFECT, percent);
                    elapsedTime += Time.deltaTime;
                    percent = 1f - (elapsedTime / desiredTime);
                    yield return null;
                }

                yield break;
            }

            for (int i = 0; i < mats.Length; ++i)
                mats[i].spriteRender.material = mats[i].matOrigin;

            // percent = 0f;
            // elapsedTime = 0f;
            // while (percent < 1f)
            // {
            //     elapsedTime += Time.deltaTime;
            //     percent = elapsedTime / desiredTime;
            //     yield return null;
            // }
        }

        public bool IsPlayingGlitch { get; private set; } = false;
        public IEnumerator CoEffectGlitch(CreatureController cc)
        {
            IsPlayingGlitch = true;
            CreatureMaterial[] mats = _creatureMats[cc.CharaData.TemplateID];

            float duration = _upgradePlayerBuffEffect.GetComponent<ParticleSystem>().main.duration;

            if (cc?.IsMonster() == false)
                Managers.Sprite.SetPlayerEmotion(Define.PlayerEmotion.Greedy);

            for (int i = 0; i < mats.Length; ++i)
                mats[i].spriteRender.material = _matGlitch;

            float delta = 0f;
            float desiredTime = duration;
            float percent = 1f;
            while (percent > 0f)
            {
                _matGlitch.SetFloat(SHADER_GLITCH, percent);
                delta += Time.deltaTime;
                percent = 1f - (delta / desiredTime);
                yield return null;
            }

            for (int i = 0; i < mats.Length; ++i)
                mats[i].spriteRender.material = mats[i].matOrigin;

            percent = 0f;
            delta = 0f;
            while (percent < 1f)
            {
                delta += Time.deltaTime;
                percent = delta / desiredTime;
                yield return null;
            }

            IsPlayingGlitch = false;
        }

        public void UpgradePlayerBuffEffect() => _upgradePlayerBuffEffect.SetActive(true);
        public void StartHitEffect(CreatureController cc)
        {
            CreatureMaterial[] mats = _creatureMats[cc.CharaData.TemplateID];
            if (cc?.IsMonster() == false)
                Managers.Sprite.SetPlayerEmotion(Define.PlayerEmotion.None);

            for (int i = 0; i < mats.Length; ++i)
            {
                if (cc?.IsMonster() == false)
                    mats[i].spriteRender.material = _matHitRed;
                else
                    mats[i].spriteRender.material = _matHitWhite;

                mats[i].spriteRender.material.SetFloat(SHADER_HIT_EFFECT, 1);
            }
        }

        public void EndHitEffect(CreatureController cc)
        {
            CreatureMaterial[] mats = _creatureMats[cc.CharaData.TemplateID];
            for (int i = 0; i < mats.Length; ++i)
                mats[i].spriteRender.material = mats[i].matOrigin;

            if (cc?.IsMonster() == false)
                Managers.Sprite.SetPlayerEmotion(Define.PlayerEmotion.Default);
        }

        public void ShowDamageFont(CreatureController cc, float damage, bool isCritical = false)
        {
            if (cc.IsValid() == false)
                return;

            Vector3 defaultSpawnPos = cc.transform.position + (Vector3.up * 2.5f);
            if (cc?.IsMonster() == false)
            {
                Managers.Resource.Load<GameObject>
                        (Define.PrefabLabels.DMG_NUMBER_TO_PLAYER).GetComponent<DamageNumber>().Spawn(defaultSpawnPos, damage);
            }
            else
            {
                if (cc.CharaData.TemplateID == (int)Define.TemplateIDs.Monster.Chicken)
                    defaultSpawnPos -= Vector3.up;

                if (isCritical)
                {
                    Managers.Resource.Load<GameObject>
                        (Define.PrefabLabels.DMG_NUMBER_TO_MONSTER_CRITICAL).GetComponent<DamageNumber>().Spawn(defaultSpawnPos, damage);

                    Managers.Resource.Load<GameObject>
                        (Define.PrefabLabels.DMG_TEXT_TO_MONSTER_CRITICAL).GetComponent<DamageNumber>().Spawn(defaultSpawnPos);
                }
                else
                {
                    Managers.Resource.Load<GameObject>
                        (Define.PrefabLabels.DMG_NUMBER_TO_MONSTER).GetComponent<DamageNumber>().Spawn(defaultSpawnPos, damage);
                }
            }
        }

        public void ShowDodgeText(CreatureController cc)
        {
            if (cc.IsValid() == false)
                return;

            Vector3 defaultSpawnPos = cc.transform.position + (Vector3.up * 3f);

            Managers.Resource.Load<GameObject>
                 (Define.PrefabLabels.DMG_TEXT_TO_PLAYER_DODGE).GetComponent<DamageNumber>().Spawn(defaultSpawnPos);
        }

        public void ShowEffectText(string prefabLabel, Vector3 pos, string text)
        {
            GameObject prefab = Managers.Resource.Load<GameObject>(prefabLabel);
            DamageNumber dmgFont = prefab.GetComponent<DamageNumber>();
            dmgFont.Spawn(pos, text);
        }
    }
}

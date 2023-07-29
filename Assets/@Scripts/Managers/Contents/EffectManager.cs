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
        private Material _matHologram;

        private GameObject _upgradePlayerBuffEffect;
        private int SHADER_HIT_EFFECT = Shader.PropertyToID("_StrongTintFade");
        private int SHADER_FADE_EFFECT = Shader.PropertyToID("_CustomFadeAlpha");
        private int SHADER_GLITCH = Shader.PropertyToID("_GlitchFade");
        private int SHADER_HOLOGRAM = Shader.PropertyToID("_HologramFade");

        private Dictionary<GameObject, CreatureMaterial[]> _creatureMats = new Dictionary<GameObject, CreatureMaterial[]>();

        public void Init()
        {
            _matHitWhite = Managers.Resource.Load<Material>(Define.MaterialLabels.MAT_HIT_WHITE);
            _matHitRed = Managers.Resource.Load<Material>(Define.MaterialLabels.MAT_HIT_RED);
            _matFade = Managers.Resource.Load<Material>(Define.MaterialLabels.MAT_FADE);
            _matGlitch = Managers.Resource.Load<Material>(Define.MaterialLabels.MAT_GLITCH);
            _matHologram = Managers.Resource.Load<Material>(Define.MaterialLabels.MAT_HOLOGRAM);

            if (_upgradePlayerBuffEffect == null)
                _upgradePlayerBuffEffect = Utils.FindChild(Managers.Game.Player.gameObject, Define.PlayerController.UPGRADE_PLAYER_BUFF);
        }

        public void ChangeCreatureMaterials(CreatureController cc)
        {
            _creatureMats.Remove(cc.gameObject);
            AddCreatureMaterials(cc);
        }

        public void SetDefaultMaterials(CreatureController cc)
        {
            if (_creatureMats.TryGetValue(cc.gameObject, out CreatureMaterial[] mats))
            {
                for (int i = 0; i < mats.Length; ++i)
                    mats[i].spriteRender.material = mats[i].matOrigin;
            }
            else
                Utils.LogError("Invalid CreatureMats !!");
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

                //_creatureMats.Add(cc.CharaData.TemplateID, playerMats);
                _creatureMats.Add(cc.gameObject, playerMats);
            }
            else
            {
                SpriteRenderer[] sprArr = cc.GetComponentsInChildren<SpriteRenderer>();
                CreatureMaterial[] creatureMats = new CreatureMaterial[sprArr.Length];
                for (int i = 0; i < sprArr.Length; ++i)
                    creatureMats[i] = new CreatureMaterial(sprArr[i], sprArr[i].material, sprArr[i].color);

                // if (_creatureMats.TryGetValue(cc.CharaData.TemplateID, out CreatureMaterial[] value) == false)
                //     _creatureMats.Add(cc.CharaData.TemplateID, creatureMats);

                if (_creatureMats.TryGetValue(cc.gameObject, out CreatureMaterial[] value) == false)
                    _creatureMats.Add(cc.gameObject, creatureMats);
            }
        }

        public IEnumerator CoHologram(CreatureController cc)
        {
            CreatureMaterial[] mats = _creatureMats[cc.gameObject];

            if (cc?.IsMonster() == false)
                Managers.Sprite.SetPlayerEmotion(Define.PlayerEmotion.None);

            for (int i = 0; i < mats.Length; ++i)
                mats[i].spriteRender.material = _matHologram;

            float elapsedTime = 0f;
            float percent = 0f;
            while (percent < 1f) // 디폴트 -> 홀로그램(0 -> 1)
            {
                _matHologram.SetFloat(SHADER_HOLOGRAM, percent);
                percent += Time.deltaTime * 20f;
                yield return null;
            }

            percent = 1f;
            elapsedTime = 0f;
            while (percent > 0f) // 홀로그램 만빵(1f) -> 디폴트
            {
                _matHologram.SetFloat(SHADER_HOLOGRAM, percent);
                elapsedTime += Time.deltaTime;
                percent = 1f - (elapsedTime * 20f);
                yield return null;
            }

            if (cc?.IsMonster() == false)
                Managers.Sprite.SetPlayerEmotion(Define.PlayerEmotion.Default);

            for (int i = 0; i < mats.Length; ++i)
                mats[i].spriteRender.material = mats[i].matOrigin;
        }

        public IEnumerator CoFadeOut(CreatureController cc, float startTime = 0f, float desiredTime = 2f, bool onDespawn = true)
        {
            float elapsedTime = 0f;
            while (elapsedTime < startTime)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            //CreatureMaterial[] mats = _creatureMats[cc.CharaData.CreatureData.TemplateID];
            CreatureMaterial[] mats = _creatureMats[cc.gameObject];
            for (int i = 0; i < mats.Length; ++i)
                mats[i].spriteRender.material = _matFade;

            elapsedTime = 0f;
            float percent = 1f;
            while (percent > 0f)
            {
                _matFade.SetFloat(SHADER_FADE_EFFECT, percent);
                elapsedTime += Time.deltaTime;
                percent = 1f - (elapsedTime / desiredTime);
                yield return null;
            }

            if (onDespawn)
                Managers.Object.Despawn(cc as MonsterController);
        }

        public IEnumerator CoFadeIn(CreatureController cc, float desiredTime)
        {
            //CreatureMaterial[] mats = _creatureMats[cc.CharaData.CreatureData.TemplateID];
            CreatureMaterial[] mats = _creatureMats[cc.gameObject];
            for (int i = 0; i < mats.Length; ++i)
                mats[i].spriteRender.material = _matFade;

            float elapsedTime = 0f;
            float percent = 0f;
            while (percent < 1f)
            {
                _matFade.SetFloat(SHADER_FADE_EFFECT, percent);
                elapsedTime += Time.deltaTime;
                percent = elapsedTime / desiredTime;
                yield return null;
            }

            for (int i = 0; i < mats.Length; ++i)
                mats[i].spriteRender.material = mats[i].matOrigin;
        }

        public IEnumerator CoFadeInAndOut(CreatureController cc, float desiredTime)
        {
            yield return null;
        }

        public bool IsPlayingGlitch { get; private set; } = false;
        public IEnumerator CoEffectGlitch(CreatureController cc)
        {
            IsPlayingGlitch = true;
            //CreatureMaterial[] mats = _creatureMats[cc.CharaData.TemplateID];
            CreatureMaterial[] mats = _creatureMats[cc.gameObject];

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
            //CreatureMaterial[] mats = _creatureMats[cc.CharaData.TemplateID];
            CreatureMaterial[] mats = _creatureMats[cc.gameObject];

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
            // CreatureMaterial[] mats = _creatureMats[cc.CharaData.TemplateID];
            CreatureMaterial[] mats = _creatureMats[cc.gameObject];

            for (int i = 0; i < mats.Length; ++i)
                mats[i].spriteRender.material = mats[i].matOrigin;

            if (cc?.IsMonster() == false)
                Managers.Sprite.SetPlayerEmotion(Define.PlayerEmotion.Default);
        }

        public void ShowSpawnEffect(string effectLabel, Vector3 position)
        {
            GameObject go = Managers.Resource.Instantiate(effectLabel, pooling: true);
            go.transform.position = position;
        }

        public void ShowPlayerDust()
        {
            GameObject go = Managers.Resource.Instantiate(Define.PrefabLabels.DUST, pooling: true);
            go.transform.position = Managers.Game.Player.LegR.position + (Vector3.down * 0.35f);
        }

        public void ShowGemGather(GemController gc)
        {
            GameObject go = Managers.Resource.Instantiate(Define.PrefabLabels.GEM_GATHER, pooling: true);
            go.transform.position = gc.transform.position;
        }

        public void ShowGemExplosion(GemController gc)
        {
            GameObject go = null;
            if (gc.GemSize == GemSize.Normal)
                go = Managers.Resource.Instantiate(Define.PrefabLabels.GEM_EXPLOSION_NORMAL, pooling: true);
            else
                go = Managers.Resource.Instantiate(Define.PrefabLabels.GEM_EXPLOSION_LARGE, pooling: true);
            go.transform.position = gc.transform.position;
            // go.transform.position = Managers.Game.Player.transform.position;
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

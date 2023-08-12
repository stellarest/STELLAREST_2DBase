using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;
using UnityEngine.Rendering;
using UnityEngine.iOS;

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

    public struct EnvMaterial
    {
        public EnvMaterial(SpriteRenderer spriteRenderer, Material matOrigin)
        {
            this.spriteRenderer = spriteRenderer;
            this.matOrigin = matOrigin;
        }

        public SpriteRenderer spriteRenderer;
        public Material matOrigin;
    }

    public class EffectManager
    {
        private Material _matHitWhite;
        public Material MatHitWhite => _matHitWhite;

        private Material _matHitRed;
        public Material MatHitRed => _matHitRed;

        private Material _matFade;
        public Material MatFade => _matFade;

        private Material _matGlitch;
        public Material MatGlitch => _matGlitch;

        private Material _matHologram;
        public Material MatHologram => _matHologram;

        private Material _matStrongTintWhite;
        public Material MatStrongTintWhite => _matStrongTintWhite;
        public void SetStrongTintWhite(float value)
                => _matStrongTintWhite.SetFloat(SHADER_STRONG_TINT_WHITE, value);

        private GameObject _upgradePlayerBuffEffect;
        private int SHADER_HIT_EFFECT = Shader.PropertyToID("_StrongTintFade");
        private int SHADER_FADE_EFFECT = Shader.PropertyToID("_CustomFadeAlpha");
        private int SHADER_GLITCH = Shader.PropertyToID("_GlitchFade");
        private int SHADER_HOLOGRAM = Shader.PropertyToID("_HologramFade");
        private int SHADER_STRONG_TINT_WHITE = Shader.PropertyToID("_StrongTintFade");

        private Dictionary<GameObject, CreatureMaterial[]> _creatureMats = new Dictionary<GameObject, CreatureMaterial[]>();
        private Dictionary<GameObject, EnvMaterial> _envSimpleMats = new Dictionary<GameObject ,EnvMaterial>();

        public void Init()
        {
            _matHitWhite = Managers.Resource.Load<Material>(Define.MaterialLabels.MAT_HIT_WHITE);
            _matHitRed = Managers.Resource.Load<Material>(Define.MaterialLabels.MAT_HIT_RED);
            _matFade = Managers.Resource.Load<Material>(Define.MaterialLabels.MAT_FADE);
            _matGlitch = Managers.Resource.Load<Material>(Define.MaterialLabels.MAT_GLITCH);
            _matHologram = Managers.Resource.Load<Material>(Define.MaterialLabels.MAT_HOLOGRAM);
            _matStrongTintWhite = Managers.Resource.Load<Material>(Define.MaterialLabels.MAT_STRONG_TINT_WHITE);

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
                Debug.LogError("Invalid CreatureMats !!");
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

        public void AddEnvSimpleMaterial(GameObject go)
        {
            if (_envSimpleMats.TryGetValue(go, out EnvMaterial envMat))
                return;

            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            Material matOrigin = sr.material;
            EnvMaterial mat = new EnvMaterial(sr, matOrigin);
            _envSimpleMats.Add(go, mat);
        }

        // ++++++++++
        // SHADER_STRONG_TINT_WHITE 이런 SetShader까지 리셋을 할 필요가 없는것임
        // 코루틴에서 메터리얼 value를 바꾸는 것은 무조건 0 ~ 1이 지나면 끝나는 것이고
        // 메터리얼을 기존 오리진 메터리얼로 바꿔주면 끝나는 것임.
        public IEnumerator EnvSimpleMaterial_StrongTintWhite(GameObject go, float desiredTime = 1f, System.Action callback = null)
        {
            if (_envSimpleMats.TryGetValue(go, out EnvMaterial envMat) == false)
                yield break;

            envMat.spriteRenderer.material = MatStrongTintWhite;

            float percent = 0f;
            while (percent < 1f)
            {
                percent += Time.deltaTime / desiredTime;
                envMat.spriteRenderer.material.SetFloat(SHADER_STRONG_TINT_WHITE, percent);
                yield return null;
            }

            callback?.Invoke();
        }

        public void ResetEnvSimpleMaterial(GameObject go)
        {
            if (_envSimpleMats.TryGetValue(go, out EnvMaterial envMat) == false)
                return;

            envMat.spriteRenderer.material = envMat.matOrigin;
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

        public GameObject ShowStunEffect()
                => Managers.Resource.Instantiate(Define.PrefabLabels.STUN_EFFECT, pooling: true);

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

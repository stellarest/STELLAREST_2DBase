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
                _upgradePlayerBuffEffect = Utils.FindChild
                    (Managers.Game.Player.gameObject, Define.PlayerController.UPGRADE_PLAYER_BUFF);
        }

        public void AddCreatureMaterials(CreatureController cc, int templateID)
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

                _creatureMats.Add(templateID, playerMats);
            }
            else
            {
                SpriteRenderer[] sprArr = cc.GetComponentsInChildren<SpriteRenderer>();
                CreatureMaterial[] creatureMats = new CreatureMaterial[sprArr.Length];
                for (int i = 0; i < sprArr.Length; ++i)
                    creatureMats[i] = new CreatureMaterial(sprArr[i], sprArr[i].material, sprArr[i].color);

                _creatureMats.Add(templateID, creatureMats);
            }
        }

        private bool _startFade = false;
        public IEnumerator CoFadeEffect(CreatureController cc, int templateID)
        {
            _startFade = true;
            CreatureMaterial[] mats = _creatureMats[templateID];

            for (int i = 0; i < mats.Length; ++i)
                mats[i].spriteRender.material = _matFade;

            float elapsedTime = 0f;
            float desiredTime = 2f;
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

            percent = 0f;
            elapsedTime = 0f;
            while (percent < 1f)
            {
                elapsedTime += Time.deltaTime;
                percent = elapsedTime / desiredTime;
                yield return null;
            }

            _startFade = false;
        }

        private bool _startGlitch = false;
        public IEnumerator CoGlitchEffect(CreatureController cc, int templateID)
        {
            Utils.Log("TEMPLATE ID : " + templateID.ToString());
            
            _startGlitch = true;
            CreatureMaterial[] mats = _creatureMats[templateID];

            if (cc?.IsMonster() == false)
                Managers.Sprite.SetPlayerEmotion(Define.PlayerEmotion.Greedy);

            for (int i = 0; i < mats.Length; ++i)
                mats[i].spriteRender.material = _matGlitch;

            float delta = 0f;
            float desiredTime = 2f;
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

            _startGlitch = false;
        }

        public bool IsPlayingEffect => _startFade || _startGlitch;

        public void UpgradePlayerBuffEffect() => _upgradePlayerBuffEffect.SetActive(true);

        public void StartHitEffect(CreatureController cc)
        {
            CreatureMaterial[] mats = _creatureMats[cc.TemplateID];
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
            CreatureMaterial[] mats = _creatureMats[cc.TemplateID];
            for (int i = 0; i < mats.Length; ++i)
                mats[i].spriteRender.material = mats[i].matOrigin;
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
                if (cc.TemplateID == (int)Define.TemplateIDs.Monster.Chicken)
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

        public void ShowEffectText(string prefabLabel, Vector3 pos, string text)
        {
            GameObject prefab = Managers.Resource.Load<GameObject>(prefabLabel);
            DamageNumber dmgFont = prefab.GetComponent<DamageNumber>();
            dmgFont.Spawn(pos, text);
        }
    }
}

//  public void SetInitialCreatureMaterials(CreatureController cc)
//         {
//             int length = 0;
//             if (cc?.IsMonster() == false)
//             {
//                 SpriteRenderer[] sprArr = cc.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
//                 for (int i = 0; i < sprArr.Length; ++i)
//                 {
//                     if (sprArr[i].sprite == null)
//                         continue;

//                     if (sprArr[i].gameObject.name.Contains(Define.PlayerController.FIRE_SOCKET))
//                         continue;

//                     ++length;
//                 }
//                 CreatureMaterial[] playerMats = new CreatureMaterial[length];

//                 int index = 0;
//                 for (int i = 0; i < sprArr.Length; ++i)
//                 {
//                     if (sprArr[i].sprite == null)
//                         continue;

//                     if (sprArr[i].gameObject.name.Contains(Define.PlayerController.FIRE_SOCKET))
//                         continue;

//                     playerMats[index++] = new CreatureMaterial(sprArr[i], sprArr[i].material, sprArr[i].color);
//                 }

//                 _creatureMats.Add(cc.TemplateID, playerMats);
//                 if (_upgradePlayerBuffEffect == null)
//                     _upgradePlayerBuffEffect = Utils.FindChild(cc.gameObject, Define.PlayerController.UPGRADE_PLAYER_BUFF);
//             }
//             else
//             {
//                 SpriteRenderer[] sprArr = cc.GetComponentsInChildren<SpriteRenderer>();
//                 CreatureMaterial[] creatureMats = new CreatureMaterial[sprArr.Length];
//                 for (int i = 0; i < sprArr.Length; ++i)
//                     creatureMats[i] = new CreatureMaterial(sprArr[i], sprArr[i].material, sprArr[i].color);

//                 _creatureMats.Add(cc.TemplateID, creatureMats);
//             }
//         }
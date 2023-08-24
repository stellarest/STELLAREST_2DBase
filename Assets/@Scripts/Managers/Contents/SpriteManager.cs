using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using Assets.FantasyMonsters.Scripts;
using System.Linq;
using Unity.VisualScripting;
using STELLAREST_2D.Data;

namespace STELLAREST_2D
{
    public class PlayerSprite
    {
        public PlayerSprite(SpriteRenderer eyebrowsRenderer, Color eyebrowsColor, SpriteRenderer eyesRenderer, Color eyesColor, SpriteRenderer mouthRenderer, Color mouthColor)
        {
            this._eyebrowsRenderer = eyebrowsRenderer;
            this._eyebrowsDefaultColor = eyebrowsColor;

            this._eyesRenderer = eyesRenderer;
            this._eyesDefaultColor = eyesColor;

            this._mouthRenderer = mouthRenderer;
            this._mouthDefaultColor = mouthColor;
        }

        public readonly SpriteRenderer _eyebrowsRenderer;
        public readonly Color _eyebrowsDefaultColor;

        public readonly SpriteRenderer _eyesRenderer;
        public readonly Color _eyesDefaultColor;

        public readonly SpriteRenderer _mouthRenderer;
        public readonly Color _mouthDefaultColor;
    }

    public class SpriteManager
    {
        public Color PlayerDefaultEyebrowsColor => _playerSprite._eyebrowsDefaultColor;
        public Color PlayerDefaultEyesColor => _playerSprite._eyesDefaultColor;
        public Color PlayerDefaultMouthColor => _playerSprite._mouthDefaultColor;
        private PlayerSprite _playerSprite = null;

        private Dictionary<int, Character[]> _playerAppearances = new Dictionary<int, Character[]>();
        
        public void InitPlayerSprite(PlayerController pc)
        {
            SpriteRenderer eyebrowsRenderer = null;
            Color eyebrowsColor = Color.white;

            SpriteRenderer eyesRenderer = null;
            Color eyesColor = Color.white;

            SpriteRenderer mouthRenderer = null;
            Color mouthColor = Color.white;

            SpriteRenderer[] sprArr = pc.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < sprArr.Length; ++i)
            {
                if (sprArr[i].sprite == null)
                    continue;

                if (sprArr[i].gameObject.name.Contains(Define.PlayerController.FIRE_SOCKET))
                    continue;

                if (sprArr[i].gameObject.name.Contains("Eyebrows"))
                {
                    eyebrowsRenderer = sprArr[i];
                    eyebrowsColor = sprArr[i].color;
                }

                if (sprArr[i].gameObject.name.Contains("Eyes"))
                {
                    eyesRenderer = sprArr[i];
                    eyesColor = sprArr[i].color;
                    continue;
                }

                if (sprArr[i].gameObject.name.Contains("Mouth"))
                {
                    mouthRenderer = sprArr[i];
                    mouthColor = sprArr[i].color;
                    continue;
                }
            }

            //_cureatureSprites.Add(cc.CharaData.TemplateID, new CreatureSprite(eyesRenderer, eyesColor, mouthRenderer, mouthColor));
            _playerSprite = new PlayerSprite(eyebrowsRenderer, eyebrowsColor, eyesRenderer, eyesColor, mouthRenderer, mouthColor);

            InitPlayerAppearances(pc);
        }

        // +++++ 주의 사항 : 캐릭터 기본 스킬은 캐릭터 데이트 시트에서 무조건 맨 앞에 배치해야함 +++++
        private void InitPlayerAppearances(PlayerController pc)
        {
            //private Dictionary<int, Character[]> _playerAppearances = new Dictionary<int, Character[]>();
            Character[] charas = new Character[(int)Define.InGameGrade.Legendary];
            for (Define.InGameGrade grade = Define.InGameGrade.Normal; grade <= Define.InGameGrade.Legendary; ++grade)
            {
                GameObject go = Managers.Resource.Load<GameObject>(pc.SkillBook.GetPlayerDefaultSkill(grade).SkillData.ModelingLabel);
                charas[(int)grade - 1] = go.GetComponent<Character>();
            }

            _playerAppearances.Add(pc.CharaData.TemplateID, charas);
        }

        public void SetPlayerEmotion(Define.PlayerEmotion emotion)
        {
            //CreatureSprite playerSprite = _cureatureSprites[Managers.Game.Player.CharaData.CreatureData.TemplateID];
            switch (emotion)
            {
                case Define.PlayerEmotion.None:
                    _playerSprite._eyebrowsRenderer.sprite = null;
                    _playerSprite._eyesRenderer.sprite = null;
                    _playerSprite._mouthRenderer.sprite = null;
                    break;

                case Define.PlayerEmotion.Default:
                    {
                        _playerSprite._eyebrowsRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYEBROWS_DEFAULT);
                        _playerSprite._eyebrowsRenderer.color = PlayerDefaultEyebrowsColor;

                        _playerSprite._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYES_MALE_DEFAULT);
                        _playerSprite._eyesRenderer.color = PlayerDefaultEyesColor;

                        _playerSprite._mouthRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.MOUTH_DEFAULT_2);
                        _playerSprite._mouthRenderer.color = PlayerDefaultMouthColor;
                    }
                    break;

                case Define.PlayerEmotion.Greedy:
                    {
                        _playerSprite._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYES_GREEDY);
                        _playerSprite._eyesRenderer.color = Color.yellow;

                        _playerSprite._mouthRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.MOUTH_GREEDY);
                        _playerSprite._mouthRenderer.color = PlayerDefaultMouthColor;
                    }
                    break;

                case Define.PlayerEmotion.Sick:
                    {
                        _playerSprite._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYES_SICK);
                        _playerSprite._eyesRenderer.color = PlayerDefaultEyesColor;

                        _playerSprite._mouthRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.MOUTH_SICK);
                        _playerSprite._mouthRenderer.color = PlayerDefaultMouthColor;
                    }
                    break;

                case Define.PlayerEmotion.Bunny:
                    {
                        _playerSprite._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYES_BUNNY);
                        _playerSprite._eyesRenderer.color = PlayerDefaultEyesColor;

                        _playerSprite._mouthRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.MOUTH_BUNNY);
                        _playerSprite._mouthRenderer.color = PlayerDefaultMouthColor;
                    }
                    break;

                case Define.PlayerEmotion.Kitty:
                    {
                        _playerSprite._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYES_KITTY);
                        _playerSprite._eyesRenderer.color = PlayerDefaultEyesColor;

                        _playerSprite._mouthRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.MOUTH_KITTY);
                        _playerSprite._mouthRenderer.color = PlayerDefaultMouthColor;
                    }
                    break;

                case Define.PlayerEmotion.Death:
                    {
                        _playerSprite._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYES_DIE);
                        _playerSprite._eyesRenderer.color = new Color(0f, 0.784f, 1f, 1f);

                        _playerSprite._mouthRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.MOUTH_DIE);
                        _playerSprite._mouthRenderer.color = PlayerDefaultMouthColor;
                    }
                    break;
            }
        }

        public void SetPlayerEyes(Define.PlayerEmotion emotion)
        {
            // CreatureSprite playerSprite = _cureatureSprites[Managers.Game.Player.CharaData.CreatureData.TemplateID];
            switch (emotion)
            {
                case Define.PlayerEmotion.Default:
                    {
                        _playerSprite._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYES_MALE_DEFAULT);
                        _playerSprite._eyesRenderer.color = PlayerDefaultEyesColor;
                    }
                    break;

                case Define.PlayerEmotion.Greedy:
                    {
                        _playerSprite._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYES_GREEDY);
                        _playerSprite._eyesRenderer.color = Color.yellow;
                    }
                    break;

                case Define.PlayerEmotion.Sick:
                    {
                        _playerSprite._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYES_SICK);
                        _playerSprite._eyesRenderer.color = PlayerDefaultEyesColor;
                    }
                    break;

                case Define.PlayerEmotion.Bunny:
                    {
                        _playerSprite._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYES_BUNNY);
                        _playerSprite._eyesRenderer.color = PlayerDefaultEyesColor;
                    }
                    break;

                case Define.PlayerEmotion.Kitty:
                    {
                        _playerSprite._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYES_KITTY);
                        _playerSprite._eyesRenderer.color = PlayerDefaultEyesColor;
                    }
                    break;
            }
        }

        public void SetMonsterFace(MonsterController mc, Define.SpriteLabels.MonsterFace monsterFace) 
                => mc?.GetComponent<Monster>().SetHead((int)monsterFace);

        public void UpgradePlayerAppearance(PlayerController pc, Define.InGameGrade grade, bool includeInactive = false)
        {
            SpriteRenderer[] currentSPRs = pc.GetComponentsInChildren<SpriteRenderer>(includeInactive);
            for (int i = 0; i < currentSPRs.Length; ++i)
                currentSPRs[i].sprite = null;

            Character next = _playerAppearances[pc.CharaData.TemplateID][(int)grade - 1];
            SpriteRenderer[] nextSPRs = next.GetComponentsInChildren<SpriteRenderer>(includeInactive);
            
            int length = Mathf.Max(currentSPRs.Length, nextSPRs.Length);

            for (int i = 0; i < length; ++i)
            {
                // Prevent out of idx
                if (i < currentSPRs.Length && i < nextSPRs.Length)
                {
                    SpriteRenderer currentSPR = currentSPRs[i];
                    SpriteRenderer nextSPR = nextSPRs[i];

                    currentSPR.sprite = nextSPR.sprite;
                    currentSPR.color = nextSPR.color;
                }
            }

            // Managers.Effect.UpgradePlayerBuffEffect(); 이것도 사실 Legendary에서만 약간 간지나게 적용하면 될 것 같음
            Managers.Effect.ChangeCreatureMaterials(pc);
        }

        private void UpgradePlayerAppearance_TEMP(PlayerController pc, Define.InGameGrade grade)
        {
            // 일단 진행중인 스킬을 멈춘다
            pc.SkillBook.StopSkills(); 

            // 싹 벗김
            foreach (var spr in pc.GetComponentsInChildren<SpriteRenderer>())
                spr.sprite = null;

            // TEMP
            // Managers.Effect.UpgradePlayerBuffEffect();

            Character current = pc.GetComponent<Character>();
            GameObject nextGo = Managers.Resource.Load<GameObject>(Define.PrefabLabels.GARY_UPGRADE_SPRITE_TEMP);
            Character next = nextGo.GetComponent<Character>();

            SpriteRenderer[] currentSPR = current.GetComponentsInChildren<SpriteRenderer>();
            SpriteRenderer[] nextSPR = next.GetComponentsInChildren<SpriteRenderer>();
            int length = Mathf.Max(currentSPR.Length, nextSPR.Length);

            for (int i = 0; i < length; ++i)
            {
                if (i < currentSPR.Length && i < nextSPR.Length)
                {
                    SpriteRenderer currentIs = currentSPR[i];
                    SpriteRenderer nextIs = nextSPR[i];

                    currentIs.sprite = nextIs.sprite;
                    currentIs.color = nextIs.color;
                }
            }

            Managers.Effect.ChangeCreatureMaterials(pc);
        }

        private void UpgradeGaryPaladin(PlayerController pc, Define.InGameGrade grade) // TEMP
        {
            Character current = pc.GetComponent<Character>();
            Character next = pc.transform.GetChild((int)grade).GetComponent<Character>();

            Managers.Effect.UpgradePlayerBuffEffect();

            for (int i = 0; i < current.Armor.Count; ++i)
                current.Armor[i] = next.Armor[i];

            for (int i = 0; i < current.ArmorRenderers.Count; ++i)
            {
                if (current.ArmorRenderers[i].gameObject.activeInHierarchy)
                {
                    string rendererName = $"{current.ArmorRenderers[i].name.Replace("[Armor]", "")}";
                    current.ArmorRenderers[i].sprite = current.Armor.FirstOrDefault(s => s.name.Contains(rendererName));
                }
            }

            current.PrimaryMeleeWeapon = next.PrimaryMeleeWeapon;
            current.PrimaryMeleeWeaponRenderer.sprite = current.PrimaryMeleeWeapon;

            current.Shield = next.Shield;
            current.ShieldRenderer.sprite = current.Shield;

            if (next.Helmet != null)
            {
                current.Helmet = next.Helmet;
                current.HelmetRenderer.sprite = current.Helmet;
            }

            if (next.Cape != null)
            {
                current.Cape = next.Cape;
                current.CapeRenderer.sprite = current.Cape;
            }

            Managers.Effect.ChangeCreatureMaterials(pc);
            // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // pc.CoEffectGlitch();
            // // TEMP
            // if (grade == Define.InGameGrade.Legendary)
            // {
            //     GameObject shild = Utils.FindChild(pc.gameObject, "Shield");
            //     shild.SetActive(true);
            // }
        }
    }
}
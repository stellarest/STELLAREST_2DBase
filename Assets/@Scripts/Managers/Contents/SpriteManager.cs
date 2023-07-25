using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using Assets.FantasyMonsters.Scripts;
using System.Linq;

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

        public void UpgradePlayerSprite(PlayerController pc, Define.InGameGrade grade)
        {
            if (grade > Define.InGameGrade.Legendary)
            {
                Utils.Log("Player Grade is maxium !!");
                return;
            }

            switch (pc.CharaData.CreatureData.TemplateID)
            {
                case (int)Define.TemplateIDs.Player.Gary_Paladin:
                    UpgradeGaryPaladin(pc, grade);
                    break;
            }
        }

        private void UpgradeGaryPaladin(PlayerController pc, Define.InGameGrade grade)
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
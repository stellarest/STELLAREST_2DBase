using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using System.Linq;

namespace STELLAREST_2D
{
    public struct CreatureSprite
    {
        public CreatureSprite(SpriteRenderer eyesRenderer, Color eyesColor, SpriteRenderer mouthRenderer, Color mouthColor)
        {
            this._eyesRenderer = eyesRenderer;
            this._eyesDefaultColor = eyesColor;

            this._mouthRenderer = mouthRenderer;
            this._mouthDefaultColor = mouthColor;
        }

        public readonly SpriteRenderer _eyesRenderer;
        public readonly Color _eyesDefaultColor;

        public readonly SpriteRenderer _mouthRenderer;
        public readonly Color _mouthDefaultColor;
    }

    public class SpriteManager
    {
        private CreatureSprite _playerSprite;

        public void Init()
        {
            SpriteRenderer eyesRenderer = null;
            Color eyesColor = Color.white;

            SpriteRenderer mouthRenderer = null;
            Color mouthColor = Color.white;

            SpriteRenderer[] sprArr = Managers.Game.Player.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < sprArr.Length; ++i)
            {
                if (sprArr[i].sprite == null)
                    continue;

                if (sprArr[i].gameObject.name.Contains(Define.PlayerController.FIRE_SOCKET))
                    continue;

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

            _playerSprite = new CreatureSprite(eyesRenderer, eyesColor, mouthRenderer, mouthColor);
        }

        public Color GetDefaultEyesColor => _playerSprite._eyesDefaultColor;
        public Color GetDefaultMouthColor => _playerSprite._mouthDefaultColor;

        public void SetPlayerEmotion(Define.PlayerEmotion emotion, Color eyesColor, Color mouthColor)
        {
            switch (emotion)
            {
                case Define.PlayerEmotion.Default:
                    {
                        _playerSprite._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.EYES_MALE_DEFAULT);
                        _playerSprite._mouthRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.MOUTH_BONUS_2);
                    }
                    break;

                case Define.PlayerEmotion.Sick:
                    {
                        _playerSprite._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.EYES_SICK);
                        _playerSprite._mouthRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.MOUTH_SICK);
                    }
                    break;

                case Define.PlayerEmotion.Greedy:
                    {
                        _playerSprite._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.EYES_WANTING);
                        _playerSprite._mouthRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.MOUTH_GREEDY);
                    }
                    break;
            }

            _playerSprite._eyesRenderer.color = eyesColor;
            _playerSprite._mouthRenderer.color = mouthColor;
        }

        public void UpgradePlayerSprite(PlayerController pc, Define.InGameGrade grade)
        {
            if (grade > Define.InGameGrade.Legendary)
            {
                Utils.Log("Player Grade is maxium !!");
                return;
            }

            switch (pc.CreatureData.TemplateID)
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
                current.HelmetRenderer.sprite  =  current.Helmet;
            }

            if (next.Cape != null)
            {
                current.Cape = next.Cape;
                current.CapeRenderer.sprite = current.Cape;
            }

            Managers.Effect.AddCreatureMaterials(pc, pc.CreatureData.TemplateID + (int)grade);
            //pc.CoFadeEffect(pc.CreatureData.TemplateID + (int)grade);
            pc.CoGlitchEffect(pc.CreatureData.TemplateID + (int)grade);

            // TEMP
            if (grade == Define.InGameGrade.Legendary)
            {
                GameObject shild = Utils.FindChild(pc.gameObject, "Shield");
                shild.SetActive(true);
            }
        }
    }
}

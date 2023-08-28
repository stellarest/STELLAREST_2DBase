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
    public enum PlayerExpressionType
    {
        Default,
        Sick,
        Angry,
    }


    public class PlayerEmotionController
    {
        public PlayerEmotionController(SpriteRenderer eyebrowsRenderer, SpriteRenderer eyesRenderer, SpriteRenderer mouthRenderer)
        {
            _eyebrowsRenderer = eyebrowsRenderer;
            _defaultEyebrows = eyebrowsRenderer.sprite;
            _defaultEyebrowsColor = eyebrowsRenderer.color;

            _eyesRenderer = eyesRenderer;
            _defaultEyes = eyesRenderer.sprite;
            _defaultEyesColor = eyesRenderer.color;

            _mouthRenderer = mouthRenderer;
            _defaultMouth = mouthRenderer.sprite;
            _defaultMouthColor = mouthRenderer.color;

            // InitSick();
        }

        private Dictionary<PlayerExpressionType, Sprite> _eyebrows = new Dictionary<PlayerExpressionType, Sprite>();
        private Dictionary<PlayerExpressionType, Sprite> _eyes = new Dictionary<PlayerExpressionType, Sprite>();
        private Dictionary<PlayerExpressionType, Sprite> _mouth = new Dictionary<PlayerExpressionType, Sprite>();

        private void InitEyebrows()
        {
            //Sprite eyebrows = Managers.Resource.Load<Sprite>(Define.SpriteLabels.)
        }

        private void InitEyes()
        {
        }

        private SpriteRenderer _eyebrowsRenderer;
        private SpriteRenderer _eyesRenderer;
        private SpriteRenderer _mouthRenderer;

        private Sprite _defaultEyebrows;
        private Color _defaultEyebrowsColor;

        private Sprite _defaultEyes;
        private Color _defaultEyesColor;

        private Sprite _defaultMouth;
        private Color _defaultMouthColor;

        private Sprite _sickEyebrows;
        private Sprite _sickEyes;
        private Sprite _sickMouth;

        private void InitSick()
        {
            // _sickEyebrows = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYEBROWS_SICK);
            // _sickEyes = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYES_SICK);
            // _sickMouth = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.MOUTH_SICK);
        }

        public void Sick()
        {
            _eyebrowsRenderer.sprite = _sickEyebrows;
            _eyesRenderer.sprite = _sickEyes;
            _mouthRenderer.sprite = _sickMouth;
        }

        public void Default()
        {
            _eyebrowsRenderer.sprite = _defaultEyebrows;
            _eyebrowsRenderer.color = _defaultEyebrowsColor;

            _eyesRenderer.sprite = _defaultEyes;
            _eyesRenderer.color = _defaultEyesColor;
            
            _mouthRenderer.sprite = _defaultMouth;
            _mouthRenderer.color = _defaultMouthColor;
        }

        public void Hit()
        {
            _eyebrowsRenderer.sprite = null;
            _eyesRenderer.sprite = null;
            _mouthRenderer.sprite = null;
        }
    }

    public class SpriteManager
    {
        public  PlayerEmotionController PlayerEmotion = null;

        private Dictionary<int, Character[]> _playerAppearances = new Dictionary<int, Character[]>();

        /// <summary>
        /// +++ INPUT PLAYER DEFAULT SKILL IN CREATURE DATA SHEET BEFORE +++
        /// </summary>
        private void InitPlayerAppearances(PlayerController pc)
        {
            Character[] charas = null;
            int skillCount = pc.SkillBook.RepeatSkills.Where(s => s.SkillData.IsPlayerDefaultAttack).ToArray().Length;
            if (skillCount >= 4)
                charas = new Character[(int)Define.InGameGrade.Legendary];
            else
                charas = new Character[(int)Define.InGameGrade.Normal];

            for (Define.InGameGrade grade = Define.InGameGrade.Normal; grade <= Define.InGameGrade.Legendary; ++grade)
            {
                GameObject go = Managers.Resource.Load<GameObject>(pc.SkillBook.GetPlayerDefaultSkill(grade).SkillData.ModelingLabel);
                charas[(int)grade - 1] = go.GetComponent<Character>();
                if (skillCount == 1)
                    break;
            }

            _playerAppearances.Add(pc.CharaData.TemplateID, charas);
        }

        public void InitPlayerSprites(PlayerController pc)
        {
            SpriteRenderer eyebrowsRenderer = null;
            SpriteRenderer eyesRenderer = null;
            SpriteRenderer mouthRenderer = null;

            SpriteRenderer[] sprArr = pc.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < sprArr.Length; ++i)
            {
                if (sprArr[i].sprite == null)
                    continue;

                if (sprArr[i].gameObject.name.Contains(Define.Player.FIRE_SOCKET))
                    continue;

                if (sprArr[i].gameObject.name.Contains("Eyebrows"))
                    eyebrowsRenderer = sprArr[i];

                if (sprArr[i].gameObject.name.Contains("Eyes"))
                {
                    eyesRenderer = sprArr[i];
                    continue;
                }

                if (sprArr[i].gameObject.name.Contains("Mouth"))
                {
                    mouthRenderer = sprArr[i];
                    continue;
                }
            }

            PlayerEmotion = new PlayerEmotionController(eyebrowsRenderer, eyesRenderer, mouthRenderer);
            InitPlayerAppearances(pc);
        }

        // TEMP
        // public void SetPlayerEmotion(Define.PlayerEmotion emotion)
        // {
        //     switch (emotion)
        //     {
        //         case Define.PlayerEmotion.None:
        //             _playerEmotion._eyebrowsRenderer.sprite = null;
        //             _playerEmotion._eyesRenderer.sprite = null;
        //             _playerEmotion._mouthRenderer.sprite = null;
        //             break;

        //         case Define.PlayerEmotion.Default:
        //             {
        //                 _playerEmotion._eyebrowsRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYEBROWS_DEFAULT);
        //                 _playerEmotion._eyebrowsRenderer.color = PlayerDefaultEyebrowsColor;

        //                 _playerEmotion._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYES_MALE_DEFAULT);
        //                 _playerEmotion._eyesRenderer.color = PlayerDefaultEyesColor;

        //                 _playerEmotion._mouthRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.MOUTH_DEFAULT_2);
        //                 _playerEmotion._mouthRenderer.color = PlayerDefaultMouthColor;
        //             }
        //             break;

        //         case Define.PlayerEmotion.Greedy:
        //             {
        //                 _playerEmotion._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYES_GREEDY);
        //                 _playerEmotion._eyesRenderer.color = Color.yellow;

        //                 _playerEmotion._mouthRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.MOUTH_GREEDY);
        //                 _playerEmotion._mouthRenderer.color = PlayerDefaultMouthColor;
        //             }
        //             break;

        //         case Define.PlayerEmotion.Sick:
        //             {
        //                 _playerEmotion._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYES_SICK);
        //                 _playerEmotion._eyesRenderer.color = PlayerDefaultEyesColor;

        //                 _playerEmotion._mouthRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.MOUTH_SICK);
        //                 _playerEmotion._mouthRenderer.color = PlayerDefaultMouthColor;
        //             }
        //             break;

        //         case Define.PlayerEmotion.Bunny:
        //             {
        //                 _playerEmotion._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYES_BUNNY);
        //                 _playerEmotion._eyesRenderer.color = PlayerDefaultEyesColor;

        //                 _playerEmotion._mouthRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.MOUTH_BUNNY);
        //                 _playerEmotion._mouthRenderer.color = PlayerDefaultMouthColor;
        //             }
        //             break;

        //         case Define.PlayerEmotion.Kitty:
        //             {
        //                 _playerEmotion._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYES_KITTY);
        //                 _playerEmotion._eyesRenderer.color = PlayerDefaultEyesColor;

        //                 _playerEmotion._mouthRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.MOUTH_KITTY);
        //                 _playerEmotion._mouthRenderer.color = PlayerDefaultMouthColor;
        //             }
        //             break;

        //         case Define.PlayerEmotion.Death:
        //             {
        //                 _playerEmotion._eyesRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.EYES_DIE);
        //                 _playerEmotion._eyesRenderer.color = new Color(0f, 0.784f, 1f, 1f);

        //                 _playerEmotion._mouthRenderer.sprite = Managers.Resource.Load<Sprite>(Define.SpriteLabels.Player.MOUTH_DIE);
        //                 _playerEmotion._mouthRenderer.color = PlayerDefaultMouthColor;
        //             }
        //             break;
        //     }
        // }

        public void SetMonsterFace(MonsterController mc, Define.MonsterFace monsterFace) 
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
    }
}
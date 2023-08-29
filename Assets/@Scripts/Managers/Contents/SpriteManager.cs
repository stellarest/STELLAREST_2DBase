using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using Assets.FantasyMonsters.Scripts;
using System.Linq;
using Unity.VisualScripting;
using STELLAREST_2D.Data;
using UnityEngine.Playables;

namespace STELLAREST_2D
{
    public enum FacialType
    {
        Eyebrows,
        Eyes,
        Mouth,
        Max,
    }

    // _eyesRenderer.color = new Color(0f, 0.784f, 1f, 1f);
    // _eyebrowsRenderer.sprite = _eyebrows[expression];
    // _eyesRenderer.sprite = _eyes[expression];
    // _mouthRenderer.sprite = _mouth[expression];
    public class PlayerExpressionController
    {
        public PlayerExpressionController(PlayerController player, SpriteRenderer eyebrowsRenderer, SpriteRenderer eyesRenderer, SpriteRenderer mouthRenderer)
        {
            _player = player;

            _eyebrowsRenderer = eyebrowsRenderer;
            _eyebrows.Add(Define.ExpressionType.Default, eyebrowsRenderer.sprite);
            _eyebrowsDefaultColor = eyebrowsRenderer.color;

            _eyesRenderer = eyesRenderer;
            _eyes.Add(Define.ExpressionType.Default, eyesRenderer.sprite);
            _eyesDefaultColor = eyesRenderer.color;

            _mouthRenderer = mouthRenderer;
            _mouth.Add(Define.ExpressionType.Default, mouthRenderer.sprite);
            _mouthDefaultColor = mouthRenderer.color;

            // InitSick();
            InitExpressions();
        }

        private Dictionary<Define.ExpressionType, Sprite> _eyebrows = new Dictionary<Define.ExpressionType, Sprite>();
        private Dictionary<Define.ExpressionType, Sprite> _eyes = new Dictionary<Define.ExpressionType, Sprite>();
        private Dictionary<Define.ExpressionType, Sprite> _mouth = new Dictionary<Define.ExpressionType, Sprite>();
        private Define.ExpressionType _expression = Define.ExpressionType.Default;
        private PlayerController _player = null;

        private void InitExpressions()
        {
            Define.ExpressionType type = Define.ExpressionType.Angry;
            var eyebrows = Managers.Resource.Load<Sprite>(Define.Labels.Sprites.EYEBROWS_SMALL_ANGRY);
            var mouth = Managers.Resource.Load<Sprite>(Define.Labels.Sprites.MOUTH_SMALL_ANGRY);

            _eyebrows.Add(type, eyebrows);
            _mouth.Add(type, mouth);
            
            type = Define.ExpressionType.Sick;
            eyebrows = Managers.Resource.Load<Sprite>(Define.Labels.Sprites.EYEBROWS_SICK);
            var eyes = Managers.Resource.Load<Sprite>(Define.Labels.Sprites.EYES_SICK);
            mouth = Managers.Resource.Load<Sprite>(Define.Labels.Sprites.MOUTH_SICK);

            _eyebrows.Add(type, eyebrows);
            _eyes.Add(type, eyes);
            _mouth.Add(type, mouth);

            type = Define.ExpressionType.Death;
            eyebrows = Managers.Resource.Load<Sprite>(Define.Labels.Sprites.EYEBROWS_DEATH);
            eyes = Managers.Resource.Load<Sprite>(Define.Labels.Sprites.EYES_DEATH);
            mouth = Managers.Resource.Load<Sprite>(Define.Labels.Sprites.MOUTH_DEATH);

            _eyebrows.Add(type, eyebrows);
            _eyes.Add(type, eyes);
            _mouth.Add(type, mouth);
        }

        private SpriteRenderer _eyebrowsRenderer;
        private Color _eyebrowsDefaultColor;

        private SpriteRenderer _eyesRenderer;
        private Color _eyesDefaultColor;

        private SpriteRenderer _mouthRenderer;
        private Color _mouthDefaultColor;

        private bool _hit = false;
        private bool _isEmotional = false;

        public void Expression(Define.ExpressionType expression, float duration = 3f)
        {
            if (_hit || _isEmotional)
                return;

            _expression = expression;
            switch (expression)
            {
                case Define.ExpressionType.Default:
                    {
                        _eyebrowsRenderer.color = _eyebrowsDefaultColor;
                        _eyesRenderer.color = _eyesDefaultColor;
                        _mouthRenderer.color = _mouthDefaultColor;

                        _eyebrowsRenderer.sprite = _eyebrows[expression];
                        _eyesRenderer.sprite = _eyes[expression];
                        _mouthRenderer.sprite = _mouth[expression];
                    }
                    break;

                case Define.ExpressionType.Angry:
                    {
                        // TODO : 캐릭터에 따라서 분기해야함
                        _eyebrowsRenderer.sprite = _eyebrows[expression];
                        _mouthRenderer.sprite = _mouth[expression];
                    }
                    break;

                default:
                    {
                        _isEmotional = true;
                        _player.CoExpression(expression, duration);
                    }
                    break;
            }
        }

        public IEnumerator CoExpression(Define.ExpressionType expression, float duration)
        {
            float t = 0f;
            float percent = 0f;

            if (expression == Define.ExpressionType.Death)
                _eyesRenderer.color = new Color(0f, 0.784f, 1f, 1f);

            _eyebrowsRenderer.sprite = _eyebrows[expression];
            _eyesRenderer.sprite = _eyes[expression];
            _mouthRenderer.sprite = _mouth[expression];

            while (percent < 1f)
            {
                t += Time.deltaTime;
                percent = t / duration;
                yield return null;
            }

            Reset();
            _isEmotional = false;
        }

        private void Reset()
        {
            _expression = Define.ExpressionType.Default;
            _eyebrowsRenderer.color = _eyebrowsDefaultColor;
            _eyebrowsRenderer.sprite = _eyebrows[_expression];

            _eyesRenderer.color = _eyesDefaultColor;
            _eyesRenderer.sprite = _eyes[_expression];

            _mouthRenderer.color = _mouthDefaultColor;
            _mouthRenderer.sprite = _mouth[_expression];
        }

        public void Hit()
        {
            _hit = true;
            _eyesRenderer.sprite = null;
        }

        public void EndHit()
        {
            var eyes = _eyes.TryGetValue(_expression, out var sprite) ? sprite : _eyes[Define.ExpressionType.Default];
            _eyesRenderer.sprite = eyes;
            _hit = false;
        }

        public void UpdateDefaultFace(PlayerController pc)
        {
            SpriteRenderer[] sprArr = pc.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < sprArr.Length; ++i)
            {
                if (sprArr[i].sprite == null)
                    continue;

                if (sprArr[i].gameObject.name.Contains(Define.Player.FIRE_SOCKET))
                    continue;

                if (sprArr[i].gameObject.name.Contains("Eyebrows"))
                {
                    _eyebrowsRenderer = sprArr[i];
                    _eyebrowsDefaultColor = sprArr[i].color;
                    continue;
                }

                if (sprArr[i].gameObject.name.Contains("Eyes"))
                {
                    _eyesRenderer = sprArr[i];
                    _eyesDefaultColor = sprArr[i].color;
                    continue;
                }

                if (sprArr[i].gameObject.name.Contains("Mouth"))
                {
                    _mouthRenderer = sprArr[i];
                    _mouthDefaultColor = sprArr[i].color;
                    continue;
                }
            }
        }
    }

    /// <summary>
    /// TODO
    /// Expression Duration
    /// Hit
    /// </summary>
    public class SpriteManager
    {
        public PlayerExpressionController PlayerExpressionController { get; private set; } = null;

        private Dictionary<int, Character[]> _playerAppearances = new Dictionary<int, Character[]>();

        /// <summary>
        /// +++ INPUT PLAYER DEFAULT SKILL IN CREATURE DATA SHEET BEFORE +++
        /// </summary>
        private void LoadPlayerAppearances(PlayerController pc)
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

            PlayerExpressionController = new PlayerExpressionController(pc, eyebrowsRenderer, eyesRenderer, mouthRenderer);
            LoadPlayerAppearances(pc);
        }

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
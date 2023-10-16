using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillTemplate = STELLAREST_2D.Define.TemplateIDs.Status.Skill;

namespace STELLAREST_2D
{
    public class AnimationCallback : MonoBehaviour
    {
        private CreatureController _owner = null;
        public System.Action OnCloneRepeatSkill = null;
        public System.Action OnDustVFX = null;

        public void Init(CreatureController owner) => this._owner = owner;
        public void OnCloneRepeatSkillHandler() => OnCloneRepeatSkill?.Invoke();
        public void OnFaceBattleHandler() => this._owner.RendererController.OnFaceBattleHandler();
        public void OnFaceDefaultHandler() => this._owner.RendererController.OnFaceDefaultHandler();
        public void OnFaceDeadHandler() => this._owner.RendererController.OnFaceDeadHandler();
        public void OnDustVFXHandler() => this._owner.RendererController.OnDustVFXHandler();

        //public void ShowDustEffect() => Managers.Effect.ShowPlayerDust();
        public void BattleFaceExpression()
        {
            // switch (_owner.CreatureStat.TemplateID)
            // {
            //     case (int)Define.TemplateIDs.Creatures.Player.Gary_Paladin:
            //     case (int)Define.TemplateIDs.Creatures.Player.Gary_Knight:
            //     case (int)Define.TemplateIDs.Creatures.Player.Gary_PhantomKnight:
			// 		Managers.Sprite.PlayerExpressionController.Expression(Define.ExpressionType.Battle);
            //         break;

            //     case (int)Define.TemplateIDs.Creatures.Player.Reina_ArrowMaster:
            //     case (int)Define.TemplateIDs.Creatures.Player.Reina_ElementalArcher:
            //     case (int)Define.TemplateIDs.Creatures.Player.Reina_ForestWarden:
            //         Managers.Sprite.PlayerExpressionController.Expression(Define.ExpressionType.Concentration);
            //         break;

			// 	case (int)Define.TemplateIDs.Creatures.Player.Kenneth_Assassin:
			// 	case (int)Define.TemplateIDs.Creatures.Player.Kenneth_Thief:
			// 	case (int)Define.TemplateIDs.Creatures.Player.Kenneth_Ninja:
			// 		break;

            //     case (int)Define.TemplateIDs.Creatures.Player.Lionel_Warrior:
            //     case (int)Define.TemplateIDs.Creatures.Player.Lionel_Berserker:
            //             Managers.Sprite.PlayerExpressionController.Expression(Define.ExpressionType.Angry);
            //         break;
            // }
        }

        public void ReleaseFaceExpression()
        {
            //Managers.Sprite.PlayerExpressionController.Expression(Define.ExpressionType.Default);
        } 
    }
}

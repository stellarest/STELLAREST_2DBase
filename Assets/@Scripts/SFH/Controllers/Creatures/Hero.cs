using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class Hero : Creature
    {
        private Vector2 _moveDir = Vector2.zero;

        private EHeroMoveState _heroMoveState = EHeroMoveState.None;
        public EHeroMoveState HeroMoveState
        {
            get => _heroMoveState;
            private set
            {
                switch (value)
                {
                    case EHeroMoveState.CollectEnv:
                        break;

                    case EHeroMoveState.TargetMonster:
                        break;

                    case EHeroMoveState.ForceMove:
                        break;
                }
            }
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            CreatureType = ECreatureType.Hero;
            CreatureState = ECreatureState.Idle;
            Speed = 5f;

            Managers.Game.OnJoystickStateChanged -= OnJoystickStateChangedHandler;
            Managers.Game.OnJoystickStateChanged += OnJoystickStateChangedHandler;

            return true;
        }

        private void OnJoystickStateChangedHandler(EJoystickState state)
        {
            Util.Log($"{nameof(OnJoystickStateChangedHandler)} : {state}");
            switch (state)
            {
                case EJoystickState.PointerUp:
                    BaseAnim.Idle();
                    break;

                case EJoystickState.Drag:
                    BaseAnim.Move();
                    break;
            }
        }
    }
}

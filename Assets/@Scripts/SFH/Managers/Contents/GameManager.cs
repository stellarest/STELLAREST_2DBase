using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class GameManager
    {
        private Vector2 _moveDir = Vector2.zero;
        public Vector2 MoveDir
        {
            get => _moveDir;
            set
            {
                _moveDir = value;
                OnMoveDirChanged?.Invoke(value);
            }
        }

        private EJoystickState _joystickState = EJoystickState.PointerUp;
        public EJoystickState JoystickState
        {
            get => _joystickState;
            set
            {
                _joystickState = value;
                OnJoystickStateChanged?.Invoke(value);
            }
        }

        public event Action<Vector2> OnMoveDirChanged = null;
        public event Action<EJoystickState> OnJoystickStateChanged = null;
    }
}

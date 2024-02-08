using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_SFH
{
    public static class Define
    {
        public enum EScene
        {
            Unknown,
            TitleScene,
            GameScene,
        }

        public enum EUIEvent    
        {
            Click,
            PointerDown,
            PointerUp,
            Drag
        }

        public enum EJoystickState
        {
            PointerDown,
            PointerUp,
            Drag
        }

        public enum ESound
        {
            Bgm,
            Effect,
            Max
        }

        public enum EObjectType
        {
            None,
            HeroCamp,
            Creature,
            Projectile,
            Env
        }

        public enum ECreatureType
        {
            None,
            Hero,
            Monster,
            NPC
        }

        public enum ECreatureState
        {
            None,
            Idle,
            Move,
            Skill,
            Dead
        }

        public enum EHeroMoveState
        {
            None,
            TargetMonster,
            CollectEnv,
            ReturnToCamp,
            ForceMove,
            ForcePath
        }

        public enum EEnvState
        {
            Idle,
            OnDamaged,
            Dead
        }

        public enum ELayer
        {
            Default = 0,
            TransparentFX = 1,
            IgnoreRaycast = 2,
            Dummy1 = 3,
            Water = 4,
            UI = 5,
            Hero = 6,
            Monster = 7,
            Env = 8,
            Obstacle = 9,
            Projectile = 10,
        }

        public enum EColliderSize
        {
            Small,
            Normal,
            Big
        }

        public enum EFindPathResult
        {
            Fail_LerpCell,
            Fail_NoPath,
            Fail_MoveTo,
            Success
        }

        public static class FixedValue
        {
            public static class String
            {
                public const string SFH_ = "SFH_";
                public const string PRE_LOAD = SFH_ + "PreLoad";
                public const string TEST_DATA = SFH_ + "TestData";
                public const string UI_JOYSTICK = SFH_ + "UI_Joystick";

                public const string MANAGERS = "@Managers";
                public const string UI_ROOT = "@UI_Root";
                public const string EVENT_SYSTEM = "@EventSystem";
                public const string DOT_SPRITE = ".sprite";
            }

            public static class Numeric
            {
                public const float GLOW_BG_FOCUS_MIN = -0.15F;
                public const float GLOW_BG_FOCUS_MAX = 0.15F;
            }
        }
    }
}

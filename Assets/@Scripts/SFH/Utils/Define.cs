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

        public enum ELookAtDirection
        {
            Left = 1,
            Right = -1
        }

        public static class FixedValue
        {
            public static class String
            {
                public const string SFH_ = "SFH_";
                public const string PRE_LOAD = SFH_ + "PreLoad";
                public const string TEST_DATA = SFH_ + "TestData";
                public const string CREATURE_DATA = SFH_ + "CreatureData";
                public const string UI_JOYSTICK = SFH_ + "UI_Joystick";
                public const string MANAGERS = "@Managers";
                public const string UI_ROOT = "@UI_Root";
                public const string EVENT_SYSTEM = "@EventSystem";
                public const string DOT_SPRITE = ".sprite";
                public const string ANIMATION_BODY = "AnimationBody";
                public const string ROOT_HERO = "@Heroes";
                public const string ROOT_MONSTER = "@Monsters";
                public const string ROOT_PROJECTILE = "@Projectiles";
                public const string ROOT_ENV = "@Envs";

                public const string HERO_CAMP = "HeroCamp";
                public const string PIVOT = "Pivot";
                public const string DESTINATION = "Destination";

                // UPPER vs LOWER LAYER HOW ??
                public const string ANIM_PARAM_IDLE = "Idle";
                public const string ANIM_PARAM_MOVE = "Move";
            }

            public static class Numeric
            {
                public const float GLOW_BG_FOCUS_MIN = -0.15F;
                public const float GLOW_BG_FOCUS_MAX = 0.15F;
                public const float HERO_LOCAL_SCALE_X = 1F;
                public const float HERO_LOCAL_SCALE_Y = 1F;
                public const float CAM_DEFAULT_ORTHO_SIZE = 8F;
            }
        }
    }
}

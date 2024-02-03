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

        public enum ESound
        {
            Bgm,
            Effect,
            Max
        }

        public static class FixedValue
        {
            public static class String
            {
                public const string MANAGERS = "@Managers";
                public const string UI_ROOT = "@UI_Root";
                public const string PRE_LOAD = "PreLoad";
                public const string EVENT_SYSTEM = "@EventSystem";
                public const string DOT_SPRITE = ".sprite";
                public const string TEMP_CREATURE_DATA = "Temp_CreatureData";
            }

            public static class Numeric
            {
            }
        }
    }
}

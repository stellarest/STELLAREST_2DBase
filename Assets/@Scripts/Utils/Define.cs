
namespace STELLAREST_2D
{
    // static class : Define.점까지 찍기 
    // Sprite Renderer Order in Layer도 Define에서 해주는게 좋긴함.
    // 나중에 데이터 시트로 빼주기전까지 Define으로 하드코딩해도 됨
    // public const int EGO_SWORD_ID = 10;
    // 필요한 Define은 여기서 찾는다
    public static class Define
    {
        public enum UIEvent { Click, Pressed, PointerDown, PointerUp, BeginDrag, Drag, EndDrag, }
        public enum Scene { Unknown, DevScene, GameScene, }
        public enum Sound { BGM, Effect, }
        public enum ObjectType { Player, Monster, Projectile, Env, }

        public enum StageType { Normal, MiddleBoss, Boss, }
        public enum CreatureState { Idle, Moving, Skill, Dead, }

        public enum MonsterState { Idle = 0, Ready = 1, Walk = 2, Run = 3, Attack = 4, Death = 5 }
        public enum WeaponType { None = 0, Melee1H = 1, Melee2H = 2, MeleePaired = 3, Bow = 4, Firearm1H = 5, Firearm2H = 6, }

        public static class LoadDatas
        {
            // 모두 플레이팹 데이터 테이블에서 불러와야한다
            public const string CREATURES = "CreatureData.json";
            public const string STAGES = "Stage.json";
            public const string SKILLS = "SkillsData.json";
        }

        public static class TemplateIDs
        {
            public enum Player
            {
                Gary_Default = 101001,
                Gary_Ultimate = 101002,

                Reina_Default = 201001,
                Reina_Ultimate = 201002,

                Kenneth_Default = 301001,
                Kenneth_Ultimate = 301002,

                Lionel_Ultimate = 104002,
                Dwight_Ultimate = 501002,
                Chloe_Ultimate = 601002,
            }

            public enum SkillType
            {
                None = -1,
                Gary_Default_Swing = 10001,
                Gary_Ultimate_Swing = 10002,
                Kenneth_Default_Swing = 300001,
                Kenneth_Ultimate_Swing = 300002,
                Lionel_Ultimate_Swing = 400002,
            }

            public enum Monster
            {
                RedChicken = 201000,
                Chicken = 201001,
            }
        }

        // 나중에 데이터 시트로 빼서 어드레서블에서 세팅하고 불러와서 사용해도 됨(사실 이 부분이 없어야함)
        public static class PrefabLabels
        {
            public const string NONE = "";
            public const string TEST_MAP = "Map_01.prefab";
            public const string JOYSTICK = "UI_Joystick.prefab";
            public const string EXP_GEM = "EXPGem.prefab";
        }

        public static class SpriteLabels
        {
            public const string EXP_GEM_GREEN = "EXPGem_01.sprite";
            public const string EXP_GEM_YELLOW = "EXPGem_02.sprite";
            public const string EXP_GEM_BLUE = "EXPGem_03.sprite";
        }

        public static class PlayerController
        {
            public const string GARY_DEFAULT = "Gary_Default";
            public const string GARY_ULTIMATE = "Gary_Ultimate";
            public const string LIONEL_ULTIMATE = "Lionel_Ultimate";

            public const string RIFLE_GRAB_POINT = "ArmR[1]";
            public const string FIRE_TRANSFORM = "FireTransform";

            public const string INDICATOR = "Indicator";
            public const string FIRE_SOCKET = "FireSocket";
            public const float SCALE_X = 1f; // Initial ScaleX : 0.8f
            public const float SCALE_Y = 1f; // Initial ScaleY : 0.8f
            public const float SCALE_Z = 1f;
        }
    }
}


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
        public enum ObjectType { Player, Monster, EliteMonster, MiddleBoss, Boss, Projectile, Env, }
        public enum WaveType { None, Elite, MiddleBoss, Boss }

        // Sorting Order를 여기에 적으면, 여기에 적은대로 적용되게끔. Init에서 생성할때 하면 될듯.
        // 실시간으로 바꾸는건 모바일 게임에선 좀 별로인듯
        public enum SortingOrder { Map = 100, Player = 200, Monster = 210, RepeatParticleEffect = 230 }

        public enum StageType { Normal, MiddleBoss, Boss, }
        public enum CreatureState { Idle, Moving, Skill, Dead, }

        public enum MonsterState { Idle = 0, Run = 1, Skill = 2, Attack = 3, Death = 9 }
        public enum WeaponType { None = 0, Melee1H = 1, Melee2H = 2, MeleePaired = 3, Bow = 4, Firearm1H = 5, Firearm2H = 6, }
        public enum InGameGrade { Normal = 1, Rare = 2, Epic = 3, Legendary = 4 }

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
                Gary_Paladin = 101001,
                Gary_Knight = 101002,
                Gary_PhantomKnight = 101003,

                Reina_Hunter = 102001,
                Reina_ElementalArcher = 102002,
                Reina_ForestGuard = 102003,
            }

            public enum SkillType
            {
                PaladinSwing = 1010001,
                InfernoSwing = 10401,
                BodyAttack = 99999
            }

            public enum Monster
            {
                Chicken = 201001,
            }
        }

        // 나중에 데이터 시트로 빼서 어드레서블에서 세팅하고 불러와서 사용해도 됨(사실 이 부분이 없어야함)
        public static class PrefabLabels
        {
            public const string JOYSTICK = "UI_Joystick.prefab";
            public const string EXP_GEM = "EXPGem.prefab";
            public const string DMG_NUMBER_TO_MONSTER = "DmgNumber_ToMonster.prefab";
            public const string DMG_NUMBER_TO_MONSTER_CRITICAL = "DmgNumber_ToMonsterCritical.prefab";
            public const string DMG_NUMBER_TO_PLAYER = "DmgNumber_ToPlayer.prefab";

            // 플레이어 몸뚱아리에 크리티컬 폰트를 띄우진 않을 것이다.
            public const string DMG_TEXT_TO_MONSTER_CRITICAL = "DmgText_ToMonsterCritical.prefab";
        }

        public static class SpriteLabels
        {
            public const string EXP_GEM_GREEN = "EXPGem_01.sprite";
            public const string EXP_GEM_YELLOW = "EXPGem_02.sprite";
            public const string EXP_GEM_BLUE = "EXPGem_03.sprite";
        }

        public static class MaterialLabels
        {
            public const string MAT_HIT_WHITE = "HitWhite.mat";
            public const string MAT_HIT_RED = "HitRed.mat";
        }

        public static class NameSpaceLabels
        {
            public const string STELLAREST_2D = "STELLAREST_2D";
        }

        public static class PlayerController
        {
            public const string PLAYER_GARY = "Gary";
            public const string PLAYER_LIONEL = "Lionel";

            public const string RIFLE_GRAB_POINT = "ArmR[1]";
            public const string FIRE_TRANSFORM = "FireTransform";

            public const string INDICATOR = "Indicator";
            public const string FIRE_SOCKET = "FireSocket";
            public const float CONSTANT_SCALE_X = 1.25f; // Initial ScaleX : 0.8f
            public const float CONSTANT_SCALE_Y = 1.25f; // Initial ScaleY : 0.8f
            public const float CONSTANT_SCALE_Z = 1f;
        }
    }
}

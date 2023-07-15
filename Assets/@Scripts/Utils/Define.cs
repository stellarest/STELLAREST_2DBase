
namespace STELLAREST_2D
{
    public static class Define
    {
        public enum UIEvent { Click, Pressed, PointerDown, PointerUp, BeginDrag, Drag, EndDrag, }
        public enum Scene { Unknown, DevScene, GameScene, }
        public enum Sound { BGM, Effect, }
        public enum ObjectType { Player, Monster, EliteMonster, MiddleBoss, Boss, Projectile, Env, }
        public enum WaveType { None, Elite, MiddleBoss, Boss }
        public enum SortingOrder { Map = 100, Player = 200, Monster = 210, ParticleEffect = 230 }
        public enum StageType { Normal, MiddleBoss, Boss, }
        public enum CreatureState { Idle, Moving, Skill, Dead, }
        public enum MonsterState { Idle = 0, Run = 1, Skill = 2, Attack = 3, Death = 9 }
        public enum InGameGrade { Normal = 1, Rare = 2, Epic = 3, Legendary = 4 }
        public enum CollisionLayers { Default = 0, PlayerBody = 6, PlayerAttack = 7, MonsterBody = 8, MonsterAttack = 9, }

        public static class LoadJson
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
                Gary_Paladin = 100100,
            }

            public enum Monster
            {
                Chicken = 100200,
            }

            public enum SkillType
            {
                PaladinSwing = 200100,
            }
        }

        // 나중에 데이터 시트로 빼서 어드레서블에서 세팅하고 불러와서 사용해도 됨(사실 이 부분이 없어야함)
        public static class PrefabLabels
        {
            public const string JOYSTICK = "UI_Joystick.prefab";
            public const string EXP_GEM = "EXPGem.prefab";
            public const string DMG_NUMBER_TO_MONSTER = "DmgNumber_ToMonster.prefab";
            public const string DMG_NUMBER_TO_MONSTER_CRITICAL = "DmgNumber_ToMonsterCritical.prefab";
            public const string DMG_TEXT_TO_MONSTER_CRITICAL = "DmgText_ToMonsterCritical.prefab"; // 크리티컬 text는 몬스터에게만 적용
            public const string DMG_NUMBER_TO_PLAYER = "DmgNumber_ToPlayer.prefab";
        }

        public static class SpriteLabels
        {
            public const string EXP_GEM_GREEN = "EXPGem_01.sprite";
            public const string EXP_GEM_YELLOW = "EXPGem_02.sprite";
            public const string EXP_GEM_BLUE = "EXPGem_03.sprite";
            public const string BUNNY_FACE_EYES = "BunnyFace.sprite";
            public const string MALE_DEFAULT_EYES = "Male.sprite";
        }

        public static class MaterialLabels
        {
            public const string MAT_HIT_WHITE = "HitWhite.mat";
            public const string MAT_HIT_RED = "HitRed.mat";
            public const string MAT_FADE = "Fade.mat";
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

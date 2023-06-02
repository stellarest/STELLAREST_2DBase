
namespace STELLAREST_2D
{
    // static class : Define.점까지 찍기 
    // Sprite Renderer Order in Layer도 Define에서 해주는게 좋긴함.
    // 나중에 데이터 시트로 빼주기전까지 Define으로 하드코딩해도 됨
    // public const int EGO_SWORD_ID = 10;
    public static class Define
    {
        #region Game Data
        public static class GameData
        {
            public enum Scene
            {
                Unknown,
                DevScene,
                GameScene,
            }

            public enum Sound
            {
                BGM,
                Effect,
            }

            public enum ObjectType
            {
                Player,
                Monster,
                Projectile,
                Env,
            }

            public enum SkillType
            {
                None,
                Melee,
                Projectile,
                Etc,
            }

            public static class Prefabs
            {
                public const string EXP_GEM = "EXPGem.prefab";
                public const string MAP_01 = "Map_01.prefab";
            }

            public static class Sprites
            {
                public const string EXP_GEM_GREEN = "EXPGem_01.sprite";
                public const string EXP_GEM_YELLOW = "EXPGem_02.sprite";
                public const string EXP_GEM_BLUE = "EXPGem_03.sprite";
            }

            public static class Json
            {
                // ***** 쿨타임 데이터 시트 로드 적용해야하는데 지금 파이어볼이랑 소드 로직이 조금 달라서
                // 일단 그대로 진행. 나중에 수업에서 스킬북 비슷한거 만들어질수도 있어서
                public const string SKILLS_DATA = "SkillsData.json";
            }
        }
        #endregion


        #region Player Data IDs
        public static class PlayerData
        {
            public enum SkillTemplateIDs
            {
                FireBall = 1,
                EgoSword = 10,
            }

            public static class Prefabs
            {
                public const string SLIME = "Slime_01.prefab";
            }

            public static class Json
            {
                public const string STATS = "PlayerStatData.json";
            }

            public const int INITIAL_SPAWN_TEMPLATE_ID = 1;
            public const string EGO_SWORD_CHILD_BASE = "EgoSword_Melee_";
            public const string INDICATOR = "Indicator";
            public const string FIRE_SOCKET = "FireSocket";
        }
        #endregion


        #region Monster Data IDs
        public static class MonsterData
        {
            public enum MinMaxTemplateIDs
            {
                Min = 1,
                Max = 3 // Exclusive
            }

            public enum TemplateIDs
            {
                Snake = 1,
                Goblin = 2,
            }

            public enum Type
            {
                None,
                Normal,
                Rare,
                Boss,
            }

            public static class Prefabs
            {
                public const string GOBLIN = "Goblin_01.prefab";
                public const string SNAKE = "Snake_01.prefab";
            }

            public static class Json
            {
                public const string MONSTERS_DATA = "MonstersData.json";
            }

        }
        #endregion


        #region UI Data IDs
        public static class UIData
        {
            public static class Prefabs
            {
                // *** UI JOYSTICK도 Sort Order 있음
                public const string JOYSTICK = "UI_Joystick.prefab";
            }
        }
        #endregion
    }
}

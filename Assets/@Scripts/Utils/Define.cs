
namespace STELLAREST_2D
{
    // static class : Define.점까지 찍기 귀찮아서
    public static class Define
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

        public enum MonsterType
        {
            None,
            Normal,
            Rare,
            Boss,
        }

        public enum MonsterTemplateID
        {
            Snake = 1,
            Goblin = 2,
        }

        public enum MonsterTemplateMinMaxID
        {
            Min = 1,
            Max = 3 // Exclusive
        }

        public enum PlayerSkillTemplateID
        {
            FireBall = 1,
            EgoSword = 10,
        }

        // Sprite Renderer Order in Layer도 Define에서 해주는게 좋긴함.
        public const int PLAYER_DATA_ID = 1;
        
        // Player Indicator and FireSocket
        public const string PLAYER_INDICATOR = "Indicator";
        public const string PLYAER_FIRE_SOCKET = "FireSocket";

        // // 나중에 데이터 시트로 빼주기전까지 Define으로 하드코딩해도 됨
        // public const int EGO_SWORD_ID = 10;

        public const string LOAD_PLAYER_PREFAB = "Slime_01.prefab";
        public const string LOAD_GOBLIN_PREFAB = "Goblin_01.prefab";
        public const string LOAD_SNAKE_PREFAB = "Snake_01.prefab";
        public const string LOAD_JOYSTICK_PREFAB = "UI_Joystick.prefab";
        public const string LOAD_MAP_PREFAB = "Map_01.prefab";
        public const string LOAD_PLAYER_JSON_DATA = "PlayerData.json";
        public const string LOAD_EXP_GEM_PREFAB = "EXPGem.prefab";
        public const string LOAD_FIRE_PROJECTILE_PREFAB = "FireProjectile.prefab";
        public const string LOAD_EGO_SWORD_PREFAB = "EgoSword.prefab";

        // Sprites
        public const string LOAD_EXP_GEM_GREEN_SPRITE = "EXPGem_01.sprite";
        public const string LOAD_EXP_GEM_YELLOW_SPRITE = "EXPGem_02.sprite";
        public const string LOAD_EXP_GEM_BLUE_SPRITE = "EXPGem_03.sprite";
    }
}

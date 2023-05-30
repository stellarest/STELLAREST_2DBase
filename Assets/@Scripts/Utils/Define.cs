
namespace STELLAREST_2D
{
    // static class : Define.점까지 찍기 귀찮아서
    public class Define
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

        // Sprite Renderer Order in Layer도 Define에서 해주는게 좋긴함.
        public const int PLAYER_DATA_ID = 1;
        
        public const string LOAD_PLAYER_PREFAB = "Slime_01.prefab";
        public const string LOAD_GOBLIN_PREFAB = "Goblin_01.prefab";
        public const string LOAD_SNAKE_PREFAB = "Snake_01.prefab";
        public const string LOAD_JOYSTICK_PREFAB = "UI_Joystick.prefab";
        public const string LOAD_MAP_PREFAB = "Map.prefab";
        public const string LOAD_PLAYER_JSON_DATA = "PlayerData.json";
        public const string LOAD_EXP_GEM_PREFAB = "EXPGem.prefab";
        public const string LOAD_FIRE_PROJECTILE_PREFAB = "FireProjectile.prefab";

        // Sprites
        public const string LOAD_EXP_GEM_GREEN_SPRITE = "EXPGem_01.sprite";
        public const string LOAD_EXP_GEM_YELLOW_SPRITE = "EXPGem_02.sprite";
        public const string LOAD_EXP_GEM_BLUE_SPRITE = "EXPGem_03.sprite";
        
    }
}

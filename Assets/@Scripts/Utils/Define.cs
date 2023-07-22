
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
        public enum MonsterState { Idle = 0, Run = 1, Skill = 2, Attack = 3, Death = 9 }

        public enum CreatureState { Idle = 0, Run, Skill, Attack, Death = 9 }

        public enum InGameGrade { Normal = 1, Rare = 2, Epic = 3, Legendary = 4 }
        public enum CollisionLayers { Default = 0, PlayerBody = 6, PlayerAttack = 7, MonsterBody = 8, MonsterAttack = 9, }

        public enum PlayerEmotion { None = 0, Default = 1, Greedy, Sick, Bunny, Kitty, Die }
        public enum InitialStatRatioGrade { None = 0, Low = 10, Average = 20, High = 30 }

        public const float MAX_DODGE_CHANCE = 0.6f;

        public static class LoadJson
        {
            // 모두 플레이팹 데이터 테이블에서 불러와야한다
            public const string CREATURE = "CreatureData.json";
            public const string STAGE = "Stage.json";
            public const string SKILL = "SkillData.json";
            public const string PASSIVE_ITEM = "PassiveItemData.json";
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
                BodyAttack = 200200,

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

            public const string DMG_TEXT_TO_MONSTER_CRITICAL = "DmgText_ToMonsterCritical.prefab"; // 크리티컬 text는 몬스터에게만 적용
            public const string DMG_TEXT_TO_PLAYER_DODGE = "DmgText_ToPlayerDodge.prefab"; // 플레이어에게만 적용
        }

        public static class SpriteLabels
        {
            public const string EXP_GEM_GREEN = "EXPGem_01.sprite";
            public const string EXP_GEM_YELLOW = "EXPGem_02.sprite";
            public const string EXP_GEM_BLUE = "EXPGem_03.sprite";

            // PLAYER FACE SET
            // DEFAULT = 0
            public const string EYES_MALE_DEFAULT = "Eyes_Male_Default.sprite";
            public const string MOUTH_DEFAULT_2 = "Mouth_Default_2.sprite";

            // GREEDY = 1
            public const string EYES_GREEDY = "Eyes_Greedy.sprite";
            public const string MOUTH_GREEDY = "Mouth_Greedy.sprite";

            // SICK = 2
            public const string EYES_SICK = "Eyes_Sick.sprite";
            public const string MOUTH_SICK = "Mouth_Sick.sprite";

            // BUNNY = 3
            public const string EYES_BUNNY = "Eyes_Bunny.sprite";
            public const string MOUTH_BUNNY = "Mouth_Bunny.sprite";

            // KITTY = 4
            public const string EYES_KITTY = "Eyes_Kitty.sprite";
            public const string MOUTH_KITTY = "Mouth_Kitty.sprite";

            // DIE = 5
            public const string EYES_DIE = "Eyes_Die.sprite";
            public const string MOUTH_DIE = "Mouth_Die.sprite";
        }

        public static class MaterialLabels
        {
            public const string MAT_HIT_WHITE = "HitWhite.mat";
            public const string MAT_HIT_RED = "HitRed.mat";
            public const string MAT_FADE = "Fade.mat";
            public const string MAT_GLITCH = "Glitch.mat";
        }

        public static class NameSpaceLabels
        {
            public const string STELLAREST_2D = "STELLAREST_2D";
        }

        public static class PlayerController
        {
            public const string INDICATOR = "Indicator";
            public const string FIRE_SOCKET = "FireSocket";
            public const string UPGRADE_PLAYER_BUFF = "UpgradeBuff";

            public const float CONSTANT_SCALE_X = 1.25f; // Initial ScaleX : 0.8f
            public const float CONSTANT_SCALE_Y = 1.25f; // Initial ScaleY : 0.8f
            public const float CONSTANT_SCALE_Z = 1f;
        }
    }
}

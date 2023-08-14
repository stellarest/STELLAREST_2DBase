
#define USE_LINQ

namespace STELLAREST_2D
{
    public static class Define
    {
        public enum UIEvent { Click, Pressed, PointerDown, PointerUp, BeginDrag, Drag, EndDrag, }
        public enum Scene { Unknown, DevScene, GameScene, }
        public enum Sound { BGM, Effect, }
        public enum ObjectType { Player, Monster, EliteMonster, Boss, Projectile, Env, }

        public enum CreatureType { Creture, Gary, Chicken, }
        
        public enum WaveType { None, Elite, MiddleBoss, Boss }
        public enum SortingOrder { Map = 100, Player = 200, Item = 209, Monster = 210, Skill = 211, ParticleEffect = 230, CCStatus = 260 }
        public enum StageType { Normal, MiddleBoss, Boss, }
        public enum CCStatus { None, Stun, Poisoned, Frozen, Cursed, Confused, Silent }

        // public enum MonsterState { Idle = 0, Run = 1, Skill = 2, Attack = 3, Death = 9 }

        public enum CreatureState { Idle = 0, Walk = 1, Run = 2, Attack = 3, Skill = 4, Invincible, Death = 9 }

        public enum InGameGrade { Normal = 1, Rare = 2, Epic = 3, Legendary = 4 }
        public enum CollisionLayers { Default = 0, PlayerBody = 6, PlayerAttack = 7, MonsterBody = 8, MonsterAttack = 9 }

        public enum PlayerEmotion { None = 0, Default = 1, Greedy, Sick, Bunny, Kitty, Death }
        public enum InitialStatRatioGrade { None = 0, Low = 10, Average = 20, High = 30 }

        public const float MAX_DODGE_CHANCE = 0.6f;

        public static class LoadJson
        {
            // 모두 플레이팹 데이터 테이블에서 불러와야한다
            public const string CREATURE = "CreatureData.json";
            public const string STAGE = "Stage.json";
            public const string SKILL = "SkillData.json";
            public const string PASSIVE_SKILL = "PassiveSkillData.json";
        }

        public static class TemplateIDs
        {
            // 플레이 스타일 바꿔야할듯,,,
            // 하나의 게리 프리팹 안에 나이트, 팬텀나이트 모두 배치
            public enum Player
            {
                Gary_Paladin = 100100,
                Gary_Knight = 100101,
                Gary_PhantomKnight = 100102,

                Reina_BowMaster = 100103,
                Reina_ElementalArcher = 100104,
                Reina_Hunter = 100105,

                Kenneth_Gambler = 100106,
                Kenneth_Assassin = 100107,
                Kenneth_Ninja = 100108,

                Lionel_Barbarian = 100109,
                Lionel_Warrior = 100110,
                Lionel_Berserker = 100111,

                Fernando_Commander = 100112,
                Fernando_Desperado = 100113,
                Fernando_HeavyShooter = 100113,

                Chloe_Archmage = 100114,
                Chloe_Trickster = 100115,
                Chloe_Summoner = 100116,

                Stigma_UndeadKing = 100117,
                Stigma_Pirate = 100118,
                Stigma_Mutant = 100119,

                Eleanor_Queen = 100120 // Legendary Hidden Character from the start
            }

            public enum Monster
            {
                Chicken = 100200,
            }

            public enum SkillType
            {
                None = 200000,
                
                PaladinSwing = 200100,
                KnightSwing,
                PhantomKnightSwing,

                ThrowingStar = 200200,
                LazerBolt = 200204,
                Boomerang = 200208,
                Spear = 200212,
                BombTrap = 200216,

                BodyAttack = 200300,
            }
        }

        // 나중에 데이터 시트로 빼서 어드레서블에서 세팅하고 불러와서 사용해도 됨(사실 이 부분이 없어야함)
        public static class PrefabLabels
        {
            public const string GARY_UPGRADE_SPRITE_TEMP = "04_PhantomKnight_Legendary_Temp.prefab";

            public const string JOYSTICK = "UI_Joystick.prefab";
            public const string EXP_GEM = "EXPGem.prefab";

            public const string DMG_NUMBER_TO_MONSTER = "DmgNumber_ToMonster.prefab";
            public const string DMG_NUMBER_TO_MONSTER_CRITICAL = "DmgNumber_ToMonsterCritical.prefab";
            public const string DMG_NUMBER_TO_PLAYER = "DmgNumber_ToPlayer.prefab";

            public const string DMG_TEXT_TO_MONSTER_CRITICAL = "DmgText_ToMonsterCritical.prefab"; // 크리티컬 text는 몬스터에게만 적용
            public const string DMG_TEXT_TO_PLAYER_DODGE = "DmgText_ToPlayerDodge.prefab"; // 플레이어에게만 적용
            public const string SPAWN_EFFECT = "SpawnEffect.prefab";
            public const string STUN_EFFECT = "StunEffect.prefab";

            public const string GEM = "Gem.prefab";
            public const string GEM_GATHER = "GemGather.prefab";
            public const string GEM_EXPLOSION_NORMAL = "GemExplosion_Normal.prefab";
            public const string GEM_EXPLOSION_LARGE = "GemExplosion_Large.prefab";

            public const string DUST = "Dust.prefab";
        }

        public static class SpriteLabels
        {
            public const string EXP_GEM_GREEN = "EXPGem_01.sprite";
            public const string EXP_GEM_YELLOW = "EXPGem_02.sprite";
            public const string EXP_GEM_BLUE = "EXPGem_03.sprite";

            public const string GEM_NORMAL = "Gem_Normal.sprite";
            public const string GEM_LARGE = "Gem_Large.sprite";

            public static class Player
            {
                // PLAYER FACE SET
                // DEFAULT = 0
                public const string EYEBROWS_DEFAULT = "Eyebrows_Default.sprite";
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

            public enum MonsterFace { Normal = 0, Angry = 1, Death = 2 }
        }

        public static class MaterialLabels
        {
            public const string MAT_HIT_WHITE = "HitWhite.mat";
            public const string MAT_HIT_RED = "HitRed.mat";
            public const string MAT_FADE = "Fade.mat";
            public const string MAT_GLITCH = "Glitch.mat";
            public const string MAT_HOLOGRAM = "Hologram.mat";
            public const string MAT_STRONG_TINT_WHITE = "StrongTintWhite.mat";
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

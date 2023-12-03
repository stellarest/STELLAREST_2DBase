
#define USE_LINQ

namespace STELLAREST_2D
{
    public static class Define
    {
        // enum temps
        public enum InGameStage { Forest, Volcano } // temp
        /*
            +++++ In Game Difficulty +++++
            * Normal, Hard, Expert : Forest (FIXED_FOREST_STAGE_MAX_WAVE_COUNT : 20) (per stage level, total 60)
            * Master, Extreme : Volcano (FIXED_VOLCANO_STAGE_MAX_WAVE_COUNT : 30) (per stage level, total 60)
        */
        public enum InGameDifficulty { Normal, Hard, Expert, Master, Extreme } // temp
        public enum WaveType { None, Elite, MiddleBoss, Boss } // temp
        public enum Scene { Unknown, DevScene, GameScene, } // temp
        public enum Sound { BGM, SFX, } // temp
        public enum StageType { Normal, MiddleBoss, Boss, } // temp


        public enum UIEvent { Click, Pressed, PointerDown, PointerUp, BeginDrag, Drag, EndDrag, }
        public enum InitialStatDescGrade { None, VeryLow, Low, Average, High, VeryHigh }
        public enum ObjectType { None = -1, Player = 1, Monster, Skill, Projectile, Gem, Soul }
        public enum MonsterType { None = -1, Chicken = 1, }
        public enum InGameGrade { Default = 1, Elite, Ultimate }
        public enum SortingOrder { Map = 100, Player = 200, Item = 209, Monster = 210, Skill = 230, EnvEffect = 255 }
        public enum SkillType { None = -1, Unique = 1, Public = 2 }
        public enum CreatureState { Idle = 0, Walk = 1, Run = 2, Skill = 3, Invincible = 4, Dead = 999 }
        public enum CollisionLayers { Default = 0, PlayerBody = 6, PlayerAttack = 7, MonsterBody = 8, MonsterAttack = 9 }
        public enum LookAtDirection { Left = 1, Right = -1 }
        public enum HitFromType { None = -1, ThrowingStar = 1, LazerBolt = 2, All = 9 }
        public enum MaterialType { None = -1, Hit = 1, Hologram = 2, FadeOut = 3, StrongTint, InnerOutline }
        public enum MaterialColor { UsePreset = -1, White, Red, Green, }
        public enum FaceType { Default = 1, Combat, Dead, Bunny, }
        public enum GemSize { Normal = 1, Large = 2 }
        public enum StrongTintColor { White, Red, Green }
        public enum SkillAnimationType { None = -1, Attack = 101, ElitePlus = 201, C1ElitePlus = 202, UltimatePlus = 301, }


        public static class TemplateIDs
        {
            public enum CrowdControl
            {
                None = -1,
                Stun = 300100,
                Slow = 300101,
                KnockBack = 300102,
                Slience = 300103,
                Blind = 300104,
                Charm = 300105,
                Flee = 300106,
                Sleep = 300107,
                Targeted = 300108,
                Poison = 300109,
                MaxCount = 10
            }

            public static class VFX
            {
                public enum Muzzle
                {
                    None = -1,
                    Bow,
                }

                public enum ImpactHit
                {
                    None = -1,
                    Hit = 400100,
                    Leaves = 400101,
                    Light = 400102,
                    SmokePuff = 400103,
                    Incinvible = 400104,
                    Poison = 400105,
                }

                public enum Trail
                {
                    None = -1,
                    Wind,
                    Light,
                }

                public enum Environment
                {
                    None = -1,
                    Spawn,
                    Damage,
                    Dodge,

                    Skull,
                    Dust,
                    Stun,
                    Slow,
                    Silence,
                    Targeted,
                    Poison,

                    GemGather,
                    GemExplosion,
                    Font_Percentage,
                    QuestionMark,

                    KnockBack,
                    WindTrail,
                    Max = 999,
                }
            }
        }

        #region Public Constant Value
        /// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        /// +++++ Sprite Addressable Key Name == File Name !! (ex) Mouth_Sick.sprite = Mouth_Sick +++++
        /// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static class FixedValue
        {
            public static class Load
            {
                // GameScene
                public const string UI_JOYSTICK = "UI_Joystick.prefab";

                // ObjectManager
                public const string ENV_GEM = "Env_Gem.prefab";
                public const string ENV_SOUL = "Env_Soul.prefab";

                // GemController
                public const string SPRITE_GEM_NORMAL = "Gem_Normal.sprite";
                public const string SPRITE_GEM_LARGE = "Gem_Large.sprite";

                // DataManager
                public const string CREATURE_DATA = "CreatureData.json";
                public const string SKILL_DATA = "SkillData.json";

                // VFXManager
                public const string MAT_HIT_WHITE = "HitWhite.mat";
                public const string MAT_HIT_RED = "HitRed.mat";
                public const string MAT_HOLOGRAM = "Hologram.mat";
                public const string MAT_FADE = "Fade.mat";
                public const string MAT_STRONG_TINT = "StrongTint.mat";
                public const string MAT_INNER_OUTLINE = "InnerOutline.mat";
                public const string MAT_POISON = "Poison.mat";
                public const string SO_SPT_BLOB = "SO_SPT_BLOB.asset";
                public const string SO_SPT_POS = "SO_SPT_POS.asset";
                public const string VFX_MUZZLE_BOW = "VFX_Muzzle_Bow.prefab";
                public const string VFX_IMPACT_HIT_DEFAULT = "VFX_Impact_Hit_Default.prefab";
                public const string VFX_IMPACT_HIT_LEAVES = "VFX_Impact_Hit_Leaves.prefab";
                public const string VFX_IMPACT_HIT_LIGHT = "VFX_Impact_Hit_Light.prefab";
                public const string VFX_IMPACT_HIT_SMOKE_PUFF = "VFX_Impact_Hit_SmokePuff.prefab";
                public const string VFX_IMPACT_HIT_INVINCIBLE = "VFX_Impact_Hit_Invincible.prefab";
                public const string VFX_IMPACT_HIT_POISON = "VFX_Impact_Hit_Poison.prefab";
                public const string VFX_TRAIL_WIND = "VFX_Trail_Wind.prefab";
                public const string VFX_TRAIL_LIGHT = "VFX_Trail_Light.prefab";
                public const string VFX_ENV_SPAWN = "VFX_Env_Spawn.prefab";
                public const string VFX_ENV_DAMAGE_TO_MONSTER = "VFX_Env_Damage_To_Monster.prefab";
                public const string VFX_ENV_DAMAGE_TO_MONSTER_CRITICAL = "VFX_Env_Damage_To_Monster_Critical.prefab";
                public const string VFX_ENV_DAMAGE_TO_MONSTER_CRITICAL_FONT = "VFX_Env_Damage_To_Monster_Critical_Font.prefab";
                public const string VFX_ENV_DAMAGE_TO_PLAYER = "VFX_Env_Damage_To_Player.prefab";
                public const string VFX_ENV_DAMAGE_TO_PLAYER_SHIELD = "VFX_Env_Damage_To_Player_Shield.prefab";
                public const string VFX_ENV_DAMAGE_TO_PLAYER_DODGE_FONT = "VFX_Env_Damage_To_Player_Dodge_Font.prefab";
                public const string VFX_ENG_DAMAGE_POISON = "VFX_Env_Damage_Poison.prefab";
                public const string VFX_ENV_SKULL = "VFX_Env_Skull.prefab";
                public const string VFX_ENV_DUST = "VFX_Env_Dust.prefab";
                public const string VFX_ENV_STUN = "VFX_Env_Stun.prefab";
                public const string VFX_ENV_SLOW = "VFX_Env_Slow.prefab";
                public const string VFX_ENV_SILENCE = "VFX_Env_Silence.prefab";
                public const string VFX_ENV_GEM_GATHER = "VFX_Env_GemGather.prefab";
                public const string VFX_ENV_GEM_EXPLOSION = "VFX_Env_GemExplosion.prefab";
                public const string VFX_ENV_FONT_PERCENTAGE = "VFX_Env_Font_Percentage.prefab";
                public const string VFX_ENV_TARGETED = "VFX_Env_Targeted.prefab";
                public const string VFX_ENV_FONT_HIT = "VFX_Env_Font_Hit.prefab";
                public const string VFX_ENV_QUESTION_MARK = "VFX_Env_QuestionMark.prefab";
            }

            public static class Find
            {
                // CreatureController
                public const string INDICATOR = "Indicator";
                public const string FIRE_SOCKET = "FireSocket";
                public const string ANIMATION_BODY = "AnimationBody";
                public const string PLAYER_HAIR = "Hair";

                // PlayerController
                public const string PLAYER_ARM_LEFT = "ArmL";
                public const string PLAYER_ARM_RIGHT = "ArmR[1]";
                public const string PLAYER_FOREARM_LEFT_1 = "ForearmL[1]";
                public const string PLAYER_FOREARM_LEFT_2 = "ForearmL[2]";
                public const string PLAYER_HAND_LEFT = "HandL";
                public const string PLAYER_HAND_RIGHT = "HandR";
                public const string PLAYER_LEG_LEFT = "Leg[L]";
                public const string PLAYER_LEG_RIGHT = "Leg[R]";
                public const string PLAYER_MELEE_WEAPON = "MeleeWeapon";
                public const string PLAYER_SHIELD = "Shield";
                public const string PLAYER_BOW = "Bow";
                public const string PLAYER_PELVIS = "Pelvis";
            }

            public static class Numeric
            {
                public const int FOREST_STAGE_MAX_WAVE_COUNT = 20; // temp (memo)
                public const int VOLCANO_STAGE_MAX_WAVE_COUNT = 30; // temp (memo)

                // VFXManager
                public const float HIT_DURATION = 1F;
                public const float HOLOGRAM_SPEED_POWER = 20F;
                public const float FADE_OUT_DURATION = 1.25F;
                public const float INSTANT_FADE_ALPHA = 0.25F;
                public const float INNER_OUTLINE_FADE_PING_PONG_INTERVAL = 0.5F;
                public const float POISON_PING_PONG_INTERVAL = 0.5F;

                // CreatureStat
                public const float INITIAL_COLLECT_RANGE = 3F;
                public const float INGAME_MAX_DODGE_CHANCE = 0.7F; // temp (memo)
                public const float INGAME_MAX_ARMOR_RATE = 0.8F; // temp (memo)

                // PlayerController
                public const float PLAYER_LOCAL_SCALE_X = 1.25F;
                public const float PLAYER_LOCAL_SCALE_Y = 1.25F;
                public const float MINIMUM_ENV_COLLECT_RANGE = 5F;

                // ChickenController
                public const float STANDARD_CREATURE_SHAKE_DURATION = 0.75F;
                public const float STANDARD_CREATURE_SHAKE_POWER = 0.5F;
            }

            public static class TemplateID
            {
                public enum Player
                {
                    Gary_Paladin = 100100,
                    Gary_Knight = 100200,
                    Gary_PhantomKnight = 100300,

                    Reina_ArrowMaster = 200100,
                    Reina_ElementalArcher = 200200,
                    Reina_ForestGuardian = 200300,

                    Kenneth_Assassin = 300100,
                    Kenneth_Ninja = 300200,
                    Kenneth_Thief = 300300,
                }

                public enum Monster
                {
                    Chicken = 900100,
                }

                public enum Skill
                {
                    None = -1,

                    // +++ PALADIN +++
                    PaladinMastery = 100100,
                    Shield_Elite_Solo = 100103, // ELITE_SOLO 이름 바꿔야할듯,,,
                    JudgementOfHeaven_Ultimate_Solo = 100106,

                    // +++ KNIGHT +++
                    KnightMastery = 100200,
                    SecondWind_Elite_Solo = 100203,
                    StormBlade_Ultimate_Solo = 100206,

                    // +++ PHANTOM KNIGHT +++
                    PhantomKnightMastery = 100300,
                    SummonPhantomSoul_Elite_Solo = 100303,
                    PhantomSoul_Elite_Solo = 100306,
                    Metamorphosis_Ultimate_Solo = 100309,

                    // +++ ARROW MASTERY +++
                    ArrowMasterMastery = 200100,
                    Concentration_Elite_Solo = 200103,
                    ArrowShower_Ultimate_Solo = 200106,

                    // +++ ELEMENTAL ARCHER +++
                    ElementalArcherMastery = 200200,
                    ElementalShock_Elite_Solo = 200203,
                    ElementalCharge_Ultimate_Solo = 200206,

                    // +++ FOREST GUARDIAN +++
                    ForestGuardianMastery = 200300,
                    ForestBarrier_Elite_Solo = 200303,
                    SummonBlackPanther_Ultimate_Solo = 200306,

                    // +++ ASSASSIN +++
                    AssassinMastery = 300100,
                    PoisonDagger_Elite_Solo = 300103,
                    // SkillGroupDictionary, DictionaryGroupMaxCount(_numberGroups)으로 인해서 현재는 3씩 증가시켜야함 (임시)
                    StabPoisonDagger_Elite_Solo = 300106,
                    CounterStrike_Ultimate_Solo = 300109,

                    // +++ NINJA +++
                    NinjaMastery = 300200,
                    Cloak_Elite_Solo = 300203,
                    NinjaSlash_Elite_Solo = 300206,
                    CloneTechnique_Ultimate_Solo = 300209,

                    // +++ PUBLIC +++
                    ThrowingStar = 900100,
                    Boomerang = 900103,
                    LazerBolt = 900106,
                    Spear = 900109,
                    BombTrap = 900112,

                    BodyAttack_Solo = 901100
                }
            }
        }
        #endregion
    }
}

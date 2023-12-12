
// #define USE_LINQ

namespace STELLAREST_2D
{
    public static class Define
    {
        public enum InGameStage { Forest, Volcano } // temp
        public enum InGameDifficulty { Normal, Hard, Expert, Master, Extreme } // temp
        public enum WaveType { None, Elite, Boss } // temp
        public enum SceneType { Unknown, DevScene, GameScene, } // temp
        public enum SoundType { BGM, SFX, } // temp
        public enum StageType { Normal, Boss, } // temp
        public enum UIEvent { Click, Pressed, PointerDown, PointerUp, BeginDrag, Drag, EndDrag, }
        public enum ObjectType { None = -1, Player = 1, Monster, Skill, Projectile, Gem, Soul }
        public enum MonsterType { None = -1, Chicken = 1, }

        // Default - Yellow - Purple - Red
        public enum InGameGrade { Default = 1, Elite, Ultimate }

        public enum SortingOrder { Map = 100, Player = 200, Item = 209, Monster = 210, Skill = 230, EnvEffect = 255 }
        public enum CreatureState { Idle, Walk, Run, Skill, Invincible, Dead }
        public enum CollisionLayers { Default = 0, PlayerBody = 6, PlayerAttack = 7, MonsterBody = 8, MonsterAttack = 9 }
        public enum LookAtDirection { Left = 1, Right = -1 }
        public enum HitFromType { None = -1, ThrowingStar = 1, LazerBolt = 2, All = 9 }
        public enum MaterialType { None = -1, Hit = 1, Hologram = 2, FadeOut = 3, StrongTint, InnerOutline }
        public enum MaterialColor { UsePreset = -1, White, Red, Green, }
        public enum FaceType { Default = 1, Combat, Dead, Bunny, }
        public enum GemSize { Normal = 1, Large = 2 }
        public enum StrongTintColor { White, Red, Green }
        public enum VFXMuzzleType { None = -1, White, }
        public enum VFXTrailType { None = -1, Wind, Light }
        public enum VFXEnvType
        {
            None = -1, Spawn, Damage, Dodge, Skull, Dust, Stun, Slow, Silence, Targeted, Poison,
            GemGather, GemExplosion, Font_Percentage, QuestionMark, KnockBack, WindTrail, Max = 999,
        }

        public enum SkillType { None = -1, Unique = 100, Public = 200 }
        public enum SkillAnimationType { None = -1, Unique_Mastery = 100, Unique_Elite = 200, Unique_Elite_C1 = 201, Unique_Ultimate = 300, }
        public enum VFXImpactHitType { None = -1, Hit = 100, Leaves, Light, SmokePuff, Incinvible, Poison, }
        // Prev Stun : 300100
        public enum CrowdControlType
        {
            None = -1, Stun = 100, Slow, KnockBack, Silence, Blind, Charm,
            Flee, Sleep, Targeted, Poisoned, Frozen, MaxCount = 10
        }

        #region Public Fixed Value
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
                public const string VFX_MUZZLE_WHITE = "VFX_Muzzle_White.prefab";
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
                // Managers
                public const string MANAGERS = "@Managers";
                public const string JOYSTICK = "@Joystick";

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

                // CreatureAnimationController
                public const string ANIM_PARAM_CREATURE_IDLE = "Idle";
                public const string ANIM_PARAM_CREATURE_RUN = "Run";
                public const string ANIM_PARAM_CREATURE_STUN = "Stun";
                public const string ANIM_PARAM_CREATURE_DEAD = "Dead";
                public const string ANIM_PARAM_CREATURE_ANIM_SPEED = "AnimationSpeed";
                public const string ANIM_PARAM_CREATURE_MOVEMENT_SPEED = "MovementSpeed";
                public const string ANIM_PARAM_CREATURE_ENTER_NEXT_STATE_TRIGGER = "EnterNextState";
                public const string ANIM_PARAM_CREATURE_ENTER_NEXT_STATE_BOOLEAN = "CanEnterNextState";

                // PlayerAnimationController, Player(Childs)AnimationController
                public const string ANIM_PARAM_PLAYER_READY_MELEE_1H = "ReadyMelee1H";
                public const string ANIM_PARAM_PLAYER_READY_MELEE_2H = "ReadyMelee2H";
                public const string ANIM_PARAM_PLAYER_READY_BOW = "ReadyBow";
                public const string ANIM_PARAM_PLAYER_SLASH_MELEE_1H = "SlashMelee1H";
                public const string ANIM_PARAM_PLAYER_SLASH_MELEE_2H = "SlashMelee2H";
                public const string ANIM_PARAM_PLAYER_RANGED_ARROW_SHOT = "RangedArrowShot";
                public const string ANIM_PARAM_PLAYER_JAB_MELEE_1H = "JabMelee1H";
                public const string ANIM_PARAM_PLAYER_JAB_MELEE_PAIRED_ULTIMATE = "JabMelee_Paired_Ultimate";
                public const string ANIM_PARAM_PLAYER_THROW_KUNAI = "ThrowKunai";
                public const string ANIM_PARAM_PLAYER_THROW_KUNAI_ELITE = "ThrowKunai_Elite";
                public const string ANIM_PARAM_PLAYER_THROW_KUNAI_ULTIMATE = "ThrowKunai_Ultimate";

                public const string ANIM_PARAM_PLAYER_SHIELD = "Shield";
                public const string ANIM_PARAM_PLAYER_SECOND_WIND = "SecondWind";
                public const string ANIM_PARAM_PLAYER_PHANTOM_SOUL = "PhantomSoul";

                public const string ANIM_PARAM_PLAYER_CONCENTRATION = "Concentration";
                public const string ANIM_PARAM_PLAYER_ELEMENTAL_SHOCK = "ElementalShock";
                public const string ANIM_PARAM_PLAYER_FOREST_BARRIER = "ForestBarrier";

                public const string ANIM_PARAM_PLAYER_POISON_DAGGER = "PoisonDagger";
                public const string ANIM_PARAM_PLAYER_CLOAK = "Cloak";
                public const string ANIM_PARAM_PLAYER_NINJA_SLASH = "NinjaSlash";

                // MonsterAnimationController
                public const string ANIM_PARAM_MONSTER_ATTACK = "Attack";
            }

            public static class Numeric
            {
                public const int FOREST_STAGE_MAX_WAVE_PER_STAGE_LEVELS = 20; // temp (memo)
                public const int VOLCANO_STAGE_MAX_WAVE_PER_STAGE_LEVELS = 30; // temp (memo)

                // CreatureStat
                public const float INITIAL_ARMOR_RATE_NONE = 0F;
                public const float INITIAL_ARMOR_RATE_LOW = 0.03F;
                public const float INITIAL_ARMOR_RATE_AVERAGE = 0.1F;
                public const float INITIAL_ARMOR_RATE_HIGH = 0.3F;

                public const float CREATURE_MAX_ARMOR_RATE = 0.6F;
                public const float CREATURE_MAX_DODGE_CHANCE = 0.7F;

                public const float INITIAL_MOVEMENT_SPEED_LOW = 8F;
                public const float INITIAL_MOVEMENT_SPEED_AVERAGE = 9.5F;
                public const float INITIAL_MOVEMENT_SPEED_HIGH = 12F;

                public const float CREATURE_MAX_MOVEMENT_SPEED = 20F;
                public const float CREATURE_MAX_MOVEMENT_SPEED_ANIM_MULTIPLIER = 3F;

                // VFXManager
                public const float MAT_HIT_DURATION = 0.1F;
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

            // Data Dictionary에서 사용되어지는 목록,,
            public static class TemplateID
            {
                public enum Player
                {
                    None = -1,
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
                    None = -1,
                    Chicken = 800100,
                }

                public enum Skill
                {
                    None = -1,

                    // ***** Player Unique Skills ID *****
                    Paladin_Unique_Mastery = 100100,
                    Paladin_Unique_Elite = 100103,  // Shield
                    Paladin_Unique_Ultimate = 100106, // Judgement

                    KnightMastery = 100200,
                    Knight_Unique_Elite = 100203, // Second Wind
                    Knight_Unique_Ultimate = 100206, // Storm Blade

                    PhantomKnightMastery = 100300,
                    PhantomKnight_Unique_Elite = 100303, // Summon : Phantom Soul
                    PhantomKnight_Unique_Elite_C1 = 100306, // Phantom Soul
                    PhantomKnight_Unique_Ultimate = 100309, // Metamorphosis

                    ArrowMasterMastery = 200100,
                    ArrowMaster_Unique_Elite = 200103, // Concentration
                    ArrowMaster_Unique_Ultimate = 200106, // Arrow Time

                    ElementalArcherMastery = 200200,
                    ElementalArcher_Unique_Elite = 200203, // Elemental Shock
                    ElementalArcher_Unique_Ultimate = 200206, // Elemental Charge

                    ForestGuardianMastery = 200300,
                    ForestGuardian_Unique_Elite = 200303, // Forest Barrier
                    ForestGuardian_Unique_Ultimate = 200306, // Summon : Battle Panther

                    // SkillGroupDictionary, DictionaryGroupMaxCount(_numberGroups)으로 인해서 현재는 3씩 증가시켜야함 (임시)
                    AssassinMastery = 300100,
                    Assassin_Unique_Elite = 300103, // PoisonDagger
                    Assassin_Unique_Elite_C1 = 300106, // Stab - PoisonDagger
                    Assassin_Unique_Ultimate = 300109, // Counter Strike

                    NinjaMastery = 300200,
                    Ninja_Unique_Elite = 300203, // Cloak
                    Ninja_Unique_Elite_C1 = 300206, // Ninja Slash
                    Ninja_Unique_Ultimate = 300209, // Clone Technique

                    // ***** Monster Unique Skills ID *****
                    Monster_Unique_BodyAttack = 800100,

                    // ***** Public Skills ID *****
                    ThrowingStar = 900100,
                    Boomerang = 900103,
                    LazerBolt = 900106,
                    Spear = 900109,
                    BombTrap = 900112,

                }
            }
        }
        #endregion
    }
}

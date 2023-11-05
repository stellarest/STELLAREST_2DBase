
#define USE_LINQ

namespace STELLAREST_2D
{
    public static class Define
    {
        public const float MAX_DODGE_CHANCE = 0.6f;
        public const float MAX_ARMOR_RATE = 0.8f;

        public const float PLAYER_LOCAL_SCALE_X = 1.25f; // Initial ScaleX : 0.8f
        public const float PLAYER_LOCAL_SCALE_Y = 1.25f; // Initial ScaleY : 0.8f

        public const string INDICATOR = "Indicator";
        public const string FIRE_SOCKET = "FireSocket";
        public const string ANIMATION_BODY = "AnimationBody";

        public enum UIEvent { Click, Pressed, PointerDown, PointerUp, BeginDrag, Drag, EndDrag, }
        public enum Scene { Unknown, DevScene, GameScene, }
        public enum Sound { BGM, Effect, }
        public enum ObjectType { None = -1, Player = 1, Monster, Skill, Projectile, Gem, Soul }
        public enum MonsterType { None = -1, Chicken = 1, }

        public enum InGameGrade { Default = 1, Elite, Ultimate }
        public enum InGameMode { Forest, Volcano }
        public enum InGameDifficulty { Normal, Hard, Expert, Master, Extreme }
        public const int STAGE_MAX_WAVE_COUNT = 20;
        public const int ABILITY_MAX_POINT = 8;
        public enum WaveType { None, Elite, MiddleBoss, Boss }
        public enum SortingOrder { Map = 100, Player = 200, Item = 209, Monster = 210, Skill = 230, EnvEffect = 255 }
        public enum StageType { Normal, MiddleBoss, Boss, }
        // public enum CCStatus { None, Stun, KnockBack, Poisoned, Frozen, Cursed, Confused, Silent, Max }
        public enum SkillType { None = -1, Repeat = 1, Sequence }
        public enum CreatureState { Idle = 0, Walk = 1, Run = 2, Skill = 3, Invincible = 4, Dead = 999 }
        //public enum InGameGrade { Normal = 1, Elite = 2, Ultimate = 3 }
        public enum CollisionLayers { Default = 0, PlayerBody = 6, PlayerAttack = 7, MonsterBody = 8, MonsterAttack = 9 }
        public enum InitialStatRatioGrade { None = 0, Low = 5, Average = 15, High = 20 }
        public enum LookAtDirection { Left = 1, Right = -1 }
        public enum ExpressionType { Default, Battle, Concentration, Angry, Kitty, Sick, Death, }
        public enum MonsterFace { Normal = 0, Angry = 1, Death = 2 }
        public enum HitFromType { None = -1, ThrowingStar = 1, LazerBolt = 2, All = 9 }
        //public enum ImpactHits { None, Leaves, CriticalHit, }
        public enum MaterialType { None = -1, Hit = 1, Hologram = 2, FadeOut = 3, StrongTintWhite }
        public enum FaceExpressionType { Default = 1, Battle, Dead, }

        public enum GemSize { Normal = 1, Large = 2 }
        public enum SkillAnimationType { None = -1, ExclusiveRepeat = 1, EliteSequence = 2, UltimateSequence = 3 }

        public static class TemplateIDs
        {
            public static class Creatures
            {
                public enum Player
                {
                    Gary_Paladin = 100100,
                    Gary_Knight = 100101,
                    Gary_PhantomKnight = 100102,

                    Reina_ArrowMaster = 100103,
                    Reina_ElementalArcher = 100104,
                    Reina_ForestGuardian = 100105,

                    Kenneth_Assassin = 100106,
                    Kenneth_Thief = 100107,
                    Kenneth_Ninja = 100108,

                    Lionel_Warrior = 100109,
                    Lionel_Barbarian = 100110,
                    Lionel_Berserker = 100111,

                    Christian_Hunter = 100112,
                    Christian_Desperado = 100113,
                    Christian_Destroyer = 100114,

                    Chloe_Archmage = 100115,
                    Chloe_Trickster = 100116,
                    Chloe_Frostweaver = 100117,

                    Stigma_SkeletonKing = 100118,
                    Stigma_Pirate = 100119,
                    Stigma_Mutant = 100120,

                    Eleanor_Queen = 100121 // +++ Special Hidden Character : Will be very fun !! +++
                }

                public enum Monster
                {
                    Chicken = 100200,
                }
            }

            public static class Status
            {
                public enum Stat
                {
                    None = -1,

                    MaxHpUp_Normal = 200100,
                    MaxHpUp_Rare = 200101,
                    MaxHpUp_Epic = 200102,
                    MaxHpUp_Legendary = 200103,

                    ArmorUp_Normal = 200160,
                    ArmorUp_Rare = 200161,
                    ArmorUp_Epic = 200162,
                    ArmorUp_Legendary = 200163,
                }

                public enum Skill
                {
                    None = -1,

                    // Exclusive Skills
                    // +++ GARY +++
                    PaladinMastery = 200200,
                    KnightMastery = 200203,
                    PhantomKnightMastery = 200206,

                    // +++ REINA +++
                    ArrowMasterMastery = 200209,
                    ElementalArcherMastery = 200212,
                    ForestGuardianMastery = 200215,

                    // +++ KENNETH +++
                    AssassinMastery = 200218,
                    ThiefMastery = 200221,
                    NinjaMastery = 200224,

                    // +++ LIONEL +++
                    // WarriorMeleeSwing = 200236,
                    // BarbarianRangedShot = 200240,
                    // BerserkerMeleeSwing = 200244,

                    // // +++ CHRISTIAN +++
                    // HunterRangedShot = 200248,
                    // DesperadoRangedShot = 200252,
                    // DestroyerRangedShot = 200256,

                    // // +++ CHLOE +++
                    // ArchmageRangedMagicShot = 200260,
                    // TricksterRangedMagicShot = 200264,
                    // FrostWeaverRangedMagicShot = 200268,

                    // // +++ STIGMA +++
                    // SkeletonKingMeleeSwing = 200272,
                    // PirateMeleeSwing = 200276,
                    // DeathClaw = 200280,

                    // // +++ ELEANOR +++
                    // QueenMeleeSwing = 200284,

                    // Public Skills (+ Monster)
                    ThrowingStar = 200300,
                    Boomerang = 200303,
                    LazerBolt = 200306,
                    Spear = 200309,
                    BombTrap = 200312,
                    // LazerBolt = 200304,
                    // Boomerang = 200308,
                    // Spear = 200312,
                    // BombTrap = 200316,

                    BodyAttack = 201100,
                    Shield_Elite = 201101,
                    SecondWind_Elite = 201102,
                    PhantomSoul_Elite = 201103,
                    PhantomSoul_Elite_Child = 201104
                    // HeavensJudgment,
                    // GuardiansShield,
                    // Concentration,
                }            
            }

            public enum CrowdControl
            {
                None = -1,
                Stun = 300100,
                Slow = 300101,
                KnockBack = 300102,
                Slience = 300103,
                Blind = 300104, // ??? 뺼까
                Charm = 300105,
                Flee = 300106,
                Sleep = 300107,
                MaxCount = 8
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
                    GemGather,
                    GemExplosion,
                    Font_Percentage,



                    KnockBack,
                    WindTrail,
                    Max = 999,
                }
            }

            // +++++ TEMP +++++
            public enum ItemType
            {
                //PiggyBank = 600101,
            }
        }

        public static class Labels
        {
            public static class Prefabs
            {
                // ENV
                public const string UI_JOYSTICK = "UI_Joystick.prefab";


                // VFX_MUZZLE
                public const string VFX_MUZZLE_BOW = "VFX_Muzzle_Bow.prefab";

                // VFX_IMPACT
                public const string VFX_IMPACT_HIT_DEFAULT = "VFX_Impact_Hit_Default.prefab";
                public const string VFX_IMPACT_HIT_LEAVES = "VFX_Impact_Hit_Leaves.prefab";
                public const string VFX_IMPACT_HIT_LIGHT = "VFX_Impact_Hit_Light.prefab";
                public const string VFX_IMPACT_HIT_SMOKE_PUFF = "VFX_Impact_Hit_SmokePuff.prefab";
                public const string VFX_IMPACT_HIT_INVINCIBLE = "VFX_Impact_Hit_Invincible.prefab";

                // VFX Trail
                public const string VFX_TRAIL_WIND = "VFX_Trail_Wind.prefab";
                public const string VFX_TRAIL_LIGHT = "VFX_Trail_Light.prefab";

                // VFX_ENV : Spawn, Damage, Dodge, Skull, Dust
                public const string VFX_ENV_SPAWN = "VFX_Env_Spawn.prefab";
                public const string VFX_ENV_DAMAGE_TO_MONSTER = "VFX_Env_Damage_To_Monster.prefab";
                public const string VFX_ENV_DAMAGE_TO_MONSTER_CRITICAL = "VFX_Env_Damage_To_Monster_Critical.prefab";
                public const string VFX_ENV_DAMAGE_TO_MONSTER_CRITICAL_FONT = "VFX_Env_Damage_To_Monster_Critical_Font.prefab";
                public const string VFX_ENV_DAMAGE_TO_PLAYER = "VFX_Env_Damage_To_Player.prefab";
                public const string VFX_ENV_DAMAGE_TO_PLAYER_SHIELD = "VFX_Env_Damage_To_Player_Shield.prefab";
                public const string VFX_ENV_DAMAGE_TO_PLAYER_DODGE_FONT = "VFX_Env_Damage_To_Player_Dodge_Font.prefab";
                public const string VFX_ENV_SKULL = "VFX_Env_Skull.prefab";
                public const string VFX_ENV_DUST = "VFX_Env_Dust.prefab";
                public const string VFX_ENV_STUN = "VFX_Env_Stun.prefab";
                public const string VFX_ENV_SLOW = "VFX_Env_Slow.prefab";
                public const string VFX_ENV_SILENCE = "VFX_Env_Silence.prefab";
                public const string VFX_ENV_GEM_GATHER = "VFX_Env_GemGather.prefab";
                public const string VFX_ENV_GEM_EXPLOSION = "VFX_Env_GemExplosion.prefab";
                public const string VFX_ENV_FONT_PERCENTAGE = "VFX_Env_Font_Percentage.prefab";

                // ENV
                public const string ENV_GEM = "Env_Gem.prefab";
                public const string ENV_SOUL = "Env_Soul.prefab";


                // TEMP
                // =====================================================================================
                // =====================================================================================
                // =====================================================================================
                public const string STUN_EFFECT = "StunEffect.prefab";
                public const string GEM_GATHER = "GemGather.prefab";
                public const string GEM_EXPLOSION_NORMAL = "GemExplosion_Normal.prefab";
                public const string GEM_EXPLOSION_LARGE = "GemExplosion_Large.prefab";
                public const string CURSED_TEXT_EFFECT = "CursedTextEffect.prefab";
                public const string ARROW_SHOT_MUZZLE_EFFECT = "ArrowShotMuzzleEffect.prefab";
                public const string ARROW_SHOT_LEGENDARY_TRAIL_EFFECT = "ArrowShotLegendaryTrailEffect.prefab";


                public const string DEATH_CLAW_SLASH = "DeathClawSlash.prefab";
                public const string DEATH_CLAW_SLASH_LEGENDARY = "DeathClawSlash_Legendary.prefab";
                public const string IMPACT_BLOODY_EFFECT = "ImpactBloodyEffect.prefab";
                public const string WIND_TRAIL_EFFECT = "WindTrailEffect.prefab";
                public const string IMPACT_WIND_LV1_EFFECT = "ImpactWindLv1Effect.prefab";
                public const string IMPACT_WIND_LV2_EFFECT = "ImpactWindLv2Effect.prefab";
                public const string IMPACT_WIND_LV3_EFFECT = "ImpactWindLv3Effect.prefab";
                public const string FIRE_TRAIL_EFFECT = "FireTrailEffect.prefab";
                public const string IMPACT_FIRE_LV1_EFFECT = "ImpactFireLv1Effect.prefab";
                public const string IMPACT_FIRE_LV2_EFFECT = "ImpactFireLv2Effect.prefab";
                public const string IMPACT_FIRE_LV3_EFFECT = "ImpactFireLv3Effect.prefab";
                public const string ICE_TRAIL_EFFECT = "IceTrailEffect.prefab";
                public const string IMPACT_ICE_LV1_EFFECT = "ImpactIceLv1Effect.prefab";
                public const string IMPACT_ICE_LV2_EFFECT = "ImpactIceLv2Effect.prefab";
                public const string IMPACT_ICE_LV3_EFFECT = "ImpactIceLv3Effect.prefab";
                public const string BUBBLE_TRAIL_EFFECT = "BubbleTrailEffect.prefab";
                public const string IMPACT_BUBBLE_EFFECT = "ImpactBubbleEffect.prefab";
                public const string LIGHT_TRAIL_EFFECT = "LightTrailEffect.prefab";
                public const string IMPACT_LIGHT_EFFECT = "ImpactLightEffect.prefab";
                public const string IMPACT_HIT_LEAVES_EFFECT = "ImpactHitLeavesEffect.prefab";
                public const string IMPACT_CRITICAL_HIT_EFFECT = "ImpactCriticalHitEffect.prefab";
                public const string CONCENTRATION_BUFF = "ConcentrationBuff.prefab";
            }

            /// <summary>
            /// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            /// +++++ Sprite Addressable Key Name == File Name !! (ex) Mouth_Sick.sprite = Mouth_Sick +++++
            /// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            /// </summary>
            public static class Sprites
            {
                public const string GEM_NORMAL = "Gem_Normal.sprite";
                public const string GEM_LARGE = "Gem_Large.sprite";

                public const string EYEBROWS_BATTLE = "Eyebrows_Battle.sprite";
                public const string MOUTH_BATTLE = "Mouth_Battle.sprite";

                public const string EYEBROWS_ANGRY = "Eyebrows_Angry.sprite";
                public const string MOUTH_ANGRY = "Mouth_Angry.sprite";

                public const string MOUTH_CONCENTRATION = "Mouth_Concentration.sprite";

                public const string EYES_KITTY = "Eyes_Kitty.sprite";
                public const string MOUTH_KITTY = "Mouth_Kitty.sprite";

                public const string EYEBROWS_SICK = "Eyebrows_Sick.sprite";
                public const string EYES_SICK = "Eyes_Sick.sprite";
                public const string MOUTH_SICK = "Mouth_Sick.sprite";

                public const string EYEBROWS_DEATH = "Eyebrows_Death.sprite";
                public const string EYES_DEATH = "Eyes_Death.sprite";
                public const string MOUTH_DEATH = "Mouth_Death.sprite";
            }

            public static class Materials
            {
                public const string MAT_HIT_WHITE = "HitWhite.mat";
                public const string MAT_HIT_RED = "HitRed.mat";
                public const string MAT_FADE = "Fade.mat";
                public const string MAT_GLITCH = "Glitch.mat";
                public const string MAT_HOLOGRAM = "Hologram.mat";
                public const string MAT_STRONG_TINT_WHITE = "StrongTintWhite.mat";
            }

            public static class Data
            {
                public const string INITIAL_CREATURE_DATA = "InitialCreatureData.json";
                public const string CREATURE_STAT = "CreatureStatData.json";
                public const string SKILL = "SkillData.json";
                public const string CROWD_CONTROL = "CrowdControlData.json";

                // public const string SEQUENCE_SKILL = "SequenceSkillData.json";
                // public const string BUFF_SKILL = "BuffSkillData.json";
            }
        }
    }
}


#define USE_LINQ

namespace STELLAREST_2D
{
    public static class Define
    {
        public const string NameSpace = "STELLAREST_2D";
        public const float MAX_DODGE_CHANCE = 0.6f;
        public const float MAX_ARMOR_RATE = 0.8f;

        public enum UIEvent { Click, Pressed, PointerDown, PointerUp, BeginDrag, Drag, EndDrag, }
        public enum Scene { Unknown, DevScene, GameScene, }
        public enum Sound { BGM, Effect, }
        public enum ObjectType { Player, Monster, EliteMonster, Boss, Projectile, Env, }

        public enum CreatureType { Creture, Gary, Chicken, }
        
        public enum WaveType { None, Elite, MiddleBoss, Boss }
        public enum SortingOrder { Map = 100, Player = 200, Item = 209, Monster = 210, Skill = 211, ParticleEffect = 230, CCStatus = 260 }
        public enum StageType { Normal, MiddleBoss, Boss, }
        // public enum CCStatus { None, Stun, KnockBack, Poisoned, Frozen, Cursed, Confused, Silent, Max }
        public enum CCType { Stun = 1, KnockBack, Max }

        // public enum MonsterState { Idle = 0, Run = 1, Skill = 2, Attack = 3, Death = 9 }

        public enum CreatureState { Idle = 0, Walk = 1, Run = 2, Attack = 3, Skill = 4, Invincible, Death = 9 }

        public enum InGameGrade { Normal = 1, Rare = 2, Epic = 3, Legendary = 4 }
        public enum CollisionLayers { Default = 0, PlayerBody = 6, PlayerAttack = 7, MonsterBody = 8, MonsterAttack = 9 }

        public enum InitialStatRatioGrade { None = 0, Low = 10, Average = 20, High = 30 }
        public enum LookAtDirection { Left = 1, Right = -1 }

        public enum EffectLevel { Level1, level2, Level3 }
        public enum ElementalType { Fire, Ice, Wind, Bubble, }
        public enum ExpressionType { Default, Battle, Concentration, Angry, Kitty, Sick, Death, }
        public enum MonsterFace { Normal = 0, Angry = 1, Death = 2 }

        public enum ImpactHits { None, Leaves, CriticalHit, }

        public static class TemplateIDs
        {
            // ++++++++++ 나중에 하나로 통합이 가능할까? ++++++++++ 
            // GARY -> Paladin, Knight, Phantom Knight
            // 어떻게?
            public enum Player
            {
                Gary_Paladin = 100100,
                Gary_Knight = 100101,
                Gary_PhantomKnight = 100102,

                Reina_ArrowMaster = 100103,
                Reina_ElementalArcher = 100104,
                Reina_ForestWarden = 100105,

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

                Eleanor_Queen = 100121 // Special Hidden Character
            }

            public enum Monster
            {
                Chicken = 100200,
            }

            public enum SkillType
            {
                None = -1,
                
                // +++ GARY +++
                PaladinMeleeSwing = 200100,
                KnightMeleeSwing = 200104,
                PhantomKnightMeleeSwing = 200108,

                // +++ REINA +++
                ArrowMasterRangedShot = 200112,
                ElementalArcherRangedShot = 200116,
                ForestWardenRangedShot = 200120,

                // +++ KENNETH +++
                AssassinMeleeSwing = 200124,
                ThiefMeleeSwing = 200128,
                NinjaRangedShot = 200132,

                // +++ LIONEL +++
                WarriorMeleeSwing = 200136,
                BarbarianRangedShot = 200140,
                BerserkerMeleeSwing = 200144,

                // +++ CHRISTIAN +++
                HunterRangedShot = 200148,
                DesperadoRangedShot = 200152,
                DestroyerRangedShot = 200156,

                // +++ CHLOE +++
                ArchmageRangedMagicShot = 200160,
                TricksterRangedMagicShot = 200164,
                FrostWeaverRangedMagicShot = 200168,

                // +++ STIGMA +++
                SkeletonKingMeleeSwing = 200172,
                PirateMeleeSwing = 200176,
                DeathClaw = 200180,

                // +++ ELEANOR +++
                QueenMeleeSwing = 200184,

                // +++ Public Repeat Skills +++
                ThrowingStar = 200200,
                LazerBolt = 200204,
                Boomerang = 200208,
                Spear = 200212,
                BombTrap = 200216,

                BodyAttack = 200300,
            }

            public enum BonusStatType
            {
                None = -1,

                MaxHpUp_Normal = 300100,
                MaxHpUp_Rare = 300101,
                MaxHpUp_Epic = 300102,
                MaxHpUp_Legendary = 300103,


                ArmorUp_Normal = 300600,
                ArmorUp_Rare = 300601,
                ArmorUp_Epic = 300602,
                ArmorUp_Legendary = 300603,
            }

            public enum BonusBuffType
            {
                None = -1,
                GuardiansShield = 400100,
                Concentration = 400112,
            }

            public enum UltimateSequenceType
            {
                None = -1,
                HeavensJudgment = 500100,
            }

            public enum HitEffectType
            {
                None = -1,
                ImpactCriticalHitEffect = 500100,
            }


            // +++++ TEMP +++++
            public enum ItemType
            {
                //PiggyBank = 700101,
            }


            // +++++ TEMP +++++
            public enum ImpactHitEffect
            {
                Leaves = 400101, // Forest Warden Epic Effect
            }

            public enum CCType
            {

            }
        }

        public static class Player
        {
            public const string INDICATOR = "Indicator";
            public const string FIRE_SOCKET = "FireSocket";
            public const string UPGRADE_PLAYER_BUFF = "UpgradeBuff";

            public const float LOCAL_SCALE_X = 1.25f; // Initial ScaleX : 0.8f
            public const float LOCAL_SCALE_Y = 1.25f; // Initial ScaleY : 0.8f
        }

        public static class Labels
        {
            // TODO : 데이터 시트로 빼서 어드레서블에서 세팅하고 불러와서 사용해도 됨 (그렇다면 이부분 제거하고 사용할 수 있음. 나중에 고민)
            public static class Prefabs
            {
                public const string JOYSTICK = "UI_Joystick.prefab";
                public const string EXP_GEM = "EXPGem.prefab";

                public const string DMG_NUMBER_TO_MONSTER = "DmgNumber_ToMonster.prefab";
                public const string DMG_NUMBER_TO_MONSTER_CRITICAL = "DmgNumber_ToMonsterCritical.prefab";
                public const string DMG_NUMBER_TO_PLAYER = "DmgNumber_ToPlayer.prefab";
                public const string DMG_NUMBER_TO_SHIELD = "DmgNumber_ToShield.prefab";

                public const string DMG_TEXT_TO_MONSTER_CRITICAL = "DmgText_ToMonsterCritical.prefab"; // 크리티컬 text는 몬스터에게만 적용
                public const string DMG_TEXT_TO_PLAYER_DODGE = "DmgText_ToPlayerDodge.prefab"; // 플레이어에게만 적용
                public const string SPAWN_EFFECT = "SpawnEffect.prefab";
                public const string STUN_EFFECT = "StunEffect.prefab";
                public const string CURSED_TEXT_EFFECT = "CursedTextEffect.prefab";
                public const string ARROW_SHOT_MUZZLE_EFFECT = "ArrowShotMuzzleEffect.prefab";
                public const string ARROW_SHOT_LEGENDARY_TRAIL_EFFECT = "ArrowShotLegendaryTrailEffect.prefab";

                public const string GEM = "Gem.prefab";
                public const string GEM_GATHER = "GemGather.prefab";
                public const string GEM_EXPLOSION_NORMAL = "GemExplosion_Normal.prefab";
                public const string GEM_EXPLOSION_LARGE = "GemExplosion_Large.prefab";

                public const string DUST = "Dust.prefab";
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
            /// +++++ Addressable Key Name = File Name (ex) Mouth_Sick.sprite = Mouth_Sick +++++
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
                public const string CREATURE = "CreatureData.json";
                public const string STAGE = "Stage.json";
                public const string SKILL = "SkillData.json";
                public const string BONUS_STAT = "BonusStatData.json";
                public const string BONUS_BUFF = "BonusBuffData.json";
            }
        }
    }
}

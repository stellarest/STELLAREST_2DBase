using UnityEngine;
using System.Collections;

using static STELLAREST_2D.Define;
using STELLAREST_2D.Data;
using UnityEditor.Build.Pipeline.Tasks;

namespace STELLAREST_2D
{
    #region Constructor
    [System.Serializable]
    public class CreatureStat
    {
        public CreatureStat(CreatureController owner, CreatureData creatureData)
        {
            this.Owner = owner;
            this.TemplateID = (int)creatureData.TemplateID;
            this.Name = creatureData.Name;
            this.Description = creatureData.Description;

            this.InitialPowerDisplay = creatureData.StatGradeDesc_Power;
            this.InitialAttackSpeedDisplay = creatureData.StatGradeDesc_AttackSpeed;
            this.InitialArmorDisplay = creatureData.StatGradeDesc_Armor;
            this.InitialMovementSpeedDisplay = creatureData.StatGradeDesc_MovementSpeed;

            this.MaxHP = creatureData.InitialMaxHP;
            this.HP = creatureData.InitialMaxHP;

            this._initialArmor = creatureData.InitialArmor;
            this.Armor = creatureData.InitialArmor;

            this._initialMovementSpeed = creatureData.InitialMovementSpeed;
            this.MovementSpeed = creatureData.InitialMovementSpeed;

            this.MasterySkillIcon = creatureData.Icon_MasterySkill;
            this.MasterySkillDesc = creatureData.Desc_MasterySkill;

            this.UniqueEliteSkillIcon = creatureData.Icon_UniqueEliteSkill;
            this.UniqueEliteSkillDesc = creatureData.Desc_UniqueEliteSkill;

            this.UniqueUltimateSkillIcon = creatureData.Icon_UniqueUltimateSkill;
            this.UniqueUltimateSkillDesc = creatureData.Desc_UniqueUltimateSkill;
        }
        #endregion

        #region Event
        // *************************************************
        // *** Mastery Attack : Cooldown is Attack Speed ***
        // *************************************************
        public System.Action<FixedValue.TemplateID.Skill, float> OnAddSkillCooldownRatio = null;
        public System.Action<FixedValue.TemplateID.Skill> OnResetSkillCooldown = null;
        #endregion

        public CreatureController Owner { get; private set; } = null;
        public int TemplateID { get; private set; } = -1;
        [field: SerializeField] public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        public string InitialPowerDisplay { get; private set; } = string.Empty;
        public string InitialAttackSpeedDisplay { get; private set; } = string.Empty;
        public string InitialArmorDisplay { get; private set; } = string.Empty;
        public string InitialMovementSpeedDisplay { get; private set; } = string.Empty;

        #region Stat
        [field: SerializeField] public int Level { get; set; } = 0;
        [field: SerializeField] public int LevelUpPoint { get; set; } = 0;
        [field: SerializeField] public int TotalEXP { get; set; } = 0;

        [SerializeField] private float _maxHP = 0f;
        public float MaxHP { get => _maxHP; set { _maxHP = value; HP = _maxHP; } }
    
        [field: SerializeField] public float HP { get; set; } = 0f;
        public float MaxShieldHP { get; set; } = 0f;
        [field: SerializeField] public float ShieldHP { get; set; } = 0f;

        // +++ ARMOR +++
        private float _initialArmor = 0f;
        [SerializeField] private float _armor = 0f;
        public float Armor { get => _armor; set => _armor = value; }
        public IEnumerator CoBuffMaxArmor(float duration)
        {
            float currentArmor = this.Armor;
            this.Armor = FixedValue.Numeric.INGAME_MAX_ARMOR_RATE;
            yield return new WaitForSeconds(duration);
            this.Armor = currentArmor;
        }

        // +++ ATTACK SPEED +++
        [SerializeField] private float _startMasteryAttackAnimSpeed = 1f;
        [SerializeField] private float _startMasteryAttackCooldown = 0f;
        [SerializeField] private float _addedMasteryAttackSpeedRatio = 0f;
        public float AddMasteryAttackSpeedRatio
        {
            get => _addedMasteryAttackSpeedRatio;
            set
            {
                _addedMasteryAttackSpeedRatio += value;
                SetAttackCooldown(this.Owner.CreatureType, _addedMasteryAttackSpeedRatio);
            }
        }

        private void SetAttackCooldown(FixedValue.TemplateID.Creatures creatureType, float addedAttackSpeedRatio)
        {
            Utils.Log($"INPUT ATTACK SPEED RATIO : {addedAttackSpeedRatio}");

            // MASTERY ATTACK COOLDOWN
            FixedValue.TemplateID.Skill masterySkillType = this.Owner.SkillBook.UniqueDefaultTemplate;
            this.Owner.SkillBook.Deactivate(masterySkillType);
            float maxCooldownForChara = 0f;
            switch (creatureType)
            {
                case FixedValue.TemplateID.Creatures.Gary_Paladin:
                    maxCooldownForChara = FixedValue.Numeric.PALADIN_ATTACK_MAX_COOLDOWN_REDUCTION;
                    break;
            }

            float result = _startMasteryAttackCooldown * (1f - Mathf.Clamp01(addedAttackSpeedRatio)) + 
                            maxCooldownForChara * Mathf.Clamp01(addedAttackSpeedRatio);
            Utils.Log($"RESULT COOLDOWN : {result}");
            this.Owner.SkillBook.SetSkillCooldown(this.Owner.SkillBook.UniqueDefaultTemplate, result);

            // MASTERY ATTACK ANIM SPEED
            result = _startMasteryAttackAnimSpeed + addedAttackSpeedRatio;
            result = Mathf.Max(FixedValue.Numeric.CREATURE_MIN_ATTACK_ANIM_SPEED, result);
            result = Mathf.Min(result, FixedValue.Numeric.CREATURE_MAX_ATTACK_ANIM_SPEED);
            Utils.Log($"RESULT ANIM SPEED : {result}");
            this.Owner.CreatureAnimController.SetAttackSpeed(result);
            this.Owner.SkillBook.Activate(masterySkillType);
        }

        public IEnumerator CoBuffAttackSpeed(float duration)
        {
            SkillData currentMasteryInfo = this.Owner.SkillBook.GetCurrentMasterySkill().Data;
            yield return null;
        }

        // +++ MOVEMENT SPEED +++
        private float _initialMovementSpeed = 0f;
        [SerializeField] private float _movementSpeed = 0f;
        public float MovementSpeed
        {
            get => _movementSpeed;
            set
            {
                _movementSpeed = value;
                this.Owner.CreatureAnimController.SetMovementSpeed(MovementSpeed);
            }
        }

        [field: SerializeField] public float CollectRange { get; set; } = FixedValue.Numeric.INITIAL_COLLECT_RANGE;
        [field: SerializeField] public float Luck { get; set; } = 0f;
        [field: SerializeField] public float HPRegenerationRate { get; set; } = 0f;
        [field: SerializeField] public float HPStealRate { get; set; } = 0f;
        [field: SerializeField] public float DamageUpRate { get; set; } = 0f; // DMG UP RATE ALL OF SKILLS
        [field: SerializeField] public float CriticalChance { get; set; } = 0f;
        [field: SerializeField] public float CooldownRate { get; set; } = 0f; // COOLDOWN RATE ALL OF SKILLS
        [field: SerializeField] public float DodgeChance { get; set; } = 0f;
        #endregion

        public Sprite MasterySkillIcon { get; private set; } = null;
        public string MasterySkillDesc { get; private set; } = string.Empty;

        public Sprite UniqueEliteSkillIcon { get; private set; } = null;
        public string UniqueEliteSkillDesc { get; private set; } = string.Empty;

        public Sprite UniqueUltimateSkillIcon { get; private set; } = null;
        public string UniqueUltimateSkillDesc { get; private set; } = string.Empty;

        public void LateInit()
        {
            this._startMasteryAttackAnimSpeed = this.Owner.CreatureAnimController.GetAttackSpeed();
            this._startMasteryAttackCooldown = this.Owner.SkillBook.GetCurrentMasterySkill().Data.Cooldown;
        }

        public void EnterInGame()
        {
            this.HP = this.MaxHP;
        }

        // =====================================================================================
        // TODO : 확인 후 개선 필요
        private float _movementSpeedOrigin = 0f;
        public void AddMovementSpeedRatio(float ratio)
        {
            _movementSpeedOrigin = this.MovementSpeed;
            this.MovementSpeed = this.MovementSpeed + (this.MovementSpeed * ratio);
        }
        public void ResetMovementSpeed() => this.MovementSpeed = _movementSpeedOrigin;
    }
}

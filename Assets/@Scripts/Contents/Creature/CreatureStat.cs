using UnityEngine;

using static STELLAREST_2D.Define;
using STELLAREST_2D.Data;
using UnityEngine.UIElements.Experimental;

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

        private float _initialArmor = 0f;
        [SerializeField] private float _armor = 0f;
        public float Armor { get => _armor; set => _armor = value; }

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

        public void EnterInGame()
        {
            this.HP = this.MaxHP;
        }

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

using UnityEngine;

using static STELLAREST_2D.Define;
using STELLAREST_2D.Data;

namespace STELLAREST_2D
{
    [System.Serializable]
    public class CreatureStat
    {
        public CreatureStat(CreatureController owner, CreatureData creatureData)
        {
            this.Owner = owner;
            this.TemplateID = creatureData.TemplateID;
            this.Name = creatureData.Name;
            this.Description = creatureData.Description;

            if (creatureData.InitialStatDescGrade_MinPower == InitialStatDescGrade.None || creatureData.InitialStatDescGrade_MaxPower == InitialStatDescGrade.None)
                Utils.LogCritical(nameof(CreatureStat), nameof(CreatureStat), "Not allowed initial zero value for display(Min and Max Power)");

            this.InitialStatDesc_MinPower = creatureData.InitialStatDescGrade_MinPower;
            this.InitialStatDesc_MaxPower = creatureData.InitialStatDescGrade_MaxPower;
            this.InitialStatDesc_AttackSpeed = creatureData.InitialStatDescGrade_AttackSpeed;
            this.InitialStatDesc_Armor = creatureData.InitialStatDescGrade_Armor;
            this.InitialStatDesc_MovementSpeed = creatureData.InitialStatDescGrade_MovementSpeed;

            this.MaxHP = creatureData.InitialMaxHP;
            this.HP = creatureData.InitialMaxHP;
            this.Armor = creatureData.InitialArmor;
            this._movementSpeed = creatureData.InitialMovementSpeed;

            this.InitialSkillDesc_MasteryUniqueSkillIcon = creatureData.InitialSkillDesc_MasteryUniqueSkillIcon;
            this.InitialSkillDesc_MasteryUniqueDescription = creatureData.InitialSkillDesc_MasteryUniqueSkillDescription;
            this.InitialSkillDesc_EliteUniqueSkillIcon = creatureData.InitialSkillDesc_EliteUniqueSkillIcon;
            this.InitialSkillDesc_EliteUniqueSkillDescription = creatureData.InitialSkillDesc_EliteUniqueSkillDescription;
            this.InitialSkillDesc_UltimateUniqueSkillIcon = creatureData.InitialSkillDesc_UltimateUniqueSkillIcon;
            this.InitialSkillDesc_UltimateUniqueSkillDescription = creatureData.InitialSkillDesc_UltimateUniqueSkillDescription;
        }

        #region Event
        // +++++ Mastery Attack Template : Cooldown is Attack Speed +++++
        public System.Action<FixedValue.TemplateID.Skill, float> OnAddSkillCooldownRatio = null;
        public System.Action<FixedValue.TemplateID.Skill> OnResetSkillCooldown = null;
        #endregion

        public CreatureController Owner { get; private set; } = null;
        public int TemplateID { get; private set; } = -1;
        [field: SerializeField] public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        #region SET PREV FOR USING UI LATER
        public InitialStatDescGrade InitialStatDesc_MinPower { get; private set; } = InitialStatDescGrade.None;
        public InitialStatDescGrade InitialStatDesc_MaxPower { get; private set; } = InitialStatDescGrade.None;
        public InitialStatDescGrade InitialStatDesc_AttackSpeed { get; private set; } = InitialStatDescGrade.None;
        public InitialStatDescGrade InitialStatDesc_Armor { get; private set; } = InitialStatDescGrade.None;
        public InitialStatDescGrade InitialStatDesc_MovementSpeed { get; private set; } = InitialStatDescGrade.None;

        public Sprite InitialSkillDesc_MasteryUniqueSkillIcon { get; private set; } = null;
        public string InitialSkillDesc_MasteryUniqueDescription { get; private set; } = string.Empty;

        public Sprite InitialSkillDesc_EliteUniqueSkillIcon { get; private set; } = null;
        public string InitialSkillDesc_EliteUniqueSkillDescription { get; private set; } = string.Empty;

        public Sprite InitialSkillDesc_UltimateUniqueSkillIcon { get; private set; } = null;
        public string InitialSkillDesc_UltimateUniqueSkillDescription { get; private set; } = string.Empty;
        #endregion

        [field: SerializeField] public int Level { get; set; } = 0;
        [field: SerializeField] public int LevelUpPoint { get; set; } = 0;
        [field: SerializeField] public int TotalEXP { get; set; } = 0;

        [field: SerializeField] public float MaxHP { get; set; } = 0f;
        [field: SerializeField] public float HP { get; set; } = 0f;
        [field: SerializeField] public float MaxShieldHP { get; set; } = 0f;
        [field: SerializeField] public float ShieldHP { get; set; } = 0f;

        // +++++ ARMOR +++++
        [SerializeField] private float _armor = 0f;
        private float _armorPrev = 0f;
        private float _armorTotalUpRatio = 0f;
        public float Armor { get => _armor; private set => _armor = value; } // 외부에서는 Armor만 알고 있으면 됨.

        [field: SerializeField] public float CollectRange { get; set; } = FixedValue.Numeric.INITIAL_COLLECT_RANGE;
        [field: SerializeField] public float Luck { get; set; } = 0f;
        [field: SerializeField] public float HPRegenerationRate { get; set; } = 0f;
        [field: SerializeField] public float HPStealRate { get; set; } = 0f;
        [field: SerializeField] public float DamageUpRate { get; set; } = 0f;

        [field: SerializeField] public float CriticalChance { get; set; } = 0f;
        [field: SerializeField] public float PublicSkillCooldownRate { get; set; } = 0f;

        [field: SerializeField] public float DodgeChance { get; set; } = 0f;
        [SerializeField] private float _movementSpeed = 0f;
        public float MovementSpeed
        {
            get => _movementSpeed;
            set
            {
                _movementSpeed = value;
                this.Owner.AnimController.SetMovementSpeed(_movementSpeed);
            }
        }

        private float _movementSpeedOrigin = 0f;
        public void AddMovementSpeedRatio(float ratio)
        {
            _movementSpeedOrigin = this.MovementSpeed;
            this.MovementSpeed = this.MovementSpeed + (this.MovementSpeed * ratio);
        }

        public void ResetMovementSpeed() => this.MovementSpeed = _movementSpeedOrigin;
    }
}

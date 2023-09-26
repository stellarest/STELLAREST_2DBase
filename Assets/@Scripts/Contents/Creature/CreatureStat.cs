using UnityEngine;

namespace STELLAREST_2D
{
    [System.Serializable] 
    public class CreatureStat
    {
        public CreatureStat(CreatureController owner, Data.CreatureData creatureData)
        {
            this.Owner = owner;

            this.TemplateID = creatureData.TemplateID;
            this.Name = creatureData.Name;
            this.Description = creatureData.Description;
            
            this.MaxHp = creatureData.MaxHp;
            this.Hp = creatureData.MaxHp;

            this.Damage = creatureData.Damage;
            this.DefaultSkillDamage = creatureData.DefaultSkillDamage;
            this.Critical = creatureData.Critical;
            this.AttackSpeed = creatureData.AttackSpeed;
            this.CoolDown = creatureData.CoolDown;

            this.Armor = creatureData.Armor;
            this.Dodge = creatureData.Dodge;

            this.MovementSpeed = creatureData.MovementSpeed;
            this.CollectRange = creatureData.CollectRange;

            this.Luck = creatureData.Luck;
            this.TotalExp = creatureData.TotalExp;
        }

        public CreatureStat(CreatureController owner, int templateID, string name, string description, float maxHp, 
                        float damage, float defaultSkillDamage, float critical, float attackSpeed, float coolDown, 
                        float armor, float dodge, float movementSpeed, float collectRange,float luck, float totalExp)
        {
            this.Owner = owner;

            this.TemplateID = templateID;
            this.Name = name;
            this.Description = description;

            this.MaxHp = maxHp;
            this.Hp = maxHp;

            this.Damage = damage;
            this.DefaultSkillDamage = defaultSkillDamage;
            this.Critical = critical;
            this.AttackSpeed = attackSpeed;
            this.CoolDown = coolDown;

            this.Armor = armor;
            this.Dodge = dodge;

            this.MovementSpeed = movementSpeed;
            this.CollectRange = collectRange;

            this.Luck = luck;
            this.TotalExp = totalExp;
        }

        public CreatureController Owner { get; private set; } = null;

        [field: SerializeField] public int TemplateID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }

        [SerializeField] private float _shieldHp; // TEMP : SerializeField
        public float ShieldHp
        {
            get => _shieldHp;
            set
            {
                _shieldHp = value;
                if (_shieldHp <= 0f)
                {
                    _shieldHp = 0f;
                    //this.Owner.Buff.GetComponent<GuardiansShield>().Stop();
                }
            }
        }

        [field: SerializeField] public float MaxHp { get; private set; }
        [field: SerializeField] public float Hp { get; set; }

        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public float DefaultSkillDamage { get; private set; }
        [field: SerializeField] public float Critical { get; private set; }
        [field: SerializeField] public float AttackSpeed { get; private set; }
        [field: SerializeField] public float CoolDown { get; private set; }

        [field: SerializeField] public float Armor { get; private set; }
        [field: SerializeField] public float Dodge { get; private set; }

        [field: SerializeField] public float MovementSpeed { get; private set; }
        [field: SerializeField] public float CollectRange { get; private set; }
        [field: SerializeField] public float Luck { get; private set; } // -> Large Gem 드롭확률 증가, 희귀한 패시브 스킬(스탯) 등장 확률 증가
        [field: SerializeField] public float TotalExp { get; private set; }

        public CreatureStat UpgradeStat(CreatureController owner, CreatureStat currentStat, int templateID)
        {
            if (Managers.Data.StatsDict.TryGetValue(templateID, out Data.CreatureStatData value) == false)
            {
                Debug.LogError("Failed to load Bonus Stat Data !!");
                Debug.Break();
                return null;
            }

            this.MaxHp = currentStat.MaxHp + (currentStat.MaxHp * value.MaxHpUp);
            this.Damage = currentStat.Damage + (currentStat.Damage * value.DamageUp);
            this.DefaultSkillDamage = currentStat.DefaultSkillDamage + (currentStat.DefaultSkillDamage * value.DefaultSkillDamageUp);
            this.Critical = currentStat.Critical + (currentStat.Critical * value.CriticalUp);
            this.AttackSpeed = currentStat.AttackSpeed + (currentStat.AttackSpeed * value.AttackSpeedUp);
            this.CoolDown = currentStat.CoolDown - (currentStat.CoolDown * value.CoolDownUp);

            this.Armor = currentStat.Armor + (currentStat.Armor * value.ArmorUp);
            this.Dodge = currentStat.Dodge + (currentStat.Dodge * value.DodgeUp);

            this.MovementSpeed = currentStat.MovementSpeed + (currentStat.MovementSpeed * value.MovementSpeedUp);
            this.CollectRange = currentStat.CollectRange + (currentStat.CollectRange * value.CollectRangeUp);
            this.Luck = currentStat.Luck + (currentStat.Luck * value.LuckUp);
            this.TotalExp = currentStat.TotalExp;

            return new CreatureStat(owner: owner, templateID: currentStat.TemplateID, name: currentStat.Name, description: currentStat.Description, 
                    maxHp: MaxHp, damage: Damage, defaultSkillDamage: DefaultSkillDamage, critical: Critical, attackSpeed: AttackSpeed, coolDown: CoolDown, 
                    armor: Armor, dodge: Dodge, movementSpeed: MovementSpeed, collectRange: CollectRange, luck: Luck, totalExp: TotalExp);
        }
    }
}

using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;

namespace STELLAREST_2D
{
    [System.Serializable] 
    public class CharacterData
    {
        public CharacterData(CreatureController owner, Data.CreatureData creatureData)
        {
            this.Owner = owner;

            this.TemplateID = creatureData.TemplateID;
            this.Name = creatureData.Name;
            this.Description = creatureData.Description;
            
            this.MaxHp = creatureData.MaxHp;
            this.Hp = creatureData.MaxHp;

            this.Damage = creatureData.Damage;
            this.Critical = creatureData.Critical;
            this.AttackSpeed = creatureData.AttackSpeed;
            this.CoolDown = creatureData.CoolDown;

            this.Armor = creatureData.Armor;
            this.Dodge = creatureData.Dodge;

            this.MoveSpeed = creatureData.MoveSpeed;
            this.CollectRange = creatureData.CollectRange;

            this.Luck = creatureData.Luck;
            this.TotalExp = creatureData.TotalExp;
        }

        public CharacterData(CreatureController owner, int templateID, string name, string description, float maxHp, float damage, float critical, float attackSpeed, float coolDown, 
                        float armor, float dodge, float moveSpeed, float collectRange,float luck, float totalExp)
        {
            this.Owner = owner;

            this.TemplateID = templateID;
            this.Name = name;
            this.Description = description;

            this.MaxHp = maxHp;
            this.Hp = maxHp;

            this.Damage = damage;
            this.Critical = critical;
            this.AttackSpeed = attackSpeed;
            this.CoolDown = coolDown;

            this.Armor = armor;
            this.Dodge = dodge;

            this.MoveSpeed = moveSpeed;
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
                    this.Owner.Buff.GetComponent<GuardiansShield>().Stop();
                }
            }
        }

        [field: SerializeField] public float MaxHp { get; private set; }
        [field: SerializeField] public float Hp { get; set; }

        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public float Critical { get; private set; }
        [field: SerializeField] public float AttackSpeed { get; private set; }
        [field: SerializeField] public float CoolDown { get; private set; }

        [field: SerializeField] public float Armor { get; private set; }
        [field: SerializeField] public float Dodge { get; private set; }

        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public float CollectRange { get; private set; }
        [field: SerializeField] public float Luck { get; private set; } // -> Large Gem 드롭확률 증가, 희귀한 패시브 스킬(스탯) 등장 확률 증가
        [field: SerializeField] public float TotalExp { get; private set; }

        public CharacterData UpgradeCreatureStat(CreatureController owner, CharacterData currentCharaData, int templateID)
        {
            if (Managers.Data.BonusStatDict.TryGetValue(templateID, out Data.BonusStatData value) == false)
            {
                Debug.LogError("Failed to load Bonus Stat Data !!");
                Debug.Break();
                return null;
            }

            this.MaxHp = currentCharaData.MaxHp + (currentCharaData.MaxHp * value.MaxHpUp);
            this.Damage = currentCharaData.Damage + (currentCharaData.Damage * value.DamageUp);
            this.Critical = currentCharaData.Critical + (currentCharaData.Critical * value.CriticalUp);
            this.AttackSpeed = currentCharaData.AttackSpeed + (currentCharaData.AttackSpeed * value.AttackSpeedUp);
            this.CoolDown = currentCharaData.CoolDown - (currentCharaData.CoolDown * value.CoolDownUp);

            this.Armor = currentCharaData.Armor + (currentCharaData.Armor * value.ArmorUp);
            this.Dodge = currentCharaData.Dodge + (currentCharaData.Dodge * value.DodgeUp);

            this.MoveSpeed = currentCharaData.MoveSpeed + (currentCharaData.MoveSpeed * value.MoveSpeedUp);
            this.CollectRange = currentCharaData.CollectRange + (currentCharaData.CollectRange * value.CollectRangeUp);
            this.Luck = currentCharaData.Luck + (currentCharaData.Luck * value.LuckUp);
            this.TotalExp = currentCharaData.TotalExp;

            Utils.LogStrong("Success Upgrade Character Stat !!");
            return new CharacterData(owner, currentCharaData.TemplateID, currentCharaData.Name, currentCharaData.Description, MaxHp, Damage, 
                        Critical, AttackSpeed, CoolDown, Armor, Dodge, MoveSpeed, CollectRange, Luck, TotalExp);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class CharacterData
    {
        public CharacterData(Data.CreatureData creatureData)
        {
            this.Level = 1;
            this.CreatureData = creatureData;

            this.TemplateID = creatureData.TemplateID;
            this.Name = creatureData.Name;

            this.MaxHp = creatureData.MaxHp;
            this.MaxHpUpRate = creatureData.MaxHpUpRate;
            this.HpRegen = creatureData.HpRegen;
            this.LifeSteal = creatureData.LifeSteal;

            this.MinDamage = creatureData.MinDamage;
            this.MaxDamage = creatureData.MaxDamage;
            this.DamageUpRate = creatureData.DamageUpRate;
            this.AtkSpeed = creatureData.AtkSpeed;
            this.AtkSpeedUpRate = creatureData.AtkSpeedUpRate;
            this.Critical = creatureData.Critical;

            this.Armor = creatureData.Armor;
            this.ArmorUpRate = creatureData.ArmorUpRate;
            this.Dodge = creatureData.Dodge;
            this.Resistance = creatureData.Resistance;

            this.MoveSpeed = creatureData.MoveSpeed;
            this.MoveSpeedUpRate = creatureData.MoveSpeedUpRate;

            this.Luck = creatureData.Luck;
            this.TotalExp = creatureData.TotalExp;
        }

        public int Level { get; set; }
        public Data.CreatureData CreatureData;
        public int TemplateID { get; protected set; }
        public string Name { get; protected set; }


        public float MaxHp { get; protected set; }
        public float MaxHpUpRate { get; protected set; }
        public float HpRegen { get; protected set; }
        public float LifeSteal { get; protected set; }

        public float MinDamage { get; protected set; }
        public float MaxDamage { get; protected set; }
        public float DamageUpRate { get; protected set; }
        public float AtkSpeed { get; protected set; }
        public float AtkSpeedUpRate { get; protected set; }
        public float Critical { get; protected set; }

        public float Armor { get; protected set; }
        public float ArmorUpRate { get; protected set; }
        public float Dodge { get; protected set; }
        public float Resistance { get; protected set; }

        public float MoveSpeed { get; protected set; }
        public float MoveSpeedUpRate { get; protected set; }

        public float Luck { get; protected set; }
        public float TotalExp { get; protected set; }
    }
}

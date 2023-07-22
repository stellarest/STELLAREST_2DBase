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
            this.Hp = creatureData.MaxHp;
            this.MaxHpUp = creatureData.MaxHpUp;
            this.HpRegen = creatureData.HpRegen;
            this.LifeStealChance = creatureData.LifeStealChance;

            this.DamageUp = creatureData.DamageUp;
            this.CriticalChance = creatureData.CriticalChance;
            this.CoolTimeDown = creatureData.CoolTimeDown;

            this.Armor = creatureData.Armor;
            this.ArmorUp = creatureData.ArmorUp;
            this.DodgeChance = creatureData.DodgeChance;
            this.Resistance = creatureData.Resistance;

            this.MoveSpeed = creatureData.MoveSpeed;
            this.MoveSpeedUp = creatureData.MoveSpeedUp;

            this.Luck = creatureData.Luck;
            this.TotalExp = creatureData.TotalExp;

            this.MinReadyToActionTime = creatureData.MinReadyToActionTime;
            this.MaxReadyToActionTime = creatureData.MaxReadyToActionTime;
        }

        public int Level { get; set; } = 1;
        public Data.CreatureData CreatureData;
        public int TemplateID { get; set; }
        public string Name { get; set; }

        public float Hp { get; set; }
        public float MaxHp { get; set; }
        public float MaxHpUp { get; set; }
        public float HpRegen { get; set; }
        public float LifeStealChance { get; set; }

        public float DamageUp { get; set; }
        public float CriticalChance { get; set; }
        public float CoolTimeDown { get; set; }

        public float Armor { get; set; }
        public float ArmorUp { get; set; }
        public float DodgeChance { get; set; }
        public float Resistance { get; set; }

        public float MoveSpeed { get; set; }
        public float MoveSpeedUp { get; set; }

        public float Luck { get; set; }
        public float TotalExp { get; set; }

        public float MinReadyToActionTime { get; private set; }
        public float MaxReadyToActionTime { get; private set; }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++
    }
}

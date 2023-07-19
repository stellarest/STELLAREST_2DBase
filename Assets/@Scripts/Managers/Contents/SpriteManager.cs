using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using System.Linq;

namespace STELLAREST_2D
{
    public class SpriteManager
    {
        public void UpgradePlayerSprite(PlayerController pc, Define.InGameGrade grade)
        {
            if (grade > Define.InGameGrade.Legendary)
            {
                Utils.Log("Player Grade is maxium !!");
                return;
            }

            switch (pc.CreatureData.TemplateID)
            {
                case (int)Define.TemplateIDs.Player.Gary_Paladin:
                    UpgradeGaryPaladin(pc, grade);
                    break;
            }
        }

        private void UpgradeGaryPaladin(PlayerController pc, Define.InGameGrade grade)
        {
            Character current = pc.GetComponent<Character>();
            Character next = pc.transform.GetChild((int)grade).GetComponent<Character>();
            Managers.Effect.UpgradePlayerBuffEffect(grade);

            for (int i = 0; i < current.Armor.Count; ++i)
                current.Armor[i] = next.Armor[i];

            for (int i = 0; i < current.ArmorRenderers.Count; ++i)
            {
                if (current.ArmorRenderers[i].gameObject.activeInHierarchy)
                {
                    string rendererName = $"{current.ArmorRenderers[i].name.Replace("[Armor]", "")}";
                    current.ArmorRenderers[i].sprite = current.Armor.FirstOrDefault(s => s.name.Contains(rendererName));
                }
            }

            current.PrimaryMeleeWeapon = next.PrimaryMeleeWeapon;
            current.PrimaryMeleeWeaponRenderer.sprite = current.PrimaryMeleeWeapon;

            current.Shield = next.Shield;
            current.ShieldRenderer.sprite = current.Shield;

            if (next.Helmet != null)
            {
                current.Helmet = next.Helmet;
                current.HelmetRenderer.sprite  =  current.Helmet;
            }

            if (next.Cape != null)
            {
                current.Cape = next.Cape;
                current.CapeRenderer.sprite = current.Cape;
            }

            Managers.Effect.AddCreatureMaterials(pc, pc.CreatureData.TemplateID + (int)grade);
            //pc.CoFadeEffect(pc.CreatureData.TemplateID + (int)grade);
            pc.CoGlitchEffect(pc.CreatureData.TemplateID + (int)grade);


            // TEMP
            if (grade == Define.InGameGrade.Legendary)
            {
                GameObject shild = Utils.FindChild(pc.gameObject, "Shield");
                shild.SetActive(true);
            }
        }
    }
}

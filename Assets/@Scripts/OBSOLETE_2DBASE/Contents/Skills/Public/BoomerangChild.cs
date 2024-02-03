using UnityEngine;

namespace STELLAREST_2D
{
    public class BoomerangChild : Boomerang
    {
        public void Init(CreatureController owner, Data.SkillData data)
        {
            this.Owner = owner;
            this.Data = data;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CreatureController cc = other.GetComponent<CreatureController>();
            if (cc.IsValid() == false)
                return;

            cc.OnDamaged(attacker: this.Owner, from: this);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class ArrowShot : RepeatSkill
    {
        protected override void DoSkillJob()
        {
            Managers.Game.Player.CreatureState = Define.CreatureState.Attack;
        }

        public override void OnPreSpawned()
        {
            base.OnPreSpawned();

            GetComponent<Rigidbody2D>().simulated = false;
            GetComponent<Collider2D>().enabled = false;
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
    }
}

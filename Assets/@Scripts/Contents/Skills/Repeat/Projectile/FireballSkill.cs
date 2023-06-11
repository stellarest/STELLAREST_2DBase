using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class FireballSkill : RepeatSkill
    {
        // 피아식별을 셋팅하던지. 파이어볼이 몬스터한테도 나갈수있고 플레이어한테도 나갈 수 있고
        // 스킬 공용으로 만듭시다
        static int count = 0;
        public FireballSkill()
        {
            Debug.Log("파볼 생성자 타나?");
            Debug.Log("COUNT : " + (count++));
            // 한번만 들어오는데,,,
        }

        protected override void DoSkillJob()
        {
            if (Managers.Game.Player == null)
                return;
            
            Vector3 spawnPos = Managers.Game.Player.FireSocket;
            Vector3 dir = Managers.Game.Player.ShootDir;

            GenerateProjectile(1, Owner, spawnPos, dir, Vector3.zero);
        }
    }
}

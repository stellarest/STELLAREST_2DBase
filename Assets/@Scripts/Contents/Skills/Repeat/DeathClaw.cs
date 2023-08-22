using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class DeathClaw : RepeatSkill
    {
        private Collider2D _col;

        public override void SetSkillInfo(CreatureController owner, int templateID)
        {
            base.SetSkillInfo(owner, templateID);
            SkillType = Define.TemplateIDs.SkillType.DeathClaw;

            if (Owner?.IsPlayer() == true)
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);

            _col = GetComponent<CircleCollider2D>();
        }

        protected override IEnumerator CoStartSkill()
        {
            DoSkillJob();
            yield break;
        }

        protected override void DoSkillJob()
        {
            DeathClaw deathClaw = Managers.Resource.Instantiate(SkillData.PrimaryLabel, pooling: false).GetComponent<DeathClaw>();
            deathClaw.SetSkillInfo(this.Owner, SkillData.TemplateID);
            StartCoroutine(CoDeathClaw(deathClaw));
        }

        private IEnumerator CoDeathClaw(DeathClaw deathClaw)
        {
            float t = 0f;
            while (true)
            {
                deathClaw.transform.position = Owner.transform.position;

                t += Time.deltaTime;
                if (t > 0.1f)
                {
                    deathClaw._col.enabled = !(deathClaw._col.enabled);
                    t = 0f;
                }

                yield return null;
            }
        }

        public override void OnPreSpawned()
        {
            base.OnPreSpawned();

            foreach (var particle in GetComponentsInChildren<ParticleSystem>())
            {
                var emission = particle.emission;
                emission.enabled = false;
            }

            GetComponent<Collider2D>().enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            MonsterController mc = other.GetComponent<MonsterController>();
            if (mc.IsValid() == false)
                return;

            if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.MonsterBody, other.gameObject.layer))
            {
                // 생성만
                GameObject go = Managers.Resource.Instantiate(Define.PrefabLabels.DEATH_CLAW_SLASH, pooling: true);
                go.GetComponent<DeathClawSlash>().Parent = this;
                go.transform.position = mc.Body.transform.position;
                go.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
            }
        }
    }
}

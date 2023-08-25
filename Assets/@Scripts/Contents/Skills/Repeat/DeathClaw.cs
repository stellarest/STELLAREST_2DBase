using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

namespace STELLAREST_2D
{
    public class DeathClaw : RepeatSkill
    {
        private Collider2D _col;
        private DeathClaw _current;
        private Vector3 _startSize;
        private Vector3 _endSize;

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
            _current = deathClaw;

            _startSize = _current.transform.localScale;
            _endSize = _startSize + (_startSize * 0.5f);

            deathClaw.SetSkillInfo(this.Owner, SkillData.TemplateID);
            StartCoroutine(CoDeathClaw(deathClaw));

            if (SkillData.InGameGrade == Define.InGameGrade.Legendary)
                StartCoroutine(CoPingPongScale(deathClaw));
        }

        private IEnumerator CoDeathClaw(DeathClaw deathClaw)
        {
            float t = 0f;
            while (true)
            {
                deathClaw.transform.position = Owner.transform.position;
                t += Time.deltaTime;
                if (t > SkillData.CoolTime)
                {
                    deathClaw._col.enabled = !(deathClaw._col.enabled);
                    t = 0f;
                }

                yield return null;
            }
        }

        private IEnumerator CoPingPongScale(DeathClaw deathClaw)
        {
            while (true)
            {
                yield return new WaitForSeconds(3f);
                yield return new WaitUntil(() => ToScale(deathClaw, _startSize, _endSize));
                yield return new WaitForSeconds(3f);
                yield return new WaitUntil(() => ToScale(deathClaw, _endSize, _startSize));
            }
        }

        private float _scaleDelta = 0f;
        private bool ToScale(DeathClaw deathClaw, Vector3 startScale, Vector3 endScale)
        {
            _scaleDelta += Time.deltaTime;
            float percent = _scaleDelta / 3;
            deathClaw.transform.localScale = Vector3.Lerp(startScale, endScale, percent);
            if (percent >= 1f)
            {
                _scaleDelta = 0f;
                return true;
            }

            return false;
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
                GameObject go = null;
                // 생성만
                if (SkillData.InGameGrade < Define.InGameGrade.Legendary)
                    go = Managers.Resource.Instantiate(Define.PrefabLabels.DEATH_CLAW_SLASH, pooling: true);
                else
                    go = Managers.Resource.Instantiate(Define.PrefabLabels.DEATH_CLAW_SLASH_LEGENDARY, pooling: true);

                go.GetComponent<DeathClawSlash>().Parent = this;
                go.transform.position = mc.Body.transform.position;
                go.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));

                // +++++ Ultimate Sequence Skill에서 흡혈 오라가 발동되었을 때만 사용 +++++
                // if (SkillData.InGameGrade == Define.InGameGrade.Legendary)
                // {
                //     go = Managers.Resource.Instantiate(Define.PrefabLabels.IMPACT_BLOODY_EFFECT, pooling: true);
                //     go.transform.position = mc.Body.transform.position;
                // }
            }
        }

        private void OnDisable()
        {
            if (_current != null)
                Managers.Resource.Destroy(_current.gameObject);
        }
    }
}

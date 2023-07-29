using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class LazerBolt : RepeatSkill
    {
        private bool _isEnd = false;

        public override void SetSkillInfo(CreatureController owner, int templateID)
        {
            base.SetSkillInfo(owner, templateID);

            foreach (var pr in GetComponentsInChildren<ParticleSystemRenderer>())
                pr.sortingOrder = (int)Define.SortingOrder.ParticleEffect;

            if (owner?.IsPlayer() == true)
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
        }

        // 쿨타임 동기화를 위한 오버라이드
        protected override IEnumerator CoStartSkill()
        {
            WaitForSeconds wait = new WaitForSeconds(SkillData.CoolTime);
            while (true)
            {
                _isEnd = false;
                DoSkillJob();
                yield return new WaitUntil(() => _isEnd);
                yield return wait;
            }
        }

        protected override void DoSkillJob()
        {
            StartCoroutine(CoGenerateLazerBolt());
        }

        private IEnumerator CoGenerateLazerBolt()
        {
            for (int i = 0; i < SkillData.ContinuousCount; ++i)
            {
                GameObject goMon = Managers.Object.GetClosestTarget(Owner.transform, Define.TemplateIDs.SkillType.LazerBolt);
                if (goMon.IsValid() == false)
                    yield break;

                MonsterController mc = goMon.GetComponent<MonsterController>();

                GameObject go = Managers.Resource.Instantiate(SkillData.PrimaryLabel, pooling: true);
                LazerBolt lazerBolt = go.GetComponent<LazerBolt>();
                lazerBolt.SetSkillInfo(this.Owner, SkillData.TemplateID);

                // if (_coLifeOfSkill != null && i + 1 == SkillData.ContinuousCount)
                //     StopCoroutine(_coLifeOfSkill);

                StartCoroutine(CoLifeOfSkill(go, mc, SkillData.Duration));
                yield return new WaitForSeconds(SkillData.ContinuousSpacing);
            }
        }

        private IEnumerator CoLifeOfSkill(GameObject go, MonsterController mc, float duration)
        {
            go.transform.position = mc.transform.position;
            go.GetComponent<BoxCollider2D>().enabled = true;

            float delta = 0f;
            while (delta < duration)
            {
                delta += Time.deltaTime;
                yield return null;
            }

            _isEnd = true;
            Managers.Resource.Destroy(go);
            go.GetComponent<BoxCollider2D>().enabled = false;
        }

        // 되긴 되는데 ResetBounceHits까지 잘 되는데.
        // 이게 Rare에서 2번 때리는데. 모두 다 잘 되긴 하는데.
        // 1번 2번을 첫번째에 때렸다면, 그 다음 턴은 2번 3번을 때리게 되고,
        // 다시 그 다음턴은 1번 2번을 때리게됨. 이거 확인해볼것.
        private int _continuousHitCount = 0;
        private void OnTriggerEnter2D(Collider2D other)
        {
            MonsterController mc = other.GetComponent<MonsterController>();
            if (mc.IsValid() == false)
                return;


            ++_continuousHitCount;
            if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.MonsterBody, other.gameObject.layer))
            {
                if (_continuousHitCount == SkillData.ContinuousCount)
                {
                    _continuousHitCount = 0;
                    Managers.Object.ResetBounceHits(Define.TemplateIDs.SkillType.LazerBolt);
                    mc.OnDamaged(Owner, this);
                }
                else
                {
                    mc.IsLazerBoltContinuousHit = true;
                    mc.OnDamaged(Owner, this);
                }
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
        }
    }
}

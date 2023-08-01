using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class LazerBolt : RepeatSkill
    {
        private Collider2D _collider = null;
        private ParticleSystem[] _particles = null;
        private LazerBolt _generator = null;
        private bool _isLaunching = false;
        private bool _isHitted = false;

        public override void SetSkillInfo(CreatureController owner, int templateID)
        {
            base.SetSkillInfo(owner, templateID);

            _collider = GetComponent<Collider2D>();
            _collider.enabled = false;

            _particles = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < _particles.Length; ++i)
                _particles[i].GetComponent<ParticleSystemRenderer>().sortingOrder = (int)Define.SortingOrder.ParticleEffect;

            if (owner?.IsPlayer() == true)
                Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
        }

        protected override IEnumerator CoStartSkill()
        {
            WaitForSeconds wait = new WaitForSeconds(SkillData.CoolTime);
            while (true)
            {
                // 타겟이 있는지는 Generator에서 체크한다.
                // _isLaunching : 완전한 발사를 종료할 경우 false (다시 생성할 때)
                yield return new WaitUntil(() => GetInitialTarget());
                DoSkillJob();
                yield return new WaitUntil(() => _isLaunching == false);
                yield return wait; // 쿨타임은 타겟에게 쏜게 종료 후 체크되는 것임.
            }
        }

        private GameObject _target = null;
        public bool GetInitialTarget()
        {
            Managers.Object.ResetBounceHits(Define.TemplateIDs.SkillType.LazerBolt);
            _target = Managers.Object.GetNextTarget(Owner.transform, Define.TemplateIDs.SkillType.LazerBolt);
            if (_target.IsValid() == false)
                return false;

            return true;
        }

        private int _bounceCount = 0;
        protected override void DoSkillJob()
        {
            _bounceCount = 0;
            _isLaunching = true;
            _collider.enabled = false; // 레이저의 이동이 완료가 끝나고 켜줄것이다.

            CoGenerateLazerBolt();
        }

        private void CoGenerateLazerBolt()
        {
            GameObject go = Managers.Resource.Instantiate(SkillData.PrimaryLabel, pooling: true);

            LazerBolt lazerBolt = go.GetComponent<LazerBolt>();
            lazerBolt.SetSkillInfo(this.Owner, SkillData.TemplateID);
            lazerBolt._generator = this;
            lazerBolt._target = _target;

            lazerBolt.transform.position = _target.transform.position;
            lazerBolt._collider.enabled = true;

            // 이렇게하면 일단, 무조건 타겟을 향해 레이저를 발사함.
            // 레이저를 타겟 위치로 무조건 발사 하지만, 타겟이 그 사이에 죽어서 TriggerEnter에 못탈수도 있음. 이걸 체크해야함.
            StartCoroutine(CoCheckNextHit(lazerBolt));
        }

        private void Play(LazerBolt lazerBolt, GameObject target)
        {
            // 발사하기 전에 false로
            if (_isHitted)
                _isHitted = false;

            ++_bounceCount;
            lazerBolt._collider.enabled = false;
            lazerBolt.transform.position = target.transform.position;
            lazerBolt._collider.enabled = true;
            for (int i = 0; i < lazerBolt._particles.Length; ++i)
                lazerBolt._particles[i].Play();
        }

        // +++ +++
        // 1. 레이저를 발사했는데 몬스터를 맞추지 못했을 경우
        // - 몬스터가 맵에 없거나, 바운스 카운트가 종료되었다면 Generator를 종료한다.
        // - 여전히 바운스 카운트가 남아있다면 다음 타겟을 받아와서 발사한다.

        // 몬스터가 맞든, 안맞든, 일단 레이저가 발사 된 상태이다.
        private IEnumerator CoCheckNextHit(LazerBolt lazerBolt)
        {
            // 처음에 생성하면 무조건 발사되기 때문에 카운트가 올라감
            ++_bounceCount;
            while (true)
            {
                if (this._isHitted == false && lazerBolt._particles[0].isPlaying == false)
                {
                    // 몬스터가 죽었을 때, 발사된 자리에 레이저의 BounceCount가 종료될 때 까지 계속 발사되는 것을
                    // 막기 위해 마지막으로 추가함. 나중에 문제가 생기게 된다면 개선할 것. 이 부분을 추가한게 가장 마지막임.
                    if (lazerBolt._target.IsCreatureDead())
                    {
                        // Utils.Log("I'M HITTED && DEAD");
                        _target = null;
                        _isLaunching = false;
                        Managers.Resource.Destroy(lazerBolt.gameObject);
                        yield break;
                    }

                    // 몬스터를 못맞추고 파티클이 끝났을 경우
                    if (Managers.Object.Monsters.Count == 0 || _bounceCount == SkillData.BounceCount)
                    {
                        _target = null;
                        _isLaunching = false;
                        Managers.Resource.Destroy(lazerBolt.gameObject);
                        yield break;
                    }
                    else // 몬스터를 못맞췄지만 Bounce Count가 여전히 남아 있을 경우, 어떻게든 무조건 발사는 해야한다.
                    {
                        _target = Managers.Object.GetNextTarget(lazerBolt._target.transform, Define.TemplateIDs.SkillType.LazerBolt);
                        if (_target != null) // 맞춰야할 녀석이 존재한다면..
                        {
                            // 이전에 진행되었던 레이저가 종료되면.. 새로운 타겟에게 다시 발사한다.
                            Play(lazerBolt, _target);
                            lazerBolt._target = _target;
                        }
                        else // 그런데 여기서, 몬스터가 1마리만 남는등, 이로 인해 _target이 null이 될 수가 있다.
                        {
                            if (Managers.Object.IsAllMonsterBounceHitStatus(Define.TemplateIDs.SkillType.LazerBolt))
                            {
                                Managers.Object.ResetBounceHits(Define.TemplateIDs.SkillType.LazerBolt);
                                _target = Managers.Object.GetNextTarget(lazerBolt._target.transform, Define.TemplateIDs.SkillType.LazerBolt);
                                Play(lazerBolt, _target);
                                lazerBolt._target = _target;
                            }
                            else
                                Play(lazerBolt, lazerBolt._target);
                        }
                    }
                }
                else if (this._isHitted && lazerBolt._particles[0].isPlaying == false)
                {
                    // 몬스터가 죽었을 때, 발사된 자리에 레이저의 BounceCount가 종료될 때 까지 계속 발사되는 것을
                    // 막기 위해 마지막으로 추가함. 나중에 문제가 생기게 된다면 개선할 것. 이 부분을 추가한게 가장 마지막임.
                    if (lazerBolt._target.IsCreatureDead())
                    {
                        // Utils.Log("I'M HITTED && DEAD");
                        _target = null;
                        _isLaunching = false;
                        Managers.Resource.Destroy(lazerBolt.gameObject);
                        yield break;
                    }

                    if (Managers.Object.Monsters.Count == 0 || _bounceCount == SkillData.BounceCount)
                    {
                        _target = null;
                        _isLaunching = false;
                        Managers.Resource.Destroy(lazerBolt.gameObject);
                        yield break;
                    }
                    else
                    {
                        _target = Managers.Object.GetNextTarget(lazerBolt._target.transform, Define.TemplateIDs.SkillType.LazerBolt);
                        if (_target != null)
                        {
                            Play(lazerBolt, _target);
                            lazerBolt._target = _target;
                        }
                        else
                        {
                            // 다음 타겟이 없을 경우, 예시로 맵에 1마리만 남아있을 경우
                            // 또는 맵에 몬스터 2마리가 있고, 바운스 카운트가 3회 일 때,
                            // Hit 된애는 모두 Set Set 되므로 이때는 리셋을 해줘야함.

                            // 단적인 예로, 몬스터가 1마리만 있을 경우 그놈만 조지면 되고
                            // 몬스터가 여러 마리 있고, 여러 마리 몬스터가 모두 번개를 맞았어. 그런데 그 다음 타겟이 없어.
                            if (Managers.Object.IsAllMonsterBounceHitStatus(Define.TemplateIDs.SkillType.LazerBolt))
                            {
                                Managers.Object.ResetBounceHits(Define.TemplateIDs.SkillType.LazerBolt);
                                _target = Managers.Object.GetNextTarget(lazerBolt._target.transform, Define.TemplateIDs.SkillType.LazerBolt);
                                Play(lazerBolt, _target);
                                lazerBolt._target = _target;
                            }
                            else
                                Play(lazerBolt, lazerBolt._target);
                        }
                    }
                }

                yield return null;
            }
        }

        // 여기는 Instantiated Object의 영역이다
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.MonsterBody, other.gameObject.layer))
            {
                MonsterController mc = other.GetComponent<MonsterController>();
                // 혹시 모를 상황 대비
                if (mc.IsValid() == false)
                {
                    Utils.LogStrong("+++ Something is wrong +++");
                    return;
                }

                _generator._isHitted = true; // +++ 핵심 +++
                mc.IsLazerBoltHit = true;
                mc.OnDamaged(Owner, this);
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

        // private IEnumerator CoCheckHit(LazerBolt instantiatedLazerBolt)
        // {
        //     // 그런데 나의 타겟이 이미 죽어있는 상태라면, 다음 타겟을 찾는다.
        //     if (instantiatedLazerBolt.Target.IsCreatureDead())
        //     {
        //     }

        //     yield return null;
        // }

        // private IEnumerator CoCheckHit(LazerBolt lazerBolt)
        // {
        //     // 만약 레이저가 몬스터를 맞추지 못했다면, 일정 시간이 지난 후, Destroy한다.
        //     // 이거 체크 하긴 해야되. 몬스터가 다른 스킬에 의해 죽어서 Hit가 안될수도 있음. 100%
        //     while (true)
        //     {
        //         if (lazerBolt.gameObject.IsValid() && lazerBolt.IsHit == false && _particles[0].isPlaying == false)
        //         {
        //             if (Commander.BounceCount != SkillData.BounceCount)
        //             {
        //                 for (int i = 0; i < _particles.Length; ++i)
        //                     _particles[i].Play();
        //             }
        //             else
        //                 yield break;
        //         }
        //         else
        //             yield break;

        //         yield return null;
        //     }
        // }

        // private IEnumerator CoCheckNextHit()
        // {
        //     Vector3 lastPos = Vector3.zero;
        //     while (true)
        //     {
        //         if (_particles[0].isPlaying == false)
        //         {
        //             gameObject.GetComponent<BoxCollider2D>().enabled = false;
        //             if (Commander.BounceCount == SkillData.BounceCount)
        //             {
        //                 // 종료는 무조건 여기서 시킨다.
        //                 this.Commander.IsEnd = true;
        //                 Managers.Resource.Destroy(gameObject);
        //                 yield break;
        //             }
        //             else
        //             {
        //                 Commander.BounceCount++;
        //                 Target = Managers.Object.GetContinuousNextTarget(Commander.Target.transform, Define.TemplateIDs.SkillType.LazerBolt);
        //                 // 몬스터는 존재하지만, 다음 타겟이 존재하지 않을 경우..
        //                 if (Target == null)
        //                 {
        //                     if (Commander.Target != null)
        //                     {
        //                         // 무작정 같은 위치에 때려박아버린다!
        //                         gameObject.transform.position = Commander.Target.transform.position;
        //                         lastPos = Commander.Target.transform.position;

        //                         // if (Commander.Target.IsCreatureDead() == false)
        //                         //     gameObject.transform.position = Commander.Target.transform.position;
        //                         // else // 이미 Commnader Target의 몬스터 조차 죽었다면..
        //                         // {
        //                         //     // 상황 종료
        //                         //     this.Commander.IsEnd = true;
        //                         //     Managers.Resource.Destroy(gameObject);
        //                         //     yield break;
        //                         // }
        //                     }
        //                     else // Commander Target조차 존재하지 않는다면.. 상황 종료!
        //                     {
        //                         gameObject.transform.position = lastPos;

        //                         // this.Commander.IsEnd = true;
        //                         // Managers.Resource.Destroy(gameObject);
        //                         // yield break;
        //                     }
        //                 }
        //                 else // 다음 타겟이 존재한다면, 다음 타겟을 때린다 !!
        //                     gameObject.transform.position = Target.transform.position;

        //                 gameObject.GetComponent<BoxCollider2D>().enabled = true;
        //                 for (int i = 0; i < _particles.Length; ++i)
        //                     _particles[i].Play();
        //             }
        //         }

        //         yield return null;
        //     }
        // }

        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     MonsterController mc = other.GetComponent<MonsterController>();
        //     if (mc.IsValid() == false)
        //     {
        //         this.Commander.IsEnd = true;
        //         Managers.Resource.Destroy(gameObject);
        //         return;
        //     }

        //     if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.MonsterBody, other.gameObject.layer))
        //     {
        //         IsHit = true;
        //         mc.OnDamaged(Owner, this);
        //         mc.IsLazerBoltHit = true;
        //         StartCoroutine(CoCheckNextHit());
        //     }
        // }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace STELLAREST_2D
{
    public struct SpearObject
    {
    }

    public class Spear : RepeatSkill
    {
        public enum Direction { Right, Left, Max }
        private Collider2D _collider = null;
        private Spear[] _spears = new Spear[(int)Direction.Max];
        private GameObject[] _targets = new GameObject[(int)Direction.Max];
        private Vector2[] _spearStartStabPos = new Vector2[(int)Direction.Max];
        private Vector2[] _targetsLastPos = new Vector2[(int)Direction.Max];
        private Vector2[] _targetExtendedLastPos = new Vector2[(int)Direction.Max];
        private Vector2[] _spearEndStabPos = new Vector2[(int)Direction.Max];
        private Quaternion[] _targetsRot = new Quaternion[(int)Direction.Max];
        private float[] _deltas = new float[(int)Direction.Max];
        private readonly float RotSpeed = 20f;

        private Vector2 DefaultRightPosition => Owner.transform.position + new Vector3(4.5f, 0, 0);
        private Vector2 DefaultLeftPosition => Owner.transform.position + new Vector3(-4.5f, 0, 0);

        private int RIGHT => (int)Direction.Right;
        private Vector2 RightSpearPos => _spears[RIGHT].transform.position;
        private Vector2 RightTargetPos => _targets[RIGHT].transform.position;

        private int LEFT => (int)Direction.Left;
        private Vector2 LeftSpearPos => _spears[LEFT].transform.position;
        private Vector2 LeftTargetPos => _targets[LEFT].transform.position;

        private bool IsRightSpear(Spear spear) => _spears[RIGHT] == spear;

        [SerializeField]
        private AnimationCurve Curve;

        public override void SetSkillInfo(CreatureController owner, int templateID)
        {
            base.SetSkillInfo(owner, templateID);
            _collider = GetComponent<Collider2D>();
        }

        private void InitSpear()
        {
            for (int i = 0; i < _spears.Length; ++i)
                _spears[i] = null;

            for (int i = 0; i < _targets.Length; ++i)
                _targets[i] = null;

            for (int i = 0; i < _spearStartStabPos.Length; ++i)
                _spearStartStabPos[i] = Vector2.zero;

            for (int i = 0; i < _targetsLastPos.Length; ++i)
                _targetsLastPos[i] = Vector2.zero;

            for (int i = 0; i < _targetExtendedLastPos.Length; ++i)
                _targetExtendedLastPos[i] = Vector2.zero;

            for (int i = 0; i < _spearEndStabPos.Length; ++i)
                _spearEndStabPos[i] = Vector2.zero;

            for (int i = 0; i < _targetsRot.Length; ++i)
                _targetsRot[i] = Quaternion.identity;

            for (int i = 0; i < _deltas.Length; ++i)
                _deltas[i] = 0;
        }

        protected override IEnumerator CoStartSkill()
        {
            InitSpear();

            GameObject go = Managers.Resource.Instantiate(SkillData.PrimaryLabel, pooling: false);
            _spears[RIGHT] = go.GetComponent<Spear>();
            _spears[RIGHT].SetSkillInfo(Owner, SkillData.TemplateID);
            _spears[RIGHT].gameObject.name += "_Right";
            _spears[RIGHT].transform.localScale = Vector3.one * 1.5f;

            // go = Managers.Resource.Instantiate(SkillData.PrimaryLabel, pooling: false);
            // _spears[LEFT] = go.GetComponent<Spear>();
            // _spears[LEFT].SetSkillInfo(Owner, SkillData.TemplateID);
            // _spears[LEFT].gameObject.name += "_Left";
            // _spears[LEFT].transform.localScale = Vector3.one * 1.5f;

            DoSkillJob();

            yield break;
        }

        protected override void DoSkillJob()
        {
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // +++++ SkillData.Duration is Scaler of toTargetDir +++++
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++

            StartCoroutine(DoStab(_spears[RIGHT]));
            //StartCoroutine(DoStab(_spears[LEFT]));
        }


        private IEnumerator DoStab(Spear spear)
        {
            yield return null;
            while (true)
            {
                yield return new WaitUntil(() => SearchTarget(spear));
                if (IsRightSpear(spear))
                {
                    // _targetsLastPos 위치는 한 번만 설정한다.
                    if (_targets[RIGHT].IsValid())
                    {
                        _spearStartStabPos[RIGHT] = RightSpearPos;
                        _targetsLastPos[RIGHT] = _targets[RIGHT].transform.position;

                        Vector2 toTargetDir = (_targetsLastPos[RIGHT] - _spearStartStabPos[RIGHT]).normalized;

                        if (SkillData.InGameGrade == Define.InGameGrade.Normal)
                            _targetExtendedLastPos[RIGHT] = _targetsLastPos[RIGHT] - (toTargetDir * SkillData.Duration);
                        else
                            _targetExtendedLastPos[RIGHT] = _targetsLastPos[RIGHT] + (toTargetDir * SkillData.Duration);
                    }
                    else
                        continue;
                }

                yield return new WaitUntil(() => StartStab(spear));
                yield return new WaitForSeconds(1f);
                if (IsRightSpear(spear))
                    _spearEndStabPos[RIGHT] = RightSpearPos;

                yield return new WaitUntil(() => EndStab(spear));
            }
        }


        private bool SearchTarget(Spear spear)
        {
            if (IsRightSpear(spear))
            {
                _targets[RIGHT] = Managers.Object.GetClosestTarget<MonsterController>(spear.gameObject, 10f);
                if (_targets[RIGHT] == null)
                {
                    float angle = Mathf.Atan2(Managers.Game.Player.ShootDir.y, Managers.Game.Player.ShootDir.x) * Mathf.Rad2Deg;
                    _targetsRot[RIGHT] = Quaternion.Euler(0, 0, angle - 90f);
                }
                else
                {
                    Vector2 toTargetDir = (RightTargetPos - RightSpearPos).normalized;
                    float angle = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
                    _targetsRot[RIGHT] = Quaternion.Euler(0, 0, angle - 90f);

                    _deltas[RIGHT] += Time.deltaTime;
                    // 쿨타임 여기다가 넣어야 할 듯.
                    // 타겟을 조준한 상태에서 1초후 발사.
                    if (_deltas[RIGHT] > SkillData.CoolTime)
                    {
                        _deltas[RIGHT] = 0f;
                        return true;
                    }
                }

                spear.transform.position = DefaultRightPosition;
                spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, _targetsRot[RIGHT], Time.deltaTime * RotSpeed);
            }
            else
            {

            }

            return false;
        }

        private float t = 0f;
        private bool StartStab(Spear spear)
        {
            if (IsRightSpear(spear))
            {
                // 거리에 따라서 조절할 수 있긴한데.. 일단 이렇게.
                t += Time.deltaTime;
                float percent = t / 0.2f;

                spear.transform.position = Vector2.Lerp(_spearStartStabPos[RIGHT], _targetExtendedLastPos[RIGHT], Curve.Evaluate(percent));
                if (percent >= 1f - Mathf.Epsilon)
                {
                    t = 0f;
                    return true;
                }
            }
            else
            {
            }

            return false;
        }

        private bool EndStab(Spear spear)
        {
            if (IsRightSpear(spear))
            {
                // Last Movement
                t += Time.deltaTime;
                float percent = t / 0.275f;
                spear.transform.position = Vector2.Lerp(_spearEndStabPos[RIGHT], DefaultRightPosition, Curve.Evaluate(percent));

                // Last Rotation
                Vector2 toTargetDir = (_spearEndStabPos[RIGHT] - RightSpearPos).normalized;
                float angle = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
                _targetsRot[RIGHT] = Quaternion.Euler(0, 0, angle - 90f);
                spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, _targetsRot[RIGHT], Time.deltaTime * RotSpeed);
                
                if (percent >= 1f - Mathf.Epsilon)
                {
                    t = 0f;
                    return true;
                }
            }
            else
            {
            }

            return false;
        }


        // private void DoStab_TEMP4()
        // {
        //     if (Owner?.IsPlayer() == true)
        //     {
        //         _targets[RIGHT] = Managers.Object.GetClosestTarget<MonsterController>(_spears[RIGHT].gameObject, 10f);
        //         if (_targets[RIGHT] == null)
        //         {
        //             float angle = Mathf.Atan2(Managers.Game.Player.ShootDir.y, Managers.Game.Player.ShootDir.x) * Mathf.Rad2Deg;
        //             _targetsRot[RIGHT] = Quaternion.Euler(0, 0, angle - 90f);
        //         }
        //         else if (_targets[RIGHT].IsValid())
        //         {
        //             _targetsLastPos[RIGHT] = _targets[RIGHT].transform.position;
        //             Vector2 toTargetDir = (_targetsLastPos[RIGHT] - RightSpearPos).normalized;
        //             float angle = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
        //             _targetsRot[RIGHT] = Quaternion.Euler(0, 0, angle - 90f);

        //             _deltas[RIGHT] += Time.deltaTime;
        //             if (_deltas[RIGHT] > SkillData.CoolTime)
        //             {
        //                 _deltas[RIGHT] = 0f;

        //             }
        //         }

        //         _spears[RIGHT].transform.position = DefaultRightPosition;
        //         _spears[RIGHT].transform.rotation = Quaternion.Slerp(_spears[RIGHT].transform.rotation, _targetsRot[RIGHT], Time.deltaTime * _rotSpeed);
        //     }
        // }

        // float t = 0f;
        // private void DoSpearJob_TEMP3()
        // {
        //     Quaternion rotRight = Quaternion.identity;
        //     Quaternion rotLeft = Quaternion.identity;
        //     if (Owner?.IsPlayer() == true)
        //     {
        //         if (_stabStarts[RIGHT] == false)
        //         {
        //             _targets[RIGHT] = Managers.Object.GetClosestTarget<MonsterController>(_spears[(int)Direction.Right].gameObject, 10f);
        //             if (_targets[RIGHT] == null)
        //             {
        //                 float angle = Mathf.Atan2(Managers.Game.Player.ShootDir.y, Managers.Game.Player.ShootDir.x) * Mathf.Rad2Deg;
        //                 rotRight = Quaternion.Euler(0, 0, angle - 90f);
        //                 _deltas[RIGHT] = 0f;
        //             }
        //             else
        //             {
        //                 _targetsLastPos[RIGHT] = _targets[RIGHT].transform.position;
        //                 Vector2 spearRightPos = _spears[RIGHT].transform.position;
        //                 Vector2 targetDir = (_targetsLastPos[RIGHT] - spearRightPos).normalized;
        //                 float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        //                 rotRight = Quaternion.Euler(0, 0, angle - 90f);

        //                 _deltas[RIGHT] += Time.deltaTime;
        //                 if (_deltas[RIGHT] > SkillData.CoolTime)
        //                 {
        //                     _stabStarts[RIGHT] = true;
        //                     _deltas[RIGHT] = 0f;
        //                 }
        //             }

        //             _spears[RIGHT].transform.position = DefaultRightPosition;
        //             _spears[RIGHT].transform.rotation = Quaternion.Slerp(_spears[RIGHT].transform.rotation, rotRight, Time.deltaTime * _rotSpeed);
        //         }
        //         else if (_stabStarts[RIGHT])
        //         {
        //             if (_stabEnds[RIGHT] == false)
        //             {
        //                 t += Time.deltaTime;
        //                 float percent = t / 5f;
        //                 // t = Mathf.Clamp01(t / 2f);

        //                 // +++ GO TO STAB +++
        //                 Vector2 spearRightPos = _spears[RIGHT].transform.position;
        //                 _spears[RIGHT].transform.position = Vector2.Lerp(spearRightPos, _targetsLastPos[RIGHT], percent);

        //                 Debug.Log("PERCENT : " + percent);
        //                 if (t >= 1f - Mathf.Epsilon)
        //                 {
        //                     Debug.Log("T1 END");
        //                     t = 0f;
        //                     _targetsLastPos[RIGHT] = spearRightPos;
        //                     _stabEnds[RIGHT] = true;
        //                 }
        //             }
        //             else if (_stabEnds[RIGHT])
        //             {
        //                 Vector2 spearRightPos = _spears[RIGHT].transform.position;

        //                 t += Time.deltaTime;
        //                 _spears[RIGHT].transform.position = Vector2.Lerp(spearRightPos, DefaultRightPosition, t);

        //                 Vector2 targetDir = (_targetsLastPos[RIGHT] - spearRightPos).normalized;
        //                 float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        //                 rotRight = Quaternion.Euler(0, 0, angle - 90f);
        //                 _spears[RIGHT].transform.rotation = Quaternion.Slerp(_spears[RIGHT].transform.rotation, rotRight, Time.deltaTime * _rotSpeed);

        //                 // PAUSE DURING SECOND
        //                 // _deltas[RIGHT] += Time.deltaTime;
        //                 // if (_deltas[RIGHT] > 1f)
        //                 // {
        //                 // RETURN TO SPEAR SMOOTHLY


        //                 //_spears[RIGHT].transform.position = Vector2.Lerp(spearRightPos, DefaultRightPosition, _curve.Evaluate(t));

        //                 Debug.LogWarning("T2 : " + t);
        //                 if (t >= 1f - Mathf.Epsilon)
        //                 {
        //                     Debug.LogWarning("T2 END");
        //                     t = 0f;
        //                     _stabStarts[RIGHT] = false;
        //                     _stabEnds[RIGHT] = false;
        //                     return;
        //                 }
        //             }
        //         }
        //     }
        //     else
        //     {
        //         // DO SOMETHING WHEN OWNER IS MONSTER
        //         Utils.LogStrong("DO SOMETHING WHEN OWNER IS MONSTER");
        //     }
        // }

        // private void DoSpearJob_TEMP2()
        // {
        //     Quaternion rightTargetRot = Quaternion.identity;
        //     Quaternion targetLeftRot = Quaternion.identity;

        //     if (Owner?.IsPlayer() == true)
        //     {
        //         // Spear : Right
        //         if (_attackStarts[Right] == false)
        //         {
        //             _spears[(int)Direction.Right].transform.position = RightPosition;
        //             _targets[(int)Direction.Right] = Managers.Object.GetClosestTarget<MonsterController>(_spears[(int)Direction.Right].gameObject, 10f);
        //             if (_targets[(int)Direction.Right] == null)
        //             {
        //                 float angle = Mathf.Atan2(Managers.Game.Player.ShootDir.y, Managers.Game.Player.ShootDir.x) * Mathf.Rad2Deg;
        //                 rightTargetRot = Quaternion.Euler(0, 0, angle - 90f);
        //                 _rightAttackStartDelta = 0f;

        //                 _spears[(int)Direction.Right].transform.rotation = Quaternion.Slerp(_spears[(int)Direction.Right].transform.rotation,
        //                                                                  rightTargetRot, Time.deltaTime * _rotSpeed);
        //             }
        //             else
        //             {
        //                 Vector2 targetDir = (_targets[(int)Direction.Right].transform.position - _spears[(int)Direction.Right].transform.position).normalized;
        //                 float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        //                 rightTargetRot = Quaternion.Euler(0, 0, angle - 90f);

        //                 _rightAttackStartDelta += Time.deltaTime;
        //                 if (_rightAttackStartDelta > 1f)
        //                 {
        //                     _attackStarts[(int)Direction.Right] = true;
        //                     _rightAttackStartDelta = 0f;
        //                 }

        //                 _spears[(int)Direction.Right].transform.rotation = Quaternion.Slerp(_spears[(int)Direction.Right].transform.rotation, 
        //                                                                         rightTargetRot, Time.deltaTime * _rotSpeed);
        //             }
        //         }
        //         else if (_attackStarts[(int)Direction.Right])
        //         {
        //             if (_stabEnds[(int)Direction.Right] == false)
        //             {
        //                 Vector2 targetPos = _targets[(int)Direction.Right].transform.position;
        //                 Vector2 spearPos = _spears[(int)Direction.Right].transform.position;
        //                 float sqrMag = (targetPos - spearPos).sqrMagnitude;

        //                 Vector2 targetDir = (targetPos - spearPos).normalized;
        //                 float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        //                 rightTargetRot = Quaternion.Euler(0, 0, angle - 90f);

        //                 _spears[(int)Direction.Right].transform.position = Vector2.Lerp(_spears[(int)Direction.Right].transform.position, targetPos, Time.deltaTime * 20f);
        //                 _spears[(int)Direction.Right].transform.rotation = Quaternion.Slerp(_spears[(int)Direction.Right].transform.rotation, rightTargetRot, Time.deltaTime * _rotSpeed);
        //                 if ((sqrMag < 1f))
        //                 {
        //                     _rightAttackStartDelta += Time.deltaTime;
        //                     if (_rightAttackStartDelta > 1f)
        //                     {
        //                         _stabEnds[(int)Direction.Right] = true;
        //                         _rightAttackStartDelta = 0f;
        //                     }
        //                 }
        //             }
        //             else
        //             {
        //                 Vector2 targetPos = _targets[(int)Direction.Right].transform.position;
        //                 Vector2 spearPos = _spears[(int)Direction.Right].transform.position;
        //                 Vector2 targetDir = (targetPos - spearPos).normalized;
        //                 float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        //                 rightTargetRot = Quaternion.Euler(0, 0, angle - 90f);

        //                 Vector2 initPos = RightPosition;
        //                 float sqrMag = (initPos - spearPos).sqrMagnitude;
        //                 _spears[(int)Direction.Right].transform.position = Vector2.Lerp(_spears[(int)Direction.Right].transform.position, targetPos, Time.deltaTime * 10f);
        //                 if ((sqrMag < 1f))
        //                 {
        //                      _attackStarts[(int)Direction.Right] = false;
        //                     _stabEnds[(int)Direction.Right] = false;
        //                 }
        //             }
        //         }

        //         // 공격중이든 아니든, 향하고 있을 것임.
        //         // _spears[(int)Direction.Right].transform.rotation = Quaternion.Slerp(_spears[(int)Direction.Right].transform.rotation, targetRightRot, Time.deltaTime * _rotSpeed);

        //         // Spear : Left
        //         // _spears[(int)Direction.Left].transform.position = Owner.transform.position + new Vector3(-4.5f, 0, 0);
        //         // _targets[(int)Direction.Left] = Managers.Object.GetClosestTarget<MonsterController>(_spears[(int)Direction.Left].gameObject, 10f);
        //         // if (_targets[(int)Direction.Left] == null)
        //         // {
        //         //     float angle = Mathf.Atan2(Managers.Game.Player.ShootDir.y, Managers.Game.Player.ShootDir.x) * Mathf.Rad2Deg;
        //         //     targetLeftRot = Quaternion.Euler(0, 0, angle - 90f);
        //         // }
        //         // else
        //         // {
        //         //     Vector2 targetDir = (_targets[(int)Direction.Left].transform.position - _spears[(int)Direction.Left].transform.position).normalized;
        //         //     float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        //         //     targetLeftRot = Quaternion.Euler(0, 0, angle - 90f);
        //         // }
        //         // _spears[(int)Direction.Left].transform.rotation = Quaternion.Slerp(_spears[(int)Direction.Left].transform.rotation, targetLeftRot, Time.deltaTime * _rotSpeed);
        //     }
        // }



        // private void DoSpearJob_Temp()
        // {
        //     if (Owner?.IsPlayer() == true)
        //     {
        //         _spears[(int)Direction.Right].transform.position = Owner.transform.position + new Vector3(4.5f, 0, 0);
        //         _targets[(int)Direction.Right] = Managers.Object.GetClosestTarget<MonsterController>(_spears[(int)Direction.Right].gameObject, 10f);
        //         if (_targets[(int)Direction.Right] == null)
        //         {
        //             float angle = Mathf.Atan2(Managers.Game.Player.ShootDir.y, Managers.Game.Player.ShootDir.x) * Mathf.Rad2Deg;
        //             _spears[(int)Direction.Right].transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        //         }
        //         else
        //         {
        //             Vector2 targetDir = (_targets[(int)Direction.Right].transform.position - _spears[(int)Direction.Right].transform.position).normalized;
        //             float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        //             _spears[(int)Direction.Right].transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        //         }


        //         _spears[(int)Direction.Left].transform.position = Owner.transform.position + new Vector3(-4.5f, 0, 0);
        //         _targets[(int)Direction.Left] = Managers.Object.GetClosestTarget<MonsterController>(_spears[(int)Direction.Left].gameObject, 10f);
        //         if (_targets[(int)Direction.Left] == null)
        //         {
        //             float angle = Mathf.Atan2(Managers.Game.Player.ShootDir.y, Managers.Game.Player.ShootDir.x) * Mathf.Rad2Deg;
        //             _spears[(int)Direction.Left].transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        //         }
        //         else
        //         {
        //             Vector2 targetDir = (_targets[(int)Direction.Left].transform.position - _spears[(int)Direction.Left].transform.position).normalized;
        //             float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        //             _spears[(int)Direction.Left].transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        //         }
        //     }
        // }

        private void OnDisable()
        {
            if (_spears[(int)Direction.Right] != null)
                Managers.Resource.Destroy(_spears[(int)Direction.Right].gameObject);

            if (_spears[(int)Direction.Left] != null)
                Managers.Resource.Destroy(_spears[(int)Direction.Left].gameObject);
        }

        public override void OnPreSpawned()
        {
            base.OnPreSpawned();
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
        }
    }
}


// using System.Collections;
// using TMPro;
// using Unity.VisualScripting;
using UnityEngine;

namespace STELLAREST_2D
{
    public class Spear : RepeatSkill
    {
        public override void InitRepeatSkill(RepeatSkill originRepeatSkill)
        {
            throw new System.NotImplementedException();
        }

        public override void SetParticleInfo(Vector3 startAngle, Define.LookAtDirection lookAtDir, float continuousAngle, float continuousFlipX, float continuousFlipY)
        {
        }

        protected override void DoSkillJob()
        {
        }
    }
}

// namespace STELLAREST_2D
// {
//     public class Spear : RepeatSkill
//     {
//         public enum Direction { Right, Left, Max }
//         private Collider2D _collider = null;
//         private SpriteTrail.SpriteTrail _spriteTrail = null;

//         private Spear[] _spears = new Spear[(int)Direction.Max];
//         private GameObject[] _targets = new GameObject[(int)Direction.Max];
//         private Vector2[] _spearStartStabPos = new Vector2[(int)Direction.Max];
//         private Vector2[] _targetsLastPos = new Vector2[(int)Direction.Max]; // 이 변수는 필요없어보임
//         private Vector2[] _targetExtendedLastPos = new Vector2[(int)Direction.Max];
//         private Vector2[] _spearEndStabPos = new Vector2[(int)Direction.Max];
//         private Quaternion[] _targetsRot = new Quaternion[(int)Direction.Max];
//         private float[] _deltas = new float[(int)Direction.Max];
//         private readonly float RotSpeed = 20f;

//         private Vector2 DefaultRightPosition => Owner.transform.position + new Vector3(4.5f, 0, 0);
//         private Vector2 DefaultLeftPosition => Owner.transform.position + new Vector3(-4.5f, 0, 0);

//         private int RIGHT => (int)Direction.Right;
//         private Vector2 RightSpearPos => _spears[RIGHT].transform.position;
//         private Vector2 RightTargetPos => _targets[RIGHT].transform.position;

//         private int LEFT => (int)Direction.Left;
//         private Vector2 LeftSpearPos => _spears[LEFT].transform.position;
//         private Vector2 LeftTargetPos => _targets[LEFT].transform.position;

//         private bool IsRightSpear(Spear spear) => _spears[RIGHT] == spear;

//         [SerializeField]
//         private AnimationCurve Curve;

//         public override void SetSkillInfo(CreatureController owner, int templateID)
//         {
//             base.SetSkillInfo(owner, templateID);
//             _collider = GetComponent<Collider2D>();
//             _collider.enabled = false;

//             Managers.Collision.InitCollisionLayer(gameObject, Define.CollisionLayers.PlayerAttack);
//         }

//         private void InitSpear()
//         {
//             for (int i = 0; i < _spears.Length; ++i)
//                 _spears[i] = null;

//             for (int i = 0; i < _targets.Length; ++i)
//                 _targets[i] = null;

//             for (int i = 0; i < _spearStartStabPos.Length; ++i)
//                 _spearStartStabPos[i] = Vector2.zero;

//             for (int i = 0; i < _targetsLastPos.Length; ++i)
//                 _targetsLastPos[i] = Vector2.zero;

//             for (int i = 0; i < _targetExtendedLastPos.Length; ++i)
//                 _targetExtendedLastPos[i] = Vector2.zero;

//             for (int i = 0; i < _spearEndStabPos.Length; ++i)
//                 _spearEndStabPos[i] = Vector2.zero;

//             for (int i = 0; i < _targetsRot.Length; ++i)
//                 _targetsRot[i] = Quaternion.identity;

//             for (int i = 0; i < _deltas.Length; ++i)
//                 _deltas[i] = 0;
//         }

//         protected override IEnumerator CoStartSkill()
//         {
//             InitSpear();

//             GameObject go = Managers.Resource.Instantiate(SkillData.PrimaryLabel, pooling: false);
//             _spears[RIGHT] = go.GetComponent<Spear>();
//             _spears[RIGHT].SetSkillInfo(Owner, SkillData.TemplateID);
//             _spears[RIGHT].gameObject.name += "_Right";
//             _spears[RIGHT].transform.localScale = Vector3.one * 1.5f;
//             _spears[RIGHT].transform.position = DefaultRightPosition;

//             go = Managers.Resource.Instantiate(SkillData.PrimaryLabel, pooling: false);
//             _spears[LEFT] = go.GetComponent<Spear>();
//             _spears[LEFT].SetSkillInfo(Owner, SkillData.TemplateID);
//             _spears[LEFT].gameObject.name += "_Left";
//             _spears[LEFT].transform.localScale = Vector3.one * 1.5f;
//             _spears[LEFT].transform.position = DefaultLeftPosition;

//             if (SkillData.InGameGrade == Define.InGameGrade.Legendary)
//             {
//                 _spears[RIGHT]._spriteTrail = _spears[RIGHT].GetComponent<SpriteTrail.SpriteTrail>();
//                 _spears[RIGHT]._spriteTrail.enabled = false;

//                 _spears[LEFT]._spriteTrail = _spears[LEFT].GetComponent<SpriteTrail.SpriteTrail>();
//                 _spears[LEFT]._spriteTrail.enabled = false;
//             }

//             DoSkillJob();
//             yield break;
//         }

//         protected override void DoSkillJob()
//         {
//             // +++++++++++++++++++++++++++++++++++++++++++++++++++++++
//             // +++++ SkillData.Duration is Scaler of toTargetDir +++++
//             // +++++++++++++++++++++++++++++++++++++++++++++++++++++++

//             // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//             // +++++ SkillData.ContinuousSpacing is WaitForSeconds between StartStab and EndStab +++++
//             // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//             StartCoroutine(DoStab(_spears[RIGHT]));
//             StartCoroutine(DoStab(_spears[LEFT]));
//         }

//         private float _totalDist = 0f;
//         private IEnumerator DoStab(Spear spear)
//         {
//             // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//             yield return null;
//             while (true)
//             {
//                 yield return new WaitUntil(() => SearchTarget(spear));
//                 if (IsRightSpear(spear))
//                 {
//                     // _targetsLastPos 위치는 한 번만 설정한다.
//                     if (_targets[RIGHT].IsValid())
//                     {
//                         _spearStartStabPos[RIGHT] = RightSpearPos;
//                         _targetsLastPos[RIGHT] = _targets[RIGHT].transform.position;

//                         Vector2 toTargetDir = (_targetsLastPos[RIGHT] - _spearStartStabPos[RIGHT]).normalized;
//                         _targetExtendedLastPos[RIGHT] = _targetsLastPos[RIGHT] + (toTargetDir * SkillData.Duration);

//                         // 이게 이동거리임
//                         // Debug.Log("DIST 1 : " + (_spearStartStabPos[RIGHT] - _targetExtendedLastPos[RIGHT]).magnitude);
//                         _totalDist = (_targetExtendedLastPos[RIGHT] - _spearStartStabPos[RIGHT]).magnitude;

//                         spear._collider.enabled = true;

//                         if (_spears[RIGHT]._spriteTrail != null)
//                             _spears[RIGHT]._spriteTrail.enabled = true;
//                     }
//                     else
//                         continue;
//                 }
//                 else
//                 {
//                     if (_targets[LEFT].IsValid())
//                     {
//                         _spearStartStabPos[LEFT] = LeftSpearPos;
//                         _targetsLastPos[LEFT] = _targets[LEFT].transform.position;

//                         Vector2 toTargetDir = (_targetsLastPos[LEFT] - _spearStartStabPos[LEFT]).normalized;
//                         _targetExtendedLastPos[LEFT] = _targetsLastPos[LEFT] + (toTargetDir * SkillData.Duration);
//                         spear._collider.enabled = true;

//                         if (_spears[LEFT]._spriteTrail != null)
//                             _spears[LEFT]._spriteTrail.enabled = true;
//                     }
//                     else
//                         continue;
//                 }

//                 yield return new WaitUntil(() => StartStab(spear));
//                 yield return new WaitForSeconds(SkillData.ContinuousSpacing);
//                 if (IsRightSpear(spear))
//                     _spearEndStabPos[RIGHT] = RightSpearPos;
//                 else
//                     _spearEndStabPos[LEFT] = LeftSpearPos;

//                 yield return new WaitUntil(() => EndStab(spear));

//                 if (SkillData.InGameGrade == Define.InGameGrade.Legendary)
//                 {
//                     // Vector2 upTargetPos = Quaternion.Euler(0, 0, 30f) * spear.transform.up;
//                     // Vector2 upTargetDir = RightSpearPos + upTargetPos.normalized * _targetExtendedLastPos[RIGHT].magnitude; // 어차피 갱신됨
//                     // //Debug.DrawLine(spear.transform.position, upTargetDir, Color.magenta, -1f);

//                     // Vector2 downTargetPos = Quaternion.Euler(0, 0, -30f) * spear.transform.up;
//                     // Vector2 downTargetDir = RightSpearPos + downTargetPos.normalized * _targetExtendedLastPos[RIGHT].magnitude;
//                     // Debug.DrawLine(spear.transform.position, downTargetDir, Color.blue, -1f);
//                     // Debug.Break();

//                     if (IsRightSpear(spear))
//                     {
//                         Vector2 upTargetPos = Quaternion.Euler(0, 0, 30f) * spear.transform.up;
//                         //Vector2 upTargetDir = RightSpearPos + (upTargetPos * _targetExtendedLastPos[RIGHT].magnitude); // 어차피 갱신됨
//                         Vector2 upTargetDir = RightSpearPos + upTargetPos.normalized * _totalDist; // 어차피 갱신됨
//                         // Debug.Log("<color=white> DIST 2 : " + (upTargetDir - RightSpearPos).magnitude + "</color>");

//                         // Vector2 upTargetDir = RightSpearPos * upTargetPos.normalized;
//                         // upTargetDir *= _targetExtendedLastPos[RIGHT].magnitude;
//                         // Debug.Log("MAG2 : " + upTargetDir.magnitude);

//                         Vector2 downTargetPos = Quaternion.Euler(0, 0, -30f) * spear.transform.up;
//                         //Vector2 downTargetDir = RightSpearPos + (downTargetPos * _targetExtendedLastPos[RIGHT].magnitude);
//                         Vector2 downTargetDir = RightSpearPos + downTargetPos.normalized * _totalDist;
//                         // Debug.Log("<color=yellow> DIST 3 : " + (downTargetDir - RightSpearPos).magnitude + "</color>");

//                         // Vector2 downTargetDir = RightSpearPos * downTargetPos.normalized;
//                         // upTargetDir *= _targetExtendedLastPos[RIGHT].magnitude;
//                         // Debug.Log("MAG3 : " + downTargetDir.magnitude);

//                         //Debug.DrawLine(spear.transform.position, downTargetDir, Color.blue, -1f);
//                         //Debug.Break();

//                         // First : Rot to upTarget
//                         Quaternion startRot = spear.transform.rotation;
//                         float angle = Mathf.Atan2(upTargetPos.y, upTargetPos.x) * Mathf.Rad2Deg;
//                         _targetsRot[RIGHT] = Quaternion.Euler(0, 0, angle - 90f);
//                         yield return new WaitUntil(() => SimpleRotToTarget(spear, startRot, _targetsRot[RIGHT]));

//                         // Clear !!
//                         // Debug.DrawLine(spear.transform.position, _targetExtendedLastPos[RIGHT], Color.blue, -1f);
//                         // Debug.DrawLine(spear.transform.position, downTargetDir, Color.green, -1f);
//                         // Debug.Break();

//                         // First : Go to target
//                         _spearStartStabPos[RIGHT] = RightSpearPos; // 이건 100% 맞음
//                         _targetExtendedLastPos[RIGHT] = upTargetDir;

//                         spear._collider.enabled = true;
//                         yield return new WaitUntil(() => StartStab(spear));

//                         // First : Return to pos
//                         // _spearEndStabPos[RIGHT] = RightSpearPos;
//                         Vector2 startPos = RightSpearPos;
//                         Vector2 endPos = _spearStartStabPos[RIGHT];
//                         yield return new WaitUntil(() => EndStab(spear, startPos, endPos));

//                         // Second : Rot to downTarget
//                         angle = Mathf.Atan2(downTargetPos.y, downTargetPos.x) * Mathf.Rad2Deg;
//                         _targetsRot[RIGHT] = Quaternion.Euler(0, 0, angle - 90f);
//                         yield return new WaitUntil(() => SimpleRotToTarget(spear, startRot, _targetsRot[RIGHT]));

//                         // Second : Go to target
//                         _spearStartStabPos[RIGHT] = RightSpearPos; // 이건 100% 맞음
//                         _targetExtendedLastPos[RIGHT] = downTargetDir;

//                         spear._collider.enabled = true;
//                         yield return new WaitUntil(() => StartStab(spear));

//                         // Second : Return to pos
//                         // _spearEndStabPos[RIGHT] = RightSpearPos;
//                         startPos = RightSpearPos;
//                         endPos = _spearStartStabPos[RIGHT];
//                         yield return new WaitUntil(() => EndStab(spear, startPos, endPos));
//                         _spears[RIGHT]._spriteTrail.enabled = false;

//                         //yield return new WaitForSeconds(5f);
//                     }
//                     else
//                     {
//                         Vector2 upTargetPos = Quaternion.Euler(0, 0, 30f) * spear.transform.up;
//                         //Vector2 upTargetDir = LeftSpearPos + upTargetPos.normalized * _targetExtendedLastPos[LEFT].magnitude; // 어차피 갱신됨
//                         Vector2 upTargetDir = LeftSpearPos + upTargetPos.normalized * _totalDist; // 어차피 갱신됨

//                         Vector2 downTargetPos = Quaternion.Euler(0, 0, -30f) * spear.transform.up;
//                         //Vector2 downTargetDir = LeftSpearPos + downTargetPos.normalized * _targetExtendedLastPos[LEFT].magnitude;
//                         Vector2 downTargetDir = LeftSpearPos + downTargetPos.normalized * _totalDist;

//                         // First : Rot to upTarget
//                         Quaternion startRot = spear.transform.rotation;
//                         float angle = Mathf.Atan2(downTargetPos.y, downTargetPos.x) * Mathf.Rad2Deg;
//                         _targetsRot[LEFT] = Quaternion.Euler(0, 0, angle - 90f);
//                         yield return new WaitUntil(() => SimpleRotToTarget(spear, startRot, _targetsRot[LEFT]));

//                         // First : Go to target
//                         _spearStartStabPos[LEFT] = LeftSpearPos; // 이건 100% 맞음
//                         _targetExtendedLastPos[LEFT] = downTargetDir;
//                         spear._collider.enabled = true;
//                         yield return new WaitUntil(() => StartStab(spear));

//                         // First : Return to pos
//                         // _spearEndStabPos[LEFT] = LeftSpearPos;
//                         Vector2 startPos = LeftSpearPos;
//                         Vector2 endPos = _spearStartStabPos[LEFT];
//                         yield return new WaitUntil(() => EndStab(spear, startPos, endPos));

//                         // Second : Rot to downTarget
//                         angle = Mathf.Atan2(upTargetPos.y, upTargetPos.x) * Mathf.Rad2Deg;
//                         _targetsRot[LEFT] = Quaternion.Euler(0, 0, angle - 90f);
//                         yield return new WaitUntil(() => SimpleRotToTarget(spear, startRot, _targetsRot[LEFT]));

//                         // Second : Go to target
//                         _spearStartStabPos[LEFT] = LeftSpearPos; // 이건 100% 맞음
//                         _targetExtendedLastPos[LEFT] = upTargetDir;
//                         spear._collider.enabled = true;
//                         yield return new WaitUntil(() => StartStab(spear));

//                         // Second : Return to pos
//                         // _spearEndStabPos[LEFT] = LeftSpearPos;
//                         startPos = LeftSpearPos;
//                         endPos = _spearStartStabPos[LEFT];
//                         yield return new WaitUntil(() => EndStab(spear, startPos, endPos));
//                         _spears[LEFT]._spriteTrail.enabled = false;
//                     }
//                 }
//             }
//         }

//         private bool SearchTarget(Spear spear)
//         {
//             if (IsRightSpear(spear))
//             {
//                 _targets[RIGHT] = Managers.Object.GetClosestTarget<MonsterController>(spear.transform.GetChild(0).gameObject, 6f);
//                 if (_targets[RIGHT] == null)
//                 {
//                     _deltas[RIGHT] = 0f;
//                     float angle = Mathf.Atan2(Managers.Game.Player.ShootDir.y, Managers.Game.Player.ShootDir.x) * Mathf.Rad2Deg;
//                     _targetsRot[RIGHT] = Quaternion.Euler(0, 0, angle - 90f);
//                     spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, _targetsRot[RIGHT], Time.deltaTime * RotSpeed);
//                 }
//                 else
//                 {
//                     Vector2 toTargetDir = (RightTargetPos - RightSpearPos).normalized;
//                     float angle = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
//                     _targetsRot[RIGHT] = Quaternion.Euler(0, 0, angle - 90f);

//                     _deltas[RIGHT] += Time.deltaTime;
//                     if (_deltas[RIGHT] > SkillData.CoolTime)
//                     {
//                         _deltas[RIGHT] = 0f;
//                         return true;
//                     }
//                 }

//                 spear.transform.position = DefaultRightPosition;
//                 spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, _targetsRot[RIGHT], Time.deltaTime * RotSpeed);
//             }
//             else
//             {
//                 _targets[LEFT] = Managers.Object.GetClosestTarget<MonsterController>(spear.transform.GetChild(0).gameObject, 6f);
//                 if (_targets[LEFT] == null)
//                 {
//                     float angle = Mathf.Atan2(Managers.Game.Player.ShootDir.y, Managers.Game.Player.ShootDir.x) * Mathf.Rad2Deg;
//                     _targetsRot[LEFT] = Quaternion.Euler(0, 0, angle - 90f);
//                 }
//                 else
//                 {
//                     Vector2 toTargetDir = (LeftTargetPos - LeftSpearPos).normalized;
//                     float angle = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
//                     _targetsRot[LEFT] = Quaternion.Euler(0, 0, angle - 90f);

//                     _deltas[LEFT] += Time.deltaTime;
//                     // 쿨타임 여기다가 넣어야 할 듯. 타겟을 조준한 상태에서 1초후 발사.
//                     if (_deltas[LEFT] > SkillData.CoolTime)
//                     {
//                         _deltas[LEFT] = 0f;
//                         return true;
//                     }
//                 }

//                 spear.transform.position = DefaultLeftPosition;
//                 spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, _targetsRot[LEFT], Time.deltaTime * RotSpeed);
//             }

//             return false;
//         }

//         private bool StartStab(Spear spear)
//         {
//             if (IsRightSpear(spear))
//             {
//                 // 거리에 따라서 조절할 수 있긴한데.. 일단 이렇게.
//                 _deltas[RIGHT] += Time.deltaTime;
//                 float percent = _deltas[RIGHT] / 0.1f;

//                 spear.transform.position = Vector2.Lerp(_spearStartStabPos[RIGHT], _targetExtendedLastPos[RIGHT], Curve.Evaluate(percent));
//                 if (percent >= 1f - Mathf.Epsilon)
//                 {
//                     spear._collider.enabled = false;
//                     _deltas[RIGHT] = 0f;
//                     return true;
//                 }
//             }
//             else
//             {
//                 _deltas[LEFT] += Time.deltaTime;
//                 float percent = _deltas[LEFT] / 0.1f;

//                 spear.transform.position = Vector2.Lerp(_spearStartStabPos[LEFT], _targetExtendedLastPos[LEFT], Curve.Evaluate(percent));
//                 if (percent >= 1f - Mathf.Epsilon)
//                 {
//                     spear._collider.enabled = false;
//                     _deltas[LEFT] = 0f;
//                     return true;
//                 }
//             }

//             return false;
//         }

//         private bool EndStab(Spear spear)
//         {
//             if (IsRightSpear(spear))
//             {
//                 // Last Movement
//                 _deltas[RIGHT] += Time.deltaTime;
//                 float percent = _deltas[RIGHT] / 0.25f;
//                 spear.transform.position = Vector2.Lerp(_spearEndStabPos[RIGHT], DefaultRightPosition, Curve.Evaluate(percent));

//                 // Last Rotation
//                 Vector2 toTargetDir = (_spearEndStabPos[RIGHT] - RightSpearPos).normalized;
//                 float angle = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
//                 _targetsRot[RIGHT] = Quaternion.Euler(0, 0, angle - 90f);
//                 spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, _targetsRot[RIGHT], Time.deltaTime * RotSpeed);

//                 if (percent >= 1f - Mathf.Epsilon)
//                 {
//                     _deltas[RIGHT] = 0f;
//                     return true;
//                 }
//             }
//             else
//             {
//                 // Last Movement
//                 _deltas[LEFT] += Time.deltaTime;
//                 float percent = _deltas[LEFT] / 0.25f;
//                 spear.transform.position = Vector2.Lerp(_spearEndStabPos[LEFT], DefaultLeftPosition, Curve.Evaluate(percent));

//                 // Last Rotation
//                 Vector2 toTargetDir = (_spearEndStabPos[LEFT] - LeftSpearPos).normalized;
//                 float angle = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
//                 _targetsRot[LEFT] = Quaternion.Euler(0, 0, angle - 90f);
//                 spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, _targetsRot[LEFT], Time.deltaTime * RotSpeed);

//                 if (percent >= 1f - Mathf.Epsilon)
//                 {
//                     _deltas[LEFT] = 0f;
//                     return true;
//                 }
//             }

//             return false;
//         }

//         private bool EndStab(Spear spear, Vector2 startPos, Vector2 endPos)
//         {
//             if (IsRightSpear(spear))
//             {
//                 _deltas[RIGHT] += Time.deltaTime;
//                 float percent = _deltas[RIGHT] / 0.25f;
//                 spear.transform.position = Vector2.Lerp(startPos, endPos, Curve.Evaluate(percent));

//                 Vector2 toTargetDir = (startPos - endPos).normalized;
//                 float angle = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
//                 _targetsRot[RIGHT] = Quaternion.Euler(0, 0, angle - 90f);
//                 spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, _targetsRot[RIGHT], Time.deltaTime * RotSpeed);

//                 if (percent >= 1f - Mathf.Epsilon)
//                 {
//                     _deltas[RIGHT] = 0f;
//                     return true;
//                 }
//             }
//             else
//             {
//                 _deltas[LEFT] += Time.deltaTime;
//                 float percent = _deltas[LEFT] / 0.25f;
//                 spear.transform.position = Vector2.Lerp(startPos, endPos, Curve.Evaluate(percent));

//                 Vector2 toTargetDir = (startPos - endPos).normalized;
//                 float angle = Mathf.Atan2(toTargetDir.y, toTargetDir.x) * Mathf.Rad2Deg;
//                 _targetsRot[LEFT] = Quaternion.Euler(0, 0, angle - 90f);
//                 spear.transform.rotation = Quaternion.Slerp(spear.transform.rotation, _targetsRot[LEFT], Time.deltaTime * RotSpeed);

//                 if (percent >= 1f - Mathf.Epsilon)
//                 {
//                     _deltas[RIGHT] = 0f;
//                     return true;
//                 }
//             }

//             return false;
//         }

//         private bool SimpleRotToTarget(Spear spear, Quaternion startRot, Quaternion targetRot)
//         {
//             if (IsRightSpear(spear))
//             {
//                 _deltas[RIGHT] += Time.deltaTime * RotSpeed;
//                 float percent = _deltas[RIGHT];
//                 spear.transform.rotation = Quaternion.Slerp(startRot, targetRot, percent);
//                 if (percent >= 1f - Mathf.Epsilon)
//                 {
//                     _deltas[RIGHT] = 0f;
//                     return true;
//                 }
//             }
//             else
//             {
//                 _deltas[LEFT] += Time.deltaTime * RotSpeed;
//                 float percent = _deltas[LEFT];
//                 spear.transform.rotation = Quaternion.Slerp(startRot, targetRot, percent);
//                 if (percent >= 1f - Mathf.Epsilon)
//                 {
//                     _deltas[LEFT] = 0f;
//                     return true;
//                 }
//             }

//             return false;
//         }

//         private void OnTriggerEnter2D(Collider2D other)
//         {
//             MonsterController mc = other.GetComponent<MonsterController>();
//             if (mc.IsValid() == false)
//                 return;

//             if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.MonsterBody, other.gameObject.layer))
//             {
//                 mc.OnDamaged(Owner, this);
//             }
//         }

//         private void OnDisable()
//         {
//             if (_spears[(int)Direction.Right] != null)
//                 Managers.Resource.Destroy(_spears[(int)Direction.Right].gameObject);

//             if (_spears[(int)Direction.Left] != null)
//                 Managers.Resource.Destroy(_spears[(int)Direction.Left].gameObject);
//         }

//         public override void OnPreSpawned()
//         {
//             base.OnPreSpawned();
//             GetComponent<SpriteRenderer>().enabled = false;
//             GetComponent<Collider2D>().enabled = false;
//             if (GetComponent<SpriteTrail.SpriteTrail>() != null)
//                 GetComponent<SpriteTrail.SpriteTrail>().enabled = false;
//         }
//     }
// }


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

using static STELLAREST_2D.Define;
using STELLAREST_2D.Data;

namespace STELLAREST_2D
{
    /*
        [ Phantom Soul - Phantom Knight Mastery lv.2 ]

        3초 마다, 6회에 해당하는 Phantom Soul 발사하고, 대상으로부터 피해량의 1% ~ 3%에 해당하는 체력을 흡수함
    */

    // Phantom Soul 그대로 사용
    // 베지에 곡선 적용
    public class PhantomSoul : UniqueSkill
    {
        private ParticleSystem[] _particles = null;
        private const float INITIAL_LOCAL_POS_X = 3.5f;
        private const float INITIAL_LOCAL_POS_Y = 1f;

        private ParticleSystem[] _bursts = null;
        private const int BURST_MAX_COUNT = 2;
        private const string FIND_ELECTRIC_BURST = "ElectricBurst";
        private const string FIND_ENERGY_BURST = "EnergyBurst";
        
        private PhantomSoulChild _child = null;

        public override void InitOrigin(CreatureController owner, SkillData data)
        {
            base.InitOrigin(owner, data);
            _particles = GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            for (int i = 0; i < _particles.Length; ++i)
            {
                if (_particles[i].gameObject.name.Contains("Trail"))
                    _particles[i].GetComponent<ParticleSystemRenderer>().sortingOrder = (int)Define.SortingOrder.Player - 1;
                else
                    _particles[i].GetComponent<ParticleSystemRenderer>().sortingOrder = (int)Define.SortingOrder.Skill;

                _particles[i].gameObject.SetActive(false);
            }

            _bursts = new ParticleSystem[BURST_MAX_COUNT];
            for (int i = 0; i < _bursts.Length;)
            {
                for (int j = 0; j < _particles.Length; ++j)
                {
                    if (_particles[j].gameObject.name.Contains(FIND_ELECTRIC_BURST) || 
                        _particles[j].gameObject.name.Contains(FIND_ENERGY_BURST))
                    {
                        _bursts[i++] = _particles[j];
                        continue;
                    }
                }
            }

            // HitCollider = GetComponent<CircleCollider2D>();
            // HitCollider.enabled = false;
            _child = this.Owner.SkillBook.ForceGetSkillMember(FixedValue.TemplateID.Skill.PhantomKnight_Unique_Elite_C1, 0).GetComponent<PhantomSoulChild>();
            _child.SetParent(this);
            this.Owner.SkillBook.LevelUp(FixedValue.TemplateID.Skill.PhantomKnight_Unique_Elite_C1);

            this.Owner.OnLookAtDirChanged += this.OnLookAtDirChangedHandler;
            Utils.Log("ADD EVENT : this.OnLookAtDirChangedHandler");
        }

        protected override IEnumerator CoStartSkill()
        {
            DoSkillJob();
            yield return null;
        }

        protected override void DoSkillJob(Action callback = null)
        {
            this.Owner.SkillBook.Deactivate(FixedValue.TemplateID.Skill.PhantomKnightMastery);
            this.transform.localPosition = new Vector3(INITIAL_LOCAL_POS_X * (int)this.Owner.LookAtDir, INITIAL_LOCAL_POS_Y, 0f);

            this.Owner.CreatureSkillAnimType = this.Data.SkillAnimationTemplateID;
            this.Owner.CreatureState = CreatureState.Skill;
            callback?.Invoke();
        }

        public override void OnActiveEliteActionHandler()
        {
            for (int i = 0; i < _particles.Length; ++i)
                _particles[i].gameObject.SetActive(true);

            this.transform.DOScale(endValue: this.transform.localScale * 1.5f, 1f).SetEase(Ease.InOutSine).
                SetLoops(-1, LoopType.Yoyo);

            List<MonsterController> toMonsters = Managers.Object.Monsters.ToList();
            Target = toMonsters[0].gameObject;

            StartCoroutine(CoTickPhantomSoul());
            // StartCoroutine(CoActivatePhantomSoulChild_Temp()); // TEMP
        }

        public LineRenderer lineRenderer = null;
        public LineRenderer lineRenderer2 = null;
        public int LineCount = 20;

        // y축 한 번 발사할 때 마다 6, 9, 12, 15, 18, 21
        [Header("Point1")]
        public GameObject CurvePoint1 = null;
        //public Vector3 ControlPoint1 = new Vector3(0, 3, 0);
        public Vector3 ControlPoint1 = new Vector3(-9f, 6f, 0);


        // [Header("Point2")]
        // public GameObject CurvePoint2 = null;
        // public Vector3 ControlPoint2 = new Vector3(0, 3, 0);
        [Header("Point2")]
        public GameObject CurvePoint2 = null;
        public Vector3 ControlPoint2 = Vector3.zero;


        [Header("Point3")]
        public GameObject CurvePoint3 = null;
        //public Vector3 ControlPoint3 = new Vector3(0, -3, 0);
        public Vector3 ControlPoint3 = new Vector3(-9f, -6f, 0);


        // [Header("Point4")]
        // public GameObject CurvePoint4 = null;
        // public Vector3 ControlPoint4 = new Vector3(0, -3, 0);
        [Header("Point4")]
        public GameObject CurvePoint4 = null;
        public Vector3 ControlPoint4 = Vector3.zero;

        [Header("Target")]
        public GameObject Target = null;

        public Vector3 GetBezierCurves(Vector3 Point1, Vector3 Point2, Vector3 Point3, Vector3 Point4, float t)
        {
            Vector3 M1 = Vector3.Lerp(Point1, Point2, t);
            Vector3 M2 = Vector3.Lerp(Point2, Point3, t);
            Vector3 M3 = Vector3.Lerp(Point3, Point4, t);

            Vector3 B1 = Vector3.Lerp(M1, M2, t);
            Vector3 B2 = Vector3.Lerp(M2, M3, t);

            return Vector3.Lerp(B1, B2, t);
        }

        private IEnumerator CoActivatePhantomSoulChild_Temp()
        {
            yield return new WaitForSeconds(3f);
            this.Owner.SkillBook.Activate(FixedValue.TemplateID.Skill.PhantomKnight_Unique_Elite_C1);
            yield return null;
        }

        public void PlayBursts() 
        {
            for (int i = 0; i < _bursts.Length; ++i) 
                _bursts[i].Play();
        }

        private IEnumerator CoTickPhantomSoul()
        {
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.M))
                {
                    ControlPoint1 += new Vector3(-3f, 3f, 0f);
                    ControlPoint3 += new Vector3(-3f, -3f, 0f);
                }

                lineRenderer.positionCount = LineCount;
                lineRenderer2.positionCount = LineCount;

                CurvePoint1.transform.position = this.transform.position + ControlPoint1;
                //CurvePoint2.transform.position = Target.transform.position + ControlPoint2;
                CurvePoint2.transform.position = Target.GetComponent<CreatureController>().Center.position;

                CurvePoint3.transform.position = this.transform.position + ControlPoint3;
                // CurvePoint4.transform.position = Target.transform.position + ControlPoint4;
                CurvePoint4.transform.position = Target.transform.position + ControlPoint4;
                CurvePoint2.transform.position = Target.GetComponent<CreatureController>().Center.position;

                for (int i = 0; i < LineCount; ++i)
                {
                    float t = 0f;
                    if (i == 0)
                        t = 0;
                    else
                        t = (float)i / (LineCount - 1);

                    lineRenderer.SetPosition(i, this.GetBezierCurves(this.transform.position,
                        CurvePoint1.transform.position, CurvePoint2.transform.position, Target.transform.position, t));
                    lineRenderer2.SetPosition(i, this.GetBezierCurves(this.transform.position,
                         CurvePoint3.transform.position, CurvePoint4.transform.position, Target.transform.position, t));
                }

                this.Owner.FireSocket.position = this.transform.position;
                yield return null;
            }
        }
        
        public bool IsJumping { get; private set; } = false;

        public void OnLookAtDirChangedHandler(Define.LookAtDirection lookAtDir)
        {
            IsJumping = true;
            float jumpPower = 3.15f;
            //float jumpPower = UnityEngine.Random.Range(0f, 1f) >= 0.5f ? -3.5f : 3.15f;
            if (lookAtDir == Define.LookAtDirection.Right)
                jumpPower = -3.5f;

            transform.DOLocalJump(endValue: new Vector3(INITIAL_LOCAL_POS_X * (int)lookAtDir, INITIAL_LOCAL_POS_Y, 0f),
                       jumpPower,
                       numJumps: 1,
                       duration: 0.45f).SetEase(Ease.InOutSine).OnComplete(() => IsJumping = false);

            // transform.DOLocalJump(endValue: new Vector3(INITIAL_LOCAL_POS_X * (int)lookAtDir, INITIAL_LOCAL_POS_Y, 0f),
            //     jumpPower,
            //     numJumps: 1,
            //     duration: 0.45f).SetEase(Ease.InOutSine).OnComplete(() => IsJumping = false).OnComplete(() => _electricBurst.Play());
            // 점프 이후, DOLocalMove 또는 DORotate하는 것도 괜찮을듯
        }

        // this.Owner.SkillBook.LevelUp(SkillTemplate.Phantom_Elite_Child);
        // this.Owner.SkillBook.Activate(SkillTemplate.Phantom_Elite_Child); // 아마 플레이어로부터 발사될것임...
        // 실제로 파이어 소켓의 위치를 Phantom으로 하면 편할것같음
        // 발사할 때 파티클 펑 터지는거 있어야하고, 그거 펑 터질때 Phantom Elite Child 발사하고
        // 베지어 곡선 세팅해야되는데 이건 나중에
        // 파이어소켓으 위치를 실시간으로 이녀석으로 잡고, 파이어소켓으로부터 발사하게하면 될듯
        private void OnDestroy()
        {
            if (this.Owner.OnLookAtDirChanged != null)
            {
                this.Owner.OnLookAtDirChanged -= this.OnLookAtDirChangedHandler;
                Utils.Log("RELEASE EVENT : this.Owner.OnLookAtDirChanged");
            }
        }
    }
}

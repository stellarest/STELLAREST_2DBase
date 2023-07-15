using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class ProjectileController : SkillBase
    {
        private Rigidbody2D _rigid;

        public override bool Init()
        {
            base.Init();
            _rigid = GetComponent<Rigidbody2D>();

            return true;
        }

        private Vector3 _shootDir;
        private float _speed;
        private float _initialTurningDir = 0f; // turn -> turn 하면 다시 파티클이 플레이어를 쫓아가는 것을 막아줌, 필요시 사용
        private bool _offParticle = false;

        public void SetInfo(CreatureController owner, Data.SkillData skillData, Vector3 shootDir)
        {
            this.Owner = owner;
            this.SkillData = skillData;
            this._shootDir = shootDir;

            _initialTurningDir = Managers.Game.Player.TurningAngle;
            _offParticle = false;

            StartDestroy(skillData.Duration);

            switch (skillData.OriginTemplateID)
            {
                case (int)Define.TemplateIDs.SkillType.PaladinSwing:
                    GetComponent<PaladinSwing>().SetSkillInfo(owner, skillData.TemplateID);
                    StartCoroutine(CoPaladinSwing());
                    break;
            }
        }

        private IEnumerator CoPaladinSwing()
        {
            while (true)
            {
                // ControlCollisionTime(SkillData.Duration - 0.1f); // 필요할 때 사용

                // Projectile Script.cs
                // _speed : Projectile Speed
                if (Managers.Game.Player.IsMoving && _offParticle == false) // Moving Melee Attack
                {
                    if (Managers.Game.Player.IsInLimitMaxPosX || Managers.Game.Player.IsInLimitMaxPosY)
                    {
                        float minSpeed = Managers.Game.Player.MovementPower + SkillData.Speed;
                        float maxSpeed = Owner.CreatureData.MoveSpeed + SkillData.Speed;

                        float movementPowerRatio = Managers.Game.Player.MovementPower / Owner.CreatureData.MoveSpeed;
                        _speed = Mathf.Lerp(minSpeed, maxSpeed, movementPowerRatio);
                    } 
                    else
                        _speed = Owner.CreatureData.MoveSpeed + SkillData.Speed;
                }
                else // Static Melee Attack
                    _speed = SkillData.Speed;

                SetOffParticle();
                transform.position += _shootDir * _speed * Time.deltaTime;

                yield return null;
            }
        }

        private float _sensitivity = 0.6f;
        private void SetOffParticle()
        {
            if (_initialTurningDir != Managers.Game.Player.TurningAngle)
                _offParticle = true;
            if (Managers.Game.Player.IsMoving == false)
                _offParticle = true;
            // if (Vector3.Distance(Managers.Game.Player.ShootDir, _shootDir) > _sensitivity)
            //     _offParticle = true;
            if ((Managers.Game.Player.ShootDir - _shootDir).sqrMagnitude > _sensitivity * _sensitivity)
                _offParticle = true;
        }

        float delta = 0f;
        private void ControlCollisionTime(float controlTime)
        {
            delta += Time.deltaTime;
            if (controlTime <= delta)
            {
                GetComponent<Collider2D>().enabled = false;
                delta = 0f;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // SkillData.PenetrationCount // -1 : 무제한 관통
            MonsterController mc = other.GetComponent<MonsterController>();
            if (mc.IsValid() == false)
                return;

            if (Managers.Collision.CheckCollisionTarget(Define.CollisionLayers.MonsterBody, other.gameObject.layer))
            {
                Debug.Log("DMG : " + Owner.CreatureData.Power * SkillData.DamageUpMultiplier);
                mc.OnDamaged(Owner, this, Owner.CreatureData.Power * SkillData.DamageUpMultiplier);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using STELLAREST_2D.Data;
using UnityEngine;

namespace STELLAREST_2D
{
    /// <summary>
    /// +++ Buff : Concentration +++
    /// Duration : 5s
    /// CoolTime : 30s
    /// Attack Speed Up + 100%
    /// Damage Up Ratio + 200%
    /// </summary>
    public class Concentration : BuffBase
    {
        private PlayerController _player;

        protected override IEnumerator CoBuff()
        {
            float t = 0f;
            float percent = 0f;

            if (_player == null)
            {
                if (_target?.IsPlayer() == true)
                    _player = _target.GetComponent<PlayerController>();
            }

            // 이거 나중에 플레이어가 스탯 찍을때 자동으로 CriticalChance가 Set이 되어야하고 이를 받아와야해서 이 부분으로 인해 일단 크리티컬 찬스업은 보류
            // float originCriticalChance = originCriticalChance = _player.CharaData.CriticalChance;

            if (IsBuffOn)
            {
                // 1f -> 2f : +100% Anim Speed
                _player.PAC.AttackAnimSpeed(_buffData.BuffType.AttackSpeedUpRatio);
                _skill.DamageBuffRatio = _buffData.BuffType.DamageUpRatio;
            }

            while (true)
            {
                t += Time.deltaTime;
                if (IsBuffOn)
                {
                    percent = t / _buffData.Duration;
                    Debug.Log("Duration : " + t + "/" + _buffData.Duration);
                    if (percent >= 1f)
                    {
                        _player.PAC.AttackAnimSpeed(1f);
                        _skill.DamageBuffRatio = null;
                        IsBuffOn = false;
                        t = 0f;
                        percent = 0f;
                        Stop();
                    }
                }
                else
                {
                    percent = t / _buffData.CoolTime;
                    Debug.Log("CoolTime : " + t + "/" + _buffData.CoolTime);
                    if (percent >= 1f)
                    {
                        _player.PAC.AttackAnimSpeed(_buffData.BuffType.AttackSpeedUpRatio);
                        _skill.DamageBuffRatio = _buffData.BuffType.DamageUpRatio;
                        IsBuffOn = true;
                        t = 0f;
                        percent = 0f;
                        Play();
                    }
                }

                yield return null;
            }
        }

        protected override void Init()
        {
        }
    }
}


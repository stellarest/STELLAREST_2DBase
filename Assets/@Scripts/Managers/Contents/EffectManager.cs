using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

namespace STELLAREST_2D
{
    public struct CreatureMat
    {
        public CreatureMat(SpriteRenderer spriteRenderer, Material matOrigin, Color colorOrigin)
        {
            this.spriteRender = spriteRenderer;
            this.matOrigin = matOrigin;
            this.colorOrigin = colorOrigin;
        }

        public SpriteRenderer spriteRender;
        public Material matOrigin;
        public Color colorOrigin; // color 수정이 필요할 경우
    }

    public class EffectManager
    {
        private Material _matHitWhite;
        private Material _matHitRed;
        private int HIT_EFFECT = Shader.PropertyToID("_StrongTintFade");

        public void Init()
        {
            _matHitWhite = Managers.Resource.Load<Material>(Define.MaterialLabels.MAT_HIT_WHITE);
            _matHitRed = Managers.Resource.Load<Material>(Define.MaterialLabels.MAT_HIT_RED);
        }

        
        private CreatureMat[] _playerMats;
        public void SetInitialPlayerMat(PlayerController pc)
        {
            int length = 0;
            SpriteRenderer[] sprArr = pc.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
            for (int i = 0; i < sprArr.Length; ++i)
            {
                // sprite tex가 null인것만 패스
                if (sprArr[i].sprite == null)
                    continue;

                if (sprArr[i].gameObject.name.Contains(Define.PlayerController.FIRE_SOCKET))
                    continue;

               ++length;
            }
            _playerMats = new CreatureMat[length];

            int index = 0;
            for (int i = 0; i < sprArr.Length; ++i)
            {
                if (sprArr[i].sprite == null)
                    continue;

                if (sprArr[i].gameObject.name.Contains(Define.PlayerController.FIRE_SOCKET))
                    continue;

                _playerMats[index++] = new CreatureMat(sprArr[i], sprArr[i].material, sprArr[i].color);
            }
        }

        public void SetInitialMonsterMat(MonsterController mc)
        {
            
        }

        private bool _isPlayingEffect = false;
        public void StartHitEffectToPlayer()
        {
            if (_isPlayingEffect == false)
            {
                _isPlayingEffect = true;
                for (int i = 0; i < _playerMats.Length; ++i)
                {
                    _playerMats[i].spriteRender.material = _matHitRed;
                    _playerMats[i].spriteRender.material.SetFloat(HIT_EFFECT, 1);
                }
            }
        }

        public void EndHitEffectToPlayer()
        {
            for (int i = 0; i < _playerMats.Length; ++i)
            {
                _playerMats[i].spriteRender.material = _playerMats[i].matOrigin;
                //_playerMats[i].spriteRender.color = _playerMats[i].colorOrigin;
            }
            _isPlayingEffect = false;
        }


        // public void PrintPlayerMats()
        // {
        //     for (int i = 0; i < _playerMats.Length; ++i)
        //     {
        //         Debug.Log(_playerMats[i].spriteRender.gameObject.name);
        //         Debug.Log(_playerMats[i].matOrigin);
        //         Debug.Log(_playerMats[i].color);
        //         Debug.Log("==================");
        //     }
        // }

        // public void PrintPlayerRenderList()
        // {
        //     Debug.Log("_playerSPRs.Length : " + _playerSPRs.Length);
        //     for (int i = 0; i < _playerSPRs.Length; ++i)
        //     {
        //         Debug.Log(_playerSPRs[i].gameObject.name);
        //     }
        // }

        // private Queue<CreatureMat> matQueue = new Queue<CreatureMat>();
        // public void StartHitEffect_TEMP(GameObject go)
        // {
        //     if (_isPlayingEffect == false)
        //     {
        //         _isPlayingEffect = true;

        //         Material hitMat;
        //         CreatureController cc = go.GetComponent<CreatureController>();
        //         if (cc?.IsMonster() == true)
        //             hitMat = _matHitWhite;
        //         else
        //             hitMat = _matHitRed;

        //         SpriteRenderer[] sprArr = go.GetComponentsInChildren<SpriteRenderer>();
        //         int length = sprArr.Length;
        //         for (int i = 0; i < length; ++i)
        //         {
        //             matQueue.Enqueue(new CreatureMat(sprArr[i].material, sprArr[i].color));
        //             sprArr[i].material = hitMat;
        //             sprArr[i].material.SetFloat(HIT_EFFECT, 1);
        //         }
        //     }
        // }

        // public void EndHitEffect_TEMP(GameObject go)
        // {
        //     if (_isPlayingEffect)
        //     {
        //         SpriteRenderer[] sprArr = go.GetComponentsInChildren<SpriteRenderer>();
        //         int length = sprArr.Length;
        //         for (int i = 0; i < length; ++i)
        //         {
        //             sprArr[i].material.SetFloat(HIT_EFFECT, 0);

        //             if (matQueue.Count > 0)
        //             {
        //                 CreatureMat mat = matQueue.Dequeue();
        //                 sprArr[i].material = mat.material;
        //                 sprArr[i].color = mat.color;
        //             }
        //         }

        //         _isPlayingEffect = false;
        //     }
        // }

        public void ShowDamageNumber(string prefabLabel, Vector3 pos, float damage, Transform parent = null, bool isCritical = false, bool isText = false)
        {
            if (isCritical == false)
            {
                GameObject prefab = Managers.Resource.Load<GameObject>(prefabLabel);
                DamageNumber dmgFont = prefab.GetComponent<DamageNumber>();
                dmgFont.Spawn(pos, damage);
            }
            else
            {
                GameObject prefab = Managers.Resource.Load<GameObject>(prefabLabel);
                DamageNumber dmgFont = prefab.GetComponent<DamageNumber>();
                dmgFont.Spawn(pos, damage);
            }
        }

        public void ShowEffectText(string prefabLabel, Vector3 pos, string text)
        {
            GameObject prefab = Managers.Resource.Load<GameObject>(prefabLabel);
            DamageNumber dmgFont = prefab.GetComponent<DamageNumber>();
            dmgFont.Spawn(pos, text);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class CollisionManager
    {
        public void SetCollisionLayers()
        {
            SetCollisionLayers(Define.CollisionLayers.PlayerAttack, Define.CollisionLayers.MonsterBody, true);
            SetCollisionLayers(Define.CollisionLayers.MonsterBody, Define.CollisionLayers.MonsterBody, true);
            // Managers.Collision.SetCollisionLayers(Define.CollisionLayers.PlayerBody, Define.CollisionLayers.MonsterAttack, true);
            // Managers.Collision.SetCollisionLayers(Define.CollisionLayers.MonsterBody, Define.CollisionLayers.MonsterBody, true);
        }

        public void InitCollisionLayer(GameObject go, Define.CollisionLayers layer)
        {
            if (go.GetComponent<Collider2D>() != null)
                go.layer = (int)layer;
            else
            {
                foreach (var child in go.transform.GetComponentsInChildren<Collider2D>())
                {
                    if (child != null)
                    {
                        child.gameObject.layer = (int)layer;
                        return;
                    }
                }

                Utils.LogCritical(nameof(CollisionManager), nameof(InitCollisionLayer), "Failed to InitCollisionLayer.");
            }
        }

        private void SetCollisionLayers(Define.CollisionLayers layer1, Define.CollisionLayers layer2, bool canCollision)
        {
            if (canCollision)
                Physics2D.IgnoreLayerCollision((int)layer1, (int)layer2, false);
            else
                Physics2D.IgnoreLayerCollision((int)layer1, (int)layer2, true);
        }

        public bool IsCorrectTarget(Define.CollisionLayers targetLayer, int otherLayer) 
                                                => (1 << (int)targetLayer & 1 << otherLayer) != 0;
    }
}

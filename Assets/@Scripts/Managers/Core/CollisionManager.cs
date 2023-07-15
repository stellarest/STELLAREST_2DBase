using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STELLAREST_2D
{
    public class CollisionManager
    {
        public void InitCollisionLayer(GameObject go, Define.CollisionLayers layer)
        {
            if (go.GetComponent<Collider2D>() != null)
                go.layer = (int)layer;
            else
            {
                Utils.LogError("Failed Init Collision Layer !!");
                Debug.Break();
            }
        }

        public void SetCollisionLayers(Define.CollisionLayers layer1, Define.CollisionLayers layer2, bool canCollision)
        {
            if (canCollision)
                Physics2D.IgnoreLayerCollision((int)layer1, (int)layer2, false);
            else
                Physics2D.IgnoreLayerCollision((int)layer1, (int)layer2, true);
        }

        public bool CheckCollisionTarget(Define.CollisionLayers targetLayer, int otherLayer) 
                                                => (1 << (int)targetLayer & 1 << otherLayer) != 0;
    }
}

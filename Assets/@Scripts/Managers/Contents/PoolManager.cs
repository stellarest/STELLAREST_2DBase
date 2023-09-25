using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace STELLAREST_2D
{
    public class Pool
    {
        public Pool(GameObject prefab)
        {
            _prefab = prefab;
            _pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
        }
        private GameObject _prefab;
        private IObjectPool<GameObject> _pool;
        
        private Transform _root;
        public Transform Root
        {
            get
            {
                if (_root == null)
                {
                    GameObject go = new GameObject() { name = $"{_prefab.name} Root" };
                    _root = go.transform;
                }

                return _root;
            }
        }

        #region Events

        public void Push(GameObject go) // 반납
        {
            _pool.Release(go);
        }

        public GameObject Pop() // 꺼내옴
        {
            return _pool.Get();
        }

        public GameObject OnCreate()
        {
            GameObject go = UnityEngine.Object.Instantiate(_prefab);
            go.transform.SetParent(Root);
            //go.transform.parent = Root;
            go.name = _prefab.name;
            
            return go;
        }

        public void OnGet(GameObject go)
        {
            go.SetActive(true);
        }

        public void OnRelease(GameObject go)
        {
            go.SetActive(false);
        }

        public void OnDestroy(GameObject go)
        {
            UnityEngine.Object.Destroy(go, Time.deltaTime);
        }

        #endregion
    }

    public class PoolManager
    {
        private Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

        // 스킬을 찍는 UI가 나오면 현재 작동하고 있는 모든 스킬을 먼저 Deactivate한다.
        // 이후에 모든 스킬을 찍고 난 이후에, UI가 꺼지면 ResetPools를 실행한다.
        // 이후에는 다시 AcquiredSkills를 Activate하면 된다.
        public void ClearPools()
        {
            foreach (KeyValuePair<string, Pool> pool in _pools)
            {
                Transform root = pool.Value.Root;
                for (int i = 0; i < root.childCount; ++i)
                    UnityEngine.Object.Destroy(root.GetChild(i).gameObject);
                
                UnityEngine.Object.Destroy(root.gameObject);
            }

            _pools.Clear();
        }

        public void ClearPool(GameObject go)
        {
            if (_pools.TryGetValue(go.name, out Pool value) == false)
            {
                Utils.LogStrong(nameof(PoolManager), nameof(ClearPool), $"Failed to ClearPool.");
                return;
            }
            
            Transform root = value.Root;
            for (int i = 0; i < root.childCount; ++i)
                UnityEngine.Object.Destroy(root.GetChild(i).gameObject);

            _pools.Remove(go.name);
        }

        public GameObject Pop(GameObject prefab) // 꺼냄
        {
            // 꺼낼 때 없으면 만든다.
            if (_pools.ContainsKey(prefab.name) == false)
                CreatePool(prefab);

            return _pools[prefab.name].Pop();
        }

        public bool Push(GameObject go) // 사용 후 반납
        {
            if (_pools.ContainsKey(go.name) == false)
                return false;

            _pools[go.name].Push(go);
            return true;
        }

        private void CreatePool(GameObject prefab)
        {
            Pool pool = new Pool(prefab);
            _pools.Add(prefab.name, pool);
        }
    }
}

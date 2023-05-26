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
            go.transform.parent = Root;
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

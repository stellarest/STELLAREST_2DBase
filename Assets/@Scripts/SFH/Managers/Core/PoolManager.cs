using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace STELLAREST_SFH
{
    internal class Pool
    {
        public Pool(GameObject prefab)
        {
            _prefab = prefab;
            _pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
        }

        private GameObject _prefab;
        private IObjectPool<GameObject> _pool;

        private Transform _root = null;
        public Transform Root
        {
            get
            {
                if (_root == null)
                {
                    GameObject go = new GameObject { name = $"@{_prefab.name}_Pool" };
                    _root = go.transform;
                }

                return _root;
            }
        }

        // 반환
        public void Push(GameObject go)
        {
            if (go.activeSelf)
                _pool.Release(go);
        }

        // Get
        public GameObject Pop() => _pool.Get();

#region Constructor Funcs
        private GameObject OnCreate()
        {
            GameObject go = UnityEngine.GameObject.Instantiate(_prefab);
            go.transform.SetParent(Root);
            go.name = _prefab.name;
            return go;
        }

        private void OnGet(GameObject go) => go.SetActive(true);
        private void OnRelease(GameObject go) => go.SetActive(false);
        private void OnDestroy(GameObject go) => UnityEngine.GameObject.Destroy(go, Time.deltaTime);
#endregion
    }

    public class PoolManager
    {
        private Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

        // Get
        public GameObject Pop(GameObject prefab)
        {
            if (_pools.ContainsKey(prefab.name) == false)
                this.CreatePool(prefab);
            
            return _pools[prefab.name].Pop();
        }

        // Release
        public bool Push(GameObject go)
        {
            if (_pools.ContainsKey(go.name) == false)
                return false;

            _pools[go.name].Push(go);
            return false;
        }

        public void Clear() => _pools.Clear();

        private void CreatePool(GameObject original)
        {
            Pool pool = new Pool(original);
            _pools.Add(original.name, pool);
        }
    }
}

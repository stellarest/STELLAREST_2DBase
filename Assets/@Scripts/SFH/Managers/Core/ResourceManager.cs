using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static STELLAREST_SFH.Define;

namespace STELLAREST_SFH
{
    public class ResourceManager
    {
        private Dictionary<string, UnityEngine.Object> _resources = new Dictionary<string, UnityEngine.Object>();
        private Dictionary<string, AsyncOperationHandle> _handles = new Dictionary<string, AsyncOperationHandle>();

#region Load, Pooling, Destroy Object (NOT RELATED ASYNC OPERATION)
        public T Load<T>(string key) where T : UnityEngine.Object
        {
            if (_resources.TryGetValue(key, out UnityEngine.Object resource))
                return resource as T;

            return null;
        }

        public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
        {
            GameObject prefab = this.Load<GameObject>(key);
            if (prefab == null)
            {
                Util.LogCritical($"{nameof(ResourceManager)}, {nameof(Instantiate)}");
                return null;
            }

            if (pooling)
                return Managers.Pool.Pop(prefab);

            GameObject go = UnityEngine.Object.Instantiate(prefab, parent);
            go.name = prefab.name;
            return go;
        }

        public void Destroy(GameObject go)
        {
            if (go == null)
                return;

            if (Managers.Pool.Push(go))
                return;

            UnityEngine.Object.Destroy(go, Time.deltaTime);
        }
#endregion

#region Addressable, Async Operation
        public void LoadAllAsync<T>(string label, Action<string, int, int> callback) where T : UnityEngine.Object
        {
            var opHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T)); // LOAD LABEL
            opHandle.Completed += (op) =>
            {
                int loadCount = 0;
                int totalCount = op.Result.Count;
                
                foreach (var result in op.Result)
                {
                    if (result.PrimaryKey.Contains(FixedValue.String.DOT_SPRITE))
                    {
                        this.LoadAsync<Sprite>(result.PrimaryKey, (obj) => 
                        {
                            loadCount++;
                            callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                        });
                    }
                    else
                    {
                        this.LoadAsync<T>(result.PrimaryKey, (obj) => 
                        {
                            loadCount++;
                            callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                        });
                    }
                }
            };
        }

        private void LoadAsync<T>(string key, Action<T> callback = null) where T : UnityEngine.Object
        {
            // Cache
            if (_resources.TryGetValue(key, out UnityEngine.Object resource))
            {
                callback?.Invoke(resource as T);
                return;
            }

            // texture to sprite
            // Debug.Log("key origin : " + key); // EXPGem_01.sprite
            // Debug.Log("loadKey : " + loadKey); // EXPGem_01.sprite[EXPGem_01]
            string loadKey = key;
            if (key.Contains(FixedValue.String.DOT_SPRITE))
                loadKey = $"{key}[{key.Replace(FixedValue.String.DOT_SPRITE, "")}]";

            var asyncOperation = Addressables.LoadAssetAsync<T>(loadKey); // LOAD ASSET
            asyncOperation.Completed += (op) =>
            {
                _resources.Add(key, op.Result);
                _handles.Add(key, asyncOperation);
                callback?.Invoke(op.Result);
            };
        }
#endregion

        public void Clear()
        {
            _resources.Clear();
            foreach (var handle in _handles)
                UnityEngine.AddressableAssets.Addressables.Release(handle);
            _handles.Clear();
        }
    }
}

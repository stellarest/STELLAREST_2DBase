using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using Object = UnityEngine.Object;

namespace STELLAREST_2D
{
    public class ResourceManager
    {
        private Dictionary<string, UnityEngine.Object> _resources = new Dictionary<string, Object>();

        public T Load<T>(string key) where T : UnityEngine.Object
        {
            if (_resources.TryGetValue(key, out UnityEngine.Object resource))
                return resource as T;

            return null;
        }

        public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
        {
            GameObject prefab = Load<GameObject>($"{key}");
            if (prefab == null)
            {
                Debug.LogWarning($"Failed to load prefab : {key}");
                return null;
            }

            // Pooling
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
            {
                Debug.Log("DES PROJ..2");
                return;
            }

            UnityEngine.Object.Destroy(go, Time.deltaTime);
        }

        #region Addressable
        public void LoadAsync<T>(string key, Action<T> callback = null) where T : UnityEngine.Object
        {
            // 캐시 확인
            if (_resources.TryGetValue(key, out UnityEngine.Object resource))
            {
                callback?.Invoke(resource as T);
                return;
            }

            // texture to sprite
            string loadKey = key;
            if (key.Contains(".sprite"))
            {
                // Debug.Log("key origin : " + key); // EXPGem_01.sprite
                loadKey = $"{key}[{key.Replace(".sprite", "")}]";
            }
            // Debug.Log("loadKey : " + loadKey); // EXPGem_01.sprite[EXPGem_01]

            // 없으면 비동기 로딩 시작
            var asyncOperation = Addressables.LoadAssetAsync<T>(loadKey);
            asyncOperation.Completed += (result => 
            {
                _resources.Add(key, result.Result);
                callback?.Invoke(result.Result);
            });
        }

        public void LoadAllAsync<T>(string label, Action<string, int, int> callback) where T : UnityEngine.Object
        {
            var handle = Addressables.LoadResourceLocationsAsync(label, typeof(T));
            handle.Completed += (result => 
            {
                int loadCount = 0;
                int totalCount = result.Result.Count;

                foreach (var asyncResult in result.Result)
                {
                    this.LoadAsync<T>(asyncResult.PrimaryKey, (obj => 
                    {
                        loadCount++;
                        callback?.Invoke(asyncResult.PrimaryKey, loadCount, totalCount);
                    }));
                }
            });
        }
        #endregion
    }
}
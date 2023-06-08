using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace GameAssets.Scripts.ObjectPooling
{
    public class ObjectPooler : MonoBehaviour
    {
        [field: SerializeField] public List<Pool<MonoBehaviour>> Pools { get; private set; }
        public static ObjectPooler Instance { get; private set; } //todo: Change to Zenject Injection


        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
            }

            Instance = this;
            CreatePools();
        }


        //todo: generic create pool and spawn methods
        private void CreatePools()
        {
            Pools?.ForEach(pool =>
            {
                pool.ObjectPool = new ObjectPool<MonoBehaviour>(
                    () => Instantiate(pool.Prefab),
                    pooledObject => pooledObject.gameObject.SetActive(true),
                    pooledObject =>
                    {
                        pooledObject.gameObject.SetActive(false);
                        pooledObject.transform.parent = pool.Container?.transform;
                    },
                    Destroy, false, pool.DefaultSize, pool.MaxSize
                );

                if (pool.CreateOnStart)
                {
                    if (pool.Container == null)
                    {
                        var container = new GameObject(pool.ObjectTypeString)
                        {
                            transform =
                            {
                                parent = transform
                            }
                        };
                        pool.Container = container;
                    }

                    for (int i = 0; i < pool.DefaultSize; i++)
                    {
                        pool.ObjectPool.Release(Instantiate(pool.Prefab, pool.Container.transform));
                    }
                }
            });
        }

        public void ReturnToPool(string objectTypeString, MonoBehaviour pooledObject)
        {
            var typeString = objectTypeString.Split('.').LastOrDefault();
            var pool = Pools.Find(x => x.ObjectTypeString.Equals(typeString));
            pool.ObjectPool.Release(pooledObject);
        }

        public MonoBehaviour SpawnObject(string objectTypeString, Vector3 position, Quaternion rotation,
            Transform parentTransform)
        {
            var typeString = objectTypeString.Split('.').LastOrDefault();
            Pools.Find(x => x.ObjectTypeString.Equals(typeString)).ObjectPool.Get(out var spawnedObject);
            var spawnedTransform = spawnedObject.transform;
            if (position != null)
            {
                spawnedTransform.position = position;
            }

            if (rotation != null)
            {
                spawnedTransform.rotation = rotation;
            }

            spawnedTransform.parent = parentTransform;
            return spawnedObject;
        }
    }
}
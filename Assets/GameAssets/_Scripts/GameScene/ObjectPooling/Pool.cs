using System;
using UnityEngine;
using UnityEngine.Pool;

namespace GameAssets.Scripts.ObjectPooling
{
    [Serializable]
    public class Pool<T> where T : MonoBehaviour
    {
        public string ObjectTypeString;
        public GameObject Container;
        public int DefaultSize;
        public int MaxSize;
        public bool CreateOnStart;
        public T Prefab;
        public ObjectPool<T> ObjectPool;
    }
}
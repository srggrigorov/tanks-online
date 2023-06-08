using System;
using UnityEngine;

namespace GameAssets._Scripts.GameScene
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private int _healthPoints;
        [SerializeField] private GameObject _explosionParticlePrefab;

        public event Action<int> OnHealthChanged;

        private void Start()
        {
            OnHealthChanged?.Invoke(_healthPoints);
        }

        public void TakeDamage(int damageAmount)
        {
            _healthPoints -= damageAmount;
            OnHealthChanged?.Invoke(_healthPoints);

            if (_healthPoints <= 0)
            {
                if (_explosionParticlePrefab != null)
                {
                    var explosion = NetworkObjectPool.Singleton.GetNetworkObject(_explosionParticlePrefab,
                        transform.position,
                        Quaternion.identity);
                    if (!explosion.IsSpawned)
                    {
                        explosion.Spawn(true);
                    }
                }

                Destroy(gameObject);
            }
        }
    }
}
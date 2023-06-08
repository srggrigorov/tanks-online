using Unity.Netcode;
using UnityEngine;

namespace GameAssets._Scripts.GameScene
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Projectile : NetworkBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private int _damageAmount;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private GameObject _projectileHitPrefab;
        [field: SerializeField] public Collider2D Collider { get; private set; }


        private Collider2D _ignoredCollider;


        private void Awake()
        {
            _rigidbody2D ??= GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(_damageAmount);
            }

            if (_ignoredCollider != null)
            {
                Physics2D.IgnoreCollision(_ignoredCollider, Collider, false);
            }

            var projectileHit = NetworkObjectPool.Singleton
                .GetNetworkObject(_projectileHitPrefab, collision.contacts[0].point, Quaternion.identity);
            if (!projectileHit.IsSpawned)
            {
                projectileHit.Spawn(true);
            }

            projectileHit.transform.rotation *=
                Quaternion.FromToRotation(projectileHit.transform.up, collision.contacts[0].normal);

            NetworkObject.Despawn();
        }

        public void SetIgnoredCollider(Collider2D ignoredCollider)
        {
            _ignoredCollider = ignoredCollider;
            if (ignoredCollider == null && Collider == null)
            {
                return;
            }

            Physics2D.IgnoreCollision(_ignoredCollider, Collider, true);
        }

        public void GiveVelocity()
        {
            _rigidbody2D.velocity = transform.up * _speed;
        }
    }
}
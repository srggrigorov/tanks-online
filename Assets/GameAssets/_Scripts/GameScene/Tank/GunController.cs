using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameAssets._Scripts.GameScene
{
    public class GunController : NetworkBehaviour
    {
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private int _reloadTimeMs;
        [SerializeField] private Collider2D _ignoredCollider;
        [SerializeField] private GameObject _projectilePrefab;

        [Space(5)] [SerializeField] private ParticleSystem _shootParticle;

        private Controls _controls;
        private bool _readyToShoot = true;
        private bool _isShooting;

        private void Awake()
        {
            _controls = new Controls();
        }

        private void OnEnable()
        {
            _controls.Enable();
            _controls.TouchScreen.Shoot.started += StartShooting;
            _controls.TouchScreen.Shoot.canceled += StopShooting;
        }

        private void StopShooting(InputAction.CallbackContext context)
        {
            if (!IsOwner || !Application.isFocused)
            {
                return;
            }

            _isShooting = false;
        }

        private void StartShooting(InputAction.CallbackContext context)
        {
            _isShooting = true;
            ShootProjectile();
        }

        private async void ShootProjectile()
        {
            if (!IsOwner || !Application.isFocused || !_readyToShoot || !_isShooting)
            {
                return;
            }

            var projectileObj =
                NetworkObjectPool.Singleton.GetNetworkObject(_projectilePrefab, _shootPoint.position,
                    _shootPoint.rotation);
            if (!projectileObj.IsSpawned)
            {
                projectileObj.Spawn(true);
            }


            if (projectileObj.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.SetIgnoredCollider(_ignoredCollider);
                projectile.GiveVelocity();
            }

            _shootParticle?.Play();

            _readyToShoot = false;
            await Task.Delay(_reloadTimeMs);
            _readyToShoot = true;
            ShootProjectile();
        }

        private void OnDisable()
        {
            _controls.TouchScreen.Shoot.started -= StartShooting;
            _controls.TouchScreen.Shoot.canceled -= StopShooting;
            _controls.Disable();
        }
    }
}
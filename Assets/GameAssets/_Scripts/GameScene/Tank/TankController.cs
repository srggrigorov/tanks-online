using Unity.Netcode;
using UnityEngine;

namespace GameAssets._Scripts.GameScene
{
    [RequireComponent(typeof(Rigidbody2D)),]
    public class TankController : NetworkBehaviour
    {
        [SerializeField] private float _movementSpeed;
        [SerializeField] private ParticleSystem _trailParticle;

        private Controls _controls;
        private Rigidbody2D _rigidbody2D;

        private void Awake()
        {
            _controls = new Controls();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (!IsOwner || !Application.isFocused)
            {
                return;
            }

            var moveDirection = _controls.TouchScreen.Move.ReadValue<Vector2>();
            _rigidbody2D.velocity = moveDirection * _movementSpeed;
            _rigidbody2D.angularVelocity = 0;

            if (moveDirection.magnitude > 0.01f)
            {
                _rigidbody2D.rotation = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90f;
                _trailParticle?.Play();
            }
            else
            {
                _trailParticle?.Stop();
            }
        }

        private void OnEnable()
        {
            _controls.Enable();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }
    }
}
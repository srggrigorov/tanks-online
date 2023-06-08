using System;
using UnityEngine;
using Unity.Netcode;

namespace GameAssets._Scripts.GameScene
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class CoinCollector : NetworkBehaviour
    {
        [field: SerializeField]
        [field: Min(0.01f)]
        public float CoinCollectionStartSpeed { get; private set; }

        [field: SerializeField] public CircleCollider2D Collider { get; private set; }

        private int _coinsAmount;

        public event Action<int> OnCoinsAmountChanged;

        private void Awake()
        {
            Collider ??= GetComponent<CircleCollider2D>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            OnCoinsAmountChanged?.Invoke(_coinsAmount);
        }

        public void AddCoin()
        {
            _coinsAmount++;
            OnCoinsAmountChanged?.Invoke(_coinsAmount);
        }
    }
}
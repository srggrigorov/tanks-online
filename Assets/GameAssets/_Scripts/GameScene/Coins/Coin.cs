using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

namespace GameAssets._Scripts.GameScene
{
    [RequireComponent(typeof(Collider2D))]
    public class Coin : NetworkBehaviour
    {
        [SerializeField] private Collider2D _collider2D;

        private Tweener _collectionTweener;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!IsServer)
            {
                return;
            }

            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            if (collider.TryGetComponent<CoinCollector>(out var collector))
            {
                GoToCollector(collector);
            }
        }

        protected virtual void GoToCollector(CoinCollector collector)
        {
            Transform targetTransform = collector.transform;
            Transform coinTransform = transform;
            _collectionTweener =
                coinTransform
                    .DOMove(targetTransform.position,
                        collector.CoinCollectionStartSpeed /
                        Vector3.Distance(targetTransform.position, coinTransform.position))
                    .SetSpeedBased(true).SetAutoKill().OnComplete(() =>
                    {
                        if (!_collectionTweener.IsActive())
                        {
                            return;
                        }

                        collector.AddCoin();
                        if (NetworkObject.IsSpawned)
                        {
                            NetworkObject.Despawn();
                        }
                    });

            _collectionTweener.OnUpdate(() =>
            {
                if (coinTransform == null || targetTransform == null || !_collectionTweener.IsActive() ||
                    _collectionTweener.IsComplete())
                {
                    return;
                }

                if (Vector3.Distance(targetTransform.position, coinTransform.position) >
                    collector.Collider.bounds.extents.magnitude + _collider2D.bounds.extents.magnitude)
                {
                    _collectionTweener.Kill();
                }

                if (Vector3.Distance(targetTransform.position, coinTransform.position) >
                    _collider2D.bounds.extents.magnitude)
                {
                    _collectionTweener.ChangeEndValue(targetTransform.position,
                        collector.CoinCollectionStartSpeed /
                        Vector3.Distance(targetTransform.position, coinTransform.position),
                        true).SetSpeedBased(true);
                }
                else
                {
                    _collectionTweener.Kill(true);
                }
            });
        }
    }
}
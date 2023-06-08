using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using Unity.Netcode;
using UnityEngine.Tilemaps;

namespace GameAssets._Scripts.GameScene
{
    public class CoinSpawner : NetworkBehaviour
    {
        [SerializeField] private Vector2 _bordersAbs;
        [SerializeField] private int _spawnDelayTimeMs;
        [SerializeField] private int _maxCoinsOnArena;
        [SerializeField] private float _minDistanceFromWall;
        [SerializeField] private TilemapCollider2D _tilemapCollider;
        [SerializeField] private GameObject _coinPrefab;

        private List<Coin> _coinsList;
        private Vector2 _spawnPoint;
        private int _coinsAmount;

        private CancellationTokenSource _tokenSource;
        private CancellationToken _token;

        private void Awake()
        {
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            _coinsList = new List<Coin>();
        }

        public override async void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                return;
            }

            base.OnNetworkSpawn();
            await SpawnCoin();
        }

        private async Task SpawnCoin()
        {
            while (true)
            {
                await Task.Delay(_spawnDelayTimeMs, _token);

                _coinsAmount = _coinsList.Count(coin => coin.gameObject.activeInHierarchy);
                while (_coinsAmount >= _maxCoinsOnArena)
                {
                    if (_token.IsCancellationRequested)
                    {
                        return;
                    }

                    _coinsAmount = _coinsList.Count(coin => coin.gameObject.activeInHierarchy);
                    await Task.Yield();
                }

                _spawnPoint.x = Random.Range(-_bordersAbs.x, _bordersAbs.x);
                _spawnPoint.y = Random.Range(-_bordersAbs.y, _bordersAbs.y);
                var distanceFromWall = _spawnPoint - _tilemapCollider.ClosestPoint(_spawnPoint);
                while (distanceFromWall.magnitude < _minDistanceFromWall)
                {
                    _spawnPoint += distanceFromWall.magnitude == 0
                        ? Vector2.one * _minDistanceFromWall
                        : distanceFromWall;
                    distanceFromWall = _spawnPoint - _tilemapCollider.ClosestPoint(_spawnPoint);
                    await Task.Yield();
                }

                var coinNetworkObject =
                    NetworkObjectPool.Singleton.GetNetworkObject(_coinPrefab, _spawnPoint, Quaternion.identity);
                if (!coinNetworkObject.IsSpawned)
                {
                    coinNetworkObject.Spawn(true);
                    if (coinNetworkObject.TryGetComponent<Coin>(out var coin) && !_coinsList.Contains(coin))
                    {
                        _coinsList.Add(coin);
                    }
                }


                _tokenSource.Dispose();
                _tokenSource = new CancellationTokenSource();
                _token = _tokenSource.Token;
            }
        }

        private new void OnDestroy()
        {
            _tokenSource.Cancel();
        }
    }
}
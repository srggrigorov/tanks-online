using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameAssets._Scripts.GameScene
{
    public class TankUI : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private CoinCollector _coinCollector;
        [SerializeField] private Slider _healhBar;
        [SerializeField] private TMP_Text _coinAmountText;

        [Space(5)] [SerializeField] private Vector3 _centerOffset;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
            _coinCollector.OnCoinsAmountChanged += HandleCoinAmountChange;
            _health.OnHealthChanged += HandleHealthChange;
        }

        private void Update()
        {
            _transform.localPosition = Vector3.zero;
            _transform.rotation = Quaternion.Euler(Vector3.zero);
            _transform.position += _centerOffset;
        }

        private void HandleHealthChange(int currentHealth)
        {
            _healhBar.value = currentHealth;
        }

        private void HandleCoinAmountChange(int currentAmount)
        {
            _coinAmountText.text = currentAmount.ToString();
        }
    }
}
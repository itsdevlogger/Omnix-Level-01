using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Omnix.CharaCon.HealthSystem;

namespace Omnix.CharaCon.HealthTest
{
    public class HealthTestUi : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private GameObject _deadUi;
        [SerializeField] private GameObject _aliveUi;
        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _healthAmount;
        [SerializeField] private DamageInfoUi _damageInfoUi;
        
        public string HealthAmountText => $"{_health.Value} / {_health.StartValue}";

        private void OnEnable()
        {
            _health.OnDamaged += OnDamaged;
            _health.OnDeath += OnDeath;
            _health.OnSpawnOrRespawned += OnSpawnOrRespawned;
            
            _deadUi.SetActive(_health.IsDead);
            _aliveUi.SetActive(_health.IsAlive);
            _slider.minValue = 0;
            _slider.maxValue = _health.StartValue;
            _slider.value = _health.Value;
            _healthAmount.SetText(HealthAmountText);
        }
        
        private void OnDisable()
        {
            _health.OnDamaged -= OnDamaged;
            _health.OnDeath -= OnDeath;
            _health.OnSpawnOrRespawned -= OnSpawnOrRespawned;
        }

        private void OnSpawnOrRespawned()
        {
            _aliveUi.SetActive(true);
            _deadUi.SetActive(false);
            _slider.value = _health.Value;
            _healthAmount.SetText(HealthAmountText);
        }

        private void OnDeath()
        {
            _aliveUi.SetActive(false);
            _deadUi.SetActive(true);
            _healthAmount.SetText(HealthAmountText);
        }

        private void OnDamaged(DamageInfo damageApplied, float healthDeducted)
        {
            _damageInfoUi.Set(damageApplied);
            _slider.value = _health.Value;
            _healthAmount.SetText(HealthAmountText);
        }
    }
}
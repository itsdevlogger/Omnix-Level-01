using System.Globalization;
using Omnix.CharaCon.HealthSystem;
using TMPro;
using UnityEngine;

namespace Omnix.CharaCon.HealthTest
{
    public class DamageInfoUi : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _dealer;
        [SerializeField] private TextMeshProUGUI _position;
        [SerializeField] private TextMeshProUGUI _direction;
        [SerializeField] private TextMeshProUGUI _force;
        [SerializeField] private TextMeshProUGUI _hitCollider;
        [SerializeField] private TextMeshProUGUI _receiver;
        [SerializeField] private TextMeshProUGUI _rawAmount;
        [SerializeField] private TextMeshProUGUI _amount;

        
        public void Set(DamageInfo damageInfo)
        {
            MonoBehaviour dealer = damageInfo.dealer as MonoBehaviour;
            if (dealer != null) _dealer.SetText(dealer.gameObject.name);
            _position.SetText(damageInfo.position.ToString());
            _direction.SetText(damageInfo.direction.ToString());
            _force.SetText(damageInfo.force.ToString());
            _hitCollider.SetText(damageInfo.hitCollider.name);
            _receiver.SetText(damageInfo.receiver.name);
            _rawAmount.SetText(damageInfo.rawAmount.ToString(CultureInfo.InvariantCulture));
            _amount.SetText(damageInfo.amount.ToString(CultureInfo.InvariantCulture));
        }
    }
}
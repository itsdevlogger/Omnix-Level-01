using MenuManagement.Base;
using UnityEngine;

namespace Omnix.CharaCon.HealthSystem
{
    [GroupProperties("References", nameof(_agent))]
    public class AgentHealth : Health
    {
        [SerializeField] private Agent _agent;
        
        protected override void Reset()
        {
            base.Reset();
            _agent = GetComponent<Agent>();
        }
        
        public override void TakeDamage(DamageInfo info)
        {
            base.TakeDamage(info);
            _agent.AddForce(info.force);
        }
    }
}
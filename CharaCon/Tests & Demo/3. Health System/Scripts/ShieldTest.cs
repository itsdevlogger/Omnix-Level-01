using System;
using System.Collections.Generic;
using Omnix.CharaCon.HealthSystem;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Omnix.CharaCon.HealthTest
{
    public class ShieldTest : MonoBehaviour
    {
        [Serializable]
        private class ShieldStats
        {
            [Tooltip("Name of original shield")] public string name;
            [Tooltip("Value of original shield before applying damage")] public float valueBefore;
            [Tooltip("Damage Absorption Rate of original shield")] public float absorptionRate;
            [Tooltip("Damage that was applied to original shield")] public float totalDamageApplied;
            [Tooltip("Damage that was absorbed by the original shield")] public float totalDamageAbsorbed;
            [Tooltip("Value of original shield after damage is applied")] public float valueAfter;
            [Tooltip("Amount of damage that remain un-dealt")] public float damagePassedThrough;

            public ShieldStats(Shield shield, float damageToApply)
            {
                this.name = shield.name;
                this.valueBefore = shield.value;
                this.absorptionRate = shield.absorptionRate;
                this.totalDamageApplied = damageToApply;
                shield.restoreOnRespawn = true;
                shield.Init();
                damagePassedThrough = shield.Damage(damageToApply);
                valueAfter = shield.value;
                totalDamageAbsorbed = valueBefore - valueAfter;
                shield.Restore();
            }
        }
        
        public Shield[] shields;
        public float damageToApply;

        [SerializeField] private List<ShieldStats> _shieldsStats;

        public void ApplyDamage()
        {
            if (shields == null)
            {
                Debug.Log($"Final Damage Left: {damageToApply}");
                return;
            }
            float damage = damageToApply;
            _shieldsStats = new List<ShieldStats>();
            foreach (Shield original in shields)
            {
                var shieldStats = new ShieldStats(original, damage);
                _shieldsStats.Add(shieldStats);
                damage = shieldStats.damagePassedThrough;
                if (damage < 0) break;
            }
            Debug.Log($"Final Damage Left: {damage}");
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(ShieldTest))]
    public class ShieldTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Apply Damage"))
            {
                ((ShieldTest)target).ApplyDamage();
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }
    }
    #endif
}
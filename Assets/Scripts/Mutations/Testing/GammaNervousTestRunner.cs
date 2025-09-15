using UnityEngine;
using Mutations.Effects.NervousSystem;
using Player;

namespace Mutations.Testing
{
    public class GammaNervousTestRunner : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] private GammaNervousMajorEffect testEffect;
        [SerializeField] private int testLevel = 1;
        [SerializeField] private bool autoTest = false;
        
        private PlayerModel playerModel;
        private bool effectApplied = false;
        
        private void Start()
        {
            playerModel = FindObjectOfType<PlayerModel>();
            
            if (autoTest)
            {
                Invoke(nameof(RunTest), 1f);
            }
        }
        
        [ContextMenu("Test Apply Effect")]
        public void TestApplyEffect()
        {
            if (testEffect == null || playerModel == null)
            {
                Debug.LogError("[Test] Missing testEffect or playerModel!");
                return;
            }
            
            if (effectApplied)
            {
                Debug.LogWarning("[Test] Effect already applied! Remove first.");
                return;
            }
            
            Debug.Log($"[Test] Applying Gamma Nervous Major Effect Level {testLevel}");
            Debug.Log($"[Test] Before - Healing Multiplier: {playerModel.HealingMultiplier:F2}");
            Debug.Log($"[Test] Before - Drain Rate: {playerModel.StatContext.Source.Get(playerModel.StatRefs.passiveDrainRate):F2}");
            
            testEffect.ApplyEffect(playerModel.gameObject, testLevel);
            effectApplied = true;
            
            Debug.Log($"[Test] After - Healing Multiplier: {playerModel.HealingMultiplier:F2}");
            Debug.Log($"[Test] After - Drain Rate: {playerModel.StatContext.Source.Get(playerModel.StatRefs.passiveDrainRate):F2}");
        }
        
        [ContextMenu("Test Remove Effect")]
        public void TestRemoveEffect()
        {
            if (testEffect == null || playerModel == null)
            {
                Debug.LogError("[Test] Missing testEffect or playerModel!");
                return;
            }
            
            if (!effectApplied)
            {
                Debug.LogWarning("[Test] No effect applied to remove!");
                return;
            }
            
            Debug.Log("[Test] Removing Gamma Nervous Major Effect");
            Debug.Log($"[Test] Before Remove - Healing Multiplier: {playerModel.HealingMultiplier:F2}");
            Debug.Log($"[Test] Before Remove - Drain Rate: {playerModel.StatContext.Source.Get(playerModel.StatRefs.passiveDrainRate):F2}");
            
            testEffect.RemoveEffect(playerModel.gameObject);
            effectApplied = false;
            
            Debug.Log($"[Test] After Remove - Healing Multiplier: {playerModel.HealingMultiplier:F2}");
            Debug.Log($"[Test] After Remove - Drain Rate: {playerModel.StatContext.Source.Get(playerModel.StatRefs.passiveDrainRate):F2}");
        }
        
        [ContextMenu("Test Healing")]
        public void TestHealing()
        {
            if (playerModel == null)
            {
                Debug.LogError("[Test] Missing playerModel!");
                return;
            }
            
            float testHealAmount = 1.0f;
            Debug.Log($"[Test] Testing healing with {testHealAmount} base amount");
            Debug.Log($"[Test] Current healing multiplier: {playerModel.HealingMultiplier:F2}");
            Debug.Log($"[Test] Expected heal: {testHealAmount * playerModel.HealingMultiplier:F2}");
            
            float beforeHealth = playerModel.CurrentHealth;
            playerModel.RecoverTime(testHealAmount);
            float afterHealth = playerModel.CurrentHealth;
            float actualHealing = afterHealth - beforeHealth;
            
            Debug.Log($"[Test] Before Health: {beforeHealth:F2}");
            Debug.Log($"[Test] After Health: {afterHealth:F2}");
            Debug.Log($"[Test] Actual Healing: {actualHealing:F2}");
        }
        
        private void RunTest()
        {
            TestApplyEffect();
            Invoke(nameof(TestHealing), 1f);
            Invoke(nameof(TestRemoveEffect), 3f);
        }
    }
}
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

            testEffect.ApplyEffect(playerModel.gameObject, testLevel);
            effectApplied = true;
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
            
            testEffect.RemoveEffect(playerModel.gameObject);
            effectApplied = false;
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
            
            float beforeHealth = playerModel.CurrentHealth;
            playerModel.RecoverTime(testHealAmount);
            float afterHealth = playerModel.CurrentHealth;
            float actualHealing = afterHealth - beforeHealth;
        }
        
        private void RunTest()
        {
            TestApplyEffect();
            Invoke(nameof(TestHealing), 1f);
            Invoke(nameof(TestRemoveEffect), 3f);
        }
    }
}
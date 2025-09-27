using Mutations.Effects.NervousSystem;
using UnityEngine;

namespace Mutations.Testing
{
    public class GammaNervousMajorTester : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private GammaNervousMajorEffect effectToTest;
        [SerializeField] private GameObject testPlayer;
        [SerializeField] private int testLevel = 1;

        [Header("Test Results")]
        [SerializeField] private bool effectApplied = false;
        [SerializeField] private float currentHealingMultiplier;
        [SerializeField] private float currentHealthDrain;

        private void Start()
        {
            if (testPlayer == null)
                testPlayer = gameObject;
        }

        [NaughtyAttributes.Button("Apply Effect")]
        public void ApplyEffect()
        {
            if (effectToTest == null)
            {
                Debug.LogError("[GammaNervousMajorTester] No effect assigned!");
                return;
            }

            effectToTest.ApplyEffect(testPlayer, testLevel);
            effectApplied = true;

            UpdateTestResults();

            Debug.Log($"[GammaNervousMajorTester] Applied effect at level {testLevel}");
        }

        [NaughtyAttributes.Button("Remove Effect")]
        public void RemoveEffect()
        {
            if (effectToTest == null)
            {
                Debug.LogError("[GammaNervousMajorTester] No effect assigned!");
                return;
            }

            effectToTest.RemoveEffect(testPlayer);
            effectApplied = false;

            UpdateTestResults();

            Debug.Log("[GammaNervousMajorTester] Effect removed");
        }

        [NaughtyAttributes.Button("Test All Levels")]
        public void TestAllLevels()
        {
            if (effectToTest == null)
            {
                Debug.LogError("[GammaNervousMajorTester] No effect assigned!");
                return;
            }

            Debug.Log("=== Testing All Levels ===");

            for (int level = 1; level <= 4; level++)
            {
                float healingMult = effectToTest.GetHealingMultiplierAtLevel(level);
                float drainIncrease = effectToTest.GetHealthDrainIncreaseAtLevel(level);
                string description = effectToTest.GetDescriptionAtLevel(level);

                Debug.Log($"Level {level}: Healing x{healingMult:F1}, Drain +{drainIncrease:F1}/s");
                Debug.Log($"Description: {description}");
            }
        }

        [NaughtyAttributes.Button("Update Test Results")]
        public void UpdateTestResults()
        {
            if (effectToTest == null) return;

            currentHealingMultiplier = effectToTest.GetHealingMultiplierAtLevel(testLevel);
            currentHealthDrain = effectToTest.GetHealthDrainIncreaseAtLevel(testLevel);
        }

        private void OnValidate()
        {
            if (effectToTest != null)
            {
                UpdateTestResults();
            }
        }

        private void OnGUI()
        {
            if (!Application.isPlaying) return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label("=== Gamma Nervous Major Tester ===");
            GUILayout.Label($"Effect Applied: {effectApplied}");
            GUILayout.Label($"Test Level: {testLevel}");
            GUILayout.Label($"Healing Multiplier: x{currentHealingMultiplier:F1}");
            GUILayout.Label($"Health Drain: +{currentHealthDrain:F1}/s");

            if (effectToTest != null)
            {
                GUILayout.Label($"Description: {effectToTest.GetDescriptionAtLevel(testLevel)}");
            }

            GUILayout.EndArea();
        }
    }
}
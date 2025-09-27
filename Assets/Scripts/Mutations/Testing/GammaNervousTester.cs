using UnityEngine;
using Mutations.Effects.NervousSystem;
using Player;

namespace Mutations.Testing
{
    public class GammaNervousTester : MonoBehaviour
    {
        [Header("Gamma Nervous Testing")]
        [SerializeField] private GammaNervousMajorEffect gammaNervousEffect;
        [SerializeField] private int mutationLevel = 1;
        [SerializeField] private bool showGUI = true;
        
        private PlayerModel playerModel;
        private bool effectApplied = false;
        
        private void Start()
        {
            playerModel = FindObjectOfType<PlayerModel>();
            
            if (playerModel == null)
            {
                Debug.LogError("[GammaNervousTester] PlayerModel not found!");
            }
            
            if (gammaNervousEffect == null)
            {
                Debug.LogWarning("[GammaNervousTester] GammaNervousEffect not assigned in inspector!");
            }
        }
        
        private void Update()
        {
            // Hotkeys para testing rápido
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (effectApplied)
                    RemoveMutation();
                else
                    ApplyMutation();
            }
            
            if (Input.GetKeyDown(KeyCode.N))
            {
                TestHealing();
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                GiveHealth();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                showGUI = !showGUI;
            }
        }
        
        private void OnGUI()
        {
            if (!showGUI || playerModel == null) return;
            
            // Panel de testing
            GUILayout.BeginArea(new Rect(Screen.width - 300, 50, 280, 200), "🧬 Gamma Nervous Tester", GUI.skin.window);
            
            GUILayout.Label($"Player: {(playerModel ? "✅" : "❌")}");
            GUILayout.Label($"Effect: {(gammaNervousEffect ? "✅" : "❌")}");
            GUILayout.Label($"Applied: {(effectApplied ? "✅" : "❌")}");
            
            GUILayout.Space(10);
            
            // Level selector
            GUILayout.BeginHorizontal();
            GUILayout.Label("Level:", GUILayout.Width(50));
            mutationLevel = (int)GUILayout.HorizontalSlider(mutationLevel, 1, 4, GUILayout.Width(100));
            GUILayout.Label(mutationLevel.ToString(), GUILayout.Width(20));
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            
            // Botones
            if (GUILayout.Button(effectApplied ? "Remove Mutation (M)" : "Apply Mutation (M)"))
            {
                if (effectApplied)
                    RemoveMutation();
                else
                    ApplyMutation();
            }
            
            if (GUILayout.Button("Test Healing (N)"))
            {
                TestHealing();
            }

            if (GUILayout.Button("Give Health (H)"))
            {
                GiveHealth();
            }

            GUILayout.Space(5);
            
            // Info actual
            if (playerModel?.StatContext != null)
            {
                //float healMult = playerModel.HealingMultiplier;
                float drainRate = playerModel.StatContext.Source.Get(playerModel.StatRefs.passiveDrainRate);
                
                //GUILayout.Label($"Heal Mult: {healMult:F2}");
                GUILayout.Label($"Drain Rate: {drainRate:F2}");
            }
            
            GUILayout.EndArea();
            
            // Instrucciones
            GUI.Label(new Rect(Screen.width - 300, 260, 280, 80),
                "Controls:\n" +
                "M - Toggle Mutation\n" +
                "N - Test Healing\n" +
                "H - Give Health\n" +
                "B - Toggle GUI");
        }
        
        [ContextMenu("Apply Mutation")]
        public void ApplyMutation()
        {
            if (!ValidateComponents()) return;
            
            if (effectApplied)
            {
                Debug.LogWarning("[GammaNervousTester] Mutation already applied!");
                return;
            }
            
            Debug.Log($"[GammaNervousTester] Applying Gamma Nervous Major - Level {mutationLevel}");
            LogCurrentStats("Before Apply");
            
            gammaNervousEffect.ApplyEffect(playerModel.gameObject, mutationLevel);
            effectApplied = true;
            
            LogCurrentStats("After Apply");
        }
        
        [ContextMenu("Remove Mutation")]
        public void RemoveMutation()
        {
            if (!ValidateComponents()) return;
            
            if (!effectApplied)
            {
                Debug.LogWarning("[GammaNervousTester] No mutation applied to remove!");
                return;
            }
            
            Debug.Log("[GammaNervousTester] Removing Gamma Nervous Major");
            LogCurrentStats("Before Remove");
            
            gammaNervousEffect.RemoveEffect(playerModel.gameObject);
            effectApplied = false;
            
            LogCurrentStats("After Remove");
        }
        
        [ContextMenu("Test Healing")]
        public void TestHealing()
        {
            if (playerModel == null)
            {
                Debug.LogError("[GammaNervousTester] PlayerModel not found!");
                return;
            }
            
            float testAmount = 2.0f;
            float beforeHealth = playerModel.CurrentHealth;
            //float healMult = playerModel.HealingMultiplier;
            
            //Debug.Log($"[GammaNervousTester] Testing healing: {testAmount} × {healMult:F2} = {testAmount * healMult:F2}");
            Debug.Log($"[GammaNervousTester] Health before: {beforeHealth:F1}");
            
            playerModel.RecoverTime(testAmount);
            
            float afterHealth = playerModel.CurrentHealth;
            float actualHealing = afterHealth - beforeHealth;
            
            Debug.Log($"[GammaNervousTester] Health after: {afterHealth:F1}");
            Debug.Log($"[GammaNervousTester] Actual healing: {actualHealing:F2}");
        }
        
        private bool ValidateComponents()
        {
            if (playerModel == null)
            {
                Debug.LogError("[GammaNervousTester] PlayerModel not found!");
                return false;
            }
            
            if (gammaNervousEffect == null)
            {
                Debug.LogError("[GammaNervousTester] GammaNervousEffect not assigned!");
                return false;
            }
            
            if (playerModel.StatContext == null)
            {
                Debug.LogError("[GammaNervousTester] PlayerModel.StatContext is null!");
                return false;
            }
            
            return true;
        }
        
        [ContextMenu("Give Health")]
        public void GiveHealth()
        {
            if (playerModel == null)
            {
                Debug.LogError("[GammaNervousTester] PlayerModel not found!");
                return;
            }

            float healthAmount = 10.0f;
            float beforeHealth = playerModel.CurrentHealth;
            float maxHealth = playerModel.MaxHealth;

            Debug.Log($"[GammaNervousTester] Giving {healthAmount} health");
            Debug.Log($"[GammaNervousTester] Before: {beforeHealth:F1}/{maxHealth:F1}");

            playerModel.RecoverTime(healthAmount);

            float afterHealth = playerModel.CurrentHealth;
            float actualHealing = afterHealth - beforeHealth;

            Debug.Log($"[GammaNervousTester] After: {afterHealth:F1}/{maxHealth:F1}");
            //Debug.Log($"[GammaNervousTester] Actual healing: {actualHealing:F2} (multiplier: {playerModel.HealingMultiplier:F2})");
        }

        private void LogCurrentStats(string moment)
        {
            if (playerModel?.StatContext == null) return;

            //float healMult = playerModel.HealingMultiplier;
            float drainRate = playerModel.StatContext.Source.Get(playerModel.StatRefs.passiveDrainRate);

            //Debug.Log($"[GammaNervousTester] {moment} - Healing Multiplier: {healMult:F2}, Drain Rate: {drainRate:F2}");
        }
    }
}
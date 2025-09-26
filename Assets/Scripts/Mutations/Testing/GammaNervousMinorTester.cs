using UnityEngine;
using Mutations.Effects.NervousSystem;
using Player;

namespace Mutations.Testing
{
    public class GammaNervousMinorTester : MonoBehaviour
    {
        [Header("Gamma Nervous Minor Testing")]
        [SerializeField] private GammaNervousMinorEffect gammaNervousMinorEffect;
        [SerializeField] private int mutationLevel = 1;
        [SerializeField] private bool showGUI = true;
        
        private PlayerModel playerModel;
        private bool effectApplied = false;
        
        private void Start()
        {
            playerModel = FindObjectOfType<PlayerModel>();
            
            if (playerModel == null)
            {
                Debug.LogError("[GammaNervousMinorTester] PlayerModel not found!");
            }
            
            if (gammaNervousMinorEffect == null)
            {
                Debug.LogWarning("[GammaNervousMinorTester] GammaNervousMinorEffect not assigned in inspector!");
            }
        }
        
        private void Update()
        {
            // Hotkeys para testing rápido
            if (Input.GetKeyDown(KeyCode.K))
            {
                if (effectApplied)
                    RemoveMutation();
                else
                    ApplyMutation();
            }
            
            if (Input.GetKeyDown(KeyCode.L))
            {
                TestAttraction();
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                ToggleGUI();
            }
        }
        
        private void OnGUI()
        {
            if (!showGUI || playerModel == null) return;
            
            // Panel de testing
            GUILayout.BeginArea(new Rect(Screen.width - 300, 270, 280, 180), "🧲 Gamma Minor Tester", GUI.skin.window);
            
            GUILayout.Label($"Player: {(playerModel ? "✅" : "❌")}");
            GUILayout.Label($"Effect: {(gammaNervousMinorEffect ? "✅" : "❌")}");
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
            if (GUILayout.Button(effectApplied ? "Remove Mutation (K)" : "Apply Mutation (K)"))
            {
                if (effectApplied)
                    RemoveMutation();
                else
                    ApplyMutation();
            }
            
            if (GUILayout.Button("Test Attraction (L)"))
            {
                TestAttraction();
            }
            
            GUILayout.Space(5);
            
            // Info actual
            if (gammaNervousMinorEffect != null)
            {
                float range = gammaNervousMinorEffect.GetAttractRangeAtLevel(mutationLevel);
                float speed = gammaNervousMinorEffect.GetAttractSpeedAtLevel(mutationLevel);
                
                GUILayout.Label($"Range: {range:F1}m");
                GUILayout.Label($"Speed: x{speed:F1}");
            }
            
            GUILayout.EndArea();
            
            // Instrucciones
            GUI.Label(new Rect(Screen.width - 300, 460, 280, 60),
                "Controls:\n" +
                "K - Toggle Mutation\n" +
                "L - Test Attraction\n" +
                "O - Toggle GUI");
        }
        
        [ContextMenu("Apply Mutation")]
        public void ApplyMutation()
        {
            if (!ValidateComponents()) return;
            
            if (effectApplied)
            {
                Debug.LogWarning("[GammaNervousMinorTester] Mutation already applied!");
                return;
            }
            
            Debug.Log($"[GammaNervousMinorTester] Applying Gamma Nervous Minor - Level {mutationLevel}");
            LogCurrentStats("Before Apply");
            
            gammaNervousMinorEffect.ApplyEffect(playerModel.gameObject, mutationLevel);
            effectApplied = true;
            
            LogCurrentStats("After Apply");
        }
        
        [ContextMenu("Remove Mutation")]
        public void RemoveMutation()
        {
            if (!ValidateComponents()) return;
            
            if (!effectApplied)
            {
                Debug.LogWarning("[GammaNervousMinorTester] No mutation applied to remove!");
                return;
            }
            
            Debug.Log("[GammaNervousMinorTester] Removing Gamma Nervous Minor");
            LogCurrentStats("Before Remove");
            
            gammaNervousMinorEffect.RemoveEffect(playerModel.gameObject);
            effectApplied = false;
            
            LogCurrentStats("After Remove");
        }
        
        [ContextMenu("Test Attraction")]
        public void TestAttraction()
        {
            if (gammaNervousMinorEffect == null)
            {
                Debug.LogError("[GammaNervousMinorTester] GammaNervousMinorEffect not found!");
                return;
            }
            
            float range = gammaNervousMinorEffect.GetAttractRangeAtLevel(mutationLevel);
            float speed = gammaNervousMinorEffect.GetAttractSpeedAtLevel(mutationLevel);
            
            Debug.Log($"[GammaNervousMinorTester] Attraction Test Level {mutationLevel}:");
            Debug.Log($"[GammaNervousMinorTester] - Orb Attract Range: {range:F1}m");
            Debug.Log($"[GammaNervousMinorTester] - Orb Attract Speed: x{speed:F1}");
            Debug.LogWarning("[GammaNervousMinorTester] TODO: Visual debug showing attraction radius");
        }

        [ContextMenu("Toggle GUI")]
        public void ToggleGUI()
        {
            showGUI = !showGUI;
            Debug.Log($"[GammaNervousMinorTester] GUI {(showGUI ? "enabled" : "disabled")}");
        }
        
        private bool ValidateComponents()
        {
            if (playerModel == null)
            {
                Debug.LogError("[GammaNervousMinorTester] PlayerModel not found!");
                return false;
            }
            
            if (gammaNervousMinorEffect == null)
            {
                Debug.LogError("[GammaNervousMinorTester] GammaNervousMinorEffect not assigned!");
                return false;
            }
            
            if (playerModel.StatContext == null)
            {
                Debug.LogError("[GammaNervousMinorTester] PlayerModel.StatContext is null!");
                return false;
            }
            
            return true;
        }
        
        private void LogCurrentStats(string moment)
        {
            if (playerModel?.StatContext == null || gammaNervousMinorEffect == null) return;
            
            float range = gammaNervousMinorEffect.GetAttractRangeAtLevel(mutationLevel);
            float speed = gammaNervousMinorEffect.GetAttractSpeedAtLevel(mutationLevel);
            
            Debug.Log($"[GammaNervousMinorTester] {moment} - Range: {range:F1}m, Speed: x{speed:F1}");
        }
    }
}
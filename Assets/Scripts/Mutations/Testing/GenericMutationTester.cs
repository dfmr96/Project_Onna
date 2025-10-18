using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Mutations.Core.Categories;


namespace Mutations.Testing
{
    public class GenericMutationTester : MonoBehaviour
    {
        [Header("Mutation Testing")]
        [SerializeField] private StatModifierEffect mutationEffect; 
        [SerializeField] private int mutationLevel = 1;
        [SerializeField] private bool showGUI = true;
        [SerializeField] private float yOffset = 50;

        private PlayerModel playerModel;
        private bool effectApplied = false;

        private void Start()
        {
            playerModel = FindObjectOfType<PlayerModel>();

            if (playerModel == null)
                Debug.LogError("[GenericMutationTester] PlayerModel not found!");

            if (mutationEffect == null)
                Debug.LogWarning("[GenericMutationTester] MutationEffect not assigned in inspector!");
        }

        private void Update()
        {
            // Hotkeys para testing rápido
            if (Input.GetKeyDown(KeyCode.M))
            {
                //if (effectApplied)
                //    RemoveMutation();
                //else
                    ApplyMutation();
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                RemoveMutation();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                showGUI = !showGUI;
            }
        }

        private void OnGUI()
        {
            if (!showGUI || playerModel == null) return;

            string effectName = mutationEffect ? mutationEffect.name : "None";

            GUILayout.BeginArea(new Rect(Screen.width - 300, yOffset, 280, 200), " Generic Mutation Tester", GUI.skin.window);

            GUILayout.Label($"Player: {(playerModel ? "Ok" : "Cancel")}");
            GUILayout.Label($"Effect: {effectName}");
            GUILayout.Label($"Applied: {(effectApplied ? "Ok" : "Cancel")}");

            GUILayout.Space(10);

            // Level selector
            GUILayout.BeginHorizontal();
            GUILayout.Label("Level:", GUILayout.Width(50));
            mutationLevel = (int)GUILayout.HorizontalSlider(mutationLevel, 1, 4, GUILayout.Width(100));
            GUILayout.Label(mutationLevel.ToString(), GUILayout.Width(20));
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            // Botones
            //if (GUILayout.Button(effectApplied ? "Remove Mutation (M)" : "Apply Mutation (M)"))
            //{
            //    if (effectApplied)
            //        RemoveMutation();
            //    else
            //        ApplyMutation();
            //}
            if (GUILayout.Button("Apply Mutation (M)")) ApplyMutation();
            if (GUILayout.Button("Remove Mutation (N)")) RemoveMutation();

            GUILayout.Space(5);

            // Info actual
            //if (playerModel?.StatContext != null)
            //{
            //    float moveSpeed = playerModel.StatContext.Source.Get(playerModel.StatRefs.movementSpeed);
            //    float drainRate = playerModel.StatContext.Source.Get(playerModel.StatRefs.passiveDrainRate);

            //    GUILayout.Label($"Move Speed: {moveSpeed:F2}");
            //    GUILayout.Label($"Drain Rate: {drainRate:F2}");
            //}

            GUILayout.EndArea();

            // Instrucciones
            //GUI.Label(new Rect(Screen.width - 300, 260, 280, 80),
            //    "Controls:\n" +
            //    "M - Toggle Mutation\n" +
            //    "B - Toggle GUI");
        }

        [ContextMenu("Apply Mutation")]
        public void ApplyMutation()
        {
            if (!ValidateComponents()) return;

            if (effectApplied)
            {
                Debug.LogWarning("[GenericMutationTester] Mutation already applied!");
                return;
            }

            Debug.Log($"[GenericMutationTester] Applying {mutationEffect.name} - Level {mutationLevel}");
            mutationEffect.ApplyEffect(playerModel.gameObject, mutationLevel);
            effectApplied = true;
        }

        [ContextMenu("Remove Mutation")]
        public void RemoveMutation()
        {
            if (!ValidateComponents()) return;

            if (!effectApplied)
            {
                Debug.LogWarning("[GenericMutationTester] No mutation applied to remove!");
                return;
            }

            Debug.Log($"[GenericMutationTester] Removing {mutationEffect.name}");
            mutationEffect.RemoveEffect(playerModel.gameObject);
            effectApplied = false;
        }

        private bool ValidateComponents()
        {
            if (playerModel == null)
            {
                Debug.LogError("[GenericMutationTester] PlayerModel not found!");
                return false;
            }

            if (mutationEffect == null)
            {
                Debug.LogError("[GenericMutationTester] MutationEffect not assigned!");
                return false;
            }

            if (playerModel.StatContext == null)
            {
                Debug.LogError("[GenericMutationTester] PlayerModel.StatContext is null!");
                return false;
            }

            return true;
        }
    }
}


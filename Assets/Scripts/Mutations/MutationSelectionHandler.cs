using System;
using Mutations.Testing;
using NaughtyAttributes;
using Player;
using UnityEngine;

namespace Mutations
{
    public class MutationSelectionHandler : MonoBehaviour
    {
        [SerializeField] private MutationSelector selector;
        [SerializeField] private MutationOptionUI mutationUIPrefab;
        [SerializeField] private Transform uiContainer;

        private void Start()
        {
            if (selector == null || mutationUIPrefab == null || uiContainer == null)
            {
                Debug.LogError("❌ Faltan referencias en MutationSelectionHandler.");
                return;
            }

            RollAndDisplayMutations();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RollAndDisplayMutations();
            }
        }

        [Button("Roll Mutations and Show UI")]
        private void RollAndDisplayMutations()
        {
            foreach (Transform child in uiContainer)
                Destroy(child.gameObject);

            var mutations = selector.RollMutations(3);
            Debug.Log("Mutations rolled successfully.");

            foreach (var mutation in mutations)
            {
                var ui = Instantiate(mutationUIPrefab, uiContainer);
                ui.SetData(mutation);
                //ui.OnSelected += OnMutationSelected;
            }
            Debug.Log("Mutations View UI instantiated successfully.");
            gameObject.SetActive(true);
            
            Debug.Log("Showing UI for mutation selection.");
        }

        private void OnMutationSelected(MutationData chosen)
        {
            Debug.Log($"🧬 Seleccionaste: {chosen.MutationName}");
            ApplyMutation(chosen);
            CloseUI();
        }

        private void ApplyMutation(MutationData data)
        {
            var player = PlayerHelper.GetPlayer();
            var playerModel = player.GetComponent<PlayerModel>();
            data.UpgradeEffect.Apply(playerModel.StatContext.Meta);
        }

        private void CloseUI()
        {
            gameObject.SetActive(false); // o Destroy(gameObject) si preferís que se destruya
        }
    }
}

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

        private PlayerModel _playerModel;
        private void Start()
        {
            
            PlayerHelper.DisableInput();
            _playerModel = PlayerHelper.GetPlayer().GetComponent<PlayerModel>();
            _playerModel.EnablePassiveDrain(false);
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
                ui.SetCloseUI(CloseUI);
                //ui.OnSelected += OnMutationSelected;
            }
            Debug.Log("Mutations View UI instantiated successfully.");
            gameObject.SetActive(true);
            
            Debug.Log("Showing UI for mutation selection.");
        }

        private void CloseUI()
        {
            PlayerHelper.EnableInput();
            _playerModel.EnablePassiveDrain(true);
            gameObject.SetActive(false); 
        }
    }
}

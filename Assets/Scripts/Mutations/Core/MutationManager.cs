using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mutations.Core
{
    public class MutationManager : MonoBehaviour
    {
        public static MutationManager Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private int optionsPerSelection = 3;
        [SerializeField] private RadiationEffectFactory effectFactory;

        [Header("Current State")]
        [SerializeField] private List<MutationSystem> systems;
        [SerializeField] private GameObject currentPlayer;

        public List<MutationSystem> Systems => systems;
        public GameObject CurrentPlayer => currentPlayer;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeSystems();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeSystems()
        {
            systems = new List<MutationSystem>
            {
                new MutationSystem(SystemType.Nerve),
                new MutationSystem(SystemType.Integumentary),
                new MutationSystem(SystemType.Muscular)
            };
        }

        public void SetPlayer(GameObject player)
        {
            currentPlayer = player;
        }

        public List<MutationType> GenerateRadiationOptions()
        {
            var allRadiations = System.Enum.GetValues(typeof(MutationType)).Cast<MutationType>().ToList();
            var options = new List<MutationType>();

            for (int i = 0; i < optionsPerSelection; i++)
            {
                var randomRadiation = allRadiations[Random.Range(0, allRadiations.Count)];
                options.Add(randomRadiation);
            }

            return options;
        }

        public bool CanAssignRadiation(MutationType radiation, out SystemType targetSystem)
        {
            targetSystem = SystemType.Nerve;

            foreach (var system in systems)
            {
                if (system.CanReceiveRadiation(radiation))
                {
                    targetSystem = system.SystemType;
                    return true;
                }
            }

            return false;
        }

        public void AssignRadiation(MutationType radiation, SystemType system)
        {
            if (currentPlayer == null)
            {
                Debug.LogError("[MutationManager] No player assigned!");
                return;
            }

            var targetSystem = systems.FirstOrDefault(s => s.SystemType == system);
            if (targetSystem == null)
            {
                Debug.LogError($"[MutationManager] System {system} not found!");
                return;
            }

            var targetSlot = targetSystem.GetTargetSlot(radiation);
            var effect = effectFactory.CreateEffect(radiation, system, targetSlot);

            if (effect != null)
            {
                targetSystem.AssignRadiation(radiation, effect, currentPlayer);
                Debug.Log($"[MutationManager] Assigned {radiation} to {system} ({targetSlot} slot)");
            }
            else
            {
                Debug.LogError($"[MutationManager] Could not create effect for {radiation} on {system}");
            }
        }

        public void ClearAllMutations()
        {
            if (currentPlayer == null) return;

            foreach (var system in systems)
            {
                system.ClearAllMutations(currentPlayer);
            }

            Debug.Log("[MutationManager] All mutations cleared");
        }

        public void OnRunEnd()
        {
            ClearAllMutations();
        }

        public bool TryGetMutation(MutationType type, out SystemType system, out SlotType slot)
        {
            foreach (var sys in systems)
            {
                if (sys.TryGetSlot(type, out var foundSlot))
                {
                    system = sys.SystemType;
                    slot = foundSlot.SlotType;
                    return true;
                }
            }

            system = default;
            slot = default;
            return false;
        }

    }
}
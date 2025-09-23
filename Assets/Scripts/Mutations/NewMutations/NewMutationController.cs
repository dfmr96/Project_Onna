using Mutations;
using System.Collections.Generic;
using UnityEngine;

public class NewMutationController
{
    private Dictionary<SystemType, NewMutationSystem> systems = new();
    private NewMutationDatabase _database;

    public NewMutationController(NewMutationDatabase database)
    {
        ResetRun();
        _database = database;

        systems[SystemType.Nerve] = new NewMutationSystem { systemType = SystemType.Nerve };
        systems[SystemType.Integumentary] = new NewMutationSystem { systemType = SystemType.Integumentary };
        systems[SystemType.Muscular] = new NewMutationSystem { systemType = SystemType.Muscular };
    }

    /// <summary>
    /// Resets de run (will be innecesary if we destroy NewMutationController when the run its over)
    /// </summary>
    public void ResetRun()
    {
        foreach (var system in systems.Values)
        {
            system.mayorSlot.Mutation = null;
            system.menorSlot.Mutation = null;
        }
    }

    /// <summary>
    /// Rolling 3 radiations for showing UI
    /// </summary>
    public List<NewRadiationData> RollRadiations(int count = 3)
    {
        List<NewRadiationData> pool = new List<NewRadiationData>(_database.AllRadiations);
        List<NewRadiationData> result = new List<NewRadiationData>();
        var rng = new System.Random();

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int index = rng.Next(pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return result;
    }

    /// <summary>
    /// Equip radiation in a system
    /// </summary>
    public bool EquipRadiation(MutationType radiation, SystemType system, SlotType slot)
    {
        var targetSlot = (slot == SlotType.Major) ? systems[system].mayorSlot : systems[system].menorSlot;

        if (!targetSlot.IsEmpty)
        {
            Debug.LogWarning($"Slot {slot} in {system} is already full.");
            return false;
        }

        var mutation = _database.GetMutation(radiation, system, slot);

        if (mutation == null) 
        {
            Debug.LogError("Mutation selected is unknown or null");
            return false;
        }

        systems[system].AssignMutation(mutation, slot);
        return true;
    }

    /// <summary>
    /// Returns the system itself
    /// </summary>
    public NewMutationSystem GetSystem(SystemType type) => systems[type];

}

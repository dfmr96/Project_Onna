using Mutations;
using Mutations.Core;
using Player;
using System.Collections.Generic;
using UnityEngine;

public class NewMutationController
{
    private Dictionary<SystemType, NewMutationSystem> systems = new();
    private List<RadiationEffect> effects = new();
    private MutationDB _db;

    //TODO
    ///NECESITO NIVEL DE MUTACIONES

    public NewMutationController()
    {
        _db = Resources.Load<MutationDB>("MutationDB");

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
        List<NewRadiationData> pool = new List<NewRadiationData>(_db.AllRadiations);
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
        NewMutationSlot targetSlot = (slot == SlotType.Major) ? systems[system].mayorSlot : systems[system].menorSlot;

        if (!targetSlot.IsEmpty) return false;

        RadiationEffect mutation = _db.GetMutation(radiation, system, slot);

        if (mutation == null) 
        {
            Debug.LogError("Mutation selected is unknown or null");
            return false;
        }

        systems[system].AssignMutation(mutation, slot);
        effects.Add(mutation);
        mutation.ApplyEffect(PlayerHelper.GetPlayer(), 1);
        return true;
    }

    /// <summary>
    /// Returns the system itself
    /// </summary>
    public NewMutationSystem GetSystem(SystemType type) => systems[type];

    /// <summary>
    /// Returns mutation equiped in a system and slot
    /// </summary>
    public RadiationEffect GetEquippedMutation(SystemType system, SlotType slot)
    {
        if (!systems.TryGetValue(system, out var sys)) return null;
        return slot == SlotType.Major ? sys.mayorSlot.Mutation : sys.menorSlot.Mutation;
    }

    /// <summary>
    /// Returns NewRadiationData for the mutation equiped in a system and slot
    /// </summary>
    public NewRadiationData GetEquippedRadiationData(SystemType system, SlotType slot)
    {
        var mutation = GetEquippedMutation(system, slot);
        if (mutation == null) return null;

        return _db?.GetRadiationData(mutation.RadiationType);
    }

    /// <summary>
    /// Returns a mutation
    /// </summary>
    public RadiationEffect GetMutationForSlot(MutationType radiation, SystemType system, SlotType slot)
    {
        return _db.GetMutation(radiation, system, slot);
    }

    /// <summary>
    /// Apply an effect
    /// </summary>
    public void ApplyEffects(GameObject player)
    {
        Debug.Log("Applying effects");
        foreach (RadiationEffect effect in effects)
            effect.ApplyEffect(player, 1);
    }
}

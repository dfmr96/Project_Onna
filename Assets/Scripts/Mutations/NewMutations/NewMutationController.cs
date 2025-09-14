using System.Collections.Generic;

public class NewMutationController
{
    private Dictionary<SystemType, NewMutationSystem> systems = new();
    private List<NewMutations> mutationPool;
    private System.Random rng = new();

    public NewMutationEffect EffectManager { get; private set; }

    public NewMutationController(List<NewMutations> allMutations)
    {
        ResetRun();
        mutationPool = allMutations;

        systems[SystemType.Nerve] = new NewMutationSystem { systemType = SystemType.Nerve };
        systems[SystemType.Integumentary] = new NewMutationSystem { systemType = SystemType.Integumentary };
        systems[SystemType.Muscular] = new NewMutationSystem { systemType = SystemType.Muscular };

        EffectManager = new NewMutationEffect();
    }

    public void ResetRun()
    {
        foreach (var system in systems.Values)
        {
            system.mayorSlot.Mutation = null;
            system.menorSlot.Mutation = null;
        }
    }

    public NewMutationSystem GetSystem(SystemType type) => systems[type];

    /// <summary>
    /// Return 3 mutations for showing in UI
    /// </summary>
    public List<NewMutations> RollMutations(int count = 3)
    {
        List<NewMutations> result = new();
        List<NewMutations> tempPool = new List<NewMutations>(mutationPool);

        for (int i = 0; i < count && tempPool.Count > 0; i++)
        {
            int index = rng.Next(tempPool.Count);
            result.Add(tempPool[index]);
            tempPool.RemoveAt(index);
        }

        return result;
    }

    public void AssignMutation(NewMutations mutation, SystemType system, SlotType slot) { systems[system].AssignMutation(mutation, slot); }
}

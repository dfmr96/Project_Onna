using Mutations;

[System.Serializable]
public class NewMutationSystem
{
    public SystemType systemType;
    public NewMutationSlot mayorSlot = new NewMutationSlot { SlotType = SlotType.Major };
    public NewMutationSlot menorSlot = new NewMutationSlot { SlotType = SlotType.Minor };

    public void AssignMutation(NewMutations mutation, SlotType slot)
    {
        if (slot == SlotType.Major)
            mayorSlot.Mutation = mutation;
        else
            menorSlot.Mutation = mutation;
    }
}

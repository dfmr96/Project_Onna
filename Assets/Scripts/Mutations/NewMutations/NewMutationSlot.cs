using Mutations;
using Mutations.Core;

[System.Serializable]
public class NewMutationSlot
{
    public SlotType SlotType;
    public RadiationEffect Mutation;
    public bool IsEmpty => Mutation == null;
}

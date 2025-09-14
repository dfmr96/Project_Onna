[System.Serializable]
public class NewMutationSlot
{
    public SlotType SlotType;
    public NewMutations Mutation;
    public bool IsEmpty => Mutation == null;
}

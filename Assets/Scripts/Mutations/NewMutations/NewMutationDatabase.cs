using AYellowpaper.SerializedCollections;
using Mutations;
using Mutations.Core;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SystemsInfo
{
    [SerializeField] private MutationSlots mutationsSlots;
    public MutationSlots MutationsSlots => mutationsSlots;
}

[System.Serializable]
public struct RadiationInfo
{
    [SerializeField] private SerializedDictionary<MutationType, SystemsInfo> radiationLookup;
    public SerializedDictionary<MutationType, SystemsInfo> RadiationLookup => radiationLookup;
}

[System.Serializable]
public struct MutationSlots
{
    [SerializeField] private ScriptableObject majorSO;
    [SerializeField] private ScriptableObject minorSO;
    public ScriptableObject MajorSO => majorSO;
    public ScriptableObject MinorSO => minorSO;
    public MutationSlots(ScriptableObject majorSO, ScriptableObject minorSO)
    {
        this.majorSO = majorSO;
        this.minorSO = minorSO;
    }
}

[CreateAssetMenu(fileName = "New Mutation Database", menuName = "Mutations/Databases")]
public class NewMutationDatabase : ScriptableObject
{
    [Header("Radiation Data")]
    [SerializeField] private List<NewRadiationData> allRadiations;
    public List<NewRadiationData> AllRadiations => allRadiations;

    [Header("Database Mutations")]
    [SerializeField] private SerializedDictionary<SystemType, RadiationInfo> radiationLookup;

    public RadiationEffect GetMutation(MutationType radiation, SystemType system, SlotType slot)
    {
        if (!radiationLookup.TryGetValue(system, out RadiationInfo radInfo)) return null;

        if (!radInfo.RadiationLookup.TryGetValue(radiation, out SystemsInfo sysInfo)) return null;

        var slots = sysInfo.MutationsSlots;

        switch (slot)
        {
            case SlotType.Major:
                return slots.MajorSO as RadiationEffect;
            case SlotType.Minor:
                return slots.MinorSO as RadiationEffect;
            default:
                return null;
        }
    }
    public NewRadiationData GetRadiationData(MutationType type)
    {
        if (allRadiations == null) return null;
        return allRadiations.Find(r => r.Type == type);
    }
}

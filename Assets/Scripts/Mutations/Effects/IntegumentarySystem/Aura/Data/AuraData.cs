using UnityEngine;

public enum AuraEffectType
{
    None,
    Damage,
    Slow,
    Burn,
    Weaken,
    Heal,
    VitalTimeRestore,
    Pushback
}

[CreateAssetMenu(fileName = "AuraData", menuName = "Mutations/Aura Data")]
public class AuraData : ScriptableObject
{
    [Header("General")]
    public string auraId;
    public AuraEffectType effectType;
    public float radius = 2f;
    public float tickRate = 0.5f;

    [Header("Visuals")]
    public Color auraColor = Color.white;
    public GameObject visualPrefab;
}
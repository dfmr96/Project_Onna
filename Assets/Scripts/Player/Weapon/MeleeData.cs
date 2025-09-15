using UnityEngine;

[CreateAssetMenu(fileName = "NewMeleeData", menuName = "Player/Weapons/Melee", order = 0)]
public class MeleeData : ScriptableObject
{
    [SerializeField] private float damage;
    [SerializeField] private float range;
    [SerializeField] private float attackDelay;
    [SerializeField] private float coolDown;

    public float Damage => damage;
    public float Range => range;
    public float AttackDelay => attackDelay;
    public float CoolDown => coolDown;
}

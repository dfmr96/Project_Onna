using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/NPC")]
public class NPCData : ScriptableObject
{
    public string npcName;
    public NPCType npcType;
    public DialogueNode startingDialogue;
}
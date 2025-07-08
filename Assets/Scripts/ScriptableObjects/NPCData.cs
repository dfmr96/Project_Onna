using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Dialogue/NPC")]
public class NPCData : ScriptableObject
{
    [SerializeField] private string npcName;
    [SerializeField] private Sprite npcImage; 
    [SerializeField] private NPCType npcType;
    [SerializeField] private DialogueNode startingDialogue;

    public string NpcName => npcName;
    public Sprite NpcImage => npcImage;
    public NPCType NPCType => npcType;
    public DialogueNode StartingDialogue => startingDialogue;
}
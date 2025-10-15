using UnityEngine;

public class Boss_Dialogue_Trigger : InteractableBase
{
    [Header("Datos del diálogo del Boss")]
    [SerializeField] private NPCData bossDialogueData;

    [Header("Próximo diálogo (se cambia cuando termina el actual)")]
    [SerializeField] private NPCData nextDialogueData;

    [Header("Referencia opcional al Boss (si necesitás activar animaciones o lógica)")]
    [SerializeField] private GameObject bossObject;

    public override void Interact()
    {
        base.Interact();

        if (bossDialogueData != null)
        {
            DialogueManager.Instance.StartDialogue(bossDialogueData, this); // le pasamos this
        }
        else
        {
            //Debug.LogWarning($"{name}: No se asignó ningún NPCData para el diálogo del Boss.");
        }

        // if (bossObject != null)
        // {
        //     Animator bossAnim = bossObject.GetComponent<Animator>();
        //     if (bossAnim != null)
        //         bossAnim.SetTrigger("StartDialogue");
        // }
    }

    // ✅ Método para cambiar el diálogo
    public void SetDialogueData(NPCData newData)
    {
        bossDialogueData = newData;
    }

    public NPCData GetNextDialogue() => nextDialogueData;
}

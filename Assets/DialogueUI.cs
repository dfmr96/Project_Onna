using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image npcImage;
    [SerializeField] private Button[] optionButtons;

    public void SetName(string name) { nameText.text = name; }

    public void SetImage(Sprite image)
    {
        npcImage.sprite = image;

        if (image == null) return;

        RectTransform rect = npcImage.GetComponent<RectTransform>();

        float aspectRatio = image.rect.width / image.rect.height;

        float baseHeight = 400f; // o lo que quieras como altura base visual
        float newWidth = baseHeight * aspectRatio;

        rect.sizeDelta = new Vector2(newWidth, baseHeight);

        
        npcImage.SetNativeSize();

        // 4️⃣ Si usás "Preserve Aspect" (recomendado)
        npcImage.preserveAspect = true;
    }


    public void BindActions(DialogueNode node)
    {
        foreach (var option in node.Options)
        {
            option.onSelectedAction = null;

            // Iteramos sobre todas las acciones asignadas
            foreach (var actionId in option.actionIds)
            {
                switch (actionId)
                {
                    case DialogueActionId.OpenStore:
                        option.onSelectedAction += () =>
                        {
                            HubManager hub = HubManager.Instance;
                            if (hub != null) hub.OpenStore();
                        };
                        break;

                    case DialogueActionId.ChangeDialogue:
                        option.onSelectedAction += () =>
                        {
                            var trigger = DialogueManager.Instance.CurrentTrigger;

                            if (trigger is Engineer_Dialogue_Trigger engineer)
                                engineer.SetDialogueToNext();
                            else if (trigger is Boss_Dialogue_Trigger boss)
                                boss.SetDialogueData(boss.GetNextDialogue());
                        };
                        break;
                    case DialogueActionId.EndBoss:
                        option.onSelectedAction = () =>
                        {
                            if (DialogueManager.Instance.CurrentTrigger is Boss_Dialogue_Trigger bossTrigger)
                            {
                                var nextBossDialogue = bossTrigger.GetNextDialogue();
                                if (nextBossDialogue != null)
                                {
                                    bossTrigger.SetDialogueData(nextBossDialogue);
                                    //Debug.Log("Boss dialogue changed to next data.");
                                }
                            }
                            Engineer_Dialogue_Trigger engineer = FindObjectOfType<Engineer_Dialogue_Trigger>();
                            if (engineer != null)
                            {
                                engineer.SetDialogueToNext();
                                //Debug.Log("Engineer dialogue changed after Boss conversation.");
                            }
                            else
                            {
                                //Debug.LogWarning("Engineer not found in the scene to update dialogue.");
                            }
                        };
                        break;
                    case DialogueActionId.EngineerEnd:
                        {
                            Engineer_Dialogue_Trigger engineer = FindObjectOfType<Engineer_Dialogue_Trigger>();
                            if (engineer != null)
                            {
                                engineer.SetDialogueToLoop(); // Cambia el diálogo al loop final
                                engineer.OnEngineerEndAction(); // Ejecuta la lógica de activación/desactivación
                            }
                            break;
                            }
                    case DialogueActionId.ONNAPreTutorial:
                        option.onSelectedAction += () =>
                        {
                            var trigger = DialogueManager.Instance.CurrentTrigger;
                            if (trigger is WeaponDialogueTrigger weaponTrigger)
                            {
                                NPCData nextData = weaponTrigger.GetNextDialogue();
                                weaponTrigger.SetDialogueData(nextData);

                                if (nextData != null)
                                {
                                    DialogueManager.Instance.SetCurrentNPCData(nextData);
                                    DialogueManager.Instance.StartCoroutine(
                                        DialogueManager.Instance.PreTutorialTimer(nextData, trigger)
                                    );
                                }
                                else
                                {
                                    Debug.LogWarning("ONNAPreTutorial: No se encontró un nextData válido para el trigger.");
                                }
                            }
                        };
                        break;

                                                    
                    case DialogueActionId.StartChecklist:
                        option.onSelectedAction += () =>
                        {
                            var tracker = FindObjectOfType<PlayerTutorialTracker>();
                            if (tracker != null)
                                tracker.StartCountingInputs(); // 🔹 aquí empieza a contar inputs
                        };
                        break;

                    case DialogueActionId.ONNAAfterChecklist:
                        option.onSelectedAction += () =>
                        {
                            PlayerTutorialTracker tracker = FindObjectOfType<PlayerTutorialTracker>();
                            if (tracker != null)
                            {
                                tracker.ActivateEnemySpawner();
                                var trigger = DialogueManager.Instance.CurrentTrigger;
                                if (trigger is WeaponDialogueTrigger weaponTrigger)
                                {
                                    var nextData = weaponTrigger.defeatedEnemiesDialogue;
                                    if (nextData != null)
                                    {
                                        weaponTrigger.SetDialogueData(nextData);
                                        Debug.Log("WeaponDialogueTrigger: Preparado defeatedEnemiesDialogue tras ONNAAfterChecklist");
                                    }
                                    else
                                    {
                                        Debug.LogWarning("ONNAAfterChecklist: defeatedEnemiesDialogue no asignado en WeaponDialogueTrigger.");
                                    }
                                }
                            }
                            else
                            {
                                Debug.LogWarning("ONNAAfterChecklist: No se encontró PlayerTutorialTracker en la escena");
                            }
                        };
                        break;

                    case DialogueActionId.EndTutorial:
                        option.onSelectedAction += () =>
                        {
                            var trigger = DialogueManager.Instance.CurrentTrigger;
                            if (trigger is EndTutorialTrigger endTrigger)
                            {
                                endTrigger.HandleAction("EndTutorial");
                            }
                            else
                            {
                                Debug.LogWarning("EndTutorial: Trigger actual no es EndTutorialTrigger");
                            }
                        };
                        break;


                    case DialogueActionId.None:
                    default:
                        break;
                }
            }
        }
    }


    public void DisplayNode(DialogueNode node, System.Action<int> onOptionSelected)
    {
        dialogueText.text = node.DialogueText;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < node.Options.Length)
            {
                optionButtons[i].gameObject.SetActive(true);
                int index = i;
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = node.Options[i].optionText;
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => onOptionSelected(index));
            }
            else optionButtons[i].gameObject.SetActive(false);
        }
    }
}

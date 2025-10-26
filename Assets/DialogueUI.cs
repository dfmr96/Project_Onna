using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image npcImage;
    [SerializeField] private Button[] optionButtons;

    public void SetName(string name) => nameText.text = name;

    public void SetImage(Sprite image)
    {
        npcImage.sprite = image;

        if (image == null) return;

        RectTransform rect = npcImage.GetComponent<RectTransform>();

        float aspectRatio = image.rect.width / image.rect.height;

        float baseHeight = 400f;
        float newWidth = baseHeight * aspectRatio;

        rect.sizeDelta = new Vector2(newWidth, baseHeight);

        
        npcImage.SetNativeSize();

        npcImage.preserveAspect = true;
    }


    public void BindActions(DialogueNode node)
    {
        foreach (var option in node.Options)
        {
            option.onSelectedAction = null;

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
                                    SaveSystem.ReplaceCoins(30);
                                }
                            }
                            Engineer_Dialogue_Trigger engineer = FindObjectOfType<Engineer_Dialogue_Trigger>();
                            if (engineer != null)
                                engineer.SetDialogueToNext();
                        };
                        break;
                    case DialogueActionId.EngineerEnd:
                        {
                            Engineer_Dialogue_Trigger engineer = FindObjectOfType<Engineer_Dialogue_Trigger>();
                            if (engineer != null)
                            {
                                engineer.SetDialogueToLoop();
                                engineer.OnEngineerEndAction();
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
                            }
                        };
                        break;

                                                    
                    case DialogueActionId.StartChecklist:
                        option.onSelectedAction += () =>
                        {
                            var tracker = FindObjectOfType<PlayerTutorialTracker>();
                            if (tracker != null)
                                tracker.StartCountingInputs();
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
                                        weaponTrigger.SetDialogueData(nextData);
                                }
                            }
                        };
                        break;

                    case DialogueActionId.EndTutorial:
                        option.onSelectedAction += () =>
                        {
                            var trigger = DialogueManager.Instance.CurrentTrigger;
                            if (trigger is EndTutorialTrigger endTrigger)
                                endTrigger.HandleAction("EndTutorial");
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

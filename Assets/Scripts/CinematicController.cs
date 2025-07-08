using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CinematicController : MonoBehaviour
{
    [Header("Sprites de la cinemática")]
    [SerializeField] private List<Sprite> cinematicSprites;

    [Header("UI de pantalla")]
    [SerializeField] private Image cinematicImage;

    [Header("Nombre de la escena siguiente")]
    [SerializeField] private string nextSceneName = "HubScene";

    private int currentIndex = 0;
    
    private void Start()
    {
        if (cinematicSprites == null || cinematicSprites.Count == 0 || cinematicImage == null)
        {
            Debug.LogError("❌ Faltan referencias o sprites en la cinemática.");
            return;
        }

        ShowCurrentSprite();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            AdvanceCinematic();
        }
    }

    private void AdvanceCinematic()
    {
        currentIndex++;

        if (currentIndex >= cinematicSprites.Count)
        {
            GameModeSelector.SelectedMode = GameMode.Hub;
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        ShowCurrentSprite();
    }

    private void ShowCurrentSprite()
    {
        cinematicImage.sprite = cinematicSprites[currentIndex];
    }
}
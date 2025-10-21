using Player;
using UnityEngine;

public class LevelEndTrigger : LevelTrigger
{
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 2f, 0f);
    protected override void OnTrigger(Collider other)
    {
        //SavePlayerData(other); //TODO Que hace esto?
        PlayerHelper.DisableInput();
        LoadNextLevel();
    }


    private void Start()
    {
        //Evento para activar portal tras seleccion de mutacion
        if (GameManager.Instance != null)
            GameManager.OnMutationUIClosed += ActivatePortal;

    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.OnMutationUIClosed -= ActivatePortal;
    }

    private void ActivatePortal()
    {
        if (portalPrefab == null)
        {
            Debug.LogWarning("Portal prefab no asignado");
            return;
        }
        //Instanciamos el portal en el spawnPoint
        Vector3 position = transform.position + spawnOffset;
        Quaternion rotation = Quaternion.Euler(0f, 0f, 90f);


        Instantiate(portalPrefab, position, rotation);

        if (GameManager.Instance != null)
            GameManager.OnMutationUIClosed -= ActivatePortal;

    }
}
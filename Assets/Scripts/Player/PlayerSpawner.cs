using Core;
using Player;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    public GameObject SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        PlayerHelper.SetPlayer(player);

        EventBus.Publish(new PlayerSpawnedSignal { PlayerGO = player });

        return player;
    }
}

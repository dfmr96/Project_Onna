using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private PlayerWallet playerWallet;
    [SerializeField] private PlayerItemsHolder playerItemsHolder;

    public PlayerWallet PlayerWallet => playerWallet;
    public PlayerItemsHolder PlayerItemsHolder => playerItemsHolder;

    public void Init()
    {
        playerWallet = new PlayerWallet();
        playerItemsHolder = new PlayerItemsHolder();
    }
}

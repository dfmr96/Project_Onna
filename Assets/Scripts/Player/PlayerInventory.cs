
using System;
using UnityEngine;

[Serializable]
public class PlayerInventory
{
    [SerializeField] private PlayerWallet playerWallet;
    [SerializeField] private PlayerItemsHolder playerItemsHolder;
    public PlayerWallet PlayerWallet => playerWallet;
    public PlayerItemsHolder PlayerItemsHolder => playerItemsHolder;

    public PlayerInventory() { Init(); }

    private void Init()
    {
        playerWallet = new PlayerWallet();
        playerItemsHolder = new PlayerItemsHolder();
    }
}

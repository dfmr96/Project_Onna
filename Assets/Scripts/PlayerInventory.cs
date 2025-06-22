public class PlayerInventory
{
    private PlayerWallet playerWallet;
    private PlayerItemsHolder playerItemsHolder;
    public PlayerWallet PlayerWallet => playerWallet;
    public PlayerItemsHolder PlayerItemsHolder => playerItemsHolder;

    public PlayerInventory() { Init(); }
    public void Init()
    {
        playerWallet = new PlayerWallet();
        playerItemsHolder = new PlayerItemsHolder();
    }
}

using System;
using UnityEngine;

public class PlayerWallet
{
    public int Coins { private set; get; }
    public PlayerWallet() { LoadCoins(); }

    private void LoadCoins() { Coins = SaveSystem.LoadData().totalCoins; }
    private void SaveCoins() { SaveSystem.SaveCoins(Coins); }

    public void AddCoins(int amount)
    {
        Coins += amount;
        SaveCoins();
        Debug.Log($"Se sumaron {amount} monedas. Total: {Coins}");
    }

    public bool CheckCost(int ammount)
    {
        if (Coins >= ammount) return true;
        else return false;
    }

    public bool TrySpend(int amount)
    {
        if (Coins >= amount)
        {
            Coins -= amount;
            SaveCoins();
            return true;
        }
        return false;
    }
}

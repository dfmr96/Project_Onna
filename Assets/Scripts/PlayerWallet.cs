using System;
using UnityEngine;
[System.Serializable]
public class PlayerWallet
{
    [SerializeField] private int coins;
    public int Coins => coins;

    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.LogWarning($"Se sumaron {amount} monedas. Total: {Coins}");
    }

    public bool CheckCost(int ammount)
    {
        if (Coins >= ammount) return true;
        return false;
    }

    public bool TrySpend(int amount)
    {
        if (Coins >= amount)
        {
            coins -= amount;
            return true;
        }
        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    static public PlayerInfo instance;
    public int money { get; private set; } = 20;
    public int happinessRate { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void GetCoin(int amount)
    {
        money += amount;
    }

    public bool PayMoney(int amount)
    {
        if (money - amount < 0)
        {
            return false;
        }

        money -= amount;
        return true;
    }
}

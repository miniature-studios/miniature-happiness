using UnityEngine;

public class Finances : MonoBehaviour
{
    [InspectorReadOnly] private int moneyCount;
    public int MoneyCount => moneyCount;

    public void SetMoney(int value)
    {
        moneyCount = value;
    }

    public void TakeMoney(int value)
    {
        moneyCount -= value;
    }

    public void AddMoney(int value)
    {
        moneyCount += value;
    }
}


using UnityEngine;

public class FinancesController : MonoBehaviour
{
    [SerializeField] private FinancesCounter financesCounter;
    public int MoneyCount => financesCounter.MoneyCount;
    public void SetMoney(int value)
    {
        financesCounter.MoneyCount = value;
    }
    public void TakeMoney(int value)
    {
        financesCounter.MoneyCount -= value;
    }
    public void AddMoney(int value)
    {
        financesCounter.MoneyCount += value;
    }
}


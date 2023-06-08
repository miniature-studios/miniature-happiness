using Common;
using UnityEngine;
using UnityEngine.Events;

public struct Money : IReadonlyData<Money>
{
    public int Count { get; set; }
    public readonly Money Data => this;
}

public class Finances : MonoBehaviour
{
    [SerializeField, InspectorReadOnly]
    private Money money;
    public UnityEvent<IReadonlyData<Money>> MoneyChange;

    public void SetMoney(int money_count)
    {
        money.Count = money_count;
        MoneyChange?.Invoke(money);
    }

    public Result TryTakeMoney(int money_count)
    {
        if (money.Count >= money_count)
        {
            money.Count -= money_count;
            MoneyChange?.Invoke(money);
            return new SuccessResult();
        }
        else
        {
            return new FailResult("Not enough money");
        }
    }

    public void AddMoney(int money_count)
    {
        money.Count += money_count;
        MoneyChange?.Invoke(money);
    }
}

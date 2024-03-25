using Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Level.Finances
{
    public struct Money
    {
        public float Value;
    }

    public struct MoneyEarned
    {
        public float Value;
    }

    [AddComponentMenu("Scripts/Level/Finances/Level.Finances.Model")]
    public class Model : MonoBehaviour
    {
        private DataProvider<Money> moneyDataProvider;
        private DataProvider<MoneyEarned> moneyEarnedDataProvider;

        [ReadOnly]
        [SerializeField]
        private int money;
        public UnityEvent<int> MoneyChange;

        private int moneyEarned = 0;

        private void Start()
        {
            moneyDataProvider = new DataProvider<Money>(
                () => new Money() { Value = money },
                DataProviderServiceLocator.ResolveType.Singleton
            );
            moneyEarnedDataProvider = new DataProvider<MoneyEarned>(
                () => new MoneyEarned() { Value = moneyEarned },
                DataProviderServiceLocator.ResolveType.Singleton
            );
        }

        public Result TryTakeMoney(int moneyCount)
        {
            if (money >= moneyCount)
            {
                money -= moneyCount;
                MoneyChange?.Invoke(money);
                return new SuccessResult();
            }
            else
            {
                return new FailResult("Not enough money");
            }
        }

        public void AddMoney(int moneyCount)
        {
            money += moneyCount;
            moneyEarned += moneyCount;
            MoneyChange?.Invoke(money);
        }
    }
}

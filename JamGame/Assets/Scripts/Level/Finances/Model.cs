using Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Level.Finances
{
    public struct MoneyEarned
    {
        public float Value;
    }

    [AddComponentMenu("Scripts/Level/Finances/Level.Finances.Model")]
    public class Model : MonoBehaviour
    {
        private DataProvider<MoneyEarned> moneyEarnedDataProvider;

        [ReadOnly]
        [SerializeField]
        private int money;
        public UnityEvent<int> MoneyChange;

        private void Start()
        {
            moneyEarnedDataProvider = new DataProvider<MoneyEarned>(
                () => new MoneyEarned() { Value = money },
                DataProviderServiceLocator.ResolveType.Singleton
            );
        }

        public void SetMoney(int moneyCount)
        {
            money = moneyCount;
            MoneyChange?.Invoke(money);
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
            MoneyChange?.Invoke(money);
        }
    }
}

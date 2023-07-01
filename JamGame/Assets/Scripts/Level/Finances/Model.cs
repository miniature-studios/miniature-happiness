using Common;
using UnityEngine;
using UnityEngine.Events;

namespace Level.Finances
{
    [AddComponentMenu("Level.Finances.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField, InspectorReadOnly]
        private int money;
        public UnityEvent<int> MoneyChange;

        public void SetMoney(int money_count)
        {
            money = money_count;
            MoneyChange?.Invoke(money);
        }

        public Result TryTakeMoney(int money_count)
        {
            if (money >= money_count)
            {
                money -= money_count;
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
            money += money_count;
            MoneyChange?.Invoke(money);
        }
    }
}

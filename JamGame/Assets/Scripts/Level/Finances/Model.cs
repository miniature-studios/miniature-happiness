using Common;
using UnityEngine;
using UnityEngine.Events;

namespace Level.Finances
{
    [AddComponentMenu("Scripts/Level.Finances.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField, InspectorReadOnly]
        private int money;
        public UnityEvent<int> MoneyChange;

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

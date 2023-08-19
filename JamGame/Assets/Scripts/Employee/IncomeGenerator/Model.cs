using UnityEngine;
using UnityEngine.Events;

namespace Employee.IncomeGenerator
{
    [AddComponentMenu("Scripts/Employee.IncomeGenerator.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField] private Level.Finances.Model finances;
        [SerializeField] private int incomePerWorkingSession;

        private UnityEvent<int> newIncome = new();
        public UnityEvent<int> NewIncome => newIncome;

        public void NeedComplete(Need completed_need)
        {
            if (completed_need.NeedType == NeedType.Work)
            {
                finances.AddMoney(incomePerWorkingSession);
                newIncome.Invoke(incomePerWorkingSession);
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Employee.Needs;
using UnityEngine;
using UnityEngine.Events;

namespace Employee.IncomeGenerator
{
    [AddComponentMenu("Scripts/Employee/IncomeGenerator/Employee.IncomeGenerator.Model")]
    public class Model : MonoBehaviour, IEffectExecutor<EarnedMoneyEffect>
    {
        [SerializeField]
        private Level.Finances.Model finances;

        [SerializeField]
        private int incomePerWorkingSession;

        private List<float> registeredMultipliers = new();
        private float aggregatedMultiplier = 1f;

        private UnityEvent<int> newIncome = new();
        public UnityEvent<int> NewIncome => newIncome;

        public void NeedComplete(Need completed_need)
        {
            if (completed_need.NeedType == NeedType.Work)
            {
                int income = Mathf.RoundToInt(incomePerWorkingSession * aggregatedMultiplier);
                finances.AddMoney(income);
                newIncome.Invoke(income);
            }
        }

        public void RegisterEffect(EarnedMoneyEffect effect)
        {
            registeredMultipliers.Add(effect.Multiplier);
            aggregatedMultiplier = registeredMultipliers.Aggregate(1.0f, (a, b) => a * b);
        }

        public void UnregisterEffect(EarnedMoneyEffect effect)
        {
            for (int i = 0; i < registeredMultipliers.Count; i++)
            {
                if (Mathf.Approximately(registeredMultipliers[i], effect.Multiplier))
                {
                    registeredMultipliers.RemoveAt(i);
                    aggregatedMultiplier = registeredMultipliers.Aggregate(1.0f, (a, b) => a * b);
                    return;
                }
            }

            Debug.LogError("Failed to unregister EarnedMoneyEffect: not registered");
        }
    }
}

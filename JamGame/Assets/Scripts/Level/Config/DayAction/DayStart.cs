using System;
using UnityEngine;

namespace Level.Config.DayAction
{
    [Serializable]
    public class DayStart : IDayAction
    {
        [SerializeField]
        private int morningMoney;
        public int MorningMoney => morningMoney;

        [SerializeField]
        private float duration;
        public float Duration => duration;

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }
}

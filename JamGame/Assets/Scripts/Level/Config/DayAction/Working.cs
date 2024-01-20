using System;
using Level.GlobalTime;
using UnityEngine;

namespace Level.Config.DayAction
{
    [Serializable]
    public class Working : IDayAction
    {
        [SerializeField]
        private Days duration = new();
        public Days Duration => duration;

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }
}

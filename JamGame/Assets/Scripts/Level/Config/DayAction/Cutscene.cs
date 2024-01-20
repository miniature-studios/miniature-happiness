using System;
using UnityEngine;

namespace Level.Config.DayAction
{
    [Serializable]
    public class Cutscene : IDayAction
    {
        [SerializeField]
        private float duration;
        public float Duration => duration;

        [SerializeField]
        private string text;
        public string Text => text;

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }
}

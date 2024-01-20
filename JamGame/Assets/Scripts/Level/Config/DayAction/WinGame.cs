using System;

namespace Level.Config.DayAction
{
    [Serializable]
    public class WinGame : IDayAction
    {
        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }
}

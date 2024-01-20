using System;

namespace Level.Config.DayAction
{
    [Serializable]
    public class LoseGame : IDayAction
    {
        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }
}

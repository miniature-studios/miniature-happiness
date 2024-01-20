using System;

namespace Level.Config.DayAction
{
    [Serializable]
    public class PreDayEnd : IDayAction
    {
        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }
}

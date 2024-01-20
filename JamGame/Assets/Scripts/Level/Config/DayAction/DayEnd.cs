using System;

namespace Level.Config.DayAction
{
    [Serializable]
    public class DayEnd : IDayAction
    {
        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }
}

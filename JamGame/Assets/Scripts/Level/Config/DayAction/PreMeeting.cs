using System;

namespace Level.Config.DayAction
{
    [Serializable]
    public class PreMeeting : IDayAction
    {
        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }
}

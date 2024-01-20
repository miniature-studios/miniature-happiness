using System;

namespace Level.Config.DayAction
{
    public interface IDayAction
    {
        public void Execute(Executor executor);
    }

    [Serializable]
    public class DayTest : IDayAction
    {
        public void Execute(Executor executor)
        {
            throw new NotImplementedException();
        }
    }
}

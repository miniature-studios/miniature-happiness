using System;

[Serializable]
public class DayEnd : IDayAction
{
    public void Execute(LevelExecutor executor, Action next_action)
    {
        executor.Execute(this, next_action);
    }
}

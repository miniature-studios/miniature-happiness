using System;

[InterfaceEditor]
public interface IDayAction
{
    public void Execute(LevelExecutor executor, Action next_action);
}

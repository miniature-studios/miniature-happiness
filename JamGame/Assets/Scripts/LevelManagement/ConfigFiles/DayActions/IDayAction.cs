using System;

[InterfaceEditor]
public interface IDayAction
{
    public Action ActionEnd { get; set; }
}

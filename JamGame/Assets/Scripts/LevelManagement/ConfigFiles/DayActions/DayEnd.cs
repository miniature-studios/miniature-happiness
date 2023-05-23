using System;

[Serializable]
public class DayEnd : IDayAction
{
    public Action ActionEnd { get; set; }
}

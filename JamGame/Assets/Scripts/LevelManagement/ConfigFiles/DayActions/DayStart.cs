using System;

[Serializable]
public class DayStart : IDayAction
{
    public Action ActionEnd { get; set; }
}


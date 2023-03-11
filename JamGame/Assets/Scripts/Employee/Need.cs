using System;

public enum NeedType
{
    DebugNeed0,
    DebugNeed1,

    Work,
}

[Serializable]
public class Need
{
    public NeedType NeedType;
    public float satisfied;

    float satisfaction_time = 5.0f;

    public Need(NeedType ty)
    {
        NeedType = ty;
        satisfied = 0f;
    }

    public float GetSatisfactionTime()
    {
        return satisfaction_time;
    }
}

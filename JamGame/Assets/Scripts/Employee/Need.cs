public enum NeedType
{
    Work,
}

public class Need
{
    public NeedType NeedType;
    public float satisfied;

    float satisfaction_time = 5.0f;

    public float GetSatisfactionTime()
    {
        return satisfaction_time;
    }
}

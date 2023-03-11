using System;
using UnityEngine;

public enum NeedType
{
    DebugNeed0,
    DebugNeed1,

    Work,
}

public class Need
{
    public NeedParameters Parameters;
    public float satisfied;

    public Need(NeedParameters parameters)
    {
        Parameters = parameters;
        satisfied = 0f;
    }

    public void Update(float delta_time)
    {
        satisfied -= delta_time * Parameters.decrease_speed;
    }

    public void Satisfy()
    {
        satisfied += Parameters.satisfaction_gained;
    }
}

[Serializable]
public class NeedParameters
{
    public NeedType NeedType;

    [SerializeField] public float decrease_speed = 0.0f;
    [SerializeField] public float satisfaction_time = 5.0f;
    [SerializeField] public float satisfaction_gained = 1.0f;

    public NeedParameters(NeedParameters prototype)
    {
        NeedType = prototype.NeedType;
        decrease_speed = prototype.decrease_speed;
        satisfaction_time = prototype.satisfaction_time;
        satisfaction_gained = prototype.satisfaction_gained;
    }

    public float GetSatisfactionTime()
    {
        return satisfaction_time;
    }

    public float GetSatisfaction()
    {
        return satisfaction_gained;
    }
}

using System;
using UnityEngine;

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
    public float satisfied = 0.0f;

    [SerializeField] float decrease_speed = 0.0f;
    [SerializeField] float satisfaction_time = 5.0f;
    [SerializeField] float satisfaction_gained = 1.0f;

    public Need(NeedType ty)
    {
        NeedType = ty;
        satisfied = 0f;
    }

    public Need(Need prototype)
    {
        NeedType = prototype.NeedType;
        satisfied = prototype.satisfied;
        decrease_speed = prototype.decrease_speed;
        satisfaction_time = prototype.satisfaction_time;
        satisfaction_gained = prototype.satisfaction_gained;
    }

    public void Update(float delta_time)
    {
        satisfied -= delta_time * decrease_speed;
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

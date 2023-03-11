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
    public NeedParameters Parameters;

    // FIXMENOW: not needed more?
    public float decrease_speed = 1.0f;
    public float satisfied = 0.0f;

    public Need(Need prototype)
    {
        decrease_speed = prototype.decrease_speed;
        satisfied = prototype.satisfied;
        Parameters = new NeedParameters(prototype.Parameters);
    }

    public Need(NeedParameters parameters)
    {
        Parameters = parameters;
    }

    public void Desatisfy(float delta)
    {
        satisfied -= delta * decrease_speed;
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

    [SerializeField] public float satisfaction_time = 5.0f;
    [SerializeField] public float satisfaction_gained = 1.0f;

    public NeedParameters(NeedType ty)
    {
        NeedType = ty;
    }

    public NeedParameters(NeedParameters prototype)
    {
        NeedType = prototype.NeedType;
        satisfaction_time = prototype.satisfaction_time;
        satisfaction_gained = prototype.satisfaction_gained;
    }

    public void ApplyModifiers(NeedParametersModifiers modifiers)
    {
        satisfaction_time *= modifiers.satisfaction_time;
        satisfaction_gained *= modifiers.satisfaction_gained;
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

using System;
using System.Collections.Generic;
using UnityEngine;

public enum NeedType
{
    Work,
    Piss,
    Eat
}

[Serializable]
public class Need
{
    [Serializable]
    public struct NeedProperties
    {
        public NeedType NeedType;
        public float SatisfactionTime;
        public float SatisfactionGained;
        public float DecreaseSpeed;

        public NeedProperties(NeedType ty)
        {
            NeedType = ty;
            SatisfactionTime = 5.0f;
            SatisfactionGained = 1.0f;
            DecreaseSpeed = 1.0f;
        }

        public NeedProperties Combine(NeedProperties other)
        {
            return new NeedProperties
            {
                NeedType = NeedType,
                SatisfactionTime = SatisfactionTime * other.SatisfactionTime,
                SatisfactionGained = SatisfactionGained * other.SatisfactionGained,
                DecreaseSpeed = DecreaseSpeed * other.DecreaseSpeed,
            };
        }
    }

    [SerializeField] private NeedProperties properties;
    public NeedType NeedType => properties.NeedType;

    public float satisfied = 0.0f;

    // TODO: refactor
    [SerializeField]
    [HideInInspector]
    private List<NeedModifiers> registeredModifiers;

    public Need(Need prototype)
    {
        satisfied = prototype.satisfied;
        properties = prototype.properties;
        registeredModifiers = new List<NeedModifiers>();
    }

    public Need(NeedProperties parameters)
    {
        properties = parameters;
        registeredModifiers = new List<NeedModifiers>();
    }

    // TODO: Cache DecreaseSpeed every 1s instead of computing it every frame?
    public void Desatisfy(float delta_time)
    {
        satisfied -= delta_time * GetProperties().DecreaseSpeed;
    }

    public void Satisfy()
    {
        satisfied += GetProperties().SatisfactionGained;
    }

    public NeedProperties GetProperties()
    {
        NeedModifiersCollection modifiers = NeedModifiersCollection.Default();
        foreach (NeedModifiers mods in registeredModifiers)
        {
            modifiers.Merge(mods.Modifiers);
        }
        return modifiers.Apply(properties);
    }

    public void RegisterModifier(NeedModifiers modifiers)
    {
        if (!registeredModifiers.Contains(modifiers))
        {
            registeredModifiers.Add(modifiers);
        }
    }

    public void UnregisterModifier(NeedModifiers modifiers)
    {
        _ = registeredModifiers.Remove(modifiers);
    }
}
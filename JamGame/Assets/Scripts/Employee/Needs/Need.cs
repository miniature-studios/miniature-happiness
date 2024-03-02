using System;
using System.Collections.Generic;
using Level.GlobalTime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Employee.Needs
{
    public enum NeedType
    {
        Work,
        Piss,
        Eat,
        Meeting,
        Leave,
        Smoke
    }

    [Serializable]
    public class Need
    {
        [HideLabel]
        [InlineProperty]
        [Serializable]
        public struct NeedProperties
        {
            public NeedType NeedType;
            public RealTimeSeconds SatisfactionTime;
            public float SatisfactionGained;
            public float DecreaseSpeed;
            public bool OverrideSatisfaction;
            public float OverrideSatisfactionValue;

            public NeedProperties(NeedType ty)
            {
                NeedType = ty;
                SatisfactionTime = new RealTimeSeconds(5.0f);
                SatisfactionGained = 1.0f;
                DecreaseSpeed = 1.0f;
                OverrideSatisfaction = false;
                OverrideSatisfactionValue = 0.0f;
            }

            public NeedProperties Combine(NeedProperties other)
            {
                return new NeedProperties
                {
                    NeedType = NeedType,
                    SatisfactionTime = SatisfactionTime * other.SatisfactionTime.Value,
                    SatisfactionGained = SatisfactionGained * other.SatisfactionGained,
                    DecreaseSpeed = DecreaseSpeed * other.DecreaseSpeed,
                    OverrideSatisfaction = OverrideSatisfaction || other.OverrideSatisfaction,
                    OverrideSatisfactionValue =
                        (OverrideSatisfactionValue * (OverrideSatisfaction ? 1.0f : 0.0f))
                        + (
                            other.OverrideSatisfactionValue
                            * (other.OverrideSatisfaction ? 1.0f : 0.0f)
                        )
                };
            }
        }

        [SerializeField]
        private NeedProperties properties;
        public NeedType NeedType => properties.NeedType;

        [SerializeField]
        private float satisfied = 0.0f;

        public float Satisfied => satisfied;

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
        public void Dissatisfy(RealTimeSeconds time)
        {
            NeedProperties properties = GetProperties();

            if (properties.OverrideSatisfaction)
            {
                satisfied = properties.OverrideSatisfactionValue;
            }
            else
            {
                satisfied -= time.Value * properties.DecreaseSpeed;
            }
        }

        public void Satisfy()
        {
            NeedProperties properties = GetProperties();

            if (properties.OverrideSatisfaction)
            {
                satisfied = properties.OverrideSatisfactionValue;
            }
            else
            {
                satisfied += properties.SatisfactionGained;
            }
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
}

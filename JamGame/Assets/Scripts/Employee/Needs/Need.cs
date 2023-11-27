using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Employee
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
        [Serializable]
        public struct NeedProperties
        {
            public NeedType NeedType;
            public float SatisfactionTime;
            public float SatisfactionGained;
            public float DecreaseSpeed;
            public bool OverrideSatisfaction;
            public float OverrideSatisfactionValue;

            public NeedProperties(NeedType ty)
            {
                NeedType = ty;
                SatisfactionTime = 5.0f;
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
                    SatisfactionTime = SatisfactionTime * other.SatisfactionTime,
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
        [FoldoutGroup("@NeedType")]
        private NeedProperties properties;
        public NeedType NeedType => properties.NeedType;

        [OdinSerialize]
        [FoldoutGroup("@NeedType")]
        public float Satisfied { get; private set; }

        [ReadOnly]
        [SerializeField]
        [FoldoutGroup("@NeedType")]
        private List<NeedModifiers> registeredModifiers = new();

        public Need(Need prototype)
        {
            Satisfied = prototype.Satisfied;
            properties = prototype.properties;
            registeredModifiers = new List<NeedModifiers>();
        }

        public Need(NeedProperties parameters)
        {
            properties = parameters;
            registeredModifiers = new List<NeedModifiers>();
        }

        // TODO: Cache DecreaseSpeed every 1s instead of computing it every frame?
        public void Dissatisfy(float delta_time)
        {
            NeedProperties properties = GetProperties();

            if (properties.OverrideSatisfaction)
            {
                Satisfied = properties.OverrideSatisfactionValue;
            }
            else
            {
                Satisfied -= delta_time * properties.DecreaseSpeed;
            }
        }

        public void Satisfy()
        {
            NeedProperties properties = GetProperties();

            if (properties.OverrideSatisfaction)
            {
                Satisfied = properties.OverrideSatisfactionValue;
            }
            else
            {
                Satisfied += properties.SatisfactionGained;
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

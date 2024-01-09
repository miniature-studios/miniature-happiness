using System.Collections.Generic;
using UnityEngine;

namespace Employee.Needs
{
    public struct NeedModifiersCollection
    {
        private Dictionary<NeedType, Need.NeedProperties> modifiers;

        public static NeedModifiersCollection Default()
        {
            return new NeedModifiersCollection
            {
                modifiers = new Dictionary<NeedType, Need.NeedProperties>()
            };
        }

        public NeedModifiersCollection(List<Need.NeedProperties> modifiers_raw)
        {
            modifiers = new Dictionary<NeedType, Need.NeedProperties>();
            foreach (Need.NeedProperties modifier in modifiers_raw)
            {
                if (modifiers.ContainsKey(modifier.NeedType))
                {
                    modifiers[modifier.NeedType] = modifiers[modifier.NeedType].Combine(modifier);
                }
                else
                {
                    modifiers.Add(modifier.NeedType, modifier);
                }
            }
        }

        public readonly void Merge(NeedModifiersCollection other)
        {
            foreach (KeyValuePair<NeedType, Need.NeedProperties> mods in other.modifiers)
            {
                if (modifiers.ContainsKey(mods.Key))
                {
                    modifiers[mods.Key] = modifiers[mods.Key].Combine(mods.Value);
                }
                else
                {
                    modifiers.Add(mods.Key, mods.Value);
                }
            }
        }

        public readonly Need.NeedProperties Apply(Need.NeedProperties properties)
        {
            if (modifiers.ContainsKey(properties.NeedType))
            {
                Need.NeedProperties modifier = modifiers[properties.NeedType];

                properties.OverrideSatisfaction = modifier.OverrideSatisfaction;
                properties.OverrideSatisfactionValue = modifier.OverrideSatisfactionValue;
                properties.SatisfactionGained *= modifier.SatisfactionGained;
                properties.SatisfactionTime *= modifier.SatisfactionTime;
                properties.DecreaseSpeed *= modifier.DecreaseSpeed;
            }

            return properties;
        }
    }

    [AddComponentMenu("Scripts/Employee.NeedModifiers")]
    public class NeedModifiers : MonoBehaviour
    {
        [SerializeField]
        private List<Need.NeedProperties> modifiersRaw;
        private NeedModifiersCollection modifiers;
        public NeedModifiersCollection Modifiers => modifiers;

        public void SetRawModifiers(List<Need.NeedProperties> raw)
        {
            modifiersRaw = raw;
            PrepareModifiers();
        }

        private void OnValidate()
        {
            PrepareModifiers();
        }

        private void Awake()
        {
            PrepareModifiers();
        }

        private void PrepareModifiers()
        {
            modifiersRaw ??= new List<Need.NeedProperties>();
            modifiers = new NeedModifiersCollection(modifiersRaw);
        }
    }
}

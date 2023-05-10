using System.Collections.Generic;
using UnityEngine;

public struct NeedModifiersCollection
{
    private Dictionary<NeedType, Need.NeedProperties> Modifiers;

    public static NeedModifiersCollection Default()
    {
        return new NeedModifiersCollection
        {
            Modifiers = new Dictionary<NeedType, Need.NeedProperties>()
        };
    }

    public NeedModifiersCollection(List<Need.NeedProperties> modifiers_raw)
    {
        Modifiers = new Dictionary<NeedType, Need.NeedProperties>();
        foreach (Need.NeedProperties modifier in modifiers_raw)
        {
            Modifiers.Add(modifier.NeedType, modifier);
        }
    }

    public void Merge(NeedModifiersCollection other)
    {
        foreach (KeyValuePair<NeedType, Need.NeedProperties> mods in other.Modifiers)
        {
            if (Modifiers.ContainsKey(mods.Key))
            {
                Modifiers[mods.Key] = Modifiers[mods.Key].Combine(mods.Value);
            }
            else
            {
                Modifiers.Add(mods.Key, mods.Value);
            }
        }
    }

    public Need.NeedProperties Apply(Need.NeedProperties properties)
    {
        if (Modifiers.ContainsKey(properties.NeedType))
        {
            Need.NeedProperties modifier = Modifiers[properties.NeedType];
            properties.SatisfactionGained *= modifier.SatisfactionGained;
            properties.SatisfactionTime *= modifier.SatisfactionTime;
            properties.DecreaseSpeed *= modifier.DecreaseSpeed;
        }

        return properties;
    }
}

public class NeedModifiers : MonoBehaviour
{
    [SerializeField] private List<Need.NeedProperties> modifiersRaw;
    private NeedModifiersCollection modifiers;
    public NeedModifiersCollection Modifiers => modifiers;

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
        modifiers = new NeedModifiersCollection(modifiersRaw);
    }
}
